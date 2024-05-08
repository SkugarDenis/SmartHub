using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SmartHub.DataContext.DbModels
{
    public class GroupDevice
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int GroupEntityId { get; set; }
        public GroupEntity GroupEntity { get; set; }

        public int DeviceId { get; set; }
        public Device Device { get; set; }
    }
}
