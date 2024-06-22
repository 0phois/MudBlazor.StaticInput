using Microsoft.AspNetCore.Components;

namespace MudBlazor.StaticInput;

public partial class MudStaticSwitch : MudSwitch<bool>
{
    /**********************************************
     * Hide these inherited properties to prevent *
     * consumers from modifying them directly.    *
     **********************************************/
    protected new bool Value { get; set; }
    protected new EventCallback<bool> ValueChanged { get; set; }

    /*
     * Options for SwitchClassname
     */
    private string CheckedTextColor => $"mud-{Color.ToDescriptionString()}-text";
    private string CheckedHoverColor => $"hover:mud-{Color.ToDescriptionString()}-hover";
    private string UncheckedTextColor => $"mud-{UncheckedColor.ToDescriptionString()}-text";
    private string UncheckedHoverColor => $"hover:mud-{UncheckedColor.ToDescriptionString()}-hover";

    /*
     * Options for TrackClassname
     */
    private string CheckedColor => $"mud-{Color.ToDescriptionString()}";
    private string UnCheckedColor => $"mud-{UncheckedColor.ToDescriptionString()}";
}