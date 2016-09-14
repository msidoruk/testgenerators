CREATE TABLE [Jp].[EmployeeDocuments]
(
	[EmployeeDocumentId] INT IDENTITY(1,1) NOT NULL,
	[EmployeeId] INT NULL,
	[EmployeeDocumentTypeId] INT NULL,
	[DocumentType] VARCHAR (100) NULL,
	[Number] VARCHAR (100) NULL,
	[IssueLocation] VARCHAR (200) NULL,
	[IssuedByDept] VARCHAR (100) NULL,
	[IssueDate] DATETIME NULL,
	[ValidUntilDate] DATETIME NULL,
	-- Constraints
	CONSTRAINT PK_EmployeeDocuments PRIMARY KEY(EmployeeDocumentId),
	CONSTRAINT FK_Jp_Employees_Jp_EmployeeDocuments_EmployeeId FOREIGN KEY (EmployeeId) REFERENCES Jp.[Employees](EmployeeId),
	CONSTRAINT FK_Dicts_EmployeeDocumentTypes_Jp_EmployeeDocuments_EmployeeDocumentTypeId FOREIGN KEY (EmployeeDocumentTypeId) REFERENCES Dicts.[EmployeeDocumentTypes](EmployeeDocumentTypeId),
)

