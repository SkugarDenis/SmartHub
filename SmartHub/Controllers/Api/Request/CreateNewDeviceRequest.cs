using SmartHub.DataContext.DbModels;

namespace SmartHub.Controllers.Api.Request
{
    public class CreateNewDeviceRequest
    {
        public string name { get; set; }
        public string externalId { get; set; }
        public DeviceType type { get; set; }
        public List<DeviceIterfaceRequest> interfaces { get; set; }
        public List<string> groups { get; set; }
    }
}
