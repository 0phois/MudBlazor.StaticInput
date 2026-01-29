using Bunit;
using FluentAssertions;
using MudBlazor.StaticInput;
using StaticInput.UnitTests.Fixtures;
using StaticInput.UnitTests.Viewer.Components.Tests.Select;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace StaticInput.UnitTests.Components
{
    public class SelectTests(ContextFixture contextFixture) : BaseComponentTest(contextFixture)
    {
        [Fact]
        public void MudStaticSelect_Should_Render_Select()
        {
            var comp = Context.Render<SelectBasicTest>();

            comp.Markup.Should().Contain("mud-input-control")
                .And.Contain("<select")
                .And.Contain("<option value=\"1\">One</option>")
                .And.Contain("selected");
        }

        [Fact]
        public async Task Select_State_Changes_When_Value_Selected()
        {
            var url = typeof(SelectBasicTest).ToQueryString();

            await Page.GotoAsync(url);

            var select = Page.Locator("select");
            var presentation = Page.Locator(".mud-input-slot:not(select)");

            await Expect(select).ToHaveValueAsync("2");
            await Expect(presentation).ToHaveTextAsync("Two");

            await select.SelectOptionAsync(new[] { "3" });

            await Expect(select).ToHaveValueAsync("3");
            await Expect(presentation).ToHaveTextAsync("Three");
        }

        [Fact]
        public async Task Select_Multi_Should_Work()
        {
            var url = typeof(SelectMultiTest).ToQueryString();

            await Page.GotoAsync(url);

            var select = Page.Locator("select");
            var presentation = Page.Locator(".mud-input-slot:not(select)");

            await Expect(select).ToHaveValuesAsync(new[] { "1", "3" });
            await Expect(presentation).ToHaveTextAsync("One, Three");

            await select.SelectOptionAsync(new[] { "1", "2" });

            await Expect(select).ToHaveValuesAsync(new[] { "1", "2" });
            await Expect(presentation).ToHaveTextAsync("One, Two");
        }

        [Fact]
        public async Task Select_Required_Should_Show_Error()
        {
            var url = typeof(SelectRequiredTest).ToQueryString();

            await Page.GotoAsync(url);

            var select = Page.Locator("select");
            var button = Page.GetByRole(AriaRole.Button, new() { Name = "Submit" });

            // Initially select the empty option
            await select.SelectOptionAsync("");

            await button.ClickAsync();

            var content = await Page.ContentAsync();
            content.Should().Contain("Selection required!");

            await select.SelectOptionAsync("1");
            await button.ClickAsync();

            // After valid submit, title might change or stay same depending on test app behavior.
            // In these tests usually they go back to Home or stay.
        }
    }
}
