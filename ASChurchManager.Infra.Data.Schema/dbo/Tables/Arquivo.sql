CREATE TABLE [dbo].[Arquivo] (
    [Id]            INT           IDENTITY (1, 1) NOT NULL,
    [Nome]          VARCHAR (100) NOT NULL,
    [Extensao]      VARCHAR (10)  NULL,
    [TipoConteudo]  VARCHAR (100) NULL,
    [Conteudo]      IMAGE         NULL,
    [CaminhoDisco]  VARCHAR (100) NULL,
    [DataCriacao]   DATETIMEOFFSET      NULL,
    [DataAlteracao] DATETIMEOFFSET      NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

