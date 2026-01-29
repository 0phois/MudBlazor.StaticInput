using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;

namespace MudBlazor.StaticInput;

public partial class MudStaticCheckBox : MudCheckBox<bool>
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
    protected new bool TriState { get; set; }
    protected new bool KeyboardEnabled { get; set; }
    protected new EventCallback<bool> ValueChanged { get; set; }
    protected new string IndeterminateIcon { get; set; } = Icons.Material.Filled.IndeterminateCheckBox;
}