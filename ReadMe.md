# About FlashMessage

FlashMessage is a simple container for rendering flash messages.

To use this, create a FlashMessage object, add messages to the object, and then store that in a session.  When returning, if there are messages, those can be rendered.

FlashMessage is designed for use with Boostrap 3.4.1+ or Toastr.  You must include and activate these dependencies separately.  FlashMessage expects them to be there.

## Setting Up FlashMessage

### Installing

You can grab the package from https://www.nuget.org/packages/FlashMessage/ if you want to install using NuGet in your C# Project.

### Update \_ViewImports.cshtml for Tag Helper

Your \_ViewImports.cshtml file must contain the following for the Razor Page to be able to replace the <flash></flash> tags.

```
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
@addTagHelper *, FlashMessage
```

### Add Toastr or Bootstrap

After the NuGet Package is installed, make sure you have a reference to either Boostrap 3.4.1+ or Toastr in your HTML depending on which you prefer to use.  You must have JQuery available for this to work, and that must be loaded prior to loading Toastr.

The \_Layout.cshtml head section should contain the following:

```
<link href="~/css/toastr.min.css" rel="stylesheet" type="text/css" />
<script src="~/lib/jquery/dist/jquery.min.js"></script>
<script src="~/js/toastr.min.js"></script>
```

You are recommended to have both Toastr and Bootstrap in your project layout pages, so you will have the freedom to use both on your pages.

### Register Session in your Program.cs

Using the Session is necessary for FlashMessage.  The session will always use the term "FlashMessages" as the key.

Add the service scope for dependency injection while configuring the builder.

```
builder.Services.AddDistributedMemoryCache(); // Required for session state.
builder.Services.AddRazorPages();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor(); // Required for FlashMessage to access the session.
builder.Services.AddScoped<IFlash, Flash>();

var app = builder.Build();
```

In your Program.cs, you must make sure that UseSession is put into place before the routing; otherwise, you will not have access to HttpContext.Session in your Razor Pages in time.

```
app.UseStaticFiles();
*app.UseSession();*
app.UseRouting();
```

### Set your using statement

Put the appropriate using statement in your code.  See the basic examples for actual usage examples.

```
using FlashMessage;
```

## Simple Example Using FlashMessage with Bootstrap Alerts

## Using TagHelper To Render in Razor Pages
### Dependency Injection into Razor Page
```
private readonly ILogger<IndexModel> _logger;
private readonly IFlash _flash;
public string Testing {get;set;} = string.Empty;

public IndexModel(ILogger<IndexModel> logger, IFlash flash)
{
    _logger = logger;
    _flash = flash;
    // In this example, we are using Bootstrap alerts, but you can choose Toastr instead.
    _flash.MessageFlashType = FlashMessage.Enums.FlashType.BootstrapAlert;
}
```

### Set Flash Messages During Errors or Success
```
public void OnGet()
{
    _flash.Error("This is an error message test.");
    _flash.Success("This is a success message test.");
    _flash.Warning("This is warning message test.");
    _flash.SaveSession();
}
```

### Render All Flashes in Markup
```
<flash></flash>
```

### Render Flashes of a Specific Type in Markup

```
<flash type="error"></flash>
<flash type="success"></flash>
<flash type="warning"></flash>
<flash type="info"></flash>
```

