using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeachMate.Domain;
public class PushNotificationReceiver
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PushNotificationId { get; set; }
    public Guid ReceiverId { get; set; }
    public PushNotification PushNotification { get; set; } = null!;
}
