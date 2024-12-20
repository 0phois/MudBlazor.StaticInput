using AngleSharp.Css.Dom;
using Bunit;
using FluentAssertions;
using Microsoft.Playwright;
using MudBlazor.StaticInput;
using StaticInput.UnitTests.Fixtures;
using StaticInput.UnitTests.Viewer.Components.Tests.CheckBox;
using System.Text.RegularExpressions;
using static Microsoft.Playwright.Assertions;

namespace StaticInput.UnitTests.Components
{
    public partial class CheckBoxTests(ContextFixture contextFixture) : BaseComponentTest(contextFixture)
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

        [Fact]
        public void CheckBox_Checked_Unchecked_Colors_Should_Differ()
        {
            var comp = Context.RenderComponent<CheckBoxColorsTest>();

            var boxes = comp.FindAll("svg");
            var selected = boxes.First(x => x.Id!.Contains("check-icon-"));
            var unselected = boxes.First(x => x.Id!.Contains("unchecked-icon-"));

            selected.GetStyle().CssText.Should().Be("display: none");
            unselected.GetStyle().CssText.Should().Be("display: block");

            selected.ClassList.Should().Contain("mud-primary-text");
            unselected.ClassList.Should().Contain("mud-error-text");
        }

        [Fact]
        public async Task CheckBox_State_Changes_When_Clicked()
        {
            var url = typeof(CheckBoxToggleIconTest).ToQueryString();

            await Page.GotoAsync(url);

            var checkbox = Page.Locator("input[type='checkbox']");

            await Expect(checkbox).ToBeCheckedAsync(new() { Checked = false });

            await checkbox.CheckAsync();

            await Expect(checkbox).ToBeCheckedAsync();
        }

        [Fact]
        public async Task CheckBox_Icon_Changes_When_Checked()
        {
            var url = typeof(CheckBoxToggleIconTest).ToQueryString();

            await Page.GotoAsync(url);

            var selected = Page.GetByTestId(CheckedIcon());
            var unselected = Page.GetByTestId(UncheckedIcon());
            var checkbox = Page.Locator("input[type='checkbox']");

            await Expect(selected).ToBeVisibleAsync(new() { Visible = false });
            await Expect(unselected).ToBeVisibleAsync();

            await checkbox.CheckAsync();

            await Expect(selected).ToBeVisibleAsync();
            await Expect(unselected).ToBeVisibleAsync(new() { Visible = false });

            await checkbox.ClickAsync();

            await Expect(selected).ToBeVisibleAsync(new() { Visible = false });
            await Expect(unselected).ToBeVisibleAsync();
        }

        [Fact]
        public async Task CheckBox_Disabled_Cannot_Be_Selected()
        {
            var url = typeof(CheckBoxDisabledTest).ToQueryString();

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
        public async Task CheckBox_Should_Checked_To_Submit()
        {
            var url = typeof(CheckBoxRequiredTest).ToQueryString();

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
        public async Task Checkbox_Should_Initialize_True()
        {
            var url = typeof(CheckBoxInitTest).ToQueryString();

            await Page.GotoAsync(url);

            var selected = Page.GetByTestId(CheckedIcon());
            var unselected = Page.GetByTestId(UncheckedIcon());
            var checkbox = Page.Locator("input[type='checkbox']");

            await Expect(selected).ToBeVisibleAsync();
            await Expect(unselected).ToBeVisibleAsync(new() { Visible = false });

            await Expect(checkbox).ToBeCheckedAsync();

            await checkbox.UncheckAsync();

            await Expect(checkbox).ToBeCheckedAsync(new() { Checked = false });
            await Expect(selected).ToBeVisibleAsync(new() { Visible = false });
            await Expect(unselected).ToBeVisibleAsync();
        }

        [GeneratedRegex("check-icon-*")]
        private static partial Regex CheckedIcon();

        [GeneratedRegex("unchecked-icon-*")]
        private static partial Regex UncheckedIcon();
    }
}
