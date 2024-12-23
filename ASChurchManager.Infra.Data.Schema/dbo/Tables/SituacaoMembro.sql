CREATE TABLE [dbo].[SituacaoMembro] (
    [Id]            INT                IDENTITY (1, 1) NOT NULL,
    [MembroId]      INT                NOT NULL,
    [Situacao]      INT                NOT NULL,
    [Data]          DATETIMEOFFSET (7) NOT NULL,
    [Observacao]    VARCHAR (200)      NULL,
    [DataCriacao]   DATETIMEOFFSET (7) NOT NULL,
    [DataAlteracao] DATETIMEOFFSET (7) NOT NULL,
	[SituacaoAnterior]		INT,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_SituacaoMembro_Membro] FOREIGN KEY ([MembroId]) REFERENCES [dbo].[Membro] ([Id])
);




