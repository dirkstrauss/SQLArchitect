CREATE PROCEDURE [dbo].[sproc_GetProperties] 
	@applicationName VARCHAR(150)	
AS
BEGIN
	
	SET NOCOUNT ON;

	   SELECT * FROM [dbo].[tProperties] WHERE name = @applicationName 
END
