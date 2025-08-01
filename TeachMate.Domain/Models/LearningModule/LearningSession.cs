﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeachMate.Domain;
public class LearningSession
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int Slot { get; set; }
    public string Title {  get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string LinkMeet { get; set; }

    public Question? Question { get; set; }
    public LearningModule LearningModule{ get; set; }
    public int LearningModuleId { get; set; }
    [NotMapped] public string LearningModuleName { get; set; }
}
