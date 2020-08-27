using BlogApplication.Comments;
using BlogApplication.Data;
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

        //public IndexViewModel GetAllPost( int pageNumber)
        //{
        //    int pageSize = 5;
        //    int skipAmount = pageSize * (pageNumber - 1);
        //    int postsCount = _context.Posts.Count();
        //    return new IndexViewModel
        //    {
        //        Posts = _context.Posts
        //                .Skip(skipAmount)
        //                .Take(pageSize)
        //                .ToList(),
        //        NextPage = postsCount >= skipAmount + pageSize,
        //        PageNumber = pageNumber,
        //    };
        //}

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

        //public List<Post> GetAllPost(string category)
        //{
        //    return _context.Posts.Where(post=>post.Category.ToLower().Equals(category.ToLower())).ToList();
        //}

        public void AddSubComment(SubComment subComment)
        {
            _context.SubComments.Add(subComment);
        }

        public IndexViewModel GetAllPost(int pageNumber, string category)
        {
            //Func<Post, bool> InCategory = (post) => { return post.Category.ToLower().Equals(category.ToLower()); };
            var query = _context.Posts.AsQueryable();
            if (!String.IsNullOrEmpty(category))
            {
                query = query.Where(x => x.Category.ToLower().Equals(category.ToLower()));
            }

            int pageSize = 5;
            int skipAmount = pageSize * (pageNumber - 1);
            int postsCount = query.Count();
            return new IndexViewModel {
                Posts = query
                        .Skip(skipAmount)
                        .Take(pageSize)
                        .ToList(),
                NextPage = postsCount > skipAmount + pageSize,
                Category = category,
                PageNumber = pageNumber,
                PageCount =(int) Math.Ceiling((double) postsCount/ pageSize),
            };
        }
    }
}
