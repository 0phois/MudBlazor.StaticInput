using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor.StaticInput;

public partial class MudStaticTextField<T> : MudTextField<T>
{
    /**********************************************
     * Hide these inherited properties to prevent *
     * consumers from modifying them directly.    *
     **********************************************/

    protected new int? Counter { get; set; }
    protected new int MaxLines { get; set; }
    protected new bool AutoFocus { get; set; }
    protected new bool AutoGrow { get; set; }
    protected new bool Clearable { get; set; }
    protected new bool Immediate { get; set; }
    protected new IMask? Mask { get; set; } = null;
    protected new double DebounceInterval { get; set; }
    protected new bool TextUpdateSuppression { get; set; } = true;
    protected new bool OnlyValidateIfDirty { get; set; } = false;
    protected new EventCallback<string> TextChanged { get; set; }
    protected new EventCallback<FocusEventArgs> OnBlur { get; set; }
    protected new EventCallback<KeyboardEventArgs> OnKeyDown { get; set; }
    protected new bool KeyDownPreventDefault { get; set; }
    protected new EventCallback<KeyboardEventArgs> OnKeyUp { get; set; }
    protected new bool KeyUpPreventDefault { get; set; }
    protected new EventCallback<T> ValueChanged { get; set; }
    protected new EventCallback<MouseEventArgs> OnAdornmentClick { get; set; }
    protected new EventCallback<string> OnDebounceIntervalElapsed { get; set; }
    protected new EventCallback<MouseEventArgs> OnClearButtonClick { get; set; }
    protected new EventCallback<ChangeEventArgs> OnInternalInputChanged { get; set; }
}