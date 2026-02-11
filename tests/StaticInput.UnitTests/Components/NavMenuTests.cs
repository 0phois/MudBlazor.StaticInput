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
        public async Task Toggle_MudDrawer_Persists_On_Reload()
        {
            var url = typeof(NavMenuDrawerToggleTest).ToQueryString();

            await Page.GotoAsync(url);

            var leftToggle = Page.Locator("#static-left-toggle");
            var miniDawer = Page.Locator("#static-mini");

            // Initial state: closed
            var miniClasses = await miniDawer.GetAttributeAsync("class");
            miniClasses.Should().Contain("mud-drawer--closed");

            // Toggle to open
            await leftToggle.ClickAsync();
            miniClasses = await miniDawer.GetAttributeAsync("class");
            miniClasses.Should().Contain("mud-drawer--open");

            // Reload page
            await Page.ReloadAsync();

            // Should still be open
            miniDawer = Page.Locator("#static-mini");
            await Page.WaitForFunctionAsync("id => document.getElementById(id).classList.contains('mud-drawer--open')", "static-mini");
            miniClasses = await miniDawer.GetAttributeAsync("class");
            miniClasses.Should().Contain("mud-drawer--open");

            // Toggle back to closed
            leftToggle = Page.Locator("#static-left-toggle");
            await leftToggle.ClickAsync();
            miniClasses = await miniDawer.GetAttributeAsync("class");
            miniClasses.Should().Contain("mud-drawer--closed");

            // Reload again
            await Page.ReloadAsync();
            miniDawer = Page.Locator("#static-mini");
            await Page.WaitForFunctionAsync("id => document.getElementById(id).classList.contains('mud-drawer--closed')", "static-mini");
            miniClasses = await miniDawer.GetAttributeAsync("class");
            miniClasses.Should().Contain("mud-drawer--closed");
        }

        [Fact]
        public async Task Multiple_Drawers_Persist_Independently()
        {
            var url = typeof(NavMenuDrawerToggleTest).ToQueryString();

            await Page.GotoAsync(url);

            var leftToggle = Page.Locator("#static-left-toggle");
            var rightToggle = Page.Locator("#static-right-toggle");
            var miniDawer = Page.Locator("#static-mini");
            var persistentDawer = Page.Locator("#static-persistent");

            // Open both
            await leftToggle.ClickAsync();
            await rightToggle.ClickAsync();

            await Page.WaitForFunctionAsync("id => document.getElementById(id).classList.contains('mud-drawer--open')", "static-mini");
            await Page.WaitForFunctionAsync("id => document.getElementById(id).classList.contains('mud-drawer--open')", "static-persistent");

            // Reload
            await Page.ReloadAsync();

            // Both should be open
            await Page.WaitForFunctionAsync("id => document.getElementById(id).classList.contains('mud-drawer--open')", "static-mini");
            await Page.WaitForFunctionAsync("id => document.getElementById(id).classList.contains('mud-drawer--open')", "static-persistent");

            // Close one
            leftToggle = Page.Locator("#static-left-toggle");
            await leftToggle.ClickAsync();
            await Page.WaitForFunctionAsync("id => document.getElementById(id).classList.contains('mud-drawer--closed')", "static-mini");

            // Reload
            await Page.ReloadAsync();

            // One closed, one open
            await Page.WaitForFunctionAsync("id => document.getElementById(id).classList.contains('mud-drawer--closed')", "static-mini");
            await Page.WaitForFunctionAsync("id => document.getElementById(id).classList.contains('mud-drawer--open')", "static-persistent");
        }
    }
}
