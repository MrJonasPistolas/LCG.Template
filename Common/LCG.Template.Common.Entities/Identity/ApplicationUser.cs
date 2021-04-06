using LCG.Template.Common.Data.Contracts;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace LCG.Template.Common.Entities.Identity
{
    public class ApplicationUser : IdentityUser, IIdentifiableEntity<string>
    {
        [Column(Order = 0)]
        public int ApplicationUserId { get; set; }
    }
}
