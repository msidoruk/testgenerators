CREATE TABLE [Dicts].[BankDepartments]
(
	[BankDepartmentId] INT IDENTITY(1,1) NOT NULL,
	[Code] VARCHAR (100) NULL,
	[Description] VARCHAR (200) NULL,
	[IsDefault] BIT NULL,
	[ActivationDate] DATE NULL,
	[EndDate] DATE NULL,
	-- Constraints
	CONSTRAINT PK_BankDepartments PRIMARY KEY(BankDepartmentId),
)

