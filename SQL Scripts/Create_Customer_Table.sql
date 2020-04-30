CREATE TABLE [dbo].[Customer] (
    [CustomerID] INT          IDENTITY (1, 1) NOT NULL,
    [Name]       VARCHAR (50) NULL,
    [Phone]      VARCHAR (20) NULL,
    [Address]    VARCHAR (50) NULL,
    [InvoiceID]  INT          NULL,
    CONSTRAINT [PK_Customer] PRIMARY KEY CLUSTERED ([CustomerID] ASC)
);