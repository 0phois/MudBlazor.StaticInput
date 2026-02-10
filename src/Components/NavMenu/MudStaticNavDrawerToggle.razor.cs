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
public partial class MudStaticNavDrawerToggle : MudIconButton

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

    [Inject]
    private IJSRuntime JsRuntime { get; set; } = default!;

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
        if (IsStatic())
        {
            var storageKey = !string.IsNullOrEmpty(DrawerId) && DrawerId != "_no_id_provided_"
                ? $"mud-static-drawer-open-{DrawerId}"
                : "mud-static-drawer-open-default";

            if (HttpContext?.Request.Cookies.TryGetValue(storageKey, out var value) == true)
            {
                bool storedOpen = value == "true";
                if (storedOpen != Open)
                {
                    Open = storedOpen;
                    _ = OpenChanged.InvokeAsync(Open);
                }
            }
        }
        else if (JsRuntime is IJSInProcessRuntime inProcess)
        {
            try
            {
                var stored = inProcess.Invoke<bool?>("MudDrawerInterop.getDrawerState", DrawerId);
                if (stored.HasValue && stored.Value != Open)
                {
                    Open = stored.Value;
                    _ = OpenChanged.InvokeAsync(Open);
                }
            }
            catch
            {
                // Ignore sync JS failures
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
                if (stored.HasValue && stored.Value != Open)
                {
                    Open = stored.Value;
                    await OpenChanged.InvokeAsync(Open);
                }
            }
            catch
            {
                // Ignore JS interop failures during pre-rendering or if not yet available
            }
        }

        await base.OnInitializedAsync();
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
                    if (storedOpen != Open)
                    {
                        Open = storedOpen;
                        await OpenChanged.InvokeAsync(storedOpen);
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
