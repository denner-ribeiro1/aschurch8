CREATE TABLE [dbo].[Nascimento]
(
	Id INT IDENTITY(1, 1) NOT NULL,
	IdMembroPai INT,
	IdMembroMae INT,
    NomePai VARCHAR(100),
	NomeMae VARCHAR(100),
	Crianca VARCHAR(100),
	Pastor VARCHAR(100),
	PastorId INT,
	Sexo CHAR(1),
	DataNascimento DATETIMEOFFSET NULL,
	DataApresentacao DATETIMEOFFSET NULL,
	CongregacaoID INT NOT NULL,
	DataCriacao   DATETIMEOFFSET NULL,
    DataAlteracao DATETIMEOFFSET NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
	)