﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;

namespace Abacus.Models
{
    [Guid("8BAB1468-BFCE-439D-B219-88D4ED4C0E3B")]
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