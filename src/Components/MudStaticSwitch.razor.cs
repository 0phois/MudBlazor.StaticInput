using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;

namespace MudBlazor.StaticInput;

public partial class MudStaticSwitch : MudSwitch<bool>
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

    /**********************************************
     * Hide these inherited properties to prevent *
     * consumers from modifying them directly.    *
     **********************************************/
    protected new bool Value { get; set; }
    protected new EventCallback<bool> ValueChanged { get; set; }

    /*
     * Options for SwitchClassname
     */
    private string CheckedTextColor => $"mud-{Color.ToString().ToLower()}-text";
    private string CheckedHoverColor => $"hover:mud-{Color.ToString().ToLower()}-hover";
    private string UncheckedTextColor => $"mud-{UncheckedColor.ToString().ToLower()}-text";
    private string UncheckedHoverColor => $"hover:mud-{UncheckedColor.ToString().ToLower()}-hover";

    /*
     * Options for TrackClassname
     */
    private string CheckedColor => $"mud-{Color.ToString().ToLower()}";
    private string UnCheckedColor => $"mud-{UncheckedColor.ToString().ToLower()}";
}