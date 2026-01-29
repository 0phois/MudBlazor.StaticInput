using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http;

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
}
