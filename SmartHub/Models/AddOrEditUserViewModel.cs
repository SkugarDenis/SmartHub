namespace SmartHub.Models
{
    public class AddOrEditUserViewModel
    {
        public List<UserItem> users = new List<UserItem>();
        public List<RoleItem> roles = new List<RoleItem>();
    }

    public class UserItem
    {
        public string UserName { get; set; }

        public string UserId { get; set; }
    }

}
