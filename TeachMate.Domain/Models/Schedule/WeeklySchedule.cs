using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeachMate.Domain
{
    public class WeeklySchedule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int NumberOfSlot { get; set; } = 0;

        public List<WeeklySlot> WeeklySlots { get; set; } = new List<WeeklySlot>();
    }
}
