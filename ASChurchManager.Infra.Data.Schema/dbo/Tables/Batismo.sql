CREATE TABLE [dbo].[Batismo]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [DataMaximaCadastro] DATETIMEOFFSET NULL, 
    [DataBatismo] DATETIMEOFFSET NULL, 
	[IdadeMinima] INT NULL,
    [Status] INT NOT NULL,
    [DataCriacao] DATETIMEOFFSET NULL, 
    [DataAlteracao] DATETIMEOFFSET NULL
)
