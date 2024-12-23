CREATE TABLE [dbo].Casamento
(
	Id INT IDENTITY(1, 1) NOT NULL,
	CongregacaoId INT,
	PastorId INT,
	PastorNome VARCHAR(100),
	DataHoraInicio DATETIMEOFFSET,
	DataHoraFinal DATETIMEOFFSET,
	NoivoId INT,
	NoivoNome VARCHAR(100),
	PaiNoivoId INT,
	PaiNoivoNome VARCHAR(100),
	MaeNoivoId INT,
	MaeNoivoNome VARCHAR(100),
	NoivaId INT,
	NoivaNome VARCHAR(100),
	PaiNoivaId INT,
	PaiNoivaNome VARCHAR(100),
	MaeNoivaId INT,
	MaeNoivaNome VARCHAR(100),
	DataCriacao   DATETIMEOFFSET NULL,
    DataAlteracao DATETIMEOFFSET NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
)