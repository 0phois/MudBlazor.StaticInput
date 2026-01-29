<p align="center">
  <img alt="MudBlazor.StaticInput" src="content/logo.png" style="width: 8%"/>
  <h1 align="center">MudBlazor.StaticInput</h1>
</p>

![GitHub License](https://img.shields.io/github/license/0phois/Mudblazor.StaticInput?style=for-the-badge&logo=github&link=https%3A%2F%2Fgithub.com%2F0phois%2FMudBlazor.StaticInput%2Fblob%2Fmaster%2FLICENSE.txt)
![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/0phois/MudBlazor.StaticInput/build-test.yml?style=for-the-badge&logo=githubactions&label=Unit-Tests&link=https%3A%2F%2Fgithub.com%2F0phois%2FMudBlazor.StaticInput%2Factions%2Fworkflows%2Fbuild-test.yml)
![NuGet Version](https://img.shields.io/nuget/v/Extensions.MudBlazor.StaticInput?style=for-the-badge&logo=nuget&color=%23009688&link=https%3A%2F%2Fwww.nuget.org%2Fpackages%2FExtensions.MudBlazor.StaticInput)
![NuGet Downloads](https://img.shields.io/nuget/dt/Extensions.MudBlazor.StaticInput?style=for-the-badge&logo=nuget&&color=%2300796b&link=https%3A%2F%2Fwww.nuget.org%2Fpackages%2FExtensions.MudBlazor.StaticInput)



## :book: Introduction :book:
**MudBlazor.StaticInput** is an extension package for the [MudBlazor](https://github.com/MudBlazor/MudBlazor) library.  

Tailored specifically for [Static Server-Side Rendered](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/render-modes?view=aspnetcore-8.0#static-server-side-rendering-static-ssr) (static SSR) pages. It offers seamless integration for *some* of MudBlazor's Component design into your static application pages. Focusing particularly on components designed for forms and edit forms, in situations where [interactive](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/render-modes?view=aspnetcore-8.0#render-modes) components are not feasible. For example, when scaffolding [Blazor Identity UI](https://learn.microsoft.com/en-us/aspnet/core/blazor/security/server/?view=aspnetcore-8.0&tabs=visual-studio#blazor-identity-ui-individual-accounts) in your application.  

### [Live Demo Application](http://mudblazor-staticinput.tryasp.net/)
<a href="https://github.com/0phois/MudBlazor.StaticInput/tree/master/demo/StaticSample">
  <img alt="Static Input Demo" src="content/StaticInput.png" />
</a>

> [!TIP]
> Default email: `demo@static.com`  
> Default pword: `MudStatic123!`

## :thinking: Why MudBlazor.StaticInput? :thinking:
- **Rapid SSSR Integration:** Effortlessly add MudBlazor components to your static SSR pages, saving development time.
- **Focus on Forms:** Streamline development of forms and edit forms, for use cases such as Microsoft Identity Login forms.
- **Preserved Look & Feel:** Maintains the consistent design and user experience of MudBlazor. Ensuring uniformity across all pages.
- **Maintains Flexibility:** By inheriting core MudBlazor components, StaticInput maintains the same flexibility as the original components.


## :gift: What's Included :gift:
The set of components and features may extend over time. Currently, StaticInput components include:  
### MudStaticButton
<details>
  <summary>
    A Material Design button that supports form actions such as 'submit' and 'reset'
  </summary>  

```html  
<MudStaticButton Variant="Variant.Filled" Color="Color.Primary">Login</MudStaticButton>
```
</details>  

> Note: `<MudButton>` is 100% functional in forms when used correctly. The static component simply assists in assuring the correct usage. 

### MudStaticCheckBox
<details>
  <summary>
    Checkboxes are a great way to allow the user to make a selection of choices.
  </summary>

```html
<MudStaticCheckBox @bind-Value="@RememberMe" Color="Color.Success">Remember Me</MudStaticCheckBox>
```
```cs
@code{
    public bool RememberMe { get; set; }
}
```
</details>

### MudStaticSwitch
<details>
  <summary>
    Similar to a checkbox but visually different. The switch lets the user <i>switch</i> between two values with the tap of a button.
  </summary>  

```html
<MudStaticSwitch @bind-Value="@RememberMe" Color="Color.Success" UnCheckedColor="Color.Primary">Remember Me</MudStaticSwitch>
```
```cs
@code{
    public bool RememberMe { get; set; }
}
```
</details>  

### MudStaticTextField
<details>
  <summary>
    Text field components are used for receiving user provided information
  </summary>

```html
<MudStaticTextField @bind-Value="@Password" 
                    InputType="InputType.Password" 
                    Adornment="Adornment.End" 
                    AdornmentIcon="@Icons.Material.Outlined.VisibilityOff" 
                    AdornmentClickFunction="showPassword" />
```
```cs
@code {
    public string Password { get; set; }
}
```
```js
<script>
   let timeoutId;

   function showPassword(inputElement, button) {
       if (inputElement.type === 'password') {
           inputElement.type = 'text';
           clearTimeout(timeoutId);
           timeoutId = setTimeout(function () {
               inputElement.type = 'password';
           }, 5000);
       } else {
           inputElement.type = 'password';
           clearTimeout(timeoutId);
       }
   }
</script>
```
</details>  

### MudStaticNavDrawerToggle 
<details>
  <summary>
    Open/Close a MudDrawer using a MudIconButton. When added, Responsive drawers also open/close based on page size changes.
  </summary>

```html
<MudStaticNavDrawerToggle id="static-left-toggle" DrawerId="static-mini" Icon="@Icons.Material.Filled.Menu" Color="Color.Inherit" Edge="Edge.Start" />

<MudDrawer id="static-mini" Fixed="false" Elevation="1" Anchor="Anchor.Start" Variant="@DrawerVariant.Mini" ClipMode="DrawerClipMode.Always">
    <MudNavMenu>
        <MudNavLink Match="NavLinkMatch.All" Icon="@Icons.Material.Filled.Store">Store</MudNavLink>
        <MudNavLink Match="NavLinkMatch.All" Icon="@Icons.Material.Filled.LibraryBooks">Library</MudNavLink>
        <MudNavLink Match="NavLinkMatch.All" Icon="@Icons.Material.Filled.Group">Community</MudNavLink>
    </MudNavMenu>
</MudDrawer>
```
</details>

### MudStaticNavGroup 
<details>
  <summary>
    Collapse/Expand a MudStaticNavGroup by clicking on it's title. Can you nested in a standard MudNavMenu
  </summary>

```html
<MudNavMenu>
    <MudNavLink Href="/dashboard">Dashboard</MudNavLink>
    <MudNavLink Href="/servers">Servers</MudNavLink>
    <MudNavLink Href="/billing" Disabled="true">Billing</MudNavLink>

    <MudStaticNavGroup Title="Settings" Expanded="true">
        <MudNavLink Href="/users">Users</MudNavLink>
        <MudNavLink Href="/security">Security</MudNavLink>
    </MudStaticNavGroup>

    <MudNavLink Href="/about">About</MudNavLink>
</MudNavMenu>
```
</details>

### MudStaticSelect
<details>
  <summary>
    Select (dropdown) UI in SSR pages. Supports binding and form submission without requiring client-side interactivity.
  </summary>

```html
<MudStaticSelect T="string" @bind-Value="@SelectedValue" Label="Choose One">
    <MudStaticSelectItem Value="@("One")">Item One</MudStaticSelectItem>
    <MudStaticSelectItem Value="@("Two")">Item Two</MudStaticSelectItem>
    <MudStaticSelectItem Value="@("Three")">Item Three</MudStaticSelectItem>
</MudStaticSelect>
```
```cs
@code {
    private string SelectedValue { get; set; } = "Two";
}
```
</details>

## :rocket: Getting Started :rocket:
To start using MudBlazor.StaticInput in your projects, simply install the package via NuGet Package Manager:
```bash
dotnet add package Extensions.MudBlazor.StaticInput
```
> [!NOTE]  
> Note: MudBlazor should already be setup for your application
