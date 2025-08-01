﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TeachMate.Domain
{
    public class SearchClassDto
    {
        public string? TitleOrDesc { get; set; } = string.Empty;
        public Subject Subject { get; set; } = Subject.None;
        // Calculated in minutes
        public int? GradeLevel { get; set; } = -1;
        public DateOnly? StartOpenDate { get; set; } = default; //nguoi dung muon biet lop mo khi nao, trong khoang thoi gian nao
        // 25/5 - 25/6 --> StartDate > StartdATE
        public DateOnly? EndOpenDate { get; set; } = default;
        public int? MaximumLearners { get; set; }
        public ModuleType? ModuleType { get; set; }
        public int? NumOfWeeks { get; set; } = 0;
        public double? MaxPrice { get; set;} = 0;
        public double? MinPrice { get; set; } = 0;

    }
}
