CREATE TABLE [dbo].[Template] (
    [Id]            BIGINT             IDENTITY (1, 1) NOT NULL,
    [Nome]          VARCHAR (100)      NOT NULL,
    [Conteudo]      VARCHAR (8000)     NOT NULL,
    [Tipo]          TINYINT            NOT NULL,
    [Status]        BIT                NULL,
    [DataCriacao]   DATETIMEOFFSET (7) NULL,
    [DataAlteracao] DATETIMEOFFSET (7) NULL,
    [MargemAbaixo] INT NULL, 
    [MargemAcima] INT NULL, 
    [MargemDireita] INT NULL, 
    [MargemEsquerda] INT NULL, 
    PRIMARY KEY CLUSTERED ([Id] ASC)
);



