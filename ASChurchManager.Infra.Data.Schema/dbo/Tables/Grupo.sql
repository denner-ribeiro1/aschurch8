﻿CREATE TABLE [dbo].[Grupo]
(
	[Id] INT NOT NULL IDENTITY(1, 1) PRIMARY KEY, 
    [Descricao] VARCHAR(100) NOT NULL, 
    [DataCriacao] DATETIMEOFFSET NULL, 
    [DataAlteracao] DATETIMEOFFSET NULL,
)
