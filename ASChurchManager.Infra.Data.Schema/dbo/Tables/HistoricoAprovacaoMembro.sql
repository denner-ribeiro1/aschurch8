CREATE TABLE [dbo].[HistoricoAprovacaoMembro] (
    [Id]              INT                IDENTITY (1, 1) NOT NULL,
    [MembroId]        INT                NOT NULL,
    [UsuarioId]       INT                NOT NULL,
    [MotivoReprovacao]      VARCHAR (500)      NULL,
    [StatusAprovacao] CHAR(1)           NOT NULL,
    [DataCriacao]     DATETIMEOFFSET (7) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_HistoricoAprovacaoMembro_MembroId] FOREIGN KEY ([MembroId]) REFERENCES [dbo].[Membro] ([Id]),
    CONSTRAINT [FK_HistoricoAprovacaoMembro_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [dbo].[Usuario] ([Id])
);

