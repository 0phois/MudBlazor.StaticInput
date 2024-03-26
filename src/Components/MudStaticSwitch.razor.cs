using Microsoft.AspNetCore.Components;

namespace MudBlazor.StaticInput;

public partial class MudStaticSwitch
{
    /**********************************************
     * Hide these inherited properties to prevent *
     * consumers from modifying them directly.    *
     **********************************************/
    protected new bool Checked { get; set; }
    protected new EventCallback<bool> ValueChanged { get; set; }
    protected new EventCallback<bool> CheckedChanged { get; set; }

    /*
     * Options for SwitchClassname
     */
    private string CheckedTextColor => $"mud-{Color.ToDescriptionString()}-text";
    private string CheckedHoverColor => $"hover:mud-{Color.ToDescriptionString()}-hover";
    private string UncheckedTextColor => $"mud-{UnCheckedColor.ToDescriptionString()}-text";
    private string UncheckedHoverColor => $"hover:mud-{UnCheckedColor.ToDescriptionString()}-hover";

    /*
     * Options for TrackClassname
     */
    private string CheckedColor => $"mud-{Color.ToDescriptionString()}";
    private string UncheckedColor => $"mud-{UnCheckedColor.ToDescriptionString()}";
}