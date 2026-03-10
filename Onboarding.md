# MudBlazor.StaticInput — Onboarding Guide

> **Goal:** Understand why this library exists, how it works, and how to implement a new static component from scratch.

---

## Table of Contents

1. [What Is This Library?](#1-what-is-this-library)
2. [Core Architectural Concepts](#2-core-architectural-concepts)
   - [The `IsStatic()` Guard](#21-the-isstatic-guard)
   - [`UserAttributes` as a Data Channel](#22-userattributes-as-a-data-channel)
   - [Expression-Driven Naming](#23-expression-driven-naming)
   - [The Hidden Input Trick](#24-the-hidden-input-trick)
   - [Property Hiding with `protected new`](#25-property-hiding-with-protected-new)
   - [The JavaScript Initialisation Module](#26-the-javascript-initialisation-module)
3. [Component Anatomy](#3-component-anatomy)
   - [Pattern A — Full Re-render](#pattern-a--full-re-render)
   - [Pattern B — Base Passthrough](#pattern-b--base-passthrough)
4. [Implementing a New Static Component](#4-implementing-a-new-static-component)
5. [Critical Rules & Gotchas](#5-critical-rules--gotchas)
6. [Component Reference](#6-component-reference)
7. [Advanced: The Drawer Toggle](#7-advanced-the-drawer-toggle)
8. [Testing Your Component](#8-testing-your-component)
9. [Glossary](#9-glossary)

---

## 1. What Is This Library?

`MudBlazor.StaticInput` is an extension library that wraps standard MudBlazor input components to make them work correctly in Blazor's **Static Server-Side Rendering (SSR)** mode.

In static SSR, the page is rendered as plain HTML — there is no SignalR hub, no WebAssembly runtime, and no JavaScript interop available at render time. That means standard MudBlazor components, which rely on Blazor's interactive event system (`@onclick`, `@bind-Value`), simply do not function.

This library bridges that gap. Each `MudStatic*` component inherits from its MudBlazor counterpart and adds:

- **Correct HTML form semantics** (`name`, `value`, `type` attributes) so browsers can submit forms natively.
- **Lightweight JavaScript initialisation** that replaces missing interactive event handlers with vanilla DOM listeners.
- **A dual-render strategy:** SSR-safe markup when static, passthrough to the base component when interactive.

> **Why does this matter?**  
> Blazor SSR is increasingly common for identity flows, public-facing pages, and hybrid apps where full interactivity is not needed on every page. MudBlazor out-of-the-box does not support these scenarios.

---

## 2. Core Architectural Concepts

### 2.1 The `IsStatic()` Guard

Every component in this library has an internal method that determines the rendering context at runtime:

```csharp
private bool IsStatic()
{
#if NET9_0_OR_GREATER
    return !RendererInfo.IsInteractive;
#else
    return HttpContext != null;
#endif
}
```

On `net8.0`, static rendering is detected by the presence of an injected `HttpContext` (available only in SSR). On `net9.0+`, Blazor exposes `RendererInfo.IsInteractive` directly, which is the cleaner approach.

> **This is the single most important concept in the library.** All conditional behaviour — attribute injection, event suppression, and JS initialisation data — flows from this one check.

---

### 2.2 `UserAttributes` as a Data Channel

MudBlazor components expose a `UserAttributes` dictionary (`Dictionary<string, object>`). Static components use this as a channel to pass configuration data from C# to JavaScript:

```csharp
protected override void OnParametersSet()
{
    if (IsStatic())
    {
        UserAttributes["data-mud-static-type"] = "checkbox";
        UserAttributes["data-mud-static-name"] = _name;
    }
    else
    {
        UserAttributes.Remove("data-mud-static-type");
        UserAttributes.Remove("data-mud-static-initialized");
    }

    base.OnParametersSet();
}
```

The JavaScript module then queries for `data-mud-static-type` to know which initialiser to run, and reads the other `data-mud-static-*` attributes for configuration. Once JS has run, it stamps `data-mud-static-initialized="true"` on the element to prevent double-initialisation.

---

### 2.3 Expression-Driven Naming

HTML forms require inputs to carry a `name` attribute that maps to the server-side model property. Because static components cannot use `@bind-Value`, they accept a `ValueExpression` parameter instead:

```csharp
[Parameter] public Expression<Func<bool>>? ValueExpression { get; set; }

protected override void OnInitialized()
{
    var expression = ValueExpression?.ToString();
    var index = expression?.LastIndexOf(").", StringComparison.Ordinal) ?? -1;

    if (index > 0)
    {
        _name = expression![(index + 2)..];  // e.g. "Input.RememberMe"
    }
}
```

The expression string looks like `value(model).PropertyName`. Slicing from the last `).` gives the property name, which becomes the `name` attribute. This is the same technique used internally by `InputBase<T>` for client-side validation.

> **Consumer usage** follows the same pattern as `EditForm`:
> ```razor
> <MudStaticCheckBox For="() => Input.RememberMe" />
> ```
> The `For` parameter is mapped to `ValueExpression` under the hood.

---

### 2.4 The Hidden Input Trick

A classic HTML form problem: unchecked checkboxes and toggles submit **nothing** — the field is entirely absent from the form payload. This library solves it with a companion hidden input:

```razor
<input type="hidden" name="@(_checkboxValue ? "" : _name)" value="False" />
<input type="checkbox" name="@(_checkboxValue ? _name : "")"
       value="True" ... />
```

Only one of the two inputs carries the `name` at any given time. When checked, the checkbox wins; when unchecked, the hidden input submits `False`. JavaScript keeps these in sync on change events.

---

### 2.5 Property Hiding with `protected new`

Interactive MudBlazor properties that cannot work in static mode (`@onclick`, `@bind-Value`, masks, debounce, etc.) are hidden using `protected new` to prevent consumers from accidentally wiring them up:

```csharp
// In MudStaticTextField.razor.cs
protected new bool Clearable { get; set; }
protected new bool Immediate { get; set; }
protected new EventCallback<string> TextChanged { get; set; }
protected new EventCallback<FocusEventArgs> OnBlur { get; set; }
// ... and many more
```

> **Why `protected new` and not `[Parameter]` hiding?**  
> `[Parameter]` hiding breaks parameter binding in Blazor. `protected new` removes the property from IDE autocomplete for consumers without breaking internal base class wiring.

---

### 2.6 The JavaScript Initialisation Module

The library ships a single `.lib.module.js` file. Blazor automatically loads this when the assembly is referenced. The module:

- Hooks into `enhancedload` to re-run after Blazor Enhanced Navigation page transitions.
- Uses a `MutationObserver` to catch elements added dynamically (e.g. during WASM hydration).
- Each `init*()` function queries for its `data-mud-static-type` and skips elements already stamped with `data-mud-static-initialized`.

```js
export function afterWebStarted(blazor) {
    blazor?.addEventListener('enhancedload', () => initialize());
    initialize();

    const observer = new MutationObserver(() => initialize());
    observer.observe(document.body, { childList: true, subtree: true });
}

function initialize() {
    initTextFields();
    initCheckBoxes();
    initSwitches();
    initRadios();
    initDrawers();
    initNavGroups();
}
```

---

## 3. Component Anatomy

Every static component follows a strict two-file pattern:

| File | Responsibility |
|------|---------------|
| `.razor` | HTML structure, form semantics, conditional rendering based on `IsStatic()` |
| `.razor.cs` | `HttpContext` injection, `IsStatic()` implementation, property hiding |

---

### Pattern A — Full Re-render

Used when the HTML structure needs significant changes (e.g. adding hidden inputs, changing element types). The Razor file renders the entire template directly rather than delegating to the base class.

**When to use:** Checkboxes, switches, radio buttons — anything that needs extra `<input>` elements for form submission.

```razor
@inherits MudCheckBox<bool>

<MudInputControl ...>
    <InputContent>
        <label ...>
            <input type="hidden" name="@(_checkboxValue ? "" : _name)" value="False" />
            <input type="checkbox" ... />
            ...
        </label>
    </InputContent>
</MudInputControl>
```

---

### Pattern B — Base Passthrough

Used when the base component's HTML is fine and you only need to inject `data-*` attributes or suppress events. The Razor file calls `base.BuildRenderTree(__builder)` and adds parameters in the `@code` block.

**When to use:** Text fields, nav groups, icon buttons — anything where the base HTML already works and you just need to annotate it.

```razor
@inherits MudTextField<T>

@{
    base.BuildRenderTree(__builder);
}

@code {
    // Additional parameters and lifecycle overrides
}
```

---

## 4. Implementing a New Static Component

Here is the complete process for wrapping a new MudBlazor component. The steps below use a hypothetical `MudStaticSelect<T>` as the example.

### Step 1 — Create the `.razor.cs` Partial Class

This file handles context detection and property hiding. Start with this boilerplate every time:

```csharp
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;

namespace MudBlazor.StaticInput;

public partial class MudStaticSelect<T> : MudSelect<T>
{
    [CascadingParameter]
    private HttpContext HttpContext { get; set; } = default!;

    private bool IsStatic()
    {
#if NET9_0_OR_GREATER
        return !RendererInfo.IsInteractive;
#else
        return HttpContext != null;
#endif
    }

    /***********************************************
     * Hide these inherited properties to prevent  *
     * consumers from modifying them directly.     *
     ***********************************************/
    protected new EventCallback<T> ValueChanged { get; set; }
    protected new EventCallback<IEnumerable<T>> SelectedValuesChanged { get; set; }
}
```

---

### Step 2 — Create the `.razor` File

Decide on Pattern A or B (see [Component Anatomy](#3-component-anatomy)). For most components, Pattern B is the right default:

```razor
@namespace MudBlazor.StaticInput
@typeparam T
@inherits MudSelect<T>

@{
    base.BuildRenderTree(__builder);
}

@code {
    [Parameter] public Expression<Func<T?>>? ValueExpression { get; set; }

    private string _name = string.Empty;

    protected override void OnInitialized()
    {
        var expression = ValueExpression?.ToString();
        var index = expression?.LastIndexOf(").", StringComparison.Ordinal) ?? -1;

        if (index > 0)
            _name = expression![(index + 2)..];

        base.OnInitialized();
    }

    protected override void OnParametersSet()
    {
        if (IsStatic())
        {
            UserAttributes["data-mud-static-type"] = "select";
            UserAttributes["name"] = _name;
        }
        else
        {
            UserAttributes.Remove("data-mud-static-type");
            UserAttributes.Remove("data-mud-static-initialized");
        }

        base.OnParametersSet();
    }
}
```

---

### Step 3 — Write the JavaScript Initialiser

Add an `initSelects()` function to `.lib.module.js` and call it from `initialize()`:

```js
function initialize() {
    // ... existing init calls ...
    initSelects();
}

function initSelects() {
    const selects = document.querySelectorAll(
        '[data-mud-static-type="select"]:not([data-mud-static-initialized="true"])'
    );

    selects.forEach(selectElement => {
        // Stamp immediately to prevent double-init
        selectElement.setAttribute('data-mud-static-initialized', 'true');

        // Wire up DOM behaviour without JS interop
        selectElement.addEventListener('change', () => {
            // e.g. update aria-selected state, sync visual indicators
        });
    });
}
```

> **Keep JavaScript minimal.** The goal is to patch visual state, not replicate business logic. If you find yourself writing complex JS, ask whether the component truly belongs in this library.

---

### Step 4 — Register the Component

Ensure the namespace is covered by `_Imports.razor`. Since all components share the `MudBlazor.StaticInput` namespace, this is already handled — no changes needed unless you introduce a sub-namespace.

---

### Step 5 — Consider Multi-Target Implications

This library targets `net8.0`, `net9.0`, and `net10.0`. MudBlazor's NuGet version constraints differ per framework (see `.csproj`). If your component uses APIs only available on certain versions, add an `#if` guard or a conditional `<ItemGroup Condition>` in the project file.

---

## 5. Critical Rules & Gotchas

| Rule | Why It Matters |
|------|---------------|
| Always call `base.OnParametersSet()` **last** | The base component needs to see the final `UserAttributes` after your modifications. |
| Remove `data-mud-static-initialized` in the `else` branch | If a page transitions from SSR to interactive (WASM hydration), stale `initialized` markers prevent JS from re-running. |
| Use `protected new`, not `[Parameter]` hiding | `[Parameter]` hiding breaks Blazor parameter binding. `protected new` removes IDE autocomplete without breaking internals. |
| Never use `@bind-Value` in a static component | Two-way binding requires interactive render mode. Use `ValueExpression` for initial value derivation. |
| `LastIndexOf(").")` only handles simple property access | The pattern works for `model.Prop`. Nested paths like `model.Child.Prop` include the full path — this is usually fine for form name binding but be aware. |
| Stamp `data-mud-static-initialized` in JS immediately | `MutationObserver` fires frequently. Without the guard, `initialize()` will run multiple times on the same element. |
| `base.BuildRenderTree` must be called with no surrounding HTML (Pattern B) | If you add sibling elements around the `base.BuildRenderTree` call, Blazor's diffing can break. |

---

## 6. Component Reference

| Component | Base Class | Pattern | Key Special Behaviour |
|-----------|-----------|---------|----------------------|
| `MudStaticTextField<T>` | `MudTextField<T>` | Passthrough | Shrink label logic, adornment click JS callback, `name` injection via `UserAttributes` |
| `MudStaticCheckBox` | `MudCheckBox<bool>` | Full re-render | Hidden input trick, icon show/hide via `display` style |
| `MudStaticSwitch` | `MudSwitch<bool>` | Full re-render | Hidden input trick, CSS class toggling for track/thumb colours passed via `data-*` |
| `MudStaticRadio<T>` | `MudRadio<T>` | Full re-render | Must live inside `MudStaticRadioGroup`; checked state derived from parent's `SelectedValue` |
| `MudStaticRadioGroup<T>` | `MudRadioGroup<T>` | Full re-render | Hidden input for unselected state; cascades `GroupName` and `SelectedValue` to children |
| `MudStaticButton` | `MudButton` | Full re-render | `FormAction.Post` wraps button in `<form method="post">` and injects `<AntiforgeryToken />` |
| `MudStaticNavGroup` | `MudNavGroup` | Passthrough | Collapse/expand via CSS class toggling — no SignalR needed |
| `MudStaticNavDrawerToggle` | `MudIconButton` | Passthrough | Three-layer state persistence: cookie + `PersistentComponentState` + `localStorage` |

---

## 7. Advanced: The Drawer Toggle

`MudStaticNavDrawerToggle` is the most complex component in the library because it must maintain state across the SSR → interactive transition. This is the hardest problem in hybrid Blazor rendering.

### The Problem

When a page is first rendered as SSR, the drawer is open or closed based on a server-side boolean. When Blazor hydrates to interactive mode, it re-renders from scratch — losing the user's toggle state unless it was persisted somewhere accessible to both sides of the transition.

### The Solution: Three-Layer Persistence

**Layer 1 — Cookie**

Written by JS on every toggle. Read by the server on the next SSR request via `HttpContext.Request.Cookies`. This keeps the rendered HTML in sync with the user's last-known preference before any JS runs.

```js
function updateStorage(key, value) {
    localStorage.setItem(key, value);
    document.cookie = `${key}=${value}; path=/; SameSite=Lax`;
}
```

**Layer 2 — `PersistentComponentState`**

Blazor's built-in mechanism for passing data from the SSR pre-render phase to interactive startup. The component registers a persisting callback during SSR and reads the snapshot during interactive `OnInitialized`:

```csharp
// During SSR — write state
_subscription = PersistentState.RegisterOnPersisting(() =>
{
    PersistentState.PersistAsJson(storageKey, _open);
    return Task.CompletedTask;
}, PersistMode);

// During interactive startup — read state
if (PersistentState.TryTakeFromJson<bool>(storageKey, out var restored))
{
    _open = restored;
}
```

**Layer 3 — `localStorage` Fallback**

Used in `OnAfterRenderAsync` for WASM scenarios where pre-rendering did not run and the cookie may not have been set:

```csharp
protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (firstRender && !IsStatic())
    {
        var stored = await JsRuntime.InvokeAsync<string>("localStorage.getItem", storageKey);
        if (stored != null && bool.Parse(stored) != _open)
        {
            _open = bool.Parse(stored);
            await OpenChanged.InvokeAsync(_open);
            StateHasChanged();
        }
    }
}
```

> **`PersistMode` parameter:** Defaults to `InteractiveAuto`. If your app uses only `InteractiveServer` or `InteractiveWebAssembly`, set this explicitly to avoid unnecessary state registrations.

---

## 8. Testing Your Component

Because static components depend on rendering context, testing requires deliberate setup.

### Manual Testing Checklist

- [ ] Render the page as **pure SSR** (no interactive render mode on the page). The component must emit correct HTML with `name`, `value`, and `type` attributes.
- [ ] **Submit the form.** The model binder must correctly receive values for both checked and unchecked states (or selected/unselected, etc.).
- [ ] Re-render the page after form submission **with model errors.** Error state (`HasErrors`) must be reflected in the component.
- [ ] Navigate using **Blazor Enhanced Navigation.** The JS initialiser must re-run and re-attach event listeners (check that `initialized` attributes are cleaned up correctly).
- [ ] Switch the page to an **interactive render mode.** The component must fall back to standard MudBlazor behaviour — no static `data-*` attributes in the DOM, no JS initialiser running on it.
- [ ] If the component has persistent state (e.g. drawer toggle), verify the **SSR → interactive handoff** restores the correct state without a visual flash.

### Debugging Reference

| Symptom | Likely Cause |
|---------|-------------|
| Checkbox always submits `True` regardless of state | Hidden input `name` logic inverted — check the `_checkboxValue` condition on both inputs |
| Label does not shrink when field has a value | `data-mud-static-shrink` not set, or the JS initialiser did not run |
| Component reinitialises on every DOM mutation | `data-mud-static-initialized` not being stamped in JS |
| `data-*` attributes visible in DOM in interactive mode | `IsStatic()` returning `true` in interactive mode — check `HttpContext` and `RendererInfo` logic |
| Drawer state lost on hydration | `PersistentComponentState` not wired correctly, or `PersistMode` mismatch |
| `OnParametersSet` changes not reflected by base component | `base.OnParametersSet()` called before `UserAttributes` were modified |

---

## 9. Glossary

| Term | Definition |
|------|-----------|
| **SSR / Static SSR** | Server-Side Rendering with no persistent connection. Pages are rendered to HTML and sent to the browser. There is no Blazor circuit. |
| **Interactive Render Mode** | Blazor Server or WebAssembly — a component with a live runtime that can handle events (`@onclick`, `@bind-Value`, etc.). |
| **`IsStatic()`** | The internal guard method that returns `true` when the component is rendering without an interactive runtime. |
| **`UserAttributes`** | A MudBlazor dictionary of extra HTML attributes splatted onto the root element. Used here as a C# → JS configuration data channel. |
| **`data-mud-static-initialized`** | JS attribute stamped on an element once its initialiser has run. Prevents double-initialisation by `MutationObserver`. |
| **`PersistentComponentState`** | ASP.NET Core service for passing serialised data from the SSR pre-render phase to the interactive startup phase. |
| **`ValueExpression`** | A lambda expression parameter (`Expression<Func<T>>`) used to derive the HTML `name` attribute without requiring two-way binding. |
| **Hidden input trick** | Pairing a `type="hidden"` input (submitting `False`) with a checkbox (submitting `True`) so that the unchecked state is never absent from the form payload. |
| **Pattern A** | Render pattern where the static component provides its own complete HTML template, replacing the base class output. |
| **Pattern B** | Render pattern where the static component calls `base.BuildRenderTree` and only modifies attributes via `UserAttributes`. |
| **Enhanced Navigation** | Blazor's fetch-based page navigation that updates only the changed parts of the DOM, triggering `enhancedload` instead of a full page reload. |
