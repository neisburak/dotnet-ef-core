CREATE OR ALTER VIEW [dbo].[ProductByCategory] AS
SELECT
    P.Id,
    P.Name,
    C.CategoryName,
    P.UnitPrice
FROM Products P JOIN Products.CategoryTable C ON P.CategoryId = C.Id
GO