using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Abacus.Models
{
    public class Country
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(256)]
        public string Name { get; set; }

        [StringLength(16)]
        public string Abbreviation { get; set; }

        [StringLength(64)]
        public string PostalCodeFormat { get; set; }

        public virtual ICollection<CountryState> States { get; set; }
    }
}