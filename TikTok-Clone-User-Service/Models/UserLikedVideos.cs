using System.Text.Json.Serialization;

namespace TikTok_Clone_User_Service.Models
{
    public class UserLikedVideos
    {
        public int Id { get; set; }
        public int userId { get; set; }
        public int videoId { get; set; }

        [JsonIgnore]
        public required User User { get; set; }


    }
}
