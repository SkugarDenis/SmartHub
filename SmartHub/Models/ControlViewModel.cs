using SmartHub.DataContext.DbModels;

namespace SmartHub.Models
{
    public class ControlViewModel
    {
        public List<Device> Devices { get; set; }

        public List<GroupItem> GroupItems { get; set; }

        public Array DeviceTypes { get; set; }

        public Array UserInterfaceDeviceTypes { get; set; }

        public Array DataTypes { get; set; }
    }
}
