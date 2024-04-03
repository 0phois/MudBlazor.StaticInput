using System.Windows.Input;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor.StaticInput;

public partial class MudStaticButton : MudButton
{
    /**********************************************
     * Hide these inherited properties to prevent *
     * consumers from modifying them directly.    *
     **********************************************/

    protected new ButtonType ButtonType { get; set; }
    protected new string HtmlTag { get; set; } = "button";
    protected new string? Href { get; set; }
    protected new string? Link { get; set; }
    protected new string? Target { get; set; }
    protected new ICommand? Command { get; set; }
    protected new bool ClickPropagation { get; set; }
    protected new object? CommandParameter { get; set; }
    protected new EventCallback<MouseEventArgs> OnClick { get; set; }
}