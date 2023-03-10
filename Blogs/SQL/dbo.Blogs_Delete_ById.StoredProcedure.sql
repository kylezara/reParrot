USE [ReParrot]
GO
/****** Object:  StoredProcedure [dbo].[Blogs_Delete_ById]    Script Date: 12/23/2022 10:16:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:	Kyle Zarate
-- Create date: 12/19/2022
-- Description:	Blogs Delete By Id
-- Code Reviewer: Jacob Helton 


-- MODIFIED BY: author
-- MODIFIED DATE:
-- Code Reviewer: 
-- Note: 
-- =============================================

CREATE proc [dbo].[Blogs_Delete_ById]

		@Id int

as

/*---------------TEST CODE------------------

	Declare @Id int = 5

	Execute dbo.Blogs_SelectById @Id 

	Execute [dbo].[Blogs_Delete_ById] @Id 

	Execute dbo.Blogs_SelectById @Id 

*/

BEGIN

	DELETE FROM [dbo].[Blogs]
	WHERE Id = @Id

END
GO
