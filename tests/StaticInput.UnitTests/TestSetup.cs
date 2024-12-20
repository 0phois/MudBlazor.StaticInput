using System.Diagnostics;
using Xunit.Abstractions;
using Xunit.Sdk;

[assembly: TestFramework("StaticInput.UnitTests.TestSetup", "StaticInput.UnitTests")]
namespace StaticInput.UnitTests
{
    public sealed class TestSetup : XunitTestFramework, IDisposable
    {
        private readonly Process? _testViewerProcess;

        public TestSetup(IMessageSink messageSink) : base(messageSink)
        {
            var processes = Process.GetProcessesByName("StaticInput.UnitTests.Viewer");

            foreach (var process in processes)
            {
                process.Kill();
            }

            var startInfo = new ProcessStartInfo()
            {
                FileName = "dotnet",
                Arguments = $"run --project {GetProjectPath()}"
            };

            startInfo.EnvironmentVariables.Add("TEST_ENVIRONMENT", "true");


            _testViewerProcess = Process.Start(startInfo);

            messageSink.OnMessage(new DiagnosticMessage($"Started {_testViewerProcess?.ProcessName} with ID: {_testViewerProcess?.Id}"));
        }

        private static string GetProjectPath()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var index = currentDirectory.IndexOf("tests") + 5;
            var testDirectory = currentDirectory[..index];

            return Path.GetRelativePath(currentDirectory, Path.Combine(testDirectory, "StaticInput.UnitTests.Viewer"));
        }

        public new void Dispose()
        {
            _testViewerProcess?.Kill();
            _testViewerProcess?.Dispose();

            base.Dispose();
        }
    }
}
