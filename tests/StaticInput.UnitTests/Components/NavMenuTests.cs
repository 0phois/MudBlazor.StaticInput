using FluentAssertions;
using StaticInput.UnitTests.Fixtures;
using StaticInput.UnitTests.Viewer.Components.Tests.NavMenu;

namespace StaticInput.UnitTests.Components
{
    public partial class NavMenuTests(ContextFixture contextFixture) : BaseComponentTest(contextFixture)
    {
        [Fact]
        public async Task Group_Collapses_Expands_On_Click()
        {
            var url = typeof(NavMenuCollapseExpandTest).ToQueryString();

            await Page.GotoAsync(url);

            var button = Page.Locator("nav button");
            var menuGroup = Page.Locator(".mud-collapse-container");

            var menuClasses = await menuGroup.GetAttributeAsync("class");
            var ariaValue = await menuGroup.GetAttributeAsync("aria-hidden");

            menuClasses.Should().NotBeNullOrEmpty();
            menuClasses.Should().Contain("mud-collapse-entering");
            ariaValue.Should().Be("false");

            await button.ClickAsync();

            menuClasses = await menuGroup.GetAttributeAsync("class");
            menuClasses.Should().NotContain("mud-collapse-entered").And.NotContain("mud-collapse-entering");

            ariaValue = await menuGroup.GetAttributeAsync("aria-hidden"); 
            ariaValue.Should().Be("true");

            await button.ClickAsync();

            menuClasses = await menuGroup.GetAttributeAsync("class");
            menuClasses.Should().Contain("mud-collapse-entered");

            ariaValue = await menuGroup.GetAttributeAsync("aria-hidden");
            ariaValue.Should().Be("false");
        }

        [Fact]
        public async Task Toggle_MudDrawer()
        {
            var url = typeof(NavMenuDrawerToggleTest).ToQueryString();

            await Page.GotoAsync(url);

            var leftToggle = Page.Locator("#static-left-toggle");
            var rightToggle = Page.Locator("#static-right-toggle");
            var miniDawer = Page.Locator("#static-mini");
            var persistentDawer = Page.Locator("#static-persistent");

            var miniClasses = await miniDawer.GetAttributeAsync("class");            
            miniClasses.Should().NotBeNullOrEmpty();
            miniClasses.Should().Contain("mud-drawer--closed");

            var persistentClasses = await persistentDawer.GetAttributeAsync("class");
            persistentClasses.Should().NotBeNullOrEmpty();
            persistentClasses.Should().Contain("mud-drawer--closed");

            await leftToggle.ClickAsync();
            miniClasses = await miniDawer.GetAttributeAsync("class");
            miniClasses.Should().Contain("mud-drawer--open");

            await rightToggle.ClickAsync();
            persistentClasses = await persistentDawer.GetAttributeAsync("class");
            persistentClasses.Should().Contain("mud-drawer--open");

            await rightToggle.ClickAsync();
            persistentClasses = await persistentDawer.GetAttributeAsync("class");
            persistentClasses.Should().Contain("mud-drawer--closed");

            await leftToggle.ClickAsync();
            miniClasses = await miniDawer.GetAttributeAsync("class");
            miniClasses.Should().Contain("mud-drawer--closed");
        }
    }
}
