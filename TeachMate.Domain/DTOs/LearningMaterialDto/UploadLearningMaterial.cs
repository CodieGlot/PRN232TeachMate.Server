﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeachMate.Domain
{
    public class UploadLearningMaterialDto
    {
        public string DisplayName { get; set; }
        public string LinkDownload { get; set; }
        public DateTime UploadDate { get; set; } = DateTime.UtcNow;
        public int LearningChapterId { get; set; }

    }
}
