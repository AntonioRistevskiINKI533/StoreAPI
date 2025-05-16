--CREATE VIEW Sales.vOrders
--   WITH SCHEMABINDING
--   AS
--      SELECT SUM(UnitPrice * OrderQty * (1.00 - UnitPriceDiscount)) AS Revenue,
--         OrderDate, ProductID, COUNT_BIG(*) AS COUNT
--      FROM Sales.SalesOrderDetail AS od, Sales.SalesOrderHeader AS o
--      WHERE od.SalesOrderID = o.SalesOrderID
--      GROUP BY OrderDate, ProductID;
--GO
--
----Create an index on the view.
--CREATE UNIQUE CLUSTERED INDEX IDX_V1
--   ON Sales.vOrders (OrderDate, ProductID);
--GO

---Primer

CREATE VIEW CustomerSalesView
   WITH SCHEMABINDING AS
      SELECT c.Id as CustomerId, c.Name, c.Surname, c.Email, COUNT_BIG(*) as NumberOfTransactions
      FROM dbo.Customer c, dbo.FactSale fs
      WHERE fs.CustomerId = c.Id
      GROUP BY c.Id, c.Name, c.Surname, c.Email
GO

CREATE UNIQUE CLUSTERED INDEX IDX_V1
   ON CustomerSalesView (CustomerId);
GO

----------------------------

CREATE VIEW CustomerSaleSumsView
   WITH SCHEMABINDING AS
      SELECT c.Id as CustomerId, c.Name, c.Surname, c.Email, SUM(fs.TotalSalePrice - fs.TotalPurchasePrice) as Profit,
	  COUNT_BIG(*) as SumOfSales, SUM(fs.Units) as SumOfUnits, SUM(fs.TotalSalePrice) as SumOfTotalSalePrice
      FROM dbo.Customer c, dbo.FactSale fs
      WHERE fs.CustomerId = c.Id
      GROUP BY c.Id, c.Name, c.Surname, c.Email
GO

CREATE UNIQUE CLUSTERED INDEX IDX_V1
   ON CustomerSaleSumsView (CustomerId);
GO

---

CREATE VIEW CitySaleSumsView
   WITH SCHEMABINDING AS
      SELECT ci.Id as CityId, ci.Name, SUM(fs.TotalSalePrice - fs.TotalPurchasePrice) as Profit,
	  COUNT_BIG(*) as SumOfSales, SUM(fs.Units) as SumOfUnits, SUM(fs.TotalSalePrice) as SumOfTotalSalePrice
      FROM dbo.City ci, dbo.Customer c, dbo.FactSale fs
      WHERE fs.CustomerId = c.Id AND ci.Id = c.CityId
      GROUP BY ci.Id, ci.Name
GO

CREATE UNIQUE CLUSTERED INDEX IDX_V1
   ON CitySaleSumsView (CityId);
GO

---

CREATE VIEW DateSaleSumsView
   WITH SCHEMABINDING AS
      SELECT d.Id as DateId, d.[Date], d.[DayOfWeek], SUM(fs.TotalSalePrice - fs.TotalPurchasePrice) as Profit,
	  COUNT_BIG(*) as SumOfSales, SUM(fs.Units) as SumOfUnits, SUM(fs.TotalSalePrice) as SumOfTotalSalePrice
      FROM dbo.[Date] d, dbo.FactSale fs
      WHERE fs.DateId = d.Id
      GROUP BY d.Id, d.[Date], d.[DayOfWeek]
GO

CREATE UNIQUE CLUSTERED INDEX IDX_V1
   ON DateSaleSumsView (DateId);
GO

---

CREATE VIEW ProductSaleSumsAndProfitView
   WITH SCHEMABINDING AS
      SELECT p.Id as ProductId, p.[Name], SUM(fs.TotalSalePrice - fs.TotalPurchasePrice) as Profit,
	  COUNT_BIG(*) as SumOfSales, SUM(fs.Units) as SumOfUnits, SUM(fs.TotalSalePrice) as SumOfTotalSalePrice
      FROM dbo.[Product] p, dbo.FactSale fs
      WHERE fs.ProductId = p.Id
      GROUP BY p.Id, p.[Name]
GO

CREATE UNIQUE CLUSTERED INDEX IDX_V1
   ON ProductSaleSumsAndProfitView (ProductId);
GO

---

CREATE VIEW BrandSaleSumsAndProfitView
   WITH SCHEMABINDING AS
      SELECT b.Id as BrandId, b.[Name], SUM(fs.TotalSalePrice - fs.TotalPurchasePrice) as Profit,
	  COUNT_BIG(*) as SumOfSales, SUM(fs.Units) as SumOfUnits, SUM(fs.TotalSalePrice) as SumOfTotalSalePrice
      FROM dbo.[Product] p, dbo.FactSale fs, dbo.Brand b
      WHERE fs.ProductId = p.Id AND p.BrandId = b.Id
      GROUP BY b.Id, b.[Name]
GO

CREATE UNIQUE CLUSTERED INDEX IDX_V1
   ON BrandSaleSumsAndProfitView (BrandId);
GO

---

CREATE VIEW ProductTypeSaleSumsAndProfitView
   WITH SCHEMABINDING AS
      SELECT pt.Id as ProductTypeId, pt.[Name], SUM(fs.TotalSalePrice - fs.TotalPurchasePrice) as Profit,
	  COUNT_BIG(*) as SumOfSales, SUM(fs.Units) as SumOfUnits, SUM(fs.TotalSalePrice) as SumOfTotalSalePrice
      FROM dbo.[Product] p, dbo.FactSale fs, dbo.ProductType pt
      WHERE fs.ProductId = p.Id AND p.ProductTypeId = pt.Id
      GROUP BY pt.Id, pt.[Name]
GO

CREATE UNIQUE CLUSTERED INDEX IDX_V1
   ON ProductTypeSaleSumsAndProfitView (ProductTypeId);
GO

---

CREATE VIEW DayOfWeekSaleSumsView
   WITH SCHEMABINDING AS
      SELECT d.[DayOfWeek], SUM(fs.TotalSalePrice - fs.TotalPurchasePrice) as Profit,
	  COUNT_BIG(*) as SumOfSales, SUM(fs.Units) as SumOfUnits, SUM(fs.TotalSalePrice) as SumOfTotalSalePrice
      FROM dbo.[Date] d, dbo.FactSale fs, dbo.Product p
      WHERE fs.DateId = d.Id AND p.Id = fs.ProductId
      GROUP BY d.[DayOfWeek]
GO

CREATE UNIQUE CLUSTERED INDEX IDX_V1
   ON DayOfWeekSaleSumsView ([DayOfWeek]);
GO

---
---
---

CREATE VIEW SupplierPurchaseSumsView
   WITH SCHEMABINDING AS
      SELECT s.Id as SupplierId, s.Name, s.Email, 
	  COUNT_BIG(*) as SumOfPurchases, SUM(fp.Units) as SumOfUnits, SUM(fp.TotalPrice) as SumOfTotalPurchasePrice
      FROM dbo.Supplier s, dbo.FactPurchase fp
      WHERE fp.SupplierId = s.Id
      GROUP BY s.Id, s.Name, s.Email
GO

CREATE UNIQUE CLUSTERED INDEX IDX_V1
   ON SupplierPurchaseSumsView (SupplierId);
GO

---

CREATE VIEW DatePurchaseSumsView
   WITH SCHEMABINDING AS
      SELECT d.Id as DateId, d.[Date], d.[DayOfWeek],
	  COUNT_BIG(*) as SumOfPurchases, SUM(fp.Units) as SumOfUnits, SUM(fp.TotalPrice) as SumOfTotalPurchasePrice
      FROM dbo.[Date] d, dbo.FactPurchase fp, dbo.Product p
      WHERE fp.DateId = d.Id AND p.Id = fp.ProductId
      GROUP BY d.Id, d.[Date], d.[DayOfWeek]
GO

CREATE UNIQUE CLUSTERED INDEX IDX_V1
   ON DatePurchaseSumsView (DateId);
GO

---

CREATE VIEW ProductPurchaseSumsView
   WITH SCHEMABINDING AS
      SELECT p.Id as ProductId, p.[Name],
	  COUNT_BIG(*) as SumOfPurchases, SUM(fp.Units) as SumOfUnits, SUM(fp.TotalPrice) as SumOfTotalPurchasePrice
      FROM dbo.[Product] p, dbo.FactPurchase fp
      WHERE fp.ProductId = p.Id
      GROUP BY p.Id, p.[Name]
GO

CREATE UNIQUE CLUSTERED INDEX IDX_V1
   ON ProductPurchaseSumsView (ProductId);
GO

---