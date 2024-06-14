using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TikTok_Clone_User_Service.DatabaseContext;
using TikTok_Clone_User_Service.Models;

namespace TikTok_Clone_User_Service.Services
{
    public interface ILikeActionService
    {
        Task addUserLikedVideos(string authID, int videoId);
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

            var userLikedVideos = new UserLikedVideos {userId= user.Id, videoId = videoId, User = user };

                _userContext.LikedVideos.Add(userLikedVideos);
                await _userContext.SaveChangesAsync();
                Console.WriteLine(userLikedVideos);
            }
            
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }


        }

       
    }
}
