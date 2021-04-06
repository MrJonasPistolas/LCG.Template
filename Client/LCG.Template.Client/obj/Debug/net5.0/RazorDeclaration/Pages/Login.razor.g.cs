// <auto-generated/>
#pragma warning disable 1591
#pragma warning disable 0414
#pragma warning disable 0649
#pragma warning disable 0169

namespace LCG.Template.Client.Pages
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Components;
#nullable restore
#line 1 "E:\src\private\LCG.Template\Client\LCG.Template.Client\_Imports.razor"
using System.Net.Http;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "E:\src\private\LCG.Template\Client\LCG.Template.Client\_Imports.razor"
using System.Net.Http.Json;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "E:\src\private\LCG.Template\Client\LCG.Template.Client\_Imports.razor"
using Microsoft.AspNetCore.Authorization;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "E:\src\private\LCG.Template\Client\LCG.Template.Client\_Imports.razor"
using Microsoft.AspNetCore.Components.Authorization;

#line default
#line hidden
#nullable disable
#nullable restore
#line 5 "E:\src\private\LCG.Template\Client\LCG.Template.Client\_Imports.razor"
using Microsoft.AspNetCore.Components.Forms;

#line default
#line hidden
#nullable disable
#nullable restore
#line 6 "E:\src\private\LCG.Template\Client\LCG.Template.Client\_Imports.razor"
using Microsoft.AspNetCore.Components.Routing;

#line default
#line hidden
#nullable disable
#nullable restore
#line 7 "E:\src\private\LCG.Template\Client\LCG.Template.Client\_Imports.razor"
using Microsoft.AspNetCore.Components.Web;

#line default
#line hidden
#nullable disable
#nullable restore
#line 8 "E:\src\private\LCG.Template\Client\LCG.Template.Client\_Imports.razor"
using Microsoft.AspNetCore.Components.Web.Virtualization;

#line default
#line hidden
#nullable disable
#nullable restore
#line 9 "E:\src\private\LCG.Template\Client\LCG.Template.Client\_Imports.razor"
using Microsoft.AspNetCore.Components.WebAssembly.Http;

#line default
#line hidden
#nullable disable
#nullable restore
#line 10 "E:\src\private\LCG.Template\Client\LCG.Template.Client\_Imports.razor"
using Microsoft.AspNetCore.Http;

#line default
#line hidden
#nullable disable
#nullable restore
#line 11 "E:\src\private\LCG.Template\Client\LCG.Template.Client\_Imports.razor"
using Microsoft.Extensions.Localization;

#line default
#line hidden
#nullable disable
#nullable restore
#line 12 "E:\src\private\LCG.Template\Client\LCG.Template.Client\_Imports.razor"
using Microsoft.JSInterop;

#line default
#line hidden
#nullable disable
#nullable restore
#line 13 "E:\src\private\LCG.Template\Client\LCG.Template.Client\_Imports.razor"
using LCG.Template.Client;

#line default
#line hidden
#nullable disable
#nullable restore
#line 14 "E:\src\private\LCG.Template\Client\LCG.Template.Client\_Imports.razor"
using LCG.Template.Client.Components;

#line default
#line hidden
#nullable disable
#nullable restore
#line 15 "E:\src\private\LCG.Template\Client\LCG.Template.Client\_Imports.razor"
using LCG.Template.Client.Services.Contracts;

#line default
#line hidden
#nullable disable
#nullable restore
#line 16 "E:\src\private\LCG.Template\Client\LCG.Template.Client\_Imports.razor"
using LCG.Template.Client.Shared;

#line default
#line hidden
#nullable disable
#nullable restore
#line 17 "E:\src\private\LCG.Template\Client\LCG.Template.Client\_Imports.razor"
using LCG.Template.Common.Models;

#line default
#line hidden
#nullable disable
#nullable restore
#line 6 "E:\src\private\LCG.Template\Client\LCG.Template.Client\Pages\Login.razor"
using LCG.Template.Common.Models.Account;

#line default
#line hidden
#nullable disable
    [Microsoft.AspNetCore.Components.RouteAttribute("/login")]
    public partial class Login : Microsoft.AspNetCore.Components.ComponentBase
    {
        #pragma warning disable 1998
        protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder __builder)
        {
        }
        #pragma warning restore 1998
#nullable restore
#line 72 "E:\src\private\LCG.Template\Client\LCG.Template.Client\Pages\Login.razor"
       
    private LoginModel loginModel = new LoginModel();
    private bool ShowErrors;
    private string Error = "";

    private async Task HandleLogin()
    {
        ShowErrors = false;

        var result = await AuthService.Login(loginModel);

        if (result.Successful)
        {
            Navigation.NavigateTo("/");
        }
        else
        {
            Error = result.Error;
            ShowErrors = true;
        }
    }

#line default
#line hidden
#nullable disable
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private IStringLocalizer<Login> localizer { get; set; }
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private NavigationManager Navigation { get; set; }
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private IAuthService AuthService { get; set; }
    }
}
#pragma warning restore 1591