using Microsoft.Playwright;

namespace StaticInput.UnitTests.Fixtures
{
    public sealed class ContextFixture(BrowserFixture browserFixture) : IAsyncLifetime
    {
        private readonly IBrowser? _browser = browserFixture.Browser;
        private IBrowserContext? _context;

        public async Task InitializeAsync()
        {
            _context = await _browser!.NewContextAsync(new()
            {
                BaseURL = "http://localhost:5220/",
            });
        }

        public Task<IPage> NewPageAsync()
        {
            return _context!.NewPageAsync();
        }

        public async Task DisposeAsync()
        {
            await _context!.DisposeAsync();
        }
    }
}
