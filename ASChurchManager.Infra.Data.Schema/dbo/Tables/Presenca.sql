CREATE TABLE [dbo].[Presenca]
(
	[Id] INT IDENTITY(1, 1) NOT NULL PRIMARY KEY, 
    [Descricao] VARCHAR(255) NULL, 
    [TipoEventoId] INT NOT NULL,
    [DataMaxima] DATETIME NULL, 
    [Valor] FLOAT NULL,
    [ExclusivoCongregacao] BIT, 
    [NaoMembros] BIT, 
    [GerarEventos] BIT, 
    [InscricaoAutomatica] BIT NULL, 
    [CongregacaoId] INT NULL,
    [Status] TINYINT NULL,
    [DataAlteracao] DATETIME NULL, 
    [DataCriacao] DATETIME NULL,
    CONSTRAINT [FK_Presenca_TipoEvento] FOREIGN KEY ([TipoEventoId]) REFERENCES [dbo].[TipoEvento] ([Id])
)
