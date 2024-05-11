using SmartHub.DataContext.DbModels;

namespace SmartHub.Controllers.Api.Request
{
    public class ChangeInterfaceDeviceRequest
    {
        public int DeviceId { get; set; }

        public int InterfaceId { get; set; }

        public DeviceType DeviceType { get; set; }

        public string data { get; set; }

    }
}
