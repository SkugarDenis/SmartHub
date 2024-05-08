using SmartHub.DeviceHubMiddleware.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartHub.DataContext.DbModels
{
    public class Device
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string ExternalId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public List<GroupEntity> Groups { get; set; }

        public DeviceType Type { get; set; }

        public List<DeviceInterfaceItem> Interfaces { get; set; }

        public ICollection<GroupDevice> GroupDevices { get; set; }
    }

    public class DeviceInterfaceItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }

        // добавить тип контрола
        public DataType DataType { get; set; }


        public string Control { get; set; }
    }

    public enum DeviceType
    {
        None = 0,
        Switch = 1,
        RemoteController = 2
    }

    public enum DataType
    {
        Boolean,
        String,
        Int
    }
}
