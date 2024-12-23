CREATE TABLE [dbo].[PerfilRotina] (
    [IdPerfil] INT NOT NULL,
    [IdRotina] INT NOT NULL,
    CONSTRAINT [PK_PerfilRotina_IdPerfil] PRIMARY KEY CLUSTERED ([IdPerfil] ASC, [IdRotina] ASC)
);

