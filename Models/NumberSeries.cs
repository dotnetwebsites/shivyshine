using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shivyshine.Areas.Admin.Models;

namespace Shivyshine.Models
{
    public class NumberSeries : SchemaLog
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Type { get; set; }
        public string Prefix { get; set; }
        public int Number { get; set; }
        public string OrderNo { get; set; }
    }
}