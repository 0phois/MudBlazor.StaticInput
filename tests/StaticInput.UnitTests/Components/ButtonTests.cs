using Bunit;
using Bunit.TestDoubles;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using MudBlazor.StaticInput;
using StaticInput.UnitTests.Fixtures;
using StaticInput.UnitTests.Viewer.Components.Tests.Button;
using static Microsoft.Playwright.Assertions;

namespace StaticInput.UnitTests.Components
{
    public class ButtonTests(ContextFixture contextFixture) : BaseComponentTest(contextFixture)
    {
        [Fact]
        public void MudStaticButton_Should_Render_Submit_Button()
        {
            var comp = Context.Render<MudStaticButton>();

            comp.Instance.HtmlTag.Should().Be("button");

            comp.Markup.Replace(" ", string.Empty).Should()
                .StartWith("<button")
                .And.Contain("submit")
                .And.Contain("mud-button-root");
        }

        [Fact]
        public void FormAction_Submit_Should_Render_Submit_Button()
        {
            var comp = Context.Render<MudStaticButton>(parameters => parameters.Add(x => x.FormAction, FormAction.Submit));

            comp.Instance.HtmlTag.Should().Be("button");

            comp.Markup.Replace(" ", string.Empty).Should()
                .StartWith("<button")
                .And.Contain(@"type=""submit""")
                .And.Contain("mud-button-root");
        }

        [Fact]
        public void FormAction_Reset_Should_Render_Reset_Button()
        {
            var comp = Context.Render<MudStaticButton>(parameters => parameters.Add(x => x.FormAction, FormAction.Reset));

            comp.Instance.HtmlTag.Should().Be("button");

            comp.Markup.Replace(" ", string.Empty).Should()
                .StartWith("<button")
                .And.Contain(@"type=""reset""")
                .And.Contain("mud-button-root");
        }

        [Fact]
        public void FormAction_Post_Should_Render_Submit_Button_Within_Form()
        {
            var comp = Context.Render<MudStaticButton>(parameters => parameters.Add(x => x.FormAction, FormAction.Post));

            comp.Instance.HtmlTag.Should().Be("button");

            comp.Markup.Replace(" ", string.Empty).Should()
                .StartWith("<form")
                .And.Contain(@"method=""post""")
                .And.Contain("<button")
                .And.Contain(@"type=""submit""")
                .And.Contain("mud-button-root");
        }

        [Fact]
        public void SubmitButton_Should_Not_Submit_Invalid_Form()
        {
            var comp = Context.Render<ButtonSubmitTest>();

            comp.Find("button").Click();

            comp.Markup.Should().Contain("mud-input-error")
                .And.Contain("The Email field is required.");

            var navigation = Context.Services.GetRequiredService<BunitNavigationManager>();

            navigation.History.Should().HaveCount(0);
        }

        [Fact]
        public void SubmitButton_Should_Submit_Valid_Form()
        {
            var comp = Context.Render<ButtonSubmitTest>();

            comp.Find("input").Change("test@mail.com");
            comp.Find("button").Click();

            var navigation = Context.Services.GetRequiredService<BunitNavigationManager>();

            navigation.History.Should().HaveCount(1);
            navigation.Uri.Should().Be("http://localhost/");
        }

        [Fact]
        public async Task ResetButton_Should_Clear_Form()
        {
            var email = "reset@mail.com";
            var url = typeof(ButtonResetTest).ToQueryString();

            await Page.GotoAsync(url);

            var input = Page.GetByLabel("Email");

            await input.FillAsync(email);
            await Expect(input).ToHaveValueAsync(email);

            await Page.GetByRole(AriaRole.Button, new() { Name = "Reset" }).ClickAsync();

            await Expect(input).ToBeEmptyAsync();
        }

        [Fact]
        public async Task PostButton_Should_Execute_Action()
        {
            var url = typeof(ButtonPostTest).ToQueryString();

            await Page.GotoAsync(url);
            await Page.GetByText("Execute").ClickAsync();

            await Expect(Page).ToHaveURLAsync("TestEndpoint");
        }

        [Fact]
        public async Task PostButton_Should_Redirect_To_ReturnUrl()
        {
            var url = typeof(ButtonPostTest).ToQueryString();

            await Page.GotoAsync(url);
            await Page.GetByText("Redirect").ClickAsync();

            await Expect(Page).ToHaveURLAsync("home");
        }
    }
}
