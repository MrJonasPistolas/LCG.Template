namespace LCG.Template.ServiceContracts
{
    public interface ICypherService
    {
        public string Encrypt(string text);
        public string Decrypt(string encryptedText);
    }
}
