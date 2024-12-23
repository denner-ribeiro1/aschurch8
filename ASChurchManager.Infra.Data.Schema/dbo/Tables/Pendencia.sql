CREATE TABLE [dbo].[Pendencia] (
    [Id]           INT                IDENTITY (1, 1) NOT NULL,
    [Tipo]         CHAR (1)           NOT NULL,
    [TipoChave]    CHAR (1)           NOT NULL,
    [Chave]        INT                NOT NULL,
    [UsuarioIdIni] INT                NOT NULL,
    [UsuarioIdFin] INT                NULL,
    [Observacao]   VARCHAR (500)      NULL,
    [DataIni]      DATETIMEOFFSET (7) NOT NULL,
    [DataFin]      DATETIMEOFFSET (7) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [CHECK_Tipo] CHECK ([Tipo]='R' OR [Tipo]='A'),
    CONSTRAINT [CHECK_TipoChave] CHECK ([TipoChave]='M' OR [TipoChave]='C')
);


