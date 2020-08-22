using BlogApplication.Data;
using BlogApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace BlogApplication.Repository
{
    public class Repository : IRepository
    {
        private AppDbContext _context;

        public Repository(AppDbContext context)
        {
            _context = context;
        }
        public void AddPost(Post post)
        {
            _context.Posts.Add(post);
        }

        public List<Post> GetAllPost()
        {
            return _context.Posts.ToList();
        }

        public Post GetPost(int id)
        {
            return _context.Posts.FirstOrDefault(x => x.Id == id);
        }

        public void RemovePost(int id)
        {
            _context.Posts.Remove(GetPost(id));
        }


        public void UpdatePost(Post post)
        {
            _context.Posts.Update(post);
        } 
        public async Task<bool> SaveChangesAsync()
        {
            if (await _context.SaveChangesAsync()>0)
            {
                return true;
            }
            return false;
        }

        public List<Post> GetAllPost(string category)
        {
            return _context.Posts.Where(post=>post.Category.ToLower().Equals(category.ToLower())).ToList();
        }
    }
}
