﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Abacus.Models
{
    public class ShippingCompany
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(256)]
        public string Name { get; set; }

        [StringLength(256)]
        [Display(Name = "Web site URL")]
        public string WebSite { get; set; }

        [StringLength(256)]
        [Display(Name="Tracking URL format")]
        public string TrackingUrl { get; set; }
    }
}