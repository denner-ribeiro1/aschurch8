CREATE TABLE [dbo].[Estado] (
    [Id]        INT          IDENTITY (1, 1) NOT NULL,
    [UF]        CHAR (2)     NOT NULL,
    [Descricao] VARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    UNIQUE NONCLUSTERED ([UF] ASC)
);

