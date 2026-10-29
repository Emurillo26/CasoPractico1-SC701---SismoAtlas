-- =============================================
-- SismoAtlas - Caso Práctico 1 (SC-701) - Grupo 4
-- Script de creación de la base de datos
-- =============================================

IF DB_ID('SismoAtlasDB') IS NULL
    CREATE DATABASE SismoAtlasDB;
GO

USE SismoAtlasDB;
GO

IF OBJECT_ID('dbo.Earthquakes', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Earthquakes
    (
        Id            NVARCHAR(50)   NOT NULL,  -- id que trae USGS
        Magnitude     DECIMAL(4,2)   NULL,      -- properties.mag
        Place         NVARCHAR(250)  NULL,      -- properties.place
        Country       NVARCHAR(100)  NULL,      -- sacado de "place"
        [Time]        DATETIME2      NOT NULL,  -- properties.time
        Updated       DATETIME2      NULL,      -- properties.updated
        Url           NVARCHAR(500)  NULL,      -- properties.url
        Felt          INT            NULL,      -- properties.felt
        Alert         NVARCHAR(20)   NULL,      -- properties.alert
        Status        NVARCHAR(20)   NULL,      -- properties.status
        Tsunami       BIT            NOT NULL DEFAULT 0, -- properties.tsunami
        Significance  INT            NULL,      -- properties.sig
        MagType       NVARCHAR(10)   NULL,      -- properties.magType
        Title         NVARCHAR(300)  NULL,      -- properties.title
        Longitude     FLOAT          NOT NULL,  -- coordinates[0]
        Latitude      FLOAT          NOT NULL,  -- coordinates[1]
        Depth         FLOAT          NULL,      -- coordinates[2]

        CONSTRAINT PK_Earthquakes PRIMARY KEY (Id)
    );

    -- Para buscar rápido por país
    CREATE INDEX IX_Earthquakes_Country ON dbo.Earthquakes (Country);

    -- Para saber cuál es el sismo más reciente guardado
    CREATE INDEX IX_Earthquakes_Time ON dbo.Earthquakes ([Time]);
END
GO