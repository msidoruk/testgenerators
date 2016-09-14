CREATE TABLE [Jp].[EmployeeAddresses]
(
	[EmployeeAddressId] INT IDENTITY(1,1) NOT NULL,
	[EmployeeId] INT NULL,
	[AddressType] INT NULL,
	[Address] VARCHAR (500) NULL,
	-- Constraints
	CONSTRAINT PK_EmployeeAddresses PRIMARY KEY(EmployeeAddressId),
	CONSTRAINT FK_Jp_Employees_Jp_EmployeeAddresses_EmployeeId FOREIGN KEY (EmployeeId) REFERENCES Jp.[Employees](EmployeeId),
)

