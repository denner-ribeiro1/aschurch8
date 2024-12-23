CREATE TABLE [dbo].[BatismoCandidato]
(
    [BatismoId] INT NOT NULL, 
    [MembroId] INT NOT NULL, 
    [Situacao] TINYINT NULL, 
    [DataConfirmacao] DATETIMEOFFSET NULL,
	CONSTRAINT [FK_BatismoCandidato_Batismo] FOREIGN KEY ([BatismoId]) REFERENCES [Batismo]([Id]),
	CONSTRAINT [FK_BatismoCandidato_Membro] FOREIGN KEY ([MembroId]) REFERENCES [Membro]([Id]),
	CONSTRAINT [PK_BatismoCandidato] PRIMARY KEY(BatismoId, MembroId)
)
