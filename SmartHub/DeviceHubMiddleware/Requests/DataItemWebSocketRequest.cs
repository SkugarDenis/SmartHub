using SmartHub.DataContext.DbModels;

namespace SmartHub.DeviceHubMiddleware.Requests
{
    public class DataItemWebSocketRequest
    {
        public DataType Type { get; set; }
        public string data { get; set; }
    }
}
