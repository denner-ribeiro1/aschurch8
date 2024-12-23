CREATE TABLE [dbo].[CursoArquivoMembro]
(
	[Id] INT IDENTITY (1, 1) NOT NULL, 
    [MembroId] INT NOT NULL, 
	[TipoArquivo] TINYINT NOT NULL, 
	[Descricao] VARCHAR(200) NULL, 
    [CursoId] INT NULL, 
    [Local] VARCHAR(50) NULL, 
    [NomeCurso] VARCHAR(50) NULL, 
    [DataInicioCurso] DATETIMEOFFSET NULL, 
    [DataEncerramentoCurso] DATETIMEOFFSET NULL, 
    [CargaHoraria] INT NULL, 
    [NomeArmazenado] VARCHAR(100) NULL, 
    [NomeOriginal] VARCHAR(100) NULL, 
    [Tamanho] INT NULL, 
	[DataCriacao] DATETIMEOFFSET NULL, 
	CONSTRAINT [CursoArquivoMembro_PK] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CursoArquivoMembro_Membro] FOREIGN KEY (MembroId) REFERENCES Membro(Id)
)
