using System.ComponentModel.DataAnnotations;

namespace AuthApi.Models.DTO
{
    public class UserImageDto
    {
        public int Id { get; set; }
        public string? UserImage { get; set; }
    }
}
