using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shivyshine.Models
{
    public class LogToken
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(900)")]
        [Display(Name = "Username")]
        public string UserId { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string Type { get; set; }

        [Column(TypeName = "nvarchar(512)")]
        [Display(Name = "Provider")]
        public string EMAILPHONE { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string Token { get; set; }
        public bool IsExpired { get; set; }
        public DateTime? ExpiredOn { get; set; }
        public DateTime? TimeStamp { get; set; }
    }

    public class LogTokenView : LogToken
    {
        public string UserName { get; set; }
    }

}