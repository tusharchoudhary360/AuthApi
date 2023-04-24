using System.ComponentModel.DataAnnotations;

namespace AuthApi.Models.DTO
{
    public class JustId
    {
        [Required]
        public int id { get; set; }
    }
}
