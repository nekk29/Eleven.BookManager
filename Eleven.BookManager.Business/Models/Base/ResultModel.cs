namespace Eleven.BookManager.Business.Models.Base
{
    public class ResultModel
    {
        public IEnumerable<ResultMessageModel> Messages { get; set; } = [];
        public bool IsValid => Messages.All(x => x.MessageType != ResultMessageType.Error);

        public ResultModel()
            => Messages = [];

        public ResultModel(IEnumerable<ResultMessageModel> messages)
            => Messages = messages ?? throw new ArgumentNullException(nameof(messages));

        public ResultModel(string message) : this()
            => AddOkResult(message);

        public ResultModel(Exception exception) : this()
            => AddErrorResult(exception);

        public void AddOkResult(string message)
            => AddResult(ResultMessageType.Ok, message);

        public void AddInfoResult(string message)
            => AddResult(ResultMessageType.Info, message);

        public void AddWarningResult(string message)
            => AddResult(ResultMessageType.Warning, message);

        public void AddErrorResult(string message)
            => AddResult(ResultMessageType.Error, message);

        public void AddErrorResult(Exception exception)
            => AddResult(ResultMessageType.Error, GetExceptionMessage(exception));

        public void AddErrorResult(string message, Exception exception)
            => AddResult(ResultMessageType.Error, string.Concat(message, Environment.NewLine, GetExceptionMessage(exception)));

        private static string GetExceptionMessage(Exception exception)
            => string.Concat(exception.Message, Environment.NewLine, exception.StackTrace);

        public void AddResult(ResultMessageType apiResultType, string message)
        {
            if (string.IsNullOrEmpty(message)) throw new ArgumentNullException(nameof(message));

            var messages = Messages.ToList();

            messages.Add(new ResultMessageModel { MessageType = apiResultType, Message = message });

            Messages = messages;
        }

        public void AttachResults(ResultModel result)
        {
            if (result == null) return;

            var messages = Messages.ToList();

            result.Messages.ToList().ForEach(result => messages.Add(result));

            Messages = messages;
        }

        public void AttachResultsWithType(ResultMessageType apiResultType, ResultModel result)
        {
            var messages = Messages.ToList();

            result.Messages.ToList().ForEach(result => messages.Add(new ResultMessageModel { MessageType = apiResultType, Message = result.Message }));

            Messages = messages;
        }

        public string GetFormattedMessages(bool exclamation = false)
        {
            if (Messages.Any())
            {
                if (Messages.Count() == 1)
                    return exclamation ? $"¡ {Messages.FirstOrDefault()?.Message!} !" : Messages.FirstOrDefault()?.Message!;
                else
                    return string.Join(Environment.NewLine, Messages.Select(x => $"{Enum.GetName(x.MessageType)}: {x.Message}"));
            }

            return string.Empty;
        }
    }

    public class ResultModel<T> : ResultModel
    {
        public T? Data { get; set; }

        public ResultModel() : base()
        {

        }

        public ResultModel(T data) : base()
            => Data = data;

        public ResultModel(T data, IEnumerable<ResultMessageModel> messages) : base(messages)
            => Data = data;

        public ResultModel(T data, string message) : base(message)
            => Data = data;

        public ResultModel(Exception ex) : base(ex)
        {

        }

        public void UpdateData(T data)
            => Data = data;
    }
}
