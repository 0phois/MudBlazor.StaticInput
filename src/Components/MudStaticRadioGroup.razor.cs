using Microsoft.AspNetCore.Components;

namespace MudBlazor.StaticInput
{
    public partial class MudStaticRadioGroup<T> : MudRadioGroup<T>
    {
        /**********************************************
         * Hide these inherited properties to prevent *
         * consumers from modifying them directly.    *
         **********************************************/
        protected new T? Value { get; set; }
        protected new EventCallback<T?> ValueChanged { get; set; }
    }
}
