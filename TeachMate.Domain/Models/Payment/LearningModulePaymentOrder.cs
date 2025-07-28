using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeachMate.Domain
{
    public class LearningModulePaymentOrder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }              
        public Guid LearnerId { get; set; }
        public Learner Learner { get; set; } = new Learner();
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int LearningModuleId { get; set; } 
        public LearningModule LearningModule { get; set; } = new LearningModule();
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending; 
        public double PaymentAmount { get; set; }
        public bool HasClaimed { get; set; } = false;

        public List<Transaction> Transaction { get; set; } = new List<Transaction>();
    }
}
