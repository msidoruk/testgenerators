CREATE TABLE [Eps].[DocumentScans]
(
	[DocumentScanId] INT IDENTITY(1,1) NOT NULL,
	[Image] IMAGE NULL,
	-- Constraints
	CONSTRAINT PK_DocumentScans PRIMARY KEY(DocumentScanId),
)

