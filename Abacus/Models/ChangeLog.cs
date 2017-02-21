using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Abacus.Models
{
    public class ChangeLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long LogRecordId { get; set; }
        [ForeignKey("LogRecordId")]
        public virtual LogRecord LogRecord { get; set; }

        [StringLength(64)]
        public string RecordType { get; set; }

        public int RecordID { get; set; }

    }
}