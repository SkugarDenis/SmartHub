using SmartHub.DataContext.DbModels;

namespace SmartHub.DeviceHubMiddleware.Response
{
    public class NotificationInterfecItemChange
    {
        public string ItefaceName { get; set; }

        public DataType dataType { get; set; }

        public string Value { get; set; }
    }
}
