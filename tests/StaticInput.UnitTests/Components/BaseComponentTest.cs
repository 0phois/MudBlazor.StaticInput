using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using MudBlazor.Services;
using StaticInput.UnitTests.Fixtures;

namespace StaticInput.UnitTests.Components
{
    [Collection("Browser collection")]
    public abstract class BaseComponentTest : IClassFixture<ContextFixture>, IAsyncLifetime
    {
        public IPage Page { get; private set; } = null!;
        protected BunitContext Context { get; private set; }
        private ContextFixture ContextFixture { get; set; }

        protected BaseComponentTest(ContextFixture contextFixture)
        {
            Context = new();
            ContextFixture = contextFixture;
            Context.Services.AddSingleton(TimeProvider.System);
            Context.Services.AddMudBlazorKeyInterceptor();
            Context.Services.AddMudLocalization();
            Context.JSInterop.Mode = JSRuntimeMode.Loose;
            Context.Renderer.SetRendererInfo(new RendererInfo("Static", false));
        }

        public async Task InitializeAsync()
        {
            Page = await ContextFixture.NewPageAsync().ConfigureAwait(false);
            // Go to base URL so we have access to localStorage for that domain
            await Page.GotoAsync("/").ConfigureAwait(false);
            await Page.EvaluateAsync("localStorage.clear()").ConfigureAwait(false);
        }

        public async Task DisposeAsync()
        {
            try
            {
                if (Page != null)
                    await Page.CloseAsync().ConfigureAwait(false);
            }
            finally
            {
                await Context.DisposeAsync();
            }
        }
    }
}
