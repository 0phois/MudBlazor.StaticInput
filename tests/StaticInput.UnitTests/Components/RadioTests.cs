using FluentAssertions;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components;
using MudBlazor.StaticInput;
using StaticInput.UnitTests.Fixtures;
using Bunit;
using AngleSharp.Dom;
using StaticInput.UnitTests.Viewer.Components.Tests.Radio;

namespace StaticInput.UnitTests.Components
{
    public partial class RadioTests(ContextFixture contextFixture) : BaseComponentTest(contextFixture)
    {
        [Fact]
        public void MudStaticRadioGroup_Should_Render_RadioGroup()
        {
            IRenderedComponent<MudStaticRadioGroup<string>> comp = Context.RenderComponent<MudStaticRadioGroup<string>>();

            comp.Markup.Replace(" ", string.Empty).Should()
                .Contain("mud-radio-group")
                .And.Contain(@"role=""radiogroup""");
        }

        [Fact]
        public void MudStaticRadio_Should_Render_Radio()
        {
            IRenderedComponent<RadioSingleTest> comp = Context.RenderComponent<RadioSingleTest>();

            comp.Markup.Replace(" ", string.Empty).Should()
                .Contain("mud-radio")
                .And.Contain("input")
                .And.Contain("mud-radio-input")
                .And.Contain(@"role=""radio""");
        }

        [Fact]
        public void MudStaticRadio_Should_Have_Correct_Checked_State()
        {
            IRenderedComponent<RadioGroupTest> comp = Context.RenderComponent<RadioGroupTest>();

            IRefreshableElementCollection<IElement> radios = comp.FindAll("input[type='radio']");
            IElement? radioA = radios.FirstOrDefault(r => r.GetAttribute("value") == "A");
            IElement? radioB = radios.FirstOrDefault(r => r.GetAttribute("value") == "B");

            radioA.Should().NotBeNull();
            radioB.Should().NotBeNull();

            radioA.HasAttribute("checked").Should().BeTrue();
            radioB.HasAttribute("checked").Should().BeFalse();
        }

        [Fact]
        public void MudStaticRadio_Icon_Should_Reflect_Checked_State()
        {
            IRenderedComponent<RadioGroupTest> comp = Context.RenderComponent<RadioGroupTest>();

            IRefreshableElementCollection<IElement> radioInputs = comp.FindAll("input[type='radio']");
            IElement radioA = radioInputs.First(r => r.GetAttribute("value") == "A");
            IElement radioB = radioInputs.First(r => r.GetAttribute("value") == "B");

            IElement? iconA = radioA.ParentElement.QuerySelector(".mud-icon-root");
            IElement? iconB = radioB.ParentElement.QuerySelector(".mud-icon-root");

            iconA.Should().NotBeNull("L'icône de la radio A doit être rendue.");
            iconB.Should().NotBeNull("L'icône de la radio B doit être rendue.");

            iconA.InnerHtml.Should().Contain("M12 7c-2.76", "car la radio A est cochée");
            iconB.InnerHtml.Should().Contain("M12 2C6.48", "car la radio B est décochée");
        }
    }
}
