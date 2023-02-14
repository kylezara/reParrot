using Sabio.Models;
using Sabio.Models.Domain.Blogs;
using Sabio.Models.Requests.Blogs;
using System.Collections.Generic;
namespace Sabio.Services.Interfaces
{
    public interface IBlogService
    {
        int Add(BlogAddRequest model, int currentUserId);
        void Update(BlogUpdateRequest model, int currentUserId);
        void Delete(int id);
        Paged<Blog> GetAll(int pageIndex, int pageSize);
        Blog GetById(int id);
        Paged<Blog> GetByCreatedByPagination(int pageIndex, int pageSize, int createdBy);
        Paged<Blog> GetByBlogType(int pageIndex, int pageSize, int blogTypeId); 

    }
}