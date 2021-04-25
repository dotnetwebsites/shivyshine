using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using Shivyshine.Areas.Admin.Models;

namespace Shivyshine.Models
{
    public class SerialNoMaster : SchemaLog
    {
        [Key]
        public int Id { get; set; }
        public string Type { get; set; }
        public string Prefix { get; set; }
        public DateTime StartDate { get; set; }
        public bool IsActive { get; set; }
    }
}