CREATE TABLE [dbo].[Usuario] (
    [Id]								INT					IDENTITY (1, 1) NOT NULL,
    [Nome]								VARCHAR (100)		NOT NULL,
    [Username]							VARCHAR (30)		NOT NULL,
    [Senha]								VARCHAR (1000)		NOT NULL,
    [Email]								VARCHAR (50)		NULL,
    [AlterarSenhaProxLogin]				BIT					NULL,
	[PermiteAprovarMembro]				BIT					NULL,
	[PermiteImprimirCarteirinha]		BIT					NULL,
	[PermiteExcluirSituacaoMembro]		BIT					NULL,
	[PermiteCadBatismoAposDataMaxima]	BIT					NULL,
    [PermiteExcluirMembros]				BIT					NULL, 
	[DataCriacao]						DATETIMEOFFSET (7)	NULL,
    [DataAlteracao]						DATETIMEOFFSET (7)	NULL,
    [CongregacaoId]						INT					NULL,
    [Status]							TINYINT				DEFAULT ((1)) NULL,
    [PerfilId]							INT					NULL,
    [PermiteExcluirCargoMembro] BIT NULL, 
    [PermiteExcluirObservacaoMembro] BIT NULL, 
    [Skin] VARCHAR(30) NULL, 
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Usuario_CongregacaoId] FOREIGN KEY ([CongregacaoId]) REFERENCES [dbo].[Congregacao] ([Id]),
    CONSTRAINT [FK_Usuario_PerfilId] FOREIGN KEY ([PerfilId]) REFERENCES [dbo].[Perfil] ([Id]),
    CONSTRAINT [UQ_Usuario_UserName] UNIQUE NONCLUSTERED ([Username] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_Usuario_UserName_Senha]
    ON [dbo].[Usuario]([Username] ASC)
    INCLUDE([Senha]);

