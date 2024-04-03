using Bunit;
using Bunit.TestDoubles;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.StaticInput;
using StaticInput.UnitTests.Viewer.Components.Tests.Button;

namespace StaticInput.UnitTests.Components
{
    public class ButtonTests : BunitTest
    {
        [Fact]
        public void MudStaticButton_Should_Render_Submit_Button()
        {
            var comp = Context.RenderComponent<MudStaticButton>();

            comp.Instance.HtmlTag.Should().Be("button");

            comp.Markup.Replace(" ", string.Empty).Should()
                .StartWith("<button")
                .And.Contain("submit")
                .And.Contain("mud-button-root");
        }

        [Fact]
        public void FormAction_Submit_Should_Render_Submit_Button()
        {
            var param = ComponentParameter.CreateParameter(nameof(MudStaticButton.FormAction), FormAction.Submit);
            var comp = Context.RenderComponent<MudStaticButton>(param);

            comp.Instance.HtmlTag.Should().Be("button");

            comp.Markup.Replace(" ", string.Empty).Should()
                .StartWith("<button")
                .And.Contain(@"type=""submit""")
                .And.Contain("mud-button-root");
        }

        [Fact]
        public void FormAction_Reset_Should_Render_Reset_Button()
        {
            var param = ComponentParameter.CreateParameter(nameof(MudStaticButton.FormAction), FormAction.Reset);
            var comp = Context.RenderComponent<MudStaticButton>(param);

            comp.Instance.HtmlTag.Should().Be("button");

            comp.Markup.Replace(" ", string.Empty).Should()
                .StartWith("<button")
                .And.Contain(@"type=""reset""")
                .And.Contain("mud-button-root");
        }

        [Fact]
        public void FormAction_Post_Should_Render_Submit_Button_Within_Form()
        {
            var param = ComponentParameter.CreateParameter(nameof(MudStaticButton.FormAction), FormAction.Post);
            var comp = Context.RenderComponent<MudStaticButton>(param);

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
            var comp = Context.RenderComponent<ButtonSubmitTest>();

            comp.Find("button").Click();

            var navigation = Context.Services.GetRequiredService<FakeNavigationManager>();

            navigation.History.Should().HaveCount(0);
        }

        [Fact]
        public void SubmitButton_Should_Submit_Valid_Form()
        {
            var comp = Context.RenderComponent<ButtonSubmitTest>();

            comp.Find("input").Change("test@mail.com");
            comp.Find("button").Click();

            var navigation = Context.Services.GetRequiredService<FakeNavigationManager>();

            navigation.History.Should().HaveCount(1);
            navigation.Uri.Should().Be("http://localhost/");
        }
    }
}
