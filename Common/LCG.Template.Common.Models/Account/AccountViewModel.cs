namespace LCG.Template.Common.Models.Account
{
    public class AccountViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string AccountOwner { get; set; }
        public int TypeId { get; set; }
        public string Type { get; set; }
        public string ImageUrl { get; set; }
        public bool IsAccountUserActive { get; set; }
        public string Preferences { get; set; }
        public int LanguageId { get; set; }
        public int AccountUserId { get; set; }
        public bool IsAccountActive { get; set; }
    }
}
