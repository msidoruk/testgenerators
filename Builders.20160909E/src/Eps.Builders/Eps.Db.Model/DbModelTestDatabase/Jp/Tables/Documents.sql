CREATE TABLE [Jp].[Documents]
(
	[DocumentId] INT IDENTITY(1,1) NOT NULL,
	[JuridicalPersonId] INT NULL,
	[JuridicalPersonDocumentTypeId] INT NULL,
	[DocumentScanId] INT NULL,
	[DocumentFileLink] VARCHAR NULL,
	[CopyDate] DATETIME NULL,
	[OriginalDate] DATETIME NULL,
	[DocumentsStoringPlaceId] INT NULL,
	[Description] VARCHAR (200) NULL,
	[ActivationDate] DATETIME NULL,
	-- Constraints
	CONSTRAINT PK_Documents PRIMARY KEY(DocumentId),
	CONSTRAINT FK_Eps_JuridicalPersons_Jp_Documents_JuridicalPersonId FOREIGN KEY (JuridicalPersonId) REFERENCES Eps.[JuridicalPersons](JuridicalPersonId),
	CONSTRAINT FK_Dicts_JuridicalPersonDocumentTypes_Jp_Documents_JuridicalPersonDocumentTypeId FOREIGN KEY (JuridicalPersonDocumentTypeId) REFERENCES Dicts.[JuridicalPersonDocumentTypes](JuridicalPersonDocumentTypeId),
	CONSTRAINT FK_Eps_DocumentScans_Jp_Documents_DocumentScanId FOREIGN KEY (DocumentScanId) REFERENCES Eps.[DocumentScans](DocumentScanId),
	CONSTRAINT FK_Dicts_DocumentsStoringPlaces_Jp_Documents_DocumentsStoringPlaceId FOREIGN KEY (DocumentsStoringPlaceId) REFERENCES Dicts.[DocumentsStoringPlaces](DocumentsStoringPlaceId),
)

