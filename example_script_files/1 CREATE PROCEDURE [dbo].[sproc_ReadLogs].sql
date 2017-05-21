CREATE PROCEDURE [dbo].[sproc_ReadLogs] 
	@uniqueGuid VARCHAR(150)	
AS
BEGIN
	SET NOCOUNT ON;
	   
	   DECLARE @errorId INT = (SELECT recId FROM [dbo].[tExceptions] WHERE name = @uniqueGuid)
	   EXEC dbo.sproc_GetLogInfo @errorId 
END




