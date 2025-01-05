namespace Eleven.BookManager.App.Utils
{
    public class Debouncer
    {
        private CancellationTokenSource _cancelTokenSource = null!;

        public async Task Debounce(Func<Task> method, int milliseconds = 300)
        {
            _cancelTokenSource?.Cancel();
            _cancelTokenSource?.Dispose();
            _cancelTokenSource = new CancellationTokenSource();

            try
            {
                await Task.Delay(milliseconds, _cancelTokenSource.Token);
            }
            catch { }

            await method();
        }
    }
}
