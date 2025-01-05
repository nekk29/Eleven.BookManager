namespace Eleven.BookManager.Business.Contracts.Utils
{
    public interface IEncrypter
    {
        string Encrypt(string key, string text);
        string Decrypt(string key, string encryptedText);
    }
}
