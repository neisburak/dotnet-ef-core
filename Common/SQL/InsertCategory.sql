CREATE OR ALTER PROCEDURE InsertCategory @Name NVARCHAR(MAX), @Description NVARCHAR(MAX), @Id INT OUT AS 
BEGIN
    INSERT INTO Products.CategoryTable ([CategoryName], [Description]) VALUES (@Name, @Description)
    SET @Id = SCOPE_IDENTITY()
END