﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BuildingManagement.Models
{
    public class MeterType
    {
        public int ID { get; set; }

        [Required]
        public string Type { get; set; }

        public virtual ICollection<AbstractMeter> AbstractMeters { get; set; }
    }
}