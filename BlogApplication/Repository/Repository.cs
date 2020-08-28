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

            int pageSize =2;
            int skipAmount = pageSize * (pageNumber - 1);
            int postsCount = query.Count();
            var pageCount = (int)Math.Ceiling((double)postsCount / pageSize);
            return new IndexViewModel {
                Posts = query
                        .Skip(skipAmount)
                        .Take(pageSize)
                        .ToList(),
                NextPage = postsCount > skipAmount + pageSize,
                Pages = PageNumbers(pageNumber, pageCount),
                Category = category,
                PageNumber = pageNumber,
                PageCount = pageCount,
            };
        }

         private IEnumerable<int> PageNumbers(int pageNumber, int pageCount)
        {
            if (pageCount < 5)
            {
                for (int i = 1; i <= pageCount; i++)
                {
                    yield return i;

                }
            }

            else
            {
                int midPoint = pageNumber < 3 ? 3
                : pageNumber > pageCount - 2 ? pageCount - 2
                : pageNumber;
                int lowerBound = midPoint - 2;
                int upperBound = midPoint + 2;

                if (lowerBound != 1)
                {
                    yield return 1;
                    if (lowerBound - 1 > 1)
                        yield return -1;

                }
                for (int i = lowerBound; i <= upperBound; i++)
                {
                    yield return i;

                }
                if (upperBound != pageCount)
                {
                    if (pageCount - upperBound > 1)
                    {
                        yield return -1;
                    }
                    yield return pageCount;

                }

            }
        }
    }
}
