using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.DAL.Validations
{
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxFileSizeMB;

        public MaxFileSizeAttribute(int maxFileSizeMB)
        {
            _maxFileSizeMB = maxFileSizeMB;
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                var sizeInMB = file.Length / (1024 * 1024);
                if(sizeInMB > _maxFileSizeMB)
                    return new ValidationResult(ErrorMessage = $"maximum file size is {_maxFileSizeMB}");
            }
            return ValidationResult.Success;

        }
    }
}
