using AngleSharp.Html.Dom;
using Bunit;
using FluentAssertions;
using MudBlazor.StaticInput;

namespace StaticInput.UnitTests.Components
{
    public class CheckBoxTests : BunitTest
    {
        [Fact]
        public void MudStaticCheckBox_Should_Render_CheckBox()
        {

            var comp = Context.RenderComponent<MudStaticCheckBox>();

            comp.Markup.Replace(" ", string.Empty).Should()
                .Contain("mud-checkbox")
                .And.Contain("input")
                .And.Contain(@"type=""checkbox""");
        }
    }
}
