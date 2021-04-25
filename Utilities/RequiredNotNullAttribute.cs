using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Shivyshine.Utilities
{
    public class RequiredNotNullAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return ((int)value > 0);
        }
    }
}