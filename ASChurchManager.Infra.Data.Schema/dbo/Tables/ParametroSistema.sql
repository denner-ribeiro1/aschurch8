CREATE TABLE [dbo].[ParametroSistema] (
    [Id]                  INT           IDENTITY (1, 1) NOT NULL,
    [Nome]                VARCHAR (60)  NOT NULL,
    [Descricao]           VARCHAR (100) NOT NULL,
    [Valor]               VARCHAR (100) NULL,
    [Tabela]              VARCHAR (60)  NULL,
    [Colunas]             VARCHAR (100) NULL,
    [SqlWhere]            VARCHAR (MAX) NULL,
    [DataCriacao]         DATETIMEOFFSET      NULL,
    [DataUltimaAlteracao] DATETIMEOFFSET      NOT NULL,
    [Ativo]               BIT           DEFAULT ((1)) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

