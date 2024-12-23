CREATE TABLE [dbo].[PresencaInscricaoDatas]
(
	[InscricaoId] INT NOT NULL,
	[DataId] INT NOT NULL,
	[Situacao] TINYINT NOT NULL,
	[DataAlteracao] SMALLDATETIME NULL, 
    [Tipo] TINYINT NULL, 
    [Justificativa] VARCHAR(100) NULL, 
    CONSTRAINT [PK_PresencaInscricaoDatas] PRIMARY KEY CLUSTERED ([InscricaoId] ASC, [DataId] ASC),
	CONSTRAINT [FK_PresencaInscricaoDatas_PresencaDatas] FOREIGN KEY (DataId) REFERENCES PresencaDatas(Id)
)
