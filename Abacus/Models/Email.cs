using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;

namespace Abacus.Models
{
    [Guid("7367ABE0-B139-4646-A2DB-27D6B1CCF306")]
    public class Email
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(128)]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { set; get; }
    }
}