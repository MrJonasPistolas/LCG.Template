using LCG.Template.Common.Enums.Auth;

namespace LCG.Template.Common.Models.Auth
{
    public class LoginResult
    {
        public bool Successful { get; set; }
        public string Error { get; set; }
        public LoginStatus Status { get; set; }
        public string Token { get; set; }
    }
}
