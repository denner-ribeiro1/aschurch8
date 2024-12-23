CREATE TABLE [dbo].[Carta] (
    [Id]                  INT                IDENTITY (1, 1) NOT NULL,
    [MembroId]            INT                NOT NULL,
    [TipoCarta]           TINYINT            NOT NULL,
    [CongregacaoOrigemId] INT                NULL,
    [CongregacaoDestId]   INT                NULL,
    [CongregacaoDest]     VARCHAR (100)      NULL,
    [Observacao]          VARCHAR (500)      NULL,
    [StatusCarta]         TINYINT            NULL,
    [DataValidade]        DATETIMEOFFSET (7) NOT NULL,
    [DataAlteracao]       DATETIMEOFFSET (7) NULL,
    [DataCriacao]         DATETIMEOFFSET (7) NULL,
    [CodigoRecebimento]   VARCHAR (30)       NULL,
    [TemplateId]          BIGINT             NULL,
	[DataAprovacao]		  DATETIMEOFFSET (7) NULL
    PRIMARY KEY CLUSTERED ([Id] ASC),
    [IdAprovacao] INT NULL, 
    [IdCadastro] INT NULL, 
    FOREIGN KEY ([MembroId]) REFERENCES [dbo].[Membro] ([Id])
);

