CREATE TABLE [Jp].[EmployeeDocumentScans]
(
	[EmployeeDocumentScanId] INT IDENTITY(1,1) NOT NULL,
	[EmployeeDocumentId] INT NULL,
	[EmployeeDocumentScanType] INT NULL,
	[DocumentScanId] INT NULL,
	-- Constraints
	CONSTRAINT PK_EmployeeDocumentScans PRIMARY KEY(EmployeeDocumentScanId),
	CONSTRAINT FK_Jp_EmployeeDocuments_Jp_EmployeeDocumentScans_EmployeeDocumentId FOREIGN KEY (EmployeeDocumentId) REFERENCES Jp.[EmployeeDocuments](EmployeeDocumentId),
	CONSTRAINT FK_Eps_DocumentScans_Jp_EmployeeDocumentScans_DocumentScanId FOREIGN KEY (DocumentScanId) REFERENCES Eps.[DocumentScans](DocumentScanId),
)

