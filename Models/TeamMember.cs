using System.ComponentModel.DataAnnotations;

namespace MVC_nhaSach.Models;

public class TeamMember
{
    public int Id { get; set; }

    [Required]
    [StringLength(20)]
    public string StudentId { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [StringLength(2)]
    public string Initial { get; set; } = string.Empty;

    public int SortOrder { get; set; }

    [StringLength(300)]
    public string? BackgroundImagePath { get; set; }
}
