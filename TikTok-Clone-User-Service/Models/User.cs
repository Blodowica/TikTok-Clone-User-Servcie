using TikTok_Clone_User_Service.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.AspNetCore.Http.HttpResults;
namespace TikTok_Clone_User_Service.Models
{
    public class User
    {


        public required string Auth_id { get; set; }
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public string? Description { get; set; }
        public  required string Role { get; set; }

        public ICollection<Follower>? Followers { get; set; }

        public ICollection<UserLikedVideos>? likedVideos { get; set; }
    }


   
}
