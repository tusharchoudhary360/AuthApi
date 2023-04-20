using System.ComponentModel.DataAnnotations;

namespace AuthApi.Models.Domain
{
    public class UploadImage
    {
        public int Id { get; set; }
        [Required]
        public IFormFile formFile { get; set; }
    }
}
