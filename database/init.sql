/*
    APBD - Æwiczenia 7
    Mini Helpdesk

    Skrypt inicjalizuj¹cy bazê danych
*/

------------------------------------------------------------
-- Utworzenie bazy
------------------------------------------------------------

IF DB_ID(N'APBD-CW7') IS NULL
BEGIN
    CREATE DATABASE [APBD-CW7];
END
GO

USE [APBD-CW7];
GO

------------------------------------------------------------
-- Usuniêcie istniej¹cych tabel
------------------------------------------------------------

IF OBJECT_ID('dbo.TicketComments', 'U') IS NOT NULL
    DROP TABLE dbo.TicketComments;
GO

IF OBJECT_ID('dbo.Tickets', 'U') IS NOT NULL
    DROP TABLE dbo.Tickets;
GO

------------------------------------------------------------
-- Tabela Tickets
------------------------------------------------------------

CREATE TABLE dbo.Tickets
(
    Id INT IDENTITY(1,1) NOT NULL
        CONSTRAINT PK_Tickets PRIMARY KEY,

    Title NVARCHAR(200) NOT NULL,

    Description NVARCHAR(MAX) NULL,

    Status NVARCHAR(20) NOT NULL,

    CreatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_Tickets_CreatedAt DEFAULT SYSUTCDATETIME()
);
GO

------------------------------------------------------------
-- Tabela TicketComments
------------------------------------------------------------

CREATE TABLE dbo.TicketComments
(
    Id INT IDENTITY(1,1) NOT NULL
        CONSTRAINT PK_TicketComments PRIMARY KEY,

    TicketId INT NOT NULL,

    Author NVARCHAR(100) NOT NULL,

    Content NVARCHAR(MAX) NOT NULL,

    CreatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_TicketComments_CreatedAt DEFAULT SYSUTCDATETIME(),

    CONSTRAINT FK_TicketComments_Tickets
        FOREIGN KEY (TicketId)
        REFERENCES dbo.Tickets(Id)
        ON DELETE CASCADE
);
GO

------------------------------------------------------------
-- Indeks na kluczu obcym
------------------------------------------------------------

CREATE INDEX IX_TicketComments_TicketId
ON dbo.TicketComments(TicketId);
GO

------------------------------------------------------------
-- Dane testowe
------------------------------------------------------------

INSERT INTO dbo.Tickets
(
    Title,
    Description,
    Status
)
VALUES
(
    N'Nie dzia³a drukarka',
    N'Drukarka w pokoju 105 nie drukuje.',
    N'Open'
);

INSERT INTO dbo.Tickets
(
    Title,
    Description,
    Status
)
VALUES
(
    N'Problem z VPN',
    N'Brak mo¿liwoœci po³¹czenia z VPN.',
    N'Open'
);

INSERT INTO dbo.TicketComments
(
    TicketId,
    Author,
    Content
)
VALUES
(
    1,
    N'Jan Kowalski',
    N'Drukarka pokazuje b³¹d Paper Jam.'
);

INSERT INTO dbo.TicketComments
(
    TicketId,
    Author,
    Content
)
VALUES
(
    2,
    N'Anna Nowak',
    N'Problem wystêpuje od rana.'
);

INSERT INTO dbo.TicketComments
(
    TicketId,
    Author,
    Content
)
VALUES
(
    2,
    N'Jan Kowalski',
    N'Problem nadal wystêpuje.'
);
GO

------------------------------------------------------------
-- Koniec skryptu
------------------------------------------------------------