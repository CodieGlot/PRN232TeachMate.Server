using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeachMate.Domain;

public class UserReport
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public AppUser ReportedUser { get; set; }
    public UserReportType UserReportType { get; set; }
    public Guid ReportedUserId { get; set; }
}
