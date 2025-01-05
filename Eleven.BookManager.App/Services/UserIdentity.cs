using Eleven.BookManager.Repository.Contracts;

namespace Eleven.BookManager.App.Services
{
    public class UserIdentity : IUserIdentity
    {
        public string GetCurrentUser() => "admin";
    }
}
