using BlogApplication.Comments;
using BlogApplication.Data;
using BlogApplication.Helpers;
using BlogApplication.Models;
using BlogApplication.ViewModels;
using Microsoft.EntityFrameworkCore;
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

            return _context.Posts
                .ToList();
        }


        public Post GetPost(int id)
        {
            return _context.Posts.Include(p=>p.MainComments)
                                     .ThenInclude(mc=>mc.SubComments)
                                  .FirstOrDefault(x => x.Id == id);
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


        public void AddSubComment(SubComment subComment)
        {
            _context.SubComments.Add(subComment);
        }

        public IndexViewModel GetAllPost(int pageNumber, string category)
        {
            var query = _context.Posts.AsQueryable();
            if (!String.IsNullOrEmpty(category))
            {
                query = query.Where(x => x.Category.ToLower().Equals(category.ToLower()));
            }

            int pageSize =1;
            int skipAmount = pageSize * (pageNumber - 1);
            int postsCount = query.Count();
            var pageCount = (int)Math.Ceiling((double)postsCount / pageSize);
            return new IndexViewModel {
                Posts = query
                        .Skip(skipAmount)
                        .Take(pageSize)
                        .ToList(),
                NextPage = postsCount > skipAmount + pageSize,
                Pages = PageHelper.PageNumbers(pageNumber, pageCount),
                Category = category,
                PageNumber = pageNumber,
                PageCount = pageCount,
            };
        }   
    }
}
