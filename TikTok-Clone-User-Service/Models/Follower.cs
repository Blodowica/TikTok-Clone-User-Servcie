namespace TikTok_Clone_User_Service.Models
{
    public class Follower
    {
        public int Id { get; set; }

        // the person following the user
        public int followerUserId { get; set; }

        //the user that is being followed
        public int UserId { get; set; }

       public User User { get; set; }

    }
}
