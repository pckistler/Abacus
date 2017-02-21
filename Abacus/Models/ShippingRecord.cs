using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Abacus.Models
{
    public class ShippingRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ShippingCompanyId { get; set; }
        [ForeignKey("ShippingCompanyId")]
        public virtual ShippingCompany ShippingCompany { get; set; }

        public DateTime ShippedDate { get; set; }

        [StringLength(256)]
        public string TrackingNumber{ get; set; }
    }
}