CREATE TABLE [dbo].[PresencaDatas]
(
	[Id] INT IDENTITY(1,1) NOT NULL,
	[PresencaId] INT NOT NULL,
	[DataHoraInicio] DATETIME NOT NULL,
    [DataHoraFim]    DATETIME NOT NULL,
	[EventoId] BIGINT NULL,
    [Status] TINYINT NULL, 
	[DataHoraRegistro] DATETIME NULL,
    CONSTRAINT [FK_Presenca_Datas] FOREIGN KEY ([PresencaId]) REFERENCES [dbo].[Presenca] ([Id]), 
    CONSTRAINT [PK_PresencaDatas] PRIMARY KEY ([Id])
)
