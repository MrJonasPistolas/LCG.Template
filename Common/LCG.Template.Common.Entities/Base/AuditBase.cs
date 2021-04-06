using System;

namespace LCG.Template.Common.Entities.Base
{
    public class AuditBase : EntityBase
    {
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? LastModifiedOn { get; set; } = null;
        public string LastModifiedBy { get; set; } = string.Empty;
    }
}
