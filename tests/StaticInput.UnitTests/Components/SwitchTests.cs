using FluentAssertions;
using Microsoft.Playwright;
using MudBlazor.StaticInput;
using StaticInput.UnitTests.Fixtures;
using StaticInput.UnitTests.Viewer.Components.Tests.Switch;
using System.Text.RegularExpressions;
using static Microsoft.Playwright.Assertions;

namespace StaticInput.UnitTests.Components
{
    public partial class SwitchTests(ContextFixture contextFixture) : BaseComponentTest(contextFixture)
    {
        [Fact]
        public void MudStaticSwitch_Should_Render_Switch()
        {
            var comp = Context.Render<MudStaticSwitch>();

            comp.Markup.Replace(" ", string.Empty).Should()
                .Contain("mud-switch")
                .And.Contain("input")
                .And.Contain("mud-switch-input")
                .And.Contain(@"type=""checkbox""");
        }

        [Fact]
        public async Task Switch_Checked_Unchecked_Colors_Should_Differ()
        {
            var url = typeof(SwitchColorsTest).ToQueryString();

            await Page.GotoAsync(url);

            var toggle = Page.GetByTestId(MyRegex());
            var checkedColor = Regex.Escape("mud-primary-text");
            var uncheckedColor = Regex.Escape("mud-error-text");

            await Expect(toggle).ToBeCheckedAsync(new() { Checked = false });
            await Expect(toggle).ToHaveClassAsync(new Regex(uncheckedColor));

            await toggle.CheckAsync();

            await Expect(toggle).ToBeCheckedAsync();
            await Expect(toggle).ToHaveClassAsync(new Regex(checkedColor));
        }

        [Fact]
        public async Task Switch_State_Changes_When_Clicked()
        {
            var url = typeof(SwitchToggleIconTest).ToQueryString();

            await Page.GotoAsync(url);

            var toggle = Page.Locator("input[type='checkbox']");

            await Expect(toggle).ToBeCheckedAsync(new() { Checked = false });

            await toggle.CheckAsync();

            await Expect(toggle).ToBeCheckedAsync();
        }

        [Fact]
        public void Switch_Should_Render_ThumbIcon()
        {
            var comp = Context.Render<SwitchToggleIconTest>();

            comp.Markup.Replace(" ", string.Empty).Should()
                .Contain("mud-switch")
                .And.Contain("input")
                .And.Contain("mud-icon-root")
                .And.Contain("mud-switch-input")
                .And.Contain(@"type=""checkbox""");
        }

        [Fact]
        public async Task Switch_Disabled_Cannot_Be_Selected()
        {
            var url = typeof(SwitchDisabledTest).ToQueryString();

            await Page.GotoAsync(url);

            var checkbox = Page.Locator("input[type='checkbox']");

            await Expect(checkbox).ToBeDisabledAsync();

            try
            {
                await checkbox.CheckAsync(new() { Timeout = 2500 });
            }
            catch (TimeoutException) { }

            await Expect(checkbox).ToBeCheckedAsync(new() { Checked = false });
        }

        [Fact]
        public async Task Switch_Should_BeSelected_To_Submit()
        {
            var url = typeof(SwitchRequiredTest).ToQueryString();

            await Page.GotoAsync(url);

            var checkbox = Page.Locator("input[type='checkbox']");
            var button = Page.GetByRole(AriaRole.Button, new() { Name = "Submit" });

            await Expect(checkbox).ToBeCheckedAsync(new() { Checked = false });

            await button.ClickAsync();

            var content = await Page.ContentAsync();

            content.Should().Contain("You must confirm");

            await checkbox.CheckAsync();

            await Expect(checkbox).ToBeCheckedAsync();

            await button.ClickAsync();

            await Expect(Page).ToHaveTitleAsync("Home");
        }

        [Fact]
        public async Task Switch_Should_Initialize_True()
        {
            var url = typeof(SwitchInitTest).ToQueryString();

            await Page.GotoAsync(url);

            var checkbox = Page.Locator("input[type='checkbox']");

            await Expect(checkbox).ToBeCheckedAsync();

            await checkbox.UncheckAsync();

            await Expect(checkbox).ToBeCheckedAsync(new() { Checked = false });
        }

        [GeneratedRegex("switch-container-*")]
        private static partial Regex MyRegex();
    }
}
