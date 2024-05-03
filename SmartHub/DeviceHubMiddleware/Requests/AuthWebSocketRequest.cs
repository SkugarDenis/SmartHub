namespace SmartHub.DeviceHubMiddleware.Requests
{
    public class AuthWebSocketRequest
    {
        public string ExtensionDeviceId { get; set; }

        public string Signature { get; set; }
    }
}
