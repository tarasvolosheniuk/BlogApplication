using BlogApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApplication.Repository
{
    public interface IRepository
    {
         Post GetPost(int id);
         List< Post >GetAllPost();
         void RemovePost(int id);
         void UpdatePost(Post post);
         void AddPost(Post post);
        Task<bool> SaveChangesAsync();
    }
}
