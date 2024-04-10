namespace SmartHub.Models
{
    public class GroupWithRolesViewModel
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public List<RoleViewModel> Roles { get; set; }
    }

    public class RoleViewModel
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
    }
}
