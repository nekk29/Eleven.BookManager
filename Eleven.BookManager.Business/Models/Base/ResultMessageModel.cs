namespace Eleven.BookManager.Business.Models.Base
{
    public class ResultMessageModel
    {
        public string? Code { get; set; }
        public string? Message { get; set; }
        public ResultMessageType MessageType { get; set; }
    }
}
