CREATE TABLE [dbo].[CargoMembro] (
    [Id]          INT                IDENTITY (1, 1) NOT NULL,
    [MembroId]    INT                NOT NULL,
    [CargoId]     INT                NOT NULL,
	[LocalConsagracao] VARCHAR(50)   NULL,
	[DataCargo]	  DATETIMEOFFSET (7) NULL,
    [DataCriacao] DATETIMEOFFSET (7) NOT NULL,
    [Confradesp] VARCHAR(15) NULL, 
    [CGADB] VARCHAR(15) NULL, 
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CargoMembro_Cargo] FOREIGN KEY ([CargoId]) REFERENCES [dbo].[Cargo] ([Id]),
    CONSTRAINT [FK_CargoMembro_Membro] FOREIGN KEY ([MembroId]) REFERENCES [dbo].[Membro] ([Id])
);

