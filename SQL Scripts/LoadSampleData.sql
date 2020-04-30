USE a0CRUDDB
GO

SET IDENTITY_INSERT Customer ON
GO
INSERT Customer (CustomerID, Name, Phone, Address, InvoiceID) VALUES (1, N'Luke Skywalker', N'5558-6754', N'Makati City', 87422667)
GO
INSERT Customer (CustomerID, Name, Phone, Address, InvoiceID) VALUES (2, N'Harry Potter', N'5558-6754', N'Taguig City', 75733366)
GO
INSERT Customer (CustomerID, Name, Phone, Address, InvoiceID) VALUES (3, N'Bugs Bunny', N'5558-6533', N'Quezon City', 87311434)
GO

SET IDENTITY_INSERT Customer OFF
GO