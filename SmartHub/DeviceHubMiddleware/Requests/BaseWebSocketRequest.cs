using SmartHub.DataContext.DbModels;
using SmartHub.DeviceHubMiddleware.Entity;

namespace SmartHub.DeviceHubMiddleware.Requests
{
    public class BaseWebSocketRequest<T>
    {
        public string ExtensionDeviceId { get; set; }

        public string requestId { get; set; }

        public DeviceType deviceType { get; set; }
        
        public WeboskcetTypeRequest typeRequest { get; set; }

        public T requestObject { get; set; }
    }

    public enum WeboskcetTypeRequest
    {
        Auth = 1,
        SendingData = 2,
        Ping = 3
    }
}
