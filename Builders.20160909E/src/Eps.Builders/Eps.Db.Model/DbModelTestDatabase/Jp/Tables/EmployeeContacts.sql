CREATE TABLE [Jp].[EmployeeContacts]
(
	[EmployeeContactId] INT IDENTITY(1,1) NOT NULL,
	[EmployeeId] INT NULL,
	[EmployeeContactTypeId] INT NULL,
	[ContactValue] VARCHAR (500) NULL,
	-- Constraints
	CONSTRAINT PK_EmployeeContacts PRIMARY KEY(EmployeeContactId),
	CONSTRAINT FK_Jp_Employees_Jp_EmployeeContacts_EmployeeId FOREIGN KEY (EmployeeId) REFERENCES Jp.[Employees](EmployeeId),
	CONSTRAINT FK_Dicts_EmployeeContactTypes_Jp_EmployeeContacts_EmployeeContactTypeId FOREIGN KEY (EmployeeContactTypeId) REFERENCES Dicts.[EmployeeContactTypes](EmployeeContactTypeId),
)

