using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;
using MudBlazor.Extensions;

namespace MudBlazor.StaticInput;

public partial class MudStaticSelect<T> : MudSelect<T>
{
    [CascadingParameter]
    private HttpContext HttpContext { get; set; } = default!;

    internal bool IsStatic()
    {
#if NET9_0_OR_GREATER
        return !RendererInfo.IsInteractive;
#else
        return HttpContext != null;
#endif
    }

    internal new bool GetDisabledState() => Disabled;
    internal new bool GetReadOnlyState() => ReadOnly;

    /**********************************************
     * Hide these inherited properties to prevent *
     * consumers from modifying them directly.    *
     **********************************************/
    protected new EventCallback<T> ValueChanged { get; set; }
    protected new EventCallback<IEnumerable<T>> SelectedValuesChanged { get; set; }
    protected new bool AutoFocus { get; set; }
    protected new bool Clearable { get; set; }
    protected new bool Immediate { get; set; }
    protected new bool SelectAll { get; set; }
    protected new string SelectAllText { get; set; } = string.Empty;

    protected new string InputClassname => new CssBuilder("mud-input")
        .AddClass($"mud-input-{Variant.ToDescriptionString()}")
        .AddClass($"mud-input-margin-{Margin.ToDescriptionString()}")
        .AddClass("mud-input-underline", Variant != Variant.Outlined)
        .AddClass("mud-shrink", _isShrink)
        .Build();
}
