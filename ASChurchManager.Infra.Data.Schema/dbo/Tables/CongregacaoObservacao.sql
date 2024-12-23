CREATE TABLE [dbo].[CongregacaoObservacao]
(
    [CongregacaoId] INT NOT NULL, 
    [Observacao] VARCHAR(300) NULL, 
    [UsuarioId] INT NULL, 
    [DataCadastro] SMALLDATETIME NULL,
    CONSTRAINT [FK_CongregacaoObservacao_Congregacao] FOREIGN KEY (CongregacaoId) REFERENCES Congregacao(Id)
)
GO
CREATE INDEX [IX_CongregacaoObservacao_CongregacaoId] ON [dbo].[CongregacaoObservacao] (CongregacaoId)
