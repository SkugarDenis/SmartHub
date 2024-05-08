using SmartHub.DataContext.DbModels;

namespace SmartHub.Models
{
    public class ControlViewModel
    {
        public List<Device> Devices { get; set; }

        public List<GroupItem> GroupItems { get; set; }
    }
}
