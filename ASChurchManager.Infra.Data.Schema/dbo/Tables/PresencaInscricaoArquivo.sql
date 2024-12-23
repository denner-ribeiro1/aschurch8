CREATE TABLE [dbo].[PresencaInscricaoArquivo]
(
	[Id] INT IDENTITY(1, 1) NOT NULL,
    [Nome] VARCHAR(255) NULL, 
    [DataCaptura] SMALLDATETIME NULL,
    CONSTRAINT [PK_PresencaInscricaoArquivo] PRIMARY KEY  CLUSTERED ([Id] ASC)
)
