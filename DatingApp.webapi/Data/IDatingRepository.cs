using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.webapi.Dto;
using DatingApp.webapi.Helpers;
using DatingApp.webapi.Model;

namespace DatingApp.webapi.Data
{
    public interface IDatingRepository
    {
        void Add<T>(T entity) where T: class;
        void Delete<T>(T entity) where T: class;
        Task<bool> SaveAll();
        Task<PagedList<User>> GetUsers(UserParamsDto userParams);
        Task<User> GetUser(int id);
        Task<Photo> GetPhoto(int id);
        Task<Photo> GetMainPhotoForUser(int userId);
        Task<Like> GetLike(int likerId, int likeeId);
        Task<Message> GetMessage(int id);
        Task<PagedList<Message>> GetMessagesForuser(MessageParamsDto messageParams);
        Task<IEnumerable<Message>> GetMessageThread(int senderId, int RecipientId);
    }
}