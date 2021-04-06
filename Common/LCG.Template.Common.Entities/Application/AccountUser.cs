using LCG.Template.Common.Data.Contracts;
using LCG.Template.Common.Entities.Base;

namespace LCG.Template.Common.Entities.Application
{
    public class AccountUser : EntityBase, IIdentifiableEntity<int>
    {
        public int AccountId { get; set; }
        public int UserId { get; set; }
        public int AccountUserTypeId { get; set; }
        public string AccountPreferences { get; set; }

        #region Members
        public virtual Account Account { get; set; }
        public virtual AccountUserType AccountUserType { get; set; }
        public virtual User User { get; set; }
        #endregion
    }
}
