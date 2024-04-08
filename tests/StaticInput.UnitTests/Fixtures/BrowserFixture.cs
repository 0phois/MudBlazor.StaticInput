using Microsoft.Playwright;

namespace StaticInput.UnitTests.Fixtures
{
    public sealed class BrowserFixture : IAsyncLifetime
    {
        public IBrowser? Browser { get; private set; }

        private IPlaywright _playwright = null!;

        public async Task InitializeAsync()
        {
            _playwright = await Playwright.CreateAsync();

            _playwright.Selectors.SetTestIdAttribute("id");

            await Task.Delay(TimeSpan.FromSeconds(15));

            Browser = await _playwright.Chromium.LaunchAsync(new() { Headless = true });
        }

        public async Task DisposeAsync()
        {
            await Browser!.DisposeAsync();

            _playwright!.Dispose();
        }
    }

    [CollectionDefinition("Browser collection")]
    public class BrowserCollection : ICollectionFixture<BrowserFixture> { }
}
