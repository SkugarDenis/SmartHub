namespace SmartHub.Models
{
    public class CreateGroupViewModel
    {
        public List<GroupItem> groups { get; set; }
    }

    public class GroupItem
    {
        public int id { get; set; }

        public string name { get; set; }
    }
}
