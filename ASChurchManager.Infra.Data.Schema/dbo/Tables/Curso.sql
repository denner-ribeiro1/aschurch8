CREATE TABLE [dbo].[Curso]
(
	[Id] INT IDENTITY (1, 1) NOT NULL,
	[Descricao] VARCHAR(100) NULL, 
    [DataInicio] DATETIMEOFFSET NULL, 
    [DataEncerramento] DATETIMEOFFSET NULL, 
    [CargaHoraria] INT NULL, 
    [DataCriacao] DATETIMEOFFSET NULL, 
    [DataAlteracao] DATETIMEOFFSET NULL, 
    CONSTRAINT [Curso_PK] PRIMARY KEY CLUSTERED ([Id] ASC) 
)
