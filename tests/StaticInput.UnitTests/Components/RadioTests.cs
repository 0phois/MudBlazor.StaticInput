using AngleSharp.Dom;
using Bunit;
using FluentAssertions;
using Microsoft.Playwright;
using MudBlazor.StaticInput;
using StaticInput.UnitTests.Fixtures;
using StaticInput.UnitTests.Viewer.Components.Tests.Radio;
using static Microsoft.Playwright.Assertions;

namespace StaticInput.UnitTests.Components
{
    public partial class RadioTests(ContextFixture contextFixture) : BaseComponentTest(contextFixture)
    {
        [Fact]
        public void MudStaticRadioGroup_Should_Render_RadioGroup()
        {
            IRenderedComponent<MudStaticRadioGroup<string>> comp = Context.Render<MudStaticRadioGroup<string>>();

            comp.Markup.Replace(" ", string.Empty).Should()
                .Contain("mud-radio-group")
                .And.Contain(@"role=""radiogroup""");
        }

        [Fact]
        public void MudStaticRadio_Should_Render_Radio()
        {
            IRenderedComponent<RadioSingleTest> comp = Context.Render<RadioSingleTest>();

            comp.Markup.Replace(" ", string.Empty).Should()
                .Contain("mud-radio")
                .And.Contain("input")
                .And.Contain("mud-radio-input")
                .And.Contain(@"role=""radio""");
        }

        [Fact]
        public void MudStaticRadio_Should_Have_Correct_Checked_State()
        {
            IRenderedComponent<RadioGroupTest> comp = Context.Render<RadioGroupTest>();

            var radios = comp.FindAll("input[type='radio']");
            IElement? radioA = radios.FirstOrDefault(r => r.GetAttribute("value") == "A");
            IElement? radioB = radios.FirstOrDefault(r => r.GetAttribute("value") == "B");

            radioA.Should().NotBeNull();
            radioB.Should().NotBeNull();

            radioA!.HasAttribute("checked").Should().BeTrue();
            radioB!.HasAttribute("checked").Should().BeFalse();
        }

        [Fact]
        public void MudStaticRadio_Icon_Should_Reflect_Checked_State()
        {
            IRenderedComponent<RadioGroupTest> comp = Context.Render<RadioGroupTest>();

            var radioInputs = comp.FindAll("input[type='radio']");
            IElement radioA = radioInputs.First(r => r.GetAttribute("value") == "A");
            IElement radioB = radioInputs.First(r => r.GetAttribute("value") == "B");

            var iconA = radioA.ParentElement!.QuerySelectorAll(".mud-icon-root");
            var iconB = radioB.ParentElement!.QuerySelectorAll(".mud-icon-root");

            iconA.Should().NotBeNullOrEmpty("The A radio input should be rendered.");
            iconB.Should().NotBeNullOrEmpty("The B radio input should be rendered.");

            iconA.First(x => x.Id!.StartsWith("radio-checked-icon-")).GetAttribute("style")!.Should().Contain("display: block", "The A radio input is checked");
            iconA.First(x => x.Id!.StartsWith("radio-unchecked-icon-")).GetAttribute("style")!.Should().Contain("display: none", "The A radio input is checked");

            iconB.First(x => x.Id!.StartsWith("radio-checked-icon-")).GetAttribute("style")!.Should().Contain("display: none", "The B radio input is checked");
            iconB.First(x => x.Id!.StartsWith("radio-unchecked-icon-")).GetAttribute("style")!.Should().Contain("display: block", "The B radio input is checked");
        }

        [Fact]
        public void MudStaticRadio_Checked_Unchecked_Colors_Should_Differ()
        {
            var comp = Context.Render<RadioColorsTest>();

            var radioInputs = comp.FindAll("input[type='radio']");
            IElement radioA = radioInputs.First(r => r.GetAttribute("value") == "A");
            IElement radioB = radioInputs.First(r => r.GetAttribute("value") == "B");

            var iconA = radioA.ParentElement!.QuerySelectorAll(".mud-icon-root");
            var iconB = radioB.ParentElement!.QuerySelectorAll(".mud-icon-root");


            iconA.First(x => x.Id!.StartsWith("radio-checked-icon-")).ClassList.Should().Contain("mud-primary-text", "Checked should be primary color");
            iconA.First(x => x.Id!.StartsWith("radio-unchecked-icon-")).ClassList.Should().Contain("mud-error-text", "Unchecked should be error color");

            iconB.First(x => x.Id!.StartsWith("radio-checked-icon-")).ClassList.Should().Contain("mud-primary-text", "Checked should be primary color");
            iconB.First(x => x.Id!.StartsWith("radio-unchecked-icon-")).ClassList.Should().Contain("mud-info-text", "Unchecked should be info color");
        }

        [Fact]
        public async Task CheckBox_State_Changes_When_Clicked()
        {
            var url = typeof(RadioIconTest).ToQueryString();

            await Page.GotoAsync(url);

            var radioA = Page.Locator("input.static-radio-input[value='A']");
            var radioB = Page.Locator("input.static-radio-input[value='B']");

            await Expect(radioA).ToBeCheckedAsync();
            await Expect(radioB).ToBeCheckedAsync(new() { Checked = false });

            await radioB.CheckAsync();

            await Expect(radioA).ToBeCheckedAsync(new() { Checked = false });
            await Expect(radioB).ToBeCheckedAsync();

        }

        [Fact]
        public async Task Radio_Disabled_Cannot_Be_Selected()
        {
            var url = typeof(RadioDisabledTest).ToQueryString();

            await Page.GotoAsync(url);

            var radioA = Page.Locator("input.static-radio-input[value='A']");
            var radioB = Page.Locator("input.static-radio-input[value='B']");

            await Expect(radioA).ToBeEnabledAsync();
            await Expect(radioB).ToBeDisabledAsync();

            await radioA.CheckAsync();
            
            await Expect(radioA).ToBeCheckedAsync();
            await Expect(radioB).ToBeCheckedAsync(new() { Checked = false });

            try
            {
                await radioB.CheckAsync(new() { Timeout = 2500 });
            }
            catch (TimeoutException) { }

            await Expect(radioB).ToBeCheckedAsync(new() { Checked = false });
            await Expect(radioA).ToBeCheckedAsync();
        }

        [Fact]
        public async Task Radio_Should_Checked_To_Submit()
        {
            var url = typeof(RadioRequiredTest).ToQueryString();

            await Page.GotoAsync(url);

            var button = Page.GetByRole(AriaRole.Button, new() { Name = "Submit" });

            await button.ClickAsync();

            var content = await Page.ContentAsync();

            content.Should().Contain("Selection required!");

            var radioC = Page.Locator("input.static-radio-input[value='C']");

            await radioC.CheckAsync();
            await Expect(radioC).ToBeCheckedAsync();

            await button.ClickAsync();

            await Expect(Page).ToHaveTitleAsync("Home");
        }

        [Fact]
        public async Task Radio_Should_Initialize_True()
        {
            var url = typeof(RadioInitTest).ToQueryString();

            await Page.GotoAsync(url);

            var radioA = Page.Locator("input.static-radio-input[value='A']");
            var radioB = Page.Locator("input.static-radio-input[value='B']");
            var radioC = Page.Locator("input.static-radio-input[value='C']");

            await Expect(radioA).ToBeCheckedAsync(new() { Checked = false });
            await Expect(radioB).ToBeCheckedAsync(new() { Checked = false });
            await Expect(radioC).ToBeCheckedAsync();

            await radioA.CheckAsync();

            await Expect(radioA).ToBeCheckedAsync();
            await Expect(radioB).ToBeCheckedAsync(new() { Checked = false });
            await Expect(radioC).ToBeCheckedAsync(new() { Checked = false });
        }

        [Fact]
        public async Task Radio_Should_Update_Hidden_Input_On_Change()
        {
            var url = typeof(RadioGroupTest).ToQueryString();
            await Page.GotoAsync(url);

            var radioA = Page.Locator("input.static-radio-input[value='A']");
            var radioB = Page.Locator("input.static-radio-input[value='B']");
            var hiddenInput = Page.Locator("input[type='hidden']");

            await Expect(radioA).ToBeCheckedAsync();
            await Expect(radioB).Not.ToBeCheckedAsync();
            await Expect(hiddenInput).ToHaveAttributeAsync("value", "A");

            await radioB.CheckAsync();

            await Expect(radioA).Not.ToBeCheckedAsync();
            await Expect(radioB).ToBeCheckedAsync();
            await Expect(hiddenInput).ToHaveAttributeAsync("value", "B");
        }
    }
}
