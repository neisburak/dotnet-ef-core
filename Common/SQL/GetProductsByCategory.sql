CREATE OR ALTER PROCEDURE [dbo].[GetProductsByCategory] @CategoryId INT AS
BEGIN
    SET NOCOUNT ON
    SELECT * FROM Products p WHERE p.CategoryId = @CategoryId
END