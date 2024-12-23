CREATE TABLE [dbo].[Congregacao] (
    [Id]                       INT           IDENTITY (1, 1) NOT NULL,
    [Nome]                     VARCHAR (50)  NOT NULL,
    [Logradouro]               VARCHAR (100)      NULL,
    [Numero]                   VARCHAR (10)       NULL,
    [Complemento]              VARCHAR (100)      NULL,
    [Bairro]                   VARCHAR (100)      NULL,
    [Cidade]                   VARCHAR (100)      NULL,
    [Estado]                   VARCHAR (30)        NULL,
	[Pais]				       VARCHAR (100)      NULL,
    [Cep]                      VARCHAR (15)       NULL,
    [DataCriacao]              DATETIMEOFFSET      NOT NULL,
    [DataAlteracao]            DATETIMEOFFSET      NOT NULL,
    [Sede]                     BIT           DEFAULT (0) NOT NULL,
    [CongregacaoResponsavelId] INT           NOT NULL,
    [PastorResponsavelId]                 INT           NULL,
	[CNPJ]						VARCHAr(18)	NULL
    CONSTRAINT [Congregacao_PK] PRIMARY KEY CLUSTERED ([Id] ASC), 
    [Situacao] BIT DEFAULT (1) NOT NULL
);




