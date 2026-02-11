using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;

namespace MudBlazor.StaticInput;

/// <summary>
/// A <see cref="MudIconButton"/> used to toggle the state of the navigation <see cref="MudDrawer"/> 
/// </summary>
/// <remarks>
/// <list type="bullet">
///     <item>
///         <see cref="DrawerClipMode"/> should be set to <see cref="DrawerClipMode.Always"/>
///     </item>
///     <item>
///         <see cref="DrawerVariant.Temporary"/> is not currently supported since the required <see cref="MudOverlay"/> is not implemented
///     </item>
/// </list>
/// </remarks>
public partial class MudStaticNavDrawerToggle : MudIconButton, IDisposable

{
    [CascadingParameter]
    private HttpContext HttpContext { get; set; } = default!;

    /// <summary>
    /// The open state of the drawer. Bind this to the same variable as your MudDrawer.
    /// </summary>
    [Parameter]
    public bool Open { get; set; }

    /// <summary>
    /// Event callback for when the open state changes.
    /// </summary>
    [Parameter]
    public EventCallback<bool> OpenChanged { get; set; }

    /// <summary>
    /// The render mode to persist the state for./>.
    /// Defaults to <c>null</c> to register all render modes.
    /// </summary>
    [Parameter]
    public IComponentRenderMode? PersistMode { get; set; } = RenderMode.InteractiveAuto;

    private bool _open;
    private bool? _lastOpen;

    [Inject]
    private IJSRuntime JsRuntime { get; set; } = default!;

    [Inject]
    private PersistentComponentState PersistentState { get; set; } = default!;

    private PersistingComponentStateSubscription? _subscription;

    private bool IsStatic()
    {
#if NET9_0_OR_GREATER
        return !RendererInfo.IsInteractive;
#else
        return HttpContext != null;
#endif
    }

    protected new EventCallback<MouseEventArgs> OnClick { get; set; }
    protected new ButtonType ButtonType { get; set; }
    protected new string HtmlTag { get; set; } = "button";
    protected new string? Href { get; set; }
    protected new string? Target { get; set; }
    protected new bool ClickPropagation { get; set; }

    protected override void OnInitialized()
    {
        _open = Open;
        _lastOpen = Open;

        var storageKey = !string.IsNullOrEmpty(DrawerId) && DrawerId != "_no_id_provided_"
            ? $"mud-static-drawer-open-{DrawerId}"
            : "mud-static-drawer-open-default";

        if (IsStatic())
        {
            if (PersistMode != null)
            {
                _subscription = PersistentState.RegisterOnPersisting(() =>
                {
                    PersistentState.PersistAsJson(storageKey, _open);
                    return Task.CompletedTask;
                }, PersistMode);
            }
            else
            {
                _subscription = PersistentState.RegisterOnPersisting(() =>
                {
                    PersistentState.PersistAsJson(storageKey, _open);
                    return Task.CompletedTask;
                }, RenderMode.InteractiveAuto);
            }

            if (HttpContext?.Request.Cookies.TryGetValue(storageKey, out var value) == true)
            {
                bool storedOpen = value == "true";
                if (storedOpen != _open)
                {
                    _open = storedOpen;
                    _lastOpen = _open;
                    _ = OpenChanged.InvokeAsync(_open);
                }
            }
        }
        else
        {
            if (PersistentState.TryTakeFromJson<bool>(storageKey, out var restored))
            {
                if (restored != _open)
                {
                    _open = restored;
                    _lastOpen = _open;
                    _ = OpenChanged.InvokeAsync(_open);
                }
            }
            else if (JsRuntime is IJSInProcessRuntime inProcess)
            {
                try
                {
                    var stored = inProcess.Invoke<bool?>("MudDrawerInterop.getDrawerState", DrawerId);
                    if (stored.HasValue && stored.Value != _open)
                    {
                        _open = stored.Value;
                        _lastOpen = _open;
                        _ = OpenChanged.InvokeAsync(_open);
                    }
                }
                catch
                {
                    // Ignore sync JS failures
                }
            }
        }

        base.OnInitialized();
    }

    protected override async Task OnInitializedAsync()
    {
        if (!IsStatic())
        {
            try
            {
                // In WASM, JS Interop is available in OnInitializedAsync if not pre-rendering.
                // This helps avoid the flicker by setting the state before the first render.
                var stored = await JsRuntime.InvokeAsync<bool?>("MudDrawerInterop.getDrawerState", DrawerId);
                if (stored.HasValue && stored.Value != _open)
                {
                    _open = stored.Value;
                    _lastOpen = _open;
                    await OpenChanged.InvokeAsync(_open);
                }
            }
            catch
            {
                // Ignore JS interop failures during pre-rendering or if not yet available
            }
        }

        await base.OnInitializedAsync();
    }

    public void Dispose()
    {
        _subscription?.Dispose();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && !IsStatic())
        {
            // Fallback sync in case OnInitializedAsync was pre-rendering
            var storageKey = !string.IsNullOrEmpty(DrawerId) && DrawerId != "_no_id_provided_"
                ? $"mud-static-drawer-open-{DrawerId}"
                : "mud-static-drawer-open-default";

            try
            {
                var stored = await JsRuntime.InvokeAsync<string>("localStorage.getItem", storageKey);
                if (stored != null)
                {
                    bool storedOpen = stored == "true";
                    if (storedOpen != _open)
                    {
                        _open = storedOpen;
                        _lastOpen = _open;
                        await OpenChanged.InvokeAsync(_open);
                        StateHasChanged();
                    }
                }
            }
            catch (Exception ex)
            {
                try { await JsRuntime.InvokeVoidAsync("console.error", $"Error syncing drawer state: {ex.Message}"); } catch { }
            }
        }

        await base.OnAfterRenderAsync(firstRender);
    }
}
