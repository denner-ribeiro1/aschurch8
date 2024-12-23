CREATE TABLE [dbo].[Evento] (
    [Id]             BIGINT             IDENTITY (1, 1) NOT NULL,
    [CongregacaoId]  INT                NOT NULL,
    [TipoEventoId]   INT                NOT NULL,
    [Descricao]      VARCHAR (100)      NOT NULL,
    [DataHoraInicio] DATETIMEOFFSET (7) NOT NULL,
    [DataHoraFim]    DATETIMEOFFSET (7) NOT NULL,
    [Observacoes] VARCHAR(1000) NULL, 
    [IdEventoOriginal]	BIGINT,
    [Frequencia] TINYINT NULL, 
    [Quantidade] INT NULL, 
    [AlertarEventoMesmoDia] BIT NULL, 
    [DataCriacao]    DATETIMEOFFSET (7) NULL,
    [DataAlteracao]  DATETIMEOFFSET (7) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Evento_Congregacao] FOREIGN KEY ([CongregacaoId]) REFERENCES [dbo].[Congregacao] ([Id]),
    CONSTRAINT [FK_Evento_TipoEvento] FOREIGN KEY ([TipoEventoId]) REFERENCES [dbo].[TipoEvento] ([Id])
);



