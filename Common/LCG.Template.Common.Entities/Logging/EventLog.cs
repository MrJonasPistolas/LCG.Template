using LCG.Template.Common.Data.Contracts;
using System;

namespace LCG.Template.Common.Entities.Logging
{
    public partial class EventLog : IIdentifiableEntity<int>
    {
        public int Id { get; set; }
        public bool SystemLog { get; set; }
        public int? EventId { get; set; }
        public int? LogLevelId { get; set; }
        public string LogLevel { get; set; }
        public string Message { get; set; }
        public string CategoryName { get; set; }
        public DateTime? CreatedTime { get; set; }
        public string StackTrace { get; set; }
        public int? AccountId { get; set; }
    }
}
