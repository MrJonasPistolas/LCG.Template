using Newtonsoft.Json;

namespace LCG.Template.Common.Models.Account
{
    public class AccountUserDeleteModel
    {
        [JsonProperty("userId")]
        public int UserId { get; set; }
        [JsonProperty("accountUserId")]
        public int AccountUserId { get; set; }
    }
}
