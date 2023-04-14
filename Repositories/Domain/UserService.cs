﻿using AuthApi.Models.Domain;
using AuthApi.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace AuthApi.Repositories.Domain
{
    public class UserService :IUserService
    {
        private IWebHostEnvironment environment;
        private readonly DatabaseContext _context;
        public UserService(DatabaseContext context, IWebHostEnvironment env)
        {
            _context = context;
            this.environment = env;

        }
        public async Task<AllUsers?> GetSingleUser(string email)
        {
            var user = await _context.AllUsers.SingleAsync(u => u.Email == email);
            if (user == null)
            {
                return null;
            }
            return user;
        }

        public async Task<int?>getuserId(string email)
        {
            var user = await _context.AllUsers.SingleAsync(u => u.Email == email);
            int userID = user.Id;
            if (user == null)
            {
                return null;
            }
            return userID;

        }

        public async Task<string> AddImage(IFormFile imageFile,string email)
        {
            try
            {
                var UID = await getuserId(email); 
                var contentPath = this.environment.ContentRootPath;
                var path = Path.Combine(contentPath, "Uploads",UID.ToString());
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                var ext = Path.GetExtension(imageFile.FileName);
                var allowedExtentions = new string[] { ".jpg", ".png", ".jpeg" };
                if (!allowedExtentions.Contains(ext))
                {
                    string msg = string.Format("Only {0} extensions are allowed", string.Join(",", allowedExtentions));
                    return msg;
                }
                string uniqueString = Guid.NewGuid().ToString();
                var newFileName = uniqueString + ext;
                var fileWithPath = Path.Combine(path, newFileName);
                var stream = new FileStream(fileWithPath, FileMode.Create);
                imageFile.CopyTo(stream);
                stream.Close();
                return newFileName;

            }
            catch (Exception ex)
            {
                return "Error has occured";
            }
        }

        public List<UserImages> ShowImage(string email)
        {
            List<UserImages> images = new List<UserImages>();
            images =  _context.UserImages.Where(u => u.UserEmail == email).ToList();
            if (images.Count == 0)
            {
                return null;
            }
            return images;
        }
    }
}