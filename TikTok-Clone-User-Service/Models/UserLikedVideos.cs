using System.Text.Json.Serialization;

namespace TikTok_Clone_User_Service.Models
{
    public class UserLikedVideos
    {
        public int Id { get; set; }
        public int userId { get; set; }
        public int videoId { get; set; }

        public  User? User { get; set; }


    }
}
