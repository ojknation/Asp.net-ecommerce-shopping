using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Shop14.Models.Data
{
    [Table("tblLog")]
    public class LogDTO
    {
        [Key]
        public int Id { get; set; }
        public string ProductType { get; set; }
        public string ProductCategory { get; set; }
        public int LogTrackNumber { get; set; }
    }
}