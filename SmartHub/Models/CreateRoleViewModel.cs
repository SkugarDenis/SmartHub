namespace SmartHub.Models
{
    public class CreateRoleViewModel
    {
        public List<RoleItem> Roles { get; set; }
    }

    public class RoleItem
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }

}
