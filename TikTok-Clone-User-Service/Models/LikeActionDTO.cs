namespace TikTok_Clone_User_Service.Models
{
    public class LikeActionDTO
    {
        public required string AuthId { get; set; }
        public required int VideoId { get; set; }
        public required string Status { get; set; }
    }
}
