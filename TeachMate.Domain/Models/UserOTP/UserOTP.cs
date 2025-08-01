﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeachMate.Domain
{
    public class UserOTP
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string OTP { get; set; }
        public string Gmail { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;

        public DateTime ExpireAt { get; set; } = DateTime.UtcNow.AddMinutes(2);
        [NotMapped]
        public bool IsExpired
        {
            get { return ExpireAt < DateTime.UtcNow; }
        }
    }
}
