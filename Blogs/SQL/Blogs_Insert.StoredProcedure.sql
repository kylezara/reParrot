USE [ReParrot]
GO
/****** Object:  StoredProcedure [dbo].[Blogs_Insert]    Script Date: 12/23/2022 10:16:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:	Kyle Zarate
-- Create date: 12/19/2022
-- Description:	Blogs Insert
-- Code Reviewer: Jacob Helton 


-- MODIFIED BY: author
-- MODIFIED DATE:
-- Code Reviewer: 
-- Note: 
-- =============================================

CREATE proc [dbo].[Blogs_Insert]

			@BlogTypeId int
           ,@AuthorId int 
           ,@Title nvarchar(50)
           ,@Subject nvarchar(50)
           ,@Content nvarchar(max)
           ,@ImageUrl nvarchar(255)
           ,@DatePublish datetime2(7) = null
		   ,@Id int OUTPUT

as

/*---------------TEST CODE------------------

	Declare @Id int = 0

	Declare @BlogTypeId int = 3
           ,@AuthorId int = 1
           ,@Title nvarchar(50) = 'Test Title'
           ,@Subject nvarchar(50) = 'Subject Test'
           ,@Content nvarchar(max) = 'This is test content'
           ,@ImageUrl nvarchar(255) = 'https://bit.ly/3v3r7Pz'
           ,@DatePublish datetime2(7) = null

	Execute [dbo].[Blogs_Insert]
			@BlogTypeId 
           ,@AuthorId
           ,@Title
           ,@Subject
           ,@Content 
           ,@ImageUrl 
           ,@DatePublish
		   ,@Id OUTPUT

	Execute dbo.Blogs_SelectById @Id 

*/

BEGIN

	INSERT INTO	[dbo].[Blogs]
				([BlogTypeId]
				,[AuthorId]
				,[Title]
				,[Subject]
				,[Content]
				,[ImageUrl]
				,[DatePublish])
     VALUES
				(@BlogTypeId
				,@AuthorId
				,@Title
				,@Subject
				,@Content
				,@ImageUrl
				,@DatePublish)

	SET @Id = SCOPE_IDENTITY();

END
GO
