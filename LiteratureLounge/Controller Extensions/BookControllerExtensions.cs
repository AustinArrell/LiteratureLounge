using LiteratureLounge.Models;
using Purrs_And_Prose.Data;

namespace LiteratureLounge.Controller_Extensions
{
    public class BookControllerExtensions
    {

        public static async Task SetupDefaultUserPrefs(ApplicationDbContext db, string userId) 
        {
            var columns = db.IndexColumns.ToList();
            if (columns.Count == 0)
            {
                await SetupInitialColumnData(db);
            }

            var defaultColumns = db.IndexColumns.Where(c => 
            c.Name == "Title" || 
            c.Name == "Author" || 
            c.Name == "Subtitle" ||
            c.Name == "Series" ||
            c.Name == "ISBN").ToList();
            List<UserPreferencesBookIndexColumn> userPrefIndexCols = new List<UserPreferencesBookIndexColumn>();

            foreach (var item in defaultColumns)
            {
                userPrefIndexCols.Add(new UserPreferencesBookIndexColumn { IndexColumn = item });
            }

            var defaultPref = new UserPreferences(){ 
                UserId = userId, 
                UserPreferencesBookIndexColumns = userPrefIndexCols };
            db.Add(defaultPref);
            await db.SaveChangesAsync();
           
        }

        public static async Task SetupInitialColumnData(ApplicationDbContext db) 
        {
            var newCols = new List<IndexColumn>();
            newCols.Add(new IndexColumn { Name = "Title" });
            newCols.Add(new IndexColumn { Name = "Author" });
            newCols.Add(new IndexColumn { Name = "Subtitle" });
            newCols.Add(new IndexColumn { Name = "ISBN" });
            newCols.Add(new IndexColumn { Name = "Publisher" });
            newCols.Add(new IndexColumn { Name = "Description" });
            newCols.Add(new IndexColumn { Name = "PageCount" });
            newCols.Add(new IndexColumn { Name = "Notes" });
            newCols.Add(new IndexColumn { Name = "Series" });
            newCols.Add(new IndexColumn { Name = "ReadDate" });
            newCols.Add(new IndexColumn { Name = "CatalogDate" });
            newCols.Add(new IndexColumn { Name = "ChapterLength" });
            newCols.Add(new IndexColumn { Name = "isStamped" });
            newCols.Add(new IndexColumn { Name = "isAnnotated" });
            newCols.Add(new IndexColumn { Name = "isSigned" });
            newCols.Add(new IndexColumn { Name = "isFavorite" });
            newCols.Add(new IndexColumn { Name = "ReadStatus" });
            newCols.Add(new IndexColumn { Name = "MediaType" });
            newCols.Add(new IndexColumn { Name = "Rating" });
            newCols.Add(new IndexColumn { Name = "PublishedDate" });
            newCols.Add(new IndexColumn { Name = "CheckedOutTo" });
            newCols.Add(new IndexColumn { Name = "SignatureType" });

            foreach (var col in newCols)
            {
                db.Add(col);
            }
            await db.SaveChangesAsync();
        }

    }
}


