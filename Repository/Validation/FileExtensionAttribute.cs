using System.ComponentModel.DataAnnotations;

namespace Shopping_Laptop.Repository.Validation
{
    public class FileExtensionAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName);
                string[] extentions = { "jpg", "png", "jpeg" };
                 
                bool results = extentions.Any(x => extension.EndsWith(x));

                if (!results)
                {
                    return new ValidationResult("Allowed extentions are jpg or png or jpeg");
                }
            }
            return ValidationResult.Success;
        }
    }
}
