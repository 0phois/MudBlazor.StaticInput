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

            var button = Page.GetByLabel("Toggle Settings");
            var menuGroup = Page.GetByLabel("Settings", new() { Exact = true })
                                .Locator("div")
                                .Filter(new() { HasText = "Users Security" })
                                .First;

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
        
        [Fact]
        public async Task MudDrawerTemporary_ClickOutside_Closed()
        {
            var url = typeof(NavMenuDrawerToggleNeverTest).ToQueryString();
            await Page.GotoAsync(url);

            var leftToggle = Page.Locator("#static-left-toggle");
            var temporaryDrawer = Page.Locator("#static-temporary");
            
            var drawerBox = await temporaryDrawer.BoundingBoxAsync();
            drawerBox.Should().NotBeNull();

            var temporaryClasses = await temporaryDrawer.GetAttributeAsync("class");
            temporaryClasses.Should().NotBeNullOrEmpty();
            temporaryClasses.Should().Contain("mud-drawer--closed");

            await leftToggle.ClickAsync();
            temporaryClasses = await temporaryDrawer.GetAttributeAsync("class");
            temporaryClasses.Should().Contain("mud-drawer--open");

            await temporaryDrawer.ClickAsync();
            temporaryClasses = await temporaryDrawer.GetAttributeAsync("class");
            temporaryClasses.Should().Contain("mud-drawer--open");

            await Page.Mouse.ClickAsync(drawerBox!.Width+100, 10);

            temporaryClasses = await temporaryDrawer.GetAttributeAsync("class");
            temporaryClasses.Should().Contain("mud-drawer--closed");

            await leftToggle.ClickAsync();
            temporaryClasses = await temporaryDrawer.GetAttributeAsync("class");
            temporaryClasses.Should().Contain("mud-drawer--open");
        }
    }
}
