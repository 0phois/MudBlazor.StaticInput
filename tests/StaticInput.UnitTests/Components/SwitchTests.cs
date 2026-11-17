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

        [Fact]
        public async Task Switch_Space_Toggles_When_Focused_With_Tab()
        {
            var url = typeof(SwitchKeyboardTest).ToQueryString();

            await Page.GotoAsync(url);

            var container = Page.GetByTestId(MyRegex());
            var toggle = Page.Locator("input[type='checkbox']");

            await Page.Locator("#before-switch").FocusAsync();
            await Page.Keyboard.PressAsync("Tab");

            await Page.Keyboard.PressAsync("Space");

            await Expect(toggle).ToBeCheckedAsync();
            await Expect(toggle).ToHaveAttributeAsync("aria-checked", "true");
            await Expect(container).ToHaveClassAsync(new Regex("mud-checked"));

            await Page.Keyboard.PressAsync("Space");

            await Expect(toggle).ToBeCheckedAsync(new() { Checked = false });
            await Expect(toggle).ToHaveAttributeAsync("aria-checked", "false");
            await Expect(container).Not.ToHaveClassAsync(new Regex("mud-checked"));
        }

        [Fact]
        public async Task Switch_Focused_With_Tab_Is_Accessible_Checkbox()
        {
            var url = typeof(SwitchKeyboardTest).ToQueryString();

            await Page.GotoAsync(url);

            await Page.Locator("#before-switch").FocusAsync();
            await Page.Keyboard.PressAsync("Tab");

            await Expect(Page.GetByRole(AriaRole.Checkbox, new() { Name = "Accept" })).ToBeFocusedAsync();
        }

        [Fact]
        public async Task Switch_Checked_With_Space_Should_Submit_True()
        {
            var url = typeof(SwitchKeyboardTest).ToQueryString();

            await Page.GotoAsync(url);

            await Page.Locator("#before-switch").FocusAsync();
            await Page.Keyboard.PressAsync("Tab");
            await Page.Keyboard.PressAsync("Space");

            await Page.GetByRole(AriaRole.Button, new() { Name = "Submit" }).ClickAsync();

            await Expect(Page.Locator("#submitted-value")).ToHaveTextAsync("Submitted: True");
        }

        [Fact]
        public async Task Switch_Unchecked_With_Space_Should_Submit_False()
        {
            var url = typeof(SwitchKeyboardTest).ToQueryString();

            await Page.GotoAsync(url);

            var toggle = Page.Locator("input[type='checkbox']");

            await Page.Locator("#before-switch").FocusAsync();
            await Page.Keyboard.PressAsync("Tab");
            await Page.Keyboard.PressAsync("Space");

            await Expect(toggle).ToBeCheckedAsync();

            await Page.Keyboard.PressAsync("Space");

            await Expect(toggle).ToBeCheckedAsync(new() { Checked = false });

            await Page.GetByRole(AriaRole.Button, new() { Name = "Submit" }).ClickAsync();

            await Expect(Page.Locator("#submitted-value")).ToHaveTextAsync("Submitted: False");
        }

        [Fact]
        public async Task Switch_Disabled_Is_Skipped_By_Tab_And_Not_Toggled()
        {
            var url = typeof(SwitchKeyboardDisabledTest).ToQueryString();

            await Page.GotoAsync(url);

            var toggle = Page.Locator("input[type='checkbox']");

            await Page.Locator("#before-switch").FocusAsync();
            await Page.Keyboard.PressAsync("Tab");

            await Expect(Page.Locator("#after-switch")).ToBeFocusedAsync();
            await Expect(toggle).ToBeCheckedAsync(new() { Checked = false });
        }

        [Fact]
        public async Task Switch_ReadOnly_Cannot_Be_Changed_By_Click()
        {
            var url = typeof(SwitchReadOnlyTest).ToQueryString();

            await Page.GotoAsync(url);

            var toggle = Page.Locator("input[type='checkbox']");

            await toggle.ClickAsync();

            await Expect(toggle).ToBeCheckedAsync(new() { Checked = false });
            await Expect(toggle).ToHaveAttributeAsync("aria-checked", "false");
        }

        [Fact]
        public async Task Switch_ReadOnly_Is_Focusable_But_Not_Toggled_By_Space()
        {
            var url = typeof(SwitchReadOnlyTest).ToQueryString();

            await Page.GotoAsync(url);

            var toggle = Page.GetByRole(AriaRole.Checkbox, new() { Name = "Accept" });

            await Page.Locator("#before-switch").FocusAsync();
            await Page.Keyboard.PressAsync("Tab");

            await Expect(toggle).ToBeFocusedAsync();

            await Page.Keyboard.PressAsync("Space");

            await Expect(toggle).ToBeCheckedAsync(new() { Checked = false });
            await Expect(toggle).ToHaveAttributeAsync("aria-checked", "false");
        }

        [GeneratedRegex("switch-container-*")]
        private static partial Regex MyRegex();
    }
}
