using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities;

[Table("Notes")]
public class Note
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public int AppUserId { get; set; }
    public AppUser AppUser { get; set; }
}
