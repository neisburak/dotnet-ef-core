SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
BEGIN TRANSACTION READ_UNCOMMITTED_TRAN

BEGIN TRY
    SELECT * FROM Products

    COMMIT TRANSACTION READ_UNCOMMITTED_TRAN
    PRINT 'Transaction Succeed.'
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION READ_UNCOMMITTED_TRAN
    PRINT 'Transaction Failed.'
END CATCH