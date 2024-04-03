using Bunit;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;

namespace StaticInput.UnitTests.Components
{
    public abstract class BunitTest : IDisposable
    {
        protected Bunit.TestContext Context { get; private set; }

        protected BunitTest()
        {
            Context = new();
            Context.JSInterop.Mode = JSRuntimeMode.Loose;
            Context.Services.AddMudBlazorKeyInterceptor();
        }

        public void Dispose()
        {
            try
            {
                Context.Dispose();
            }
            catch (Exception) { }
        }
    }
}
