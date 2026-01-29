using Microsoft.AspNetCore.Components;

namespace MudBlazor.StaticInput;

public partial class MudStaticSelectItem<T> : MudSelectItem<T>
{
    /**********************************************
     * Hide these inherited properties to prevent *
     * consumers from modifying them directly.    *
     **********************************************/
    // MudSelectItem doesn't have many properties that need hiding for static,
    // but we'll follow the pattern if we find any.
}
