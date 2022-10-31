namespace LiteratureLounge.Models
{
    public class UserPreferencesBookIndexColumn
    {
        public int UserPreferencesId { get; set; }
        public UserPreferences UserPreferences{ get; set; }
        public IndexColumn IndexColumn { get; set; }
        public int IndexColumnId { get; set; }
    }
}
