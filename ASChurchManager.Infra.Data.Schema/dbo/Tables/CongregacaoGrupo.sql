CREATE TABLE [dbo].[CongregacaoGrupo]
(
	[CongregacaoId] INT NOT NULL,
	[GrupoId] INT NOT NULL,
	[NomeGrupo] VARCHAR(100) NULL,
	[ResponsavelId] INT NULL,
	[DataCriacao] DATETIMEOFFSET NULL, 
    CONSTRAINT [FK_CongregacaoGrupo_Congregacao] FOREIGN KEY (CongregacaoId) REFERENCES Congregacao(Id), 
    CONSTRAINT [FK_CongregacaoGrupo_Grupo] FOREIGN KEY (GrupoId) REFERENCES Grupo(Id) 
)

GO

CREATE INDEX [IX_CongregacaoGrupo_CongregacaoId] ON [dbo].[CongregacaoGrupo] (CongregacaoId)
