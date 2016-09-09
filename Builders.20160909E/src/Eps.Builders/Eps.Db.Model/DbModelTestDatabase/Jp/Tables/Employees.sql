CREATE TABLE [Jp].[Employees]
(
	[EmployeeId] INT IDENTITY(1,1) NOT NULL,
	[JuridicalPersonId] INT NULL,
	[Position] VARCHAR (200) NULL,
	[FirstName] VARCHAR (100) NULL,
	[LastName] VARCHAR (100) NULL,
	[MiddleName] VARCHAR (100) NULL,
	[Citizenship] VARCHAR (100) NULL,
	[BirthDate] DATETIME NULL,
	[BirthPlace] VARCHAR (200) NULL,
	[ActivationDate] DATE NULL,
	[EndDate] DATE NULL,
	[Description] VARCHAR (200) NULL,
	-- Constraints
	CONSTRAINT PK_Employees PRIMARY KEY(EmployeeId),
	CONSTRAINT FK_Eps_JuridicalPersons_Jp_Employees_JuridicalPersonId FOREIGN KEY (JuridicalPersonId) REFERENCES Eps.[JuridicalPersons](JuridicalPersonId),
)

