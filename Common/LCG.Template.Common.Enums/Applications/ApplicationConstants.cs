namespace LCG.Template.Common.Enums.Applications
{
    public class ApplicationRoles
    {
        public const string Admin = "Admin";
        public const string AccountOwner = "Account Owner";
        public const string AccountUser = "Account User";
        public const string AdminAndAccountOwner = "Admin, Account Owner";
    }

    public class Token
    {
        public class Claims
        {
            public const string UserId = "id";
        }
    }

    public class Defaults
    {
        public class Names
        {
            public const string AdministratorName = "Administrator";
        }
        public class Images
        {
            public const string UserUrl = "/images/users/0.jpg";
        }
    }
}
