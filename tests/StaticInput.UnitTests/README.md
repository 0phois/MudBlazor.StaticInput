## :zap: Test Strategy :zap:
**StaticInput.UnitTests** utilizes BUnit and Playwright in order to test the input components.  
 - [BUnit](https://bunit.dev/index.html) is used to ensure that components are rendered as expected.
 - [Playwright](https://playwright.dev/dotnet/) is used to ensure the components function as expected.  

## :link: Dependencies :link:
**StaticInput.UnitTests.Viewer**: In order for the functional (end-to-end) tests to be executed, a reference to the StaticInput.UnitTests.Viewer is required. This library is used to setup various scenarios to be used in the automated testing. It can also be executed as a standalone project in order to visually review the behavior of the components under test. 
## :wrench: Setup :wrench:
Ensure PowerShell is installed and up-to-date:
```bash
dotnet tool update --global PowerShell
```

Install Playwright:
```bash
pwsh ./tests/StaticInput.UnitTests/bin/Debug/net8.0/playwright.ps1 install
```