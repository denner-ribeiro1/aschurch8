CREATE TABLE [dbo].[ObservacaoMembro]
(
	[Id] INT IDENTITY( 1,1) NOT NULL PRIMARY KEY, 
    [MembroId] INT NOT NULL, 
	[Observacao] VARCHAR(500) NULL, 
    [UsuarioId] INT NULL, 
    [DataCadastro] DATETIMEOFFSET NULL,
	CONSTRAINT [FK_ObservacaoMembro_Membro] FOREIGN KEY ([MembroId]) REFERENCES [dbo].[Membro] ([Id])
)

