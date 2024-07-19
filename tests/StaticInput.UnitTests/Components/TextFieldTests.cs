using Bunit;
using FluentAssertions;
using MudBlazor.StaticInput;
using StaticInput.UnitTests.Fixtures;
using StaticInput.UnitTests.Viewer.Components.Tests.TextField;
using static Microsoft.Playwright.Assertions;

namespace StaticInput.UnitTests.Components
{
    public class TextFieldTests(ContextFixture contextFixture) : BaseComponentTest(contextFixture)
    {
        [Fact]
        public void MudStaticTextField_Should_Render_TextField()
        {
            var comp = Context.RenderComponent<MudStaticTextField<string>>();

            comp.Markup.Replace(" ", string.Empty).Should().Contain("mud-input-control")
                .And.Contain("mud-input-root");
        }

        [Fact]
        public void TextField_Input_Should_Shink_Label()
        {
            var comp = Context.RenderComponent<TextFieldLabelTest>();

            var field = comp.FindComponents<MudStaticTextField<string>>()
                            .First(x => x.Instance.Label!.Equals("Basic Label", StringComparison.OrdinalIgnoreCase));

            var index = field.Markup.IndexOf("<script>");

            field.Markup[..index].Should().NotContain("mud-shrink"); //label should not shunk on first render

            field.Find("input").Change("random text");

            field.Markup[..index].Should().Contain("mud-shrink"); //label is shrunk after input
        }

        [Fact]
        public void TextField_AdonrmentStart_Should_Shink_Label()
        {
            var comp = Context.RenderComponent<TextFieldLabelTest>();

            var field = comp.FindComponents<MudStaticTextField<string>>()
                            .First(x => x.Instance.Label!.Equals("Adornment Start", StringComparison.OrdinalIgnoreCase));

            var index = field.Markup.IndexOf("<script>");

            field.Markup[..index].Should().Contain("mud-shrink"); //label should be shrunk on first render

            field.Find("input").Change("random text");

            field.Markup[..index].Should().Contain("mud-shrink"); //label remains shrunk after input 
        }

        [Fact]
        public void TextField_AdornmentEnd_Should_Not_Shink_Label()
        {
            var comp = Context.RenderComponent<TextFieldLabelTest>();

            var field = comp.FindComponents<MudStaticTextField<string>>()
                            .First(x => x.Instance.Label!.Equals("Adornment End", StringComparison.OrdinalIgnoreCase));

            var index = field.Markup.IndexOf("<script>");

            field.Markup[..index].Should().NotContain("mud-shrink"); //label should not shunk on first render

            field.Find("input").Change("random text");

            field.Markup[..index].Should().Contain("mud-shrink"); //label is shrunk after input
        }

        [Fact]
        public void TextField_Shrink_Label_Should_Shink_Label()
        {
            var comp = Context.RenderComponent<TextFieldLabelTest>();

            var field = comp.FindComponents<MudStaticTextField<string>>()
                            .First(x => x.Instance.Label!.Equals("Shrink Label", StringComparison.OrdinalIgnoreCase));

            var index = field.Markup.IndexOf("<script>");

            field.Markup[..index].Should().Contain("mud-shrink"); //label should be shrunk on first render

            field.Find("input").Change("random text");

            field.Markup[..index].Should().Contain("mud-shrink"); //label remains shrunk after input 
        }

        [Fact]
        public void TextField_Placeholder_Should_Shink_Label()
        {
            var comp = Context.RenderComponent<TextFieldLabelTest>();

            var field = comp.FindComponents<MudStaticTextField<string>>()
                            .First(x => x.Instance.Label!.Equals("Placeholder", StringComparison.OrdinalIgnoreCase));

            var index = field.Markup.IndexOf("<script>");

            field.Markup[..index].Should().Contain("mud-shrink"); //label should be shrunk on first render

            field.Find("input").Change("random text");

            field.Markup[..index].Should().Contain("mud-shrink"); //label remains shrunk after input 
        }

        [Fact]
        public async Task TextField_Should_Always_Show_HelpText()
        {
            var url = typeof(TextFieldHelperTextTest).ToQueryString();

            await Page.GotoAsync(url);

            var helperText = Page.GetByText("Always visible");

            await Expect(helperText).ToBeVisibleAsync();

            var field = Page.GetByLabel("Show Always");

            await field.FocusAsync();

            await Expect(helperText).ToBeVisibleAsync();

            await field.BlurAsync();

            await Expect(helperText).ToBeVisibleAsync();

            await field.FillAsync("random text");

            await Expect(helperText).ToBeVisibleAsync();
        }

        [Fact]
        public async Task TextField_Should_Show_HelpText_OnFocus()
        {
            var url = typeof(TextFieldHelperTextTest).ToQueryString();

            await Page.GotoAsync(url);

            var helperText = Page.GetByText("Show on focus", new() { Exact = true });

            await Expect(helperText).ToBeHiddenAsync();

            var field = Page.GetByLabel("Show on Focus", new() { Exact = true });

            await field.FocusAsync();

            await Expect(helperText).ToBeVisibleAsync();

            await field.BlurAsync();

            await Expect(helperText).ToBeHiddenAsync();

            await field.FillAsync("random text");
            await field.BlurAsync();

            await Expect(helperText).ToBeHiddenAsync();
        }

        [Fact]
        public async Task TextField_DataAnnotations_Are_Functional()
        {
            var url = typeof(TextFieldDataAnnotationTest).ToQueryString();

            await Page.GotoAsync(url);

            var field = Page.GetByLabel("Short string");
            var button = Page.Locator("button");

            await button.ClickAsync();

            var content = await Page.ContentAsync();
            content.Should().Contain("The Value field is required.");

            await field.FillAsync("new value");
            await button.ClickAsync();

            content = await Page.ContentAsync();
            content.Should().Contain("Should not be longer than 3");

            await field.FillAsync("yay");
            await button.ClickAsync();

            await Expect(Page).ToHaveTitleAsync("Home");
        }

        [Fact]
        public void TextField_Adornment_Should_Render_Without_Button()
        {
            var comp = Context.RenderComponent<TextFieldAdornmentTest>();

            var field = comp.FindComponents<MudStaticTextField<string>>()
                            .First(x => x.Instance.Label!.Equals("Email", StringComparison.OrdinalIgnoreCase));

            field.Markup.Should().Contain("mud-input-adornment-icon").And.Contain("svg").And.NotContain("<button");
        }

        [Fact]
        public void TextField_With_AdornmentClickFunction_Should_Render_AdornmentButton()
        {
            var comp = Context.RenderComponent<TextFieldAdornmentTest>();

            var field = comp.FindComponents<MudStaticTextField<string>>()
                            .First(x => x.Instance.Label!.Equals("Password", StringComparison.OrdinalIgnoreCase));

            field.Markup.Should().Contain("mud-input-adornment-icon").And.Contain("svg").And.Contain("<button");
        }

        [Fact]
        public async Task TextField_AdornmentButton_IsClickable()
        {
            var url = typeof(TextFieldAdornmentTest).ToQueryString();

            await Page.GotoAsync(url);

            var button = Page.Locator("button");
            var message = string.Empty;

            Page.Dialog += async (_, dialog) =>
            {
                message = dialog.Message;
                await dialog.AcceptAsync();
            };

            await button.ClickAsync();

            message.Should().Be("Adornment clicked!");
        }
    }
}
