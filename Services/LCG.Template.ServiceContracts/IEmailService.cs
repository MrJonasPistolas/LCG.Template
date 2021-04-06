namespace LCG.Template.ServiceContracts
{
    public interface IEmailService
    {
        public string SendInvitedUserTemplate(int languageId, string to, string cc, string emailConfirmationToken);
        public string SendPasswordResetTemplate(int languageId, string to, string cc, string passwordConfirmationToken);
    }
}
