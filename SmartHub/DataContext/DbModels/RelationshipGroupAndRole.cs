using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SmartHub.DataContext.DbModels
{
    public class RelationshipGroupAndRole
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int idGroup { get; set; }

        public string NameGroup { get; set; }

        public string IdRole { get; set; }
    }
}
