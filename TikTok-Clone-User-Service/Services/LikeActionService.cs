using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TikTok_Clone_User_Service.DatabaseContext;
using TikTok_Clone_User_Service.Models;

namespace TikTok_Clone_User_Service.Services
{
    public interface ILikeActionService
    {
        Task addUserLikedVideos(string authID, int videoId);
        Task removeUserLikedVideos(string aurhID, int videoId);
    }
    public class LikeActionService :ILikeActionService
    {

        private readonly DbUserContext _userContext;
        public LikeActionService(DbUserContext UserContext) {
            _userContext = UserContext;
        }


        public async Task  addUserLikedVideos(string authID, int videoId)
        {
            try
            {
            if (authID == string.Empty || videoId <= 0) {
                return ;
            }

             var user = await _userContext.Users.FirstOrDefaultAsync(u => u.Auth_id == authID);
            if(user == null){return;}

            var userLikedVideo = new UserLikedVideos {userId= user.Id, videoId = videoId, User = user };

                _userContext.LikedVideos.Add(userLikedVideo);
                await _userContext.SaveChangesAsync();
            }
            
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }


        }

        public async Task removeUserLikedVideos(string authID, int videoId)
        {
            try
            {
                if (authID == string.Empty || videoId <= 0)
                {
                    return;
                }

                var user = await _userContext.Users.FirstOrDefaultAsync(u => u.Auth_id == authID);
                if (user == null) { return; }

                //find if the video is liked by the user 
                var userlikedVideo = await _userContext.LikedVideos.FirstOrDefaultAsync(lv =>
                lv.userId == user.Id && lv.videoId == videoId);
                if(userlikedVideo == null) { return;}

                _userContext.LikedVideos.Remove(userlikedVideo);
                await _userContext.SaveChangesAsync();
            }

            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
       
    }
}
