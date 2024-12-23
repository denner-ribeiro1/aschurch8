CREATE TABLE [dbo].[BatismoCelebrante]
(
    [BatismoId] INT NOT NULL, 
    [MembroId] INT NOT NULL
)

GO
CREATE CLUSTERED INDEX BatismoCelebrante_Index ON BatismoCelebrante (BatismoId)

