CREATE OR ALTER FUNCTION GetProductFeatures(@Id int)
RETURNS TABLE AS
RETURN (
    SELECT f.[Key], f.[Value] FROM ProductFeatures f WHERE f.ProductId = @Id
)