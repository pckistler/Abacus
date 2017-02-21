using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Abacus.Models
{
    public class Address
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(256)]
        public string Street1 { get; set; }

        [StringLength(256)]
        public string Street2 { get; set; }

        [StringLength(256)]
        public string City { get; set; }

        public int CountryId{ get; set; }
        [ForeignKey("CountryId")]
        public virtual Country Country { get; set; }

        public int StateId { get; set; }
        [ForeignKey("StateId")]
        public virtual CountryState State { get; set; }
        /// <summary>
        /// Think of as ZIP code
        /// </summary>
        [StringLength(256)]
        [DataType(DataType.PostalCode)]
        public string PostalCode { get; set; }
    }
}