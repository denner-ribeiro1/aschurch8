CREATE TABLE [dbo].[CongregacaoObreiro]
(
	[MembroId] INT NOT NULL , 
    [CongregacaoId] INT NOT NULL, 
    [Dirigente] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [FK_CongregacaoObreiro_Membro] FOREIGN KEY (MembroId) REFERENCES Membro(Id), 
    CONSTRAINT [FK_CongregacaoObreiro_Congregacao] FOREIGN KEY (CongregacaoId) REFERENCES Congregacao(Id)
)

GO

CREATE INDEX [IX_CongregacaoObreiro_CongregacaoId] ON [dbo].[CongregacaoObreiro] (CongregacaoId)
