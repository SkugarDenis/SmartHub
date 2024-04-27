using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SmartHub.DataContext.DbModels
{
    public class RelationshipUserAndRole
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string idUser { get; set; }

        public string UserName { get; set; }

        public string IdRole { get; set; }
    }
}
