CREATE PROCEDURE [dbo].[ConsultarPresencaFichaMembro]
	@MembroId int
AS
BEGIN
    SELECT 
        P.Id, 
        P.Descricao, 
        PD.DataHoraInicio, 
        ISNULL(PID.Situacao, 0) AS Situacao, 
        PID.Justificativa 
    FROM Presenca P
    INNER JOIN PresencaDatas PD ON PD.PresencaId = P.Id
    INNER JOIN PresencaInscricao PI ON PI.PresencaId = P.Id
    LEFT JOIN PresencaInscricaoDatas PID ON PID.DataId = PD.Id AND PID.InscricaoId = PI.Id
    WHERE MembroId = @MembroId
    ORDER BY P.Id, PD.DataHoraInicio
END