namespace LeadManager
{
    public enum LeadSource { website, social, recommendation }
    public enum LeadStatus { @new, processed }

    public class Lead
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public LeadSource Source { get; set; }
        public LeadStatus Status { get; set; } = LeadStatus.@new;
    }
}