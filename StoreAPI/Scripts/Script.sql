CREATE TABLE [Role] (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE [User] (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(20) NOT NULL UNIQUE,
    Email NVARCHAR(254) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    [Name] NVARCHAR(100) NOT NULL,
    Surname NVARCHAR(100) NOT NULL,
    RoleId INT NOT NULL,
    FOREIGN KEY (RoleId) REFERENCES [Role](Id)
);

INSERT INTO [Role] ([Name]) VALUES ('Admin'), ('Employee');

INSERT INTO [User] (Username, Email, PasswordHash, [Name], Surname, RoleId)
VALUES (
    'Admin',
    'admin@admin.com',
    '$2a$11$VQ6jHpUpSXTZkwO1pRJZQe8YylSe5M.9aLwnfipU1vhhp6vNB7A.S',
    'Antonio',
    'Ristevski',
    1
);

CREATE TABLE [Company] (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    [Address] NVARCHAR(200) NOT NULL UNIQUE,
    Phone NVARCHAR(18) NOT NULL UNIQUE,
    [Name] NVARCHAR(500) NOT NULL UNIQUE
);

CREATE TABLE [Product] (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    RegistrationNumber NVARCHAR(7) NOT NULL UNIQUE,
    [Name] NVARCHAR(500) NOT NULL UNIQUE,
    CompanyId INT NOT NULL,
    Price DECIMAL(18, 2) NOT NULL,
    FOREIGN KEY (CompanyId) REFERENCES [Company](Id)
);

CREATE TABLE [ProductSale] (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ProductId INT NOT NULL,
	SoldAmount INT NOT NULL,
	PricePerUnit DECIMAL(18, 2) NOT NULL,
	[Date] DATETIME NOT NULL,
    FOREIGN KEY (ProductId) REFERENCES [Product](Id)
);