using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Abacus.Models
{
    public class LogRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        public int UserID { get; set; } 
        public DateTime DateTime { get; set; }
        public Utilities.RecordType RecordType { get; set; }

        [StringLength(256)]
        public string Notes { get; set; }
    }
}