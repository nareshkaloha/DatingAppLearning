using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.webapi.Dto;
using DatingApp.webapi.Helpers;
using DatingApp.webapi.Model;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.webapi.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext _context;

        public DatingRepository(DataContext context)
        {
            this._context = context;
        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }     

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            var photo = await _context.Photos.Where(u => u.UserId == userId && u.IsMain).FirstOrDefaultAsync();

            return photo;
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var Photo = await _context.Photos.FirstOrDefaultAsync(ph => ph.Id ==id);

            return Photo;
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users.Include(p=> p.Photos)
                                .FirstOrDefaultAsync(f=> f.Id == id);

            return user;
        }

        public async Task<PagedList<User>> GetUsers(UserParamsDto userParams)
        {
            var users = _context.Users.Include(p=> p.Photos)
                        .OrderByDescending(t => t.LastActiveDate)
                        .AsQueryable();

            users = users.Where(u => u.Id != userParams.UserId);
            users = users.Where(gen => gen.Gender == userParams.Gender);

            if(userParams.Likers)
            {
                var likers = await GetLikers(userParams.UserId);
                users = users.Where(u => likers.Contains(u.Id));
            }

            if(userParams.Likees)
            {
                var likees = await GetLikees(userParams.UserId);
                users = users.Where(u => likees.Contains(u.Id));
            }

            if(userParams.MinAge !=18)
            {
                users = users.Where(mg => mg.DateOfBirth <= DateTime.Today.AddYears(-userParams.MinAge));
            }

            if(userParams.MaxAge !=99)
            {
                users = users.Where(xa => xa.DateOfBirth >= DateTime.Today.AddYears(-(userParams.MaxAge-1)));
            }
            
            if(!string.IsNullOrEmpty(userParams.OrderBy))
            {
                switch(userParams.OrderBy)
                {
                    case "createDate" :
                        users = users.OrderByDescending( t => t.CreateDate);
                        break;
                    default:
                        users = users.OrderByDescending( t => t.LastActiveDate);
                        break;
                }
            }

            var pagedData = await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);            
            return pagedData;
        }   

        private async Task<IEnumerable<int>> GetLikers( int userId)
        {          
            return (await _context.Users.Include(t => t.Likers).FirstOrDefaultAsync(u => u.Id == userId))
                        .Likers.Where(t => t.LikeeId == userId).Select( m => m.LikerId);
        }

        private async Task<IEnumerable<int>> GetLikees(int userId)
        {
            return (await _context.Users.Include(t => t.Likees).FirstOrDefaultAsync(u => u.Id ==userId))
                        .Likees.Where(t => t.LikerId == userId).Select(m => m.LikeeId);
        }      

        public async Task<Like> GetLike(int likerId, int likeeId)
        {
            return await _context.Likes.Where(lk=> lk.LikerId == likerId && lk.LikeeId == likeeId).FirstOrDefaultAsync();
        }
        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}