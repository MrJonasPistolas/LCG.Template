using LCG.Template.Common.Entities.Application;
using LCG.Template.Common.Entities.Logging;
using LCG.Template.Common.Enums.Entities;
using LCG.Template.Common.Extensions.Collections;
using LCG.Template.Common.Extensions.PrimitiveTypes;
using LCG.Template.Common.Tools.Utils;
using LCG.Template.Data.Application;
using LCG.Template.Data.Application.Repositories;
using LCG.Template.Resources.Application;
using LCG.Template.ServiceContracts;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace LCG.Template.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IUrlUtils _urlUtils;

        private string SmtpHost { get; set; }
        private string SmtpFrom { get; set; }
        private string SmtpPassword { get; set; }
        private int SmtpPort { get; set; }
        private bool EmailNotificationEnable { get; set; }
        private string OverrideTo { get; set; }
        private string OverrideCc { get; set; }
        private bool SmtpSsl { get; set; }

        #region Constructor

        public EmailService(ApplicationDbContext applicationDbContext, IConfiguration configuration, IUrlUtils urlUtils)
        {
            _applicationDbContext = applicationDbContext;
            _configuration = configuration;
            _urlUtils = urlUtils;
        }
        #endregion

        public string SendInvitedUserTemplate(int languageId, string to, string cc, string emailConfirmationToken)
        {
            EmailTemplate template = GetEmailTemplate(languageId, (int)EmailTemplateNames.InvitedUser);

            if (template != null)
            {
                var queryDictionary = new Dictionary<string, string>(2);
                queryDictionary.Add("confirmationToken", emailConfirmationToken);
                queryDictionary.Add("email", to);
                var parameters = new
                {
                    ConfirmUserLink = _urlUtils.GenerateHostPathQueryStringUrl(queryDictionary, "user/confirmnewuser").ToString()
                };

                return SendEmail(to, cc, template.Subject, template.Body.Template(parameters));
            }

            return EmailMessages.EmailTemplateEmptyTemplate;
        }

        public string SendPasswordResetTemplate(int languageId, string to, string cc, string passwordConfirmationToken)
        {
            EmailTemplate template = GetEmailTemplate(languageId, (int)EmailTemplateNames.PasswordReset);

            if(template != null)
            {
                var queryDictionary = new Dictionary<string, string>(2);
                queryDictionary.Add("confirmationToken", passwordConfirmationToken);
                queryDictionary.Add("email", to);
                var parameters = new
                {
                    PasswordResetLink = _urlUtils.GenerateHostPathQueryStringUrl(queryDictionary, "user/passwordreset").ToString()
                };

                return SendEmail(to, cc, template.Subject, template.Body.Template(parameters));
            }

            return EmailMessages.EmailTemplateEmptyTemplate;
        }



        private string SendEmail(string to, string cc, string subject, string body)
        {
            StringBuilder retMessages = new StringBuilder();
            NotificationLog log = new NotificationLog(to, cc, subject, body);

            try
            {
                LoadConfigurationsParseParams(retMessages, to);

                if (string.IsNullOrEmpty(retMessages.ToString()) && EmailNotificationEnable)
                {
                    SmtpClient client = new SmtpClient(SmtpHost)
                    {
                        UseDefaultCredentials = false,
                        Port = SmtpPort,
                        Credentials = new NetworkCredential(SmtpFrom, SmtpPassword),
                        EnableSsl = SmtpSsl

                    };

                    MailMessage mailMessage = new MailMessage
                    {
                        From = new MailAddress(SmtpFrom)
                    };

                    mailMessage.Subject = subject;
                    mailMessage.Body = body;
                    mailMessage.IsBodyHtml = true;

                    if (!string.IsNullOrEmpty(OverrideTo))
                    {
                        SetOverrideEmails(to, OverrideTo, mailMessage, true);
                        SetOverrideEmails(cc, OverrideCc, mailMessage, false);
                    }
                    else
                    {
                        SetEmailRecipients(to, mailMessage, true);
                        SetEmailRecipients(cc, mailMessage, false);
                    }

                    client.Send(mailMessage);
                    retMessages.Append(EmailMessages.EmailTemplateEmailSendingSuccessfully);
                    log.WasSent = true;
                }
                else
                {
                    if (!EmailNotificationEnable)
                    {
                        retMessages.Append(EmailMessages.EmailTemplateEmailSendingOff);
                    }
                    log.Errors = retMessages.ToString();
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    retMessages.Append(ex.InnerException.Message);
                }
                else
                {
                    retMessages.Append(ex.Message);
                }
                log.Errors = retMessages.ToString();
                log.ErrorStackTrace = ex.StackTrace;
            }

            log.SentOn = DateTime.Now;
            this.LogNotification(log);

            return retMessages.ToString();
        }

        private string SendEmailWithAttachments(string to, string cc, string subject, string body, List<string> filePaths, bool isBodyHtml = true)
        {
            StringBuilder retMessages = new StringBuilder();
            NotificationLog log = new NotificationLog(to, cc, subject, body);

            try
            {
                LoadConfigurationsParseParams(retMessages, to);

                if (string.IsNullOrEmpty(retMessages.ToString()) && EmailNotificationEnable)
                {

                    SmtpClient client = new SmtpClient(SmtpHost)
                    {
                        UseDefaultCredentials = false,
                        Port = SmtpPort,
                        Credentials = new NetworkCredential(SmtpFrom, SmtpPassword),
                        EnableSsl = SmtpSsl

                    };

                    MailMessage mailMessage = new MailMessage
                    {
                        From = new MailAddress(SmtpFrom)
                    };

                    mailMessage.Subject = subject;
                    mailMessage.Body = body;
                    mailMessage.IsBodyHtml = isBodyHtml;

                    List<Attachment> attachments = new List<Attachment>();

                    filePaths.ForEach(x => attachments.Add(new Attachment(x)));

                    mailMessage.Attachments.AddRange(attachments);

                    if (!string.IsNullOrEmpty(OverrideTo))
                    {
                        SetOverrideEmails(to, OverrideTo, mailMessage, true);
                        SetOverrideEmails(cc, OverrideCc, mailMessage, false);
                    }
                    else
                    {
                        SetEmailRecipients(to, mailMessage, true);
                        SetEmailRecipients(cc, mailMessage, false);
                    }

                    client.Send(mailMessage);
                    retMessages.Append(EmailMessages.EmailTemplateEmailSendingSuccessfully);
                    log.WasSent = true;
                }
                else
                {
                    if (!EmailNotificationEnable)
                    {
                        retMessages.Append(EmailMessages.EmailTemplateEmailSendingOff);
                    }
                    log.Errors = retMessages.ToString();
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    retMessages.Append(ex.InnerException.Message);
                }
                else
                {
                    retMessages.Append(ex.Message);
                }
                log.Errors = retMessages.ToString();
                log.ErrorStackTrace = ex.StackTrace;
            }

            log.SentOn = DateTime.Now;
            this.LogNotification(log);

            return retMessages.ToString();
        }

        private EmailTemplate GetEmailTemplate(int languageId, int emailId)
        {
            var emailTemplateRepository = new EmailTemplateRepository(_applicationDbContext);
            return emailTemplateRepository.GetActiveEmailTemplateByLanguageAndName(languageId, emailId).FirstOrDefault();
        }


        private void LoadConfigurationsParseParams(StringBuilder retMessages, string to)
        {
            if (_configuration["EmailConfiguration:SmtpHost"] == null || string.IsNullOrEmpty(_configuration["EmailConfiguration:SmtpHost"]))
                retMessages.Append(EmailMessages.EmailConfigurationKeyHostEmpty);
            else
                SmtpHost = _configuration["EmailConfiguration:SmtpHost"];

            if (_configuration["EmailConfiguration:SmtpFrom"] == null || string.IsNullOrEmpty(_configuration["EmailConfiguration:SmtpFrom"]))
                retMessages.Append(EmailMessages.EmailConfigurationKeyFromEmpty);
            else
                SmtpFrom = _configuration["EmailConfiguration:SmtpFrom"];

            if (_configuration["EmailConfiguration:SmtpPort"] == null || string.IsNullOrEmpty(_configuration["EmailConfiguration:SmtpPort"]))
                retMessages.Append(EmailMessages.EmailConfigurationKeyFromEmpty);
            else
                SmtpPort = int.Parse(_configuration["EmailConfiguration:SmtpPort"]);

            if (_configuration["EmailConfiguration:SmtpPassWord"] == null || string.IsNullOrEmpty(_configuration["EmailConfiguration:SmtpPassWord"]))
                retMessages.Append(EmailMessages.EmailConfigurationKeyPasswordEmpty);
            else
                SmtpPassword = _configuration["EmailConfiguration:SmtpPassWord"];

            if (_configuration["EmailConfiguration:SmtpSsl"] == null || string.IsNullOrEmpty(_configuration["EmailConfiguration:SmtpSsl"]))
                retMessages.Append(EmailMessages.EmailConfigurationKeyPasswordEmpty);
            else
                SmtpSsl = bool.Parse(_configuration["EmailConfiguration:SmtpSsl"]);

            if (string.IsNullOrEmpty(to))
                retMessages.Append(EmailMessages.EmailTemplateEmptyRecipients);

            EmailNotificationEnable = _configuration["EmailConfiguration:EmailNotificationEnabled"] != null ? Convert.ToBoolean(_configuration["EmailConfiguration:EmailNotificationEnabled"]) : true;

            OverrideTo = _configuration["EmailConfiguration:EmailNotificationOverrideTo"] ?? string.Empty;
            OverrideCc = _configuration["EmailConfiguration:EmailNotificationOverrideCc"] ?? string.Empty;
        }


        


        private void SetOverrideEmails(string recipients, string overrideEmails, MailMessage mailMessage, bool isForTo)
        {
            if (!string.IsNullOrEmpty(recipients))
            {
                StringBuilder sbHtmlToAdd = new StringBuilder("<table>");

                foreach (string email in GetEmailRecipients(recipients))
                    sbHtmlToAdd.AppendFormat("<tr><td>{0};</td></tr>", email);

                sbHtmlToAdd.Append("</table>");
                mailMessage.Body = string.Format("{0} <br> Lista {1} : {2}", mailMessage.Body, isForTo ? "To" : "Cc", sbHtmlToAdd.ToString());
            }

            SetEmailRecipients(overrideEmails, mailMessage, isForTo);
        }

        private string[] GetEmailRecipients(string recipients)
        {
            if (recipients.Contains(";"))
                return recipients.Split(";");

            if (recipients.Contains(","))
                return recipients.Split(",");

            return new string[] { recipients };
        }

        private void SetEmailRecipients(string recipients, MailMessage mailMessage, bool isForTo)
        {
            if (!string.IsNullOrEmpty(recipients))
            {
                if (isForTo)
                {
                    foreach (string address in GetEmailRecipients(recipients))
                        mailMessage.To.Add(new MailAddress(address, address));
                }
                else
                {
                    foreach (string address in GetEmailRecipients(recipients))
                        mailMessage.CC.Add(new MailAddress(address, address));
                }
            }
        }

        private void LogNotification(NotificationLog log)
        {
            NotificationLogRepository repository = new NotificationLogRepository(_applicationDbContext);
            repository.Add(log);
        }
    }
}
