using FluentAssertions;
using MudBlazor.StaticInput;
using StaticInput.UnitTests.Fixtures;

namespace StaticInput.UnitTests.Components
{
    public class CheckBoxTests(ContextFixture contextFixture) : BaseComponentTest(contextFixture)
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
