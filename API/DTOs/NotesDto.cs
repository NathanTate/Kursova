using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class NotesDto
{
    private string title;

    [Required] [StringLength(16, MinimumLength =3)] 
    public string Title 
    { 
        get => title; 
        set => title = value.ToLower();
    }
    [Required]
    public string Description { get; set; }
}
