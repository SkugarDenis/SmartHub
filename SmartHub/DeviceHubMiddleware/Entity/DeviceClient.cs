using SmartHub.DataContext.DbModels;
using System.Net.WebSockets;

namespace SmartHub.DeviceHubMiddleware.Entity
{
    public class DeviceClient
    {
        public WebSocket webSocket { get; set; }

        public bool IsAuth { get; set; } = false;

        public DeviceType deviceType { get; set; }
    }
}
