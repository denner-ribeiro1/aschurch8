CREATE TABLE [dbo].[Rotina] (
    [Id] INT IDENTITY (1, 1) NOT NULL,
    [Area] VARCHAR (50)  NOT NULL,
    [AreaDescricao] VARCHAR (50)  NULL,
	[AreaIcone] VARCHAR (50)  NULL,
    [Controller] VARCHAR (50)  NOT NULL,
	[Action] VARCHAR(50) NOT NULL,
    [MenuDescricao] VARCHAR (50)  NULL,
    [MenuIcone] VARCHAR (50)  NULL,
	[SubMenuDescricao] VARCHAR (50)  NULL,
	[SubMenuIcone] VARCHAR (50)  NULL,
	[DataCriacao] DATETIMEOFFSET NULL,
    [DataAlteracao] DATETIMEOFFSET NULL,	
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UQ_Rotina_Area] UNIQUE NONCLUSTERED ([Area] ASC, [Controller] ASC, [Action] ASC)
);

