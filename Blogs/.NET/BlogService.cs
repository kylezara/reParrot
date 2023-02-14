using Sabio.Data;
using Sabio.Data.Providers;
using Sabio.Models;
using Sabio.Models.Domain;
using Sabio.Models.Domain.Blogs;
using Sabio.Models.Requests.Blogs;
using Sabio.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
namespace Sabio.Services
{
    public class BlogService : IBlogService
    {
        IDataProvider _data = null;
        public BlogService(IDataProvider data)
        {
            _data = data;
        }
        public Paged<Blog> GetAll(int pageIndex, int pageSize)
        {
            Paged<Blog> pagedResult = null;
            List<Blog> result = null;
            int totalCount = 0;
            _data.ExecuteCmd(
                "[dbo].[Blogs_SelectAll]",
                inputParamMapper: delegate (SqlParameterCollection parameterCollection)
                {
                    parameterCollection.AddWithValue("@PageIndex", pageIndex);
                    parameterCollection.AddWithValue("@PageSize", pageSize);
                },
                singleRecordMapper: delegate (IDataReader reader, short set)
                {
                    int index = 0;
                    Blog blog = MapSingleBlog(reader, ref index);
                    if (totalCount == 0)
                    {
                        totalCount = reader.GetSafeInt32(index++);
                    }
                    if (result == null)
                    {
                        result = new List<Blog>();
                    }
                    result.Add(blog);
                });
            if (result != null)
            {
                pagedResult = new Paged<Blog>(result, pageIndex, pageSize, totalCount);
            }
            return pagedResult;
        }
        public Blog GetById(int id)
        {
            string procName = "[dbo].[Blogs_SelectById]";
            Blog blog = null;
            _data.ExecuteCmd(procName,
                inputParamMapper: delegate (SqlParameterCollection parameterCollection)
                {
                    parameterCollection.AddWithValue("@Id", id);
                }, delegate (IDataReader reader, short set)
                {
                    int index = 0;
                    blog = MapSingleBlog(reader, ref index);
                }
                );
            return blog;
        }

        public Paged<Blog> GetByBlogType(int pageIndex, int pageSize, int blogTypeId) 
        {
            Paged<Blog> pagedResult = null;

            List<Blog> result = null;

            int totalCount = 0;

            _data.ExecuteCmd(
                "[dbo].[Blogs_Select_BlogCategory_V2]",
                inputParamMapper: delegate (SqlParameterCollection parameterCollection)
                {
                    parameterCollection.AddWithValue("@PageIndex", pageIndex);
                    parameterCollection.AddWithValue("@PageSize", pageSize);
                    parameterCollection.AddWithValue("@BlogTypeId", blogTypeId);
                },
                singleRecordMapper: delegate (IDataReader reader, short set)
                {

                    int index = 0;
                    Blog blog = MapSingleBlog(reader, ref index);

                    if (totalCount == 0)
                    {
                        totalCount = reader.GetSafeInt32(index++);
                    }


                    if (result == null)
                    {
                        result = new List<Blog>();
                    }

                    result.Add(blog);
                }

            );
            if (result != null)
            {
                pagedResult = new Paged<Blog>(result, pageIndex, pageSize, totalCount);
            }

            return pagedResult;
        }
        public Paged<Blog> GetByCreatedByPagination(int pageIndex, int pageSize, int createdBy)
        {
            Paged<Blog> pagedResult = null;
            List<Blog> result = null;
            int totalCount = 0;
            _data.ExecuteCmd(
                "[dbo].[Blogs_Select_ByCreatedBy]",
                inputParamMapper: delegate (SqlParameterCollection parameterCollection)
                {
                    parameterCollection.AddWithValue("@PageIndex", pageIndex);
                    parameterCollection.AddWithValue("@PageSize", pageSize);
                    parameterCollection.AddWithValue("@CreatedBy", createdBy);
                },
                singleRecordMapper: delegate (IDataReader reader, short set)
                {
                    int index = 0;
                    Blog blog = MapSingleBlog(reader, ref index);
                    if (totalCount == 0)
                    {
                        totalCount = reader.GetSafeInt32(index++);
                    }
                    if (result == null)
                    {
                        result = new List<Blog>();
                    }
                    result.Add(blog);
                }
            );
            if (result != null)
            {
                pagedResult = new Paged<Blog>(result, pageIndex, pageSize, totalCount);
            }
            return pagedResult;
        }
        public int Add(BlogAddRequest model, int currentUserId)
        {
            int id = 0;
            string procName = "[dbo].[Blogs_Insert_V2]";
            _data.ExecuteNonQuery(procName,
                inputParamMapper: delegate (SqlParameterCollection col)
                {
                    AddCommonParams(model, col);
                    col.AddWithValue("@AuthorId", currentUserId);
                    SqlParameter idOut = new SqlParameter("@Id", SqlDbType.Int);
                    idOut.Direction = ParameterDirection.Output;
                    col.Add(idOut);
                },
                returnParameters: delegate (SqlParameterCollection returnCollection)
                {
                    object oId = returnCollection["@Id"].Value;
                    int.TryParse(oId.ToString(), out id);
                });
            return id;
        }
        public void Update(BlogUpdateRequest model, int currentUserId)
        {
            string procName = "[dbo].[Blogs_Update_V2]";
            _data.ExecuteNonQuery(procName,
                inputParamMapper: delegate (SqlParameterCollection col)
                {
                    AddCommonParams(model, col);
                    col.AddWithValue("@Id", model.Id);
                    col.AddWithValue("@AuthorId", currentUserId);
                },
                returnParameters: null);
        }
        public void Delete(int id)
        {
            string procName = "[dbo].[Blogs_Delete_ById]";
            _data.ExecuteNonQuery(procName,
                inputParamMapper: delegate (SqlParameterCollection col)
                {
                    col.AddWithValue("@Id", id);
                },
                returnParameters: null);
        }
        private static void AddCommonParams(BlogAddRequest model, SqlParameterCollection col)
        {
            col.AddWithValue("@BlogTypeId", model.BlogTypeId);
            col.AddWithValue("@Title", model.Title);
            col.AddWithValue("@Subject", model.Subject);
            col.AddWithValue("@Content", model.Content);
            col.AddWithValue("@ImageUrl", model.ImageUrl);
        }
        private static Blog MapSingleBlog(IDataReader reader, ref int startingIndex)
        {
            Blog blog = new Blog();
            blog.Id = reader.GetSafeInt32(startingIndex++);
            blog.BlogType = new LookUp();
            blog.BlogType.Id = reader.GetSafeInt32(startingIndex++);
            blog.BlogType.Name = reader.GetSafeString(startingIndex++);
            blog.Author = new BaseUser();
            blog.Author.Id = reader.GetSafeInt32(startingIndex++);
            blog.Author.FirstName = reader.GetSafeString(startingIndex++);
            blog.Author.LastName = reader.GetSafeString(startingIndex++);
            blog.Author.AvatarUrl = reader.GetSafeString(startingIndex++);
            blog.Title = reader.GetSafeString(startingIndex++);
            blog.Subject = reader.GetSafeString(startingIndex++);
            blog.Content = reader.GetSafeString(startingIndex++);
            blog.IsPublished = reader.GetSafeBool(startingIndex++);
            blog.ImageUrl = reader.GetSafeString(startingIndex++);
            blog.DateCreated = reader.GetSafeDateTime(startingIndex++);
            blog.DateModified = reader.GetSafeDateTime(startingIndex++);
            blog.DatePublish = reader.GetSafeDateTime(startingIndex++);
            return blog;
        }
    }
}