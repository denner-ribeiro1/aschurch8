CREATE TABLE [dbo].[ConviteEvento]
(
	[Id] INT NOT NULL IDENTITY(1, 1) PRIMARY KEY, 
    [EventoId] INT NOT NULL, 
    [CongregacaoId] INT NOT NULL,
    [GrupoId] INT NULL, 
	[ConvidadoId] INT NULL 
)
