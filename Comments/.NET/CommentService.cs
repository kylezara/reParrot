using Sabio.Data;
using Sabio.Data.Providers;
using Sabio.Models;
using Sabio.Models.Domain;
using Sabio.Models.Domain.Comments;
using Sabio.Models.Requests.Comments;
using Sabio.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Services
{
    public class CommentService : ICommentService
    {

        IDataProvider _data = null;

        public CommentService(IDataProvider data)
        {
            _data = data;
        }

        public int Add(CommentAddRequest model, int currentUserId)
        {
            int id = 0;

            string procName = "[dbo].[Comments_Insert]";

            _data.ExecuteNonQuery(procName,
                inputParamMapper: delegate (SqlParameterCollection col)
                {
                    AddCommonParams(model, col);
                    col.AddWithValue("@CreatedBy", currentUserId);

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

        public void Update(CommentUpdateRequest model, int currentUserId)
        {
            string procName = "[dbo].[Comments_Update]";

            _data.ExecuteNonQuery(procName,
                inputParamMapper: delegate (SqlParameterCollection col)
                {
                    AddCommonParams(model, col);
                    col.AddWithValue("@Id", model.Id);
                    col.AddWithValue("@ModifiedBy", currentUserId);
                },

                returnParameters: null);

        }

        public void Delete(int id)
        {
            string procName = "[dbo].[Comments_Delete_ById]"; 

            _data.ExecuteNonQuery(procName,
                inputParamMapper: delegate (SqlParameterCollection col)
                {
                    col.AddWithValue("@Id", id);
                },

                returnParameters: null);

        }

        public Paged<Comment> CommentsGetPaged(int pageIndex, int pageSize) 
        {

            Paged<Comment> pagedResult = null;

            List<Comment> result = null;

            int totalCount = 0;

            _data.ExecuteCmd(
                "[dbo].[Comments_SelectAll]",
                inputParamMapper: delegate (SqlParameterCollection parameterCollection)
                {
                    parameterCollection.AddWithValue("@PageIndex", pageIndex);
                    parameterCollection.AddWithValue("@PageSize", pageSize);
                },
                singleRecordMapper: delegate (IDataReader reader, short set)
                {
                    int index = 0;
                    Comment comment = MapSingleComment(reader, ref index);

                    if (totalCount == 0)
                    {
                        totalCount = reader.GetSafeInt32(index++);
                    }

                    if (result == null)
                    {
                        result = new List<Comment>();
                    }

                    result.Add(comment);

                });

            if (result != null)
            {
                pagedResult = new Paged<Comment>(result, pageIndex, pageSize, totalCount);
            }

            return pagedResult;

        }

        public List<Comment> GetByCreatedBy(int createdBy)
        {
            List<Comment> list = null;

            string procName = "[dbo].[Comments_SelectByCreatedBy]";

            _data.ExecuteCmd(procName,
                inputParamMapper: delegate (SqlParameterCollection parameterCollection)
                {
                    parameterCollection.AddWithValue("@CreatedBy", createdBy);
                }
                , singleRecordMapper: delegate (IDataReader reader, short set)
                {
                    int index = 0;
                    Comment authorComment = MapSingleComment(reader, ref index);

                    if (list == null)
                    {
                        list = new List<Comment>();
                    }

                    list.Add(authorComment);

                }
                );

            return list;

        }

        public List<Comment> GetByEntityId(int entityId, int entityTypeId)
        {
            List<Comment> commentList = null;

            string procName = "[dbo].[Comments_Select_ByEntityId]";

            _data.ExecuteCmd(procName,
                inputParamMapper: delegate (SqlParameterCollection parameterCollection)
                {
                    parameterCollection.AddWithValue("@EntityId", entityId);
                    parameterCollection.AddWithValue("@EntityTypeId", entityTypeId);
                }
                , singleRecordMapper: delegate (IDataReader reader, short set)
                {
                    int index = 0;
                    Comment commentEntity = MapSingleComment(reader, ref index);

                    if (commentList == null)
                    {
                        commentList = new List<Comment>();
                    }

                    commentList.Add(commentEntity);

                }
                );

            return commentList;

        }

        public List<Comment> GetNestedComments(int entityId, int entityTypeId)
        {

            List<Comment> comments = GetByEntityId(entityId, entityTypeId);
            List<Comment> list = new List<Comment>();
            Dictionary<int, Comment> _dictTopLevel = new Dictionary<int, Comment>();

            if (comments != null)
            {
                foreach (Comment comment in comments) 
                {
                    if (comment.ParentId == 0) 
                    {
                        _dictTopLevel.Add(comment.Id, comment); 
                    }
                    if (comment.ParentId != 0) 
                    {
                        foreach (Comment nestComment in comments)
                        {
                            if (comment.Id == nestComment.ParentId)
                            {
                                comment.Replies ??= new List<Comment>(); 
                                comment.Replies.Add(nestComment); 
                            }
                        }
                        if(_dictTopLevel.ContainsKey(comment.ParentId)) 
                        {
                            _dictTopLevel[comment.ParentId].Replies ??= new List<Comment>(); 
                            _dictTopLevel[comment.ParentId].Replies.Add(comment); 
                        }
                    }
                }
            }

            list = _dictTopLevel.Select(item=>item.Value).ToList(); 
            list.Reverse();
            return list; 

        }

        private static Comment MapSingleComment(IDataReader reader, ref int startingIndex)
        {

            Comment comment = new Comment();
            comment.Author = new BaseUser();

            comment.Id = reader.GetSafeInt32(startingIndex++);
            comment.Subject = reader.GetSafeString(startingIndex++);
            comment.Text = reader.GetSafeString(startingIndex++);
            comment.ParentId = reader.GetSafeInt32(startingIndex++);
            comment.EntityType = new LookUp();
            comment.EntityType.Id = reader.GetSafeInt32(startingIndex++);
            comment.EntityType.Name = reader.GetSafeString(startingIndex++);
            comment.EntityId = reader.GetSafeInt32(startingIndex++);
            comment.DateCreated = reader.GetSafeDateTime(startingIndex++);
            comment.DateModified = reader.GetSafeDateTime(startingIndex++);
            comment.Author.Id = reader.GetSafeInt32(startingIndex++);
            comment.ModifiedBy = reader.GetSafeInt32(startingIndex++);
            comment.Author.FirstName = reader.GetSafeString(startingIndex++);
            comment.Author.LastName = reader.GetSafeString(startingIndex++);
            comment.Author.AvatarUrl = reader.GetSafeString(startingIndex++);

            return comment;

        }

        private static void AddCommonParams(CommentAddRequest model, SqlParameterCollection col)
        {

            col.AddWithValue("@Subject", model.Subject);
            col.AddWithValue("@Text", model.Text);
            col.AddWithValue("@ParentId", model.ParentId);
            col.AddWithValue("@EntityTypeId", model.EntityTypeId);
            col.AddWithValue("@EntityId", model.EntityId);

        }

    }
}
