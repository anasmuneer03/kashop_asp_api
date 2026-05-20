using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.DAL.Validations
{
    public class AllowedExtensionsAttribute :ValidationAttribute
    {
        string[] _extensions = { ".jpp", ".webp"};
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if(value is IFormFile file)
            {
                //test.PNG or test.png => .png
                var extension = Path.GetExtension(file.FileName).ToLower();
                if (!_extensions.Contains(extension))
                    return new ValidationResult ($"Allowed Extensions are: {string.Join(", ", _extensions)}");
            }
            return ValidationResult.Success;

        }
    }
}
