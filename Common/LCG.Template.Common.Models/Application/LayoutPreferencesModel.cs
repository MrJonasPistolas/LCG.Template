using Newtonsoft.Json;

namespace LCG.Template.Common.Models.Application
{
    public class LayoutPreferencesModel
    {
        [JsonProperty("subMenuDropDownIconThemeId")]
        public int SubMenuDropDownIconThemeId { get; set; }
        [JsonProperty("headerBrandThemeId")]
        public int HeaderBrandThemeId { get; set; }
        [JsonProperty("headerThemeId")]
        public int HeaderThemeId { get; set; }
        [JsonProperty("activeLinkThemeId")]
        public int ActiveLinkThemeId { get; set; }
        [JsonProperty("menuCaptionThemeId")]
        public int MenuCaptionThemeId { get; set; }
    }
}
