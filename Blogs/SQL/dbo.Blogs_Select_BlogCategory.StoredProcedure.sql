USE [ReParrot]
GO
/****** Object:  StoredProcedure [dbo].[Blogs_Select_BlogCategory]    Script Date: 12/23/2022 10:19:08 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:	Kyle Zarate
-- Create date: 12/19/2022
-- Description:	Blogs Select By Blog Category (Category means BlogTypeId)
-- Code Reviewer: Jacob Helton


-- MODIFIED BY: Kyle Zarate
-- MODIFIED DATE: 12/23/2022
-- Code Reviewer: Jacob Helton
-- Note: This proc is not paginated
-- =============================================

CREATE proc [dbo].[Blogs_Select_BlogCategory]

			@BlogTypeId int

as

/*---------------TEST CODE------------------

	Declare @BlogTypeId int = 1

	Execute [dbo].[Blogs_Select_BlogCategory] @BlogTypeId 

*/

BEGIN

		SELECT	 b.[Id]
				,b.[BlogTypeId]
				,bt.[Name]
				,b.[AuthorId] 
				,u.[FirstName]
				,u.[LastName]
				,u.[AvatarUrl]
				,b.[Title]
				,b.[Subject]
				,b.[Content]
				,b.[IsPublished]
				,b.[ImageUrl]
				,b.[DateCreated]
				,b.[DateModified]
				,b.[DatePublish]
				,TotalCount = COUNT(1) OVER()
		FROM [dbo].[Blogs] as b 
		inner join dbo.BlogTypes as bt
			on b.BlogTypeId = bt.Id
		inner join dbo.Users as u
			on b.AuthorId = u.Id
		WHERE b.BlogTypeId = @BlogTypeId 

END
GO
