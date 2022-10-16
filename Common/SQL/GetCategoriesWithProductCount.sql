CREATE OR ALTER FUNCTION GetCategoriesWithProductCount()
RETURNS TABLE AS
RETURN (
    SELECT c.Id, c.CategoryName as [Name], COUNT(p.Id) AS ProductCount FROM Products.CategoryTable c JOIN Products p ON c.Id = p.CategoryId GROUP BY c.Id, c.CategoryName
)