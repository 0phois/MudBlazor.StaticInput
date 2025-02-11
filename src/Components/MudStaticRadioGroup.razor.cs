using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor.StaticInput;

public partial class MudStaticRadioGroup<T> : MudRadioGroup<T>
{
    /**********************************************
     * Hide these inherited properties to prevent *
     * consumers from modifying them directly.    *
     **********************************************/
    protected new T? Value { get; set; }
    protected new EventCallback<T?> ValueChanged { get; set; }

    private string GetInputClass() =>
            new CssBuilder("mud-radio-group")
                .AddClass(InputClass)
                .Build();

    internal bool GetDisabledState() => Disabled || ParentIsDisabled;
    internal bool GetReadOnlyState() => ReadOnly || ParentIsReadOnly;
}
