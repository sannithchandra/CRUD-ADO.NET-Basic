namespace BasicCRUD.Model
{
    public class Project
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string DateOfStart { get; set; }
        public int TeamSize { get; set; }
        public bool Active { get; set; }
        public string Status { get; set; }
        public string ClientLocationId { get; set; }
    }
}
