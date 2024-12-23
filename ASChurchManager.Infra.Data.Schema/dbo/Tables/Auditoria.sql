CREATE TABLE [dbo].[Auditoria] (
    [Id]         BIGINT             IDENTITY (1, 1) NOT NULL,
    [UsuarioId]  BIGINT             NOT NULL,
    [Controle]   VARCHAR (50)       NULL,
    [Acao]       VARCHAR (50)       NULL,
    [Ip]         VARCHAR (50)       NULL,
    [Url]        VARCHAR (100)      NULL,
    [DataHora]   DATETIMEOFFSET (7) NULL,
    [Parametros] VARCHAR (MAX)      NULL,
    [Navegador] VARCHAR(200) NULL, 
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


