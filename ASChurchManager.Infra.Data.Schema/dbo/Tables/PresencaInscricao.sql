CREATE TABLE [dbo].[PresencaInscricao]
(
	[Id] INT IDENTITY(1, 1) NOT NULL,
    [PresencaId] INT NOT NULL, 
    [MembroId] INT NULL, 
    [Nome] VARCHAR(100) NULL, 
    [CPF] VARCHAR(15) NULL, 
    [Igreja] VARCHAR(100) NULL, 
    [Cargo] VARCHAR(100) NULL,
    [Pago] BIT NULL, 
    [DataCriacao] DATETIME NULL, 
    [DataAlteracao] DATETIME NULL, 
    [Usuario] INT NULL, 
    [ArquivoId] INT NULL, 
    [CongregacaoId] INT NULL, 
    CONSTRAINT [PK_PresencaInscricao] PRIMARY KEY  CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_PresencaInscricao] FOREIGN KEY ([PresencaId]) REFERENCES [dbo].[Presenca] ([Id])
)

GO

CREATE INDEX [IX_PresencaInscricao_PresencaId] ON [dbo].[PresencaInscricao] ([PresencaId], [MembroId])

GO
