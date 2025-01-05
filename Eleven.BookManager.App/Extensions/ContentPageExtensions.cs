namespace Eleven.BookManager.App.Extensions
{
    public static class ContentPageExtensions
    {
        public static void Execute(this ContentPage contentPage, Func<Task> func, Action onStart = null!, Action onEnd = null!)
        {
            Task.Run(async () =>
            {
                onStart?.Invoke();
                contentPage.BatchBegin();
                await func();
                contentPage.BatchCommit();
                onEnd?.Invoke();
            });
        }
    }
}
