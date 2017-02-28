using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;

namespace Abacus.Models
{
    [Guid("0AB35639-E240-4CF8-9D06-C6F5DFB75644")]
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