namespace AuthApi.Models.Domain
{
    public class UploadImage
    {
        public string Id { get; set; }
        public IFormFile formFile { get; set; }
    }
}
