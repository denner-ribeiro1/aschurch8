CREATE TABLE [dbo].[Perfil] (
    [Id]            INT                IDENTITY (1, 1) NOT NULL,
    [Nome]          VARCHAR (60)       NOT NULL,
    [TipoPerfil]    TINYINT            NOT NULL,
    [Status]        BIT                NOT NULL,
    [DataCriacao]   DATETIMEOFFSET (7) NOT NULL,
    [DataAlteracao] DATETIMEOFFSET (7) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

