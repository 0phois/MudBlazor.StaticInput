using Microsoft.AspNetCore.Components;

namespace MudBlazor.StaticInput;

public partial class MudStaticCheckBox : MudCheckBox<bool>
{
    /**********************************************
     * Hide these inherited properties to prevent *
     * consumers from modifying them directly.    *
     **********************************************/
    protected new bool Checked { get; set; }
    protected new bool TriState { get; set; }
    protected new bool KeyboardEnabled { get; set; }
    protected new EventCallback<bool> ValueChanged { get; set; }
    protected new EventCallback<bool> CheckedChanged { get; set; }
    protected new string IndeterminateIcon { get; set; } = Icons.Material.Filled.IndeterminateCheckBox;
}