CREATE TABLE [dbo].[Cargo] (
    [Id]            INT                IDENTITY (1, 1) NOT NULL,
    [Descricao]     VARCHAR (100)      NULL,
    [DataCriacao]   DATETIMEOFFSET (7) NULL,
    [DataAlteracao] DATETIMEOFFSET (7) NULL,
    [Obreiro]       BIT                NULL,
    [Lider]         BIT                NULL,
	[TipoCarteirinha] TINYINT		   NULL,
    [Confradesp] BIT NULL, 
    [CGADB] BIT NULL, 
    CONSTRAINT [Cargo_PK] PRIMARY KEY CLUSTERED ([Id] ASC)
);

