using LCG.Template.Common.Data.Contracts;
using System;

namespace LCG.Template.Common.Entities.Logging
{
    public class NotificationLog : IIdentifiableEntity<int>
    {
        public NotificationLog()
        {
        }

        public NotificationLog(string to, string cc, string subject, string email)
        {
            To = to;
            CC = cc;
            Subject = subject;
            Email = email;
        }

        #region Properties
        public int Id { get; set; }
        public string To { get; set; }
        public string CC { get; set; }
        public string Subject { get; set; }
        public string Email { get; set; }
        public DateTime SentOn { get; set; }
        public bool WasSent { get; set; }
        public string Errors { get; set; }
        public string ErrorStackTrace { get; set; }
        #endregion
    }
}
