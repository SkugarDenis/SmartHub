using System.Net.WebSockets;

namespace SmartHub.DeviceHubMiddleware.Entity
{
    public class WebSocketMessageResult
    {
        public WebSocketReceiveResult webScoketResult { get; set; }

        public string Message { get; set; }
    }
}
