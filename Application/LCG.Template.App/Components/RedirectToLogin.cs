using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Threading.Tasks;

namespace LCG.Template.App.Components
{
    public class RedirectToLogin : ComponentBase
    {
        [Inject]
        protected NavigationManager Navigation { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState> AuthenticationStateTask { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var authenticationState = await AuthenticationStateTask;

            if (authenticationState?.User?.Identity is null || !authenticationState.User.Identity.IsAuthenticated)
            {
                var returnUrl = Navigation.ToBaseRelativePath(Navigation.Uri);

                if (string.IsNullOrWhiteSpace(returnUrl))
                    Navigation.NavigateTo("/login", true);
                else
                    Navigation.NavigateTo($"/login?returnUrl={returnUrl}", true);
            }
        }
    }
}
