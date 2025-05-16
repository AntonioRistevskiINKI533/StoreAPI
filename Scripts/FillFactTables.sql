
---------------------------------------------------------------------------------------------------------------------------------
  DELETE FROM FactSale

  DECLARE @CustomerId AS INT
  DECLARE @ProductId AS INT
  DECLARE @StoreId AS INT
  DECLARE @DateId AS INT = 30
  DECLARE @TotalPrice AS DECIMAL(18,2)
  DECLARE @Units AS INT = 1

  DECLARE @Num INT

  DECLARE @TotalPurchasedUnitsUntilNow INT = 0
  DECLARE @TotalSoldUnitsUntilNow INT = 1

  WHILE (@DateId<=1030)
  BEGIN

	---@CustomerId

	SET @CustomerId = (SELECT TOP 1 Id FROM Customer ORDER BY NEWID())

	--@DateId

	SET @Num = ROUND(RAND()*3,0) + 1
	IF (@Num>3)
	BEGIN
		SET @DateId = @DateId + 1

		IF ((SELECT [DayOfWeek] FROM Date WHERE Id = @DateId) = 7)--vo nedela nema prodazbi
		BEGIN
			SET @DateId = @DateId + 1
			CONTINUE
		END
	END

	---@ProductId

	WHILE ((@TotalPurchasedUnitsUntilNow-(@TotalSoldUnitsUntilNow+@Units)) < 0 OR @TotalPurchasedUnitsUntilNow IS NULL) 
	BEGIN
		SET @ProductId = (SELECT TOP 1 Id FROM Product ORDER BY NEWID())

			---@Units

			SET @Num = ROUND(RAND()*37,0) + 1
			SET @Units = CHOOSE(@Num, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 1, 1, 3, 1, 1, 8, 2, 2, 1, 2, 1, 1, 1, 3, 1, 2, 1, 1, 1, 2, 3, 1, 1, 1, 1, 1, 1, 3) 
			--povekje edinici za poverojatno da e se pjavi 1 ili 2 ili ova pokje za vo sales e

		SET @TotalPurchasedUnitsUntilNow = (SELECT SUM(Units) FROM FactPurchase WHERE ProductId = @ProductId AND DateId <= @DateId)
		SET @TotalSoldUnitsUntilNow = (SELECT SUM(Units) FROM FactSale WHERE ProductId = @ProductId AND DateId <= @DateId)
		IF (@TotalSoldUnitsUntilNow IS NULL)
		BEGIN
			SET @TotalSoldUnitsUntilNow = 0
		END

		--SELECT @ProductId as ProductId
		--SELECT @TotalPurchasedUnitsUntilNow as TotalPurchasedUnitsUntilNow
		--SELECT @TotalSoldUnitsUntilNow as TotalSoldUnitsUntilNow
		--SELECT @TotalPurchasedUnitsUntilNow-@TotalSoldUnitsUntilNow as Minus
	END

	SET @TotalPurchasedUnitsUntilNow = 0
	SET @TotalSoldUnitsUntilNow = 1

	---@StoreId

	SET @StoreId = (SELECT TOP 1 Id FROM Store WHERE CityId = (SELECT c.CityId FROM Customer c WHERE c.Id = @CustomerId) ORDER BY NEWID())
	
	---@TotalPrice

	SET @TotalPrice = @Units * (SELECT SalePrice FROM Product WHERE Id = @ProductId)

	---

	INSERT INTO [dbo].[FactSale] ([CustomerId],[ProductId],[StoreId],[DateId],[TotalPrice],[Units])
    VALUES (@CustomerId,@ProductId,@StoreId,@DateId,@TotalPrice,@Units)

	SET @Units = 1
  END

  ------------------------------------------------------------------------------------------------
  DELETE FROM FactPurchase

  DECLARE @SupplierId AS INT
  DECLARE @ProductId AS INT
  DECLARE @DateId AS INT = 1
  DECLARE @TotalSalePrice AS DECIMAL(18,2)
  DECLARE @TotalPurchasePrice AS DECIMAL(18,2)
  DECLARE @Units AS INT

  DECLARE @Num INT

  WHILE (@DateId<1000)
  BEGIN

	---@ProductId

	SET @ProductId = (SELECT TOP 1 Id FROM Product ORDER BY NEWID())

	---@Units

	SET @Num = ROUND(RAND()*11,0) + 1
	SET @Units = CHOOSE(@Num, 20, 10, 10, 30, 30, 10, 10, 20, 30, 10, 10, 10) 
	--povekje edinici za poverojatno da e se pjavi 1 ili 2 ili ova pokje za vo sales e
	
	---@SupplierId

	SET @SupplierId = (SELECT TOP 1 s.Id FROM Supplier s, SupplierBrand sb, Product p WHERE  s.Id = sb.SupplierId AND p.BrandId = sb.BrandId AND p.Id = @ProductId ORDER BY NEWID())

	---@TotalSalePrice

	SET @TotalSalePrice = @Units * (SELECT SalePrice FROM Product WHERE Id = @ProductId)

	---@TotalPurchasePrice

	SET @TotalPurchasePrice = @Units * (SELECT PurchasePrice FROM Product WHERE Id = @ProductId)

	---

	INSERT INTO [dbo].[FactPurchase] ([SupplierId],[ProductId],[DateId],[TotalPrice],[Units])
    VALUES (@SupplierId,@ProductId,@DateId,@TotalPrice,@Units)

	--@DateId
	SET @Num = ROUND(RAND()*2,0) + 1
	IF (@Num = 1)
	BEGIN
		SET @DateId = @DateId + 1
	END
	ELSE IF (@Num = 2)
	BEGIN
		SET @DateId = @DateId + 2
	END

  END

  ------------------------------------------

  SELECT c.Name, c.Surname, 
  (SELECT SUM(fc.TotalPrice) FROM FactSale fc WHERE fc.CustomerId = c.Id) as TotalMoneySpent,
  (SELECT SUM(fc.Units) FROM FactSale fc WHERE fc.CustomerId = c.Id) as TotalUnitsBought
  FROM Customer c

  SELECT p.Id, p.Name, 
  (SELECT SUM(fs.TotalPrice) FROM FactSale fs WHERE fs.ProductId = p.Id AND (fs.DateId > 0 AND fs.DateId <= 90)) as TotalMoneySpent,
  (SELECT SUM(fs.Units) FROM FactSale fs WHERE fs.ProductId = p.Id AND (fs.DateId > 0 AND fs.DateId <= 90)) as TotalUnitsBought
  FROM Product p
  ORDER BY p.Id
  OFFSET (1) ROWS FETCH NEXT (1) ROWS ONLY

  -------------------------------------------

  SELECT SUM(Units), p.Name FROM FactPurchase fp, Product p WHERE DateId <= 120 AND fp.ProductId = p.Id GROUP BY p.Name

  SELECT SUM(Units), p.Name FROM FactSale fp, Product p WHERE DateId <= 120 AND fp.ProductId = p.Id GROUP BY p.Name
