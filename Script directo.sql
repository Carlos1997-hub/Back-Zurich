
IF DB_ID('ZurichApp') IS NOT NULL
BEGIN
    ALTER DATABASE ZurichApp SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE ZurichApp;
END
GO

CREATE DATABASE ZurichApp;
GO
USE ZurichApp;
GO

CREATE TABLE dbo.Roles
(
    RoleId      INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Roles PRIMARY KEY,
    RoleName    VARCHAR(30) NOT NULL CONSTRAINT UQ_Roles_RoleName UNIQUE
);
GO

CREATE TABLE dbo.Clients
(
    ClientId             INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Clients PRIMARY KEY,

    IdentificationNumber CHAR(10) NOT NULL CONSTRAINT UQ_Clients_Identification UNIQUE,
    FullName             NVARCHAR(150) NOT NULL,
    Email                VARCHAR(254) NOT NULL CONSTRAINT UQ_Clients_Email UNIQUE,
    Phone                VARCHAR(20) NOT NULL,
    Address              NVARCHAR(250) NOT NULL,

    IsDeleted            BIT NOT NULL CONSTRAINT DF_Clients_IsDeleted DEFAULT(0),
    CreatedAt            DATETIME2(0) NOT NULL CONSTRAINT DF_Clients_CreatedAt DEFAULT SYSUTCDATETIME(),
    UpdatedAt            DATETIME2(0) NOT NULL CONSTRAINT DF_Clients_UpdatedAt DEFAULT SYSUTCDATETIME(),

    CONSTRAINT CK_Clients_Identification_10Digits
        CHECK (IdentificationNumber NOT LIKE '%[^0-9]%'),

    CONSTRAINT CK_Clients_FullName_NoDigitsOrSpecial
        CHECK (
            FullName NOT LIKE '%[0-9]%' AND
            FullName NOT LIKE '%[^A-Za-zÁÉÍÓÚÜÑáéíóúüñ ''-]%' COLLATE Latin1_General_100_CI_AI
        ),

    CONSTRAINT CK_Clients_Email_Basic
        CHECK (Email LIKE '%_@_%._%')
);
GO

CREATE INDEX IX_Clients_IdentificationNumber ON dbo.Clients (IdentificationNumber);
CREATE INDEX IX_Clients_Email ON dbo.Clients (Email);
CREATE INDEX IX_Clients_FullName ON dbo.Clients (FullName);
GO

CREATE TABLE dbo.Users
(
    UserId          INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Users PRIMARY KEY,
    Username        VARCHAR(50) NOT NULL CONSTRAINT UQ_Users_Username UNIQUE,
    Email           VARCHAR(254) NOT NULL CONSTRAINT UQ_Users_Email UNIQUE,
    PasswordHash    VARBINARY(64) NOT NULL,
    PasswordSalt    VARBINARY(32) NOT NULL,
    RoleId          INT NOT NULL,
    ClientId        INT NULL,
    DisplayName     NVARCHAR(150) NULL,
    IsActive        BIT NOT NULL CONSTRAINT DF_Users_IsActive DEFAULT(1),
    CreatedAt       DATETIME2(0) NOT NULL CONSTRAINT DF_Users_CreatedAt DEFAULT SYSUTCDATETIME(),
    UpdatedAt       DATETIME2(0) NOT NULL CONSTRAINT DF_Users_UpdatedAt DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) REFERENCES dbo.Roles(RoleId),
    CONSTRAINT FK_Users_Clients FOREIGN KEY (ClientId) REFERENCES dbo.Clients(ClientId),
    CONSTRAINT CK_Users_Email_Basic CHECK (Email LIKE '%_@_%._%')
);
GO

CREATE INDEX IX_Users_RoleId ON dbo.Users (RoleId);
CREATE INDEX IX_Users_ClientId ON dbo.Users (ClientId);
GO

CREATE TABLE dbo.Policies
(
    PolicyId        INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Policies PRIMARY KEY,

    ClientId        INT NOT NULL,
    CreatedByUserId INT NOT NULL,

    PolicyType      VARCHAR(20) NOT NULL,
    StartDate       DATE NOT NULL,
    ExpirationDate  DATE NOT NULL,
    InsuredAmount   DECIMAL(18,2) NOT NULL,

    PolicyStatus    VARCHAR(10) NOT NULL CONSTRAINT DF_Policies_Status DEFAULT('Activa'),
    AssignedAt      DATETIME2(0) NOT NULL CONSTRAINT DF_Policies_AssignedAt DEFAULT SYSUTCDATETIME(),
    CancelledAt     DATETIME2(0) NULL,

    CreatedAt       DATETIME2(0) NOT NULL CONSTRAINT DF_Policies_CreatedAt DEFAULT SYSUTCDATETIME(),
    UpdatedAt       DATETIME2(0) NOT NULL CONSTRAINT DF_Policies_UpdatedAt DEFAULT SYSUTCDATETIME(),

    CONSTRAINT FK_Policies_Clients FOREIGN KEY (ClientId) REFERENCES dbo.Clients(ClientId),
    CONSTRAINT FK_Policies_CreatedBy FOREIGN KEY (CreatedByUserId) REFERENCES dbo.Users(UserId),

    CONSTRAINT CK_Policies_Type CHECK (PolicyType IN ('Vida','Automóvil','Salud','Hogar')),
    CONSTRAINT CK_Policies_Dates CHECK (ExpirationDate > StartDate),
    CONSTRAINT CK_Policies_Amount CHECK (InsuredAmount > 0),
    CONSTRAINT CK_Policies_Status CHECK (PolicyStatus IN ('Activa','Cancelada'))
);
GO

CREATE INDEX IX_Policies_ClientId ON dbo.Policies (ClientId);
CREATE INDEX IX_Policies_CreatedByUserId ON dbo.Policies (CreatedByUserId);
CREATE INDEX IX_Policies_Type_Status_Dates ON dbo.Policies (PolicyType, PolicyStatus, StartDate, ExpirationDate);
GO

CREATE TABLE dbo.Quotes
(
    QuoteId         INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Quotes PRIMARY KEY,
    ClientId        INT NOT NULL,

    PolicyType      VARCHAR(20) NOT NULL,
    InsuredAmount   DECIMAL(18,2) NOT NULL,
    TermMonths      INT NOT NULL,
    MonthlyPremium  DECIMAL(18,2) NOT NULL,

    QuoteStatus     VARCHAR(15) NOT NULL CONSTRAINT DF_Quotes_Status DEFAULT('Generada'),
    Notes           NVARCHAR(300) NULL,

    CreatedAt       DATETIME2(0) NOT NULL CONSTRAINT DF_Quotes_CreatedAt DEFAULT SYSUTCDATETIME(),
    UpdatedAt       DATETIME2(0) NOT NULL CONSTRAINT DF_Quotes_UpdatedAt DEFAULT SYSUTCDATETIME(),

    CONSTRAINT FK_Quotes_Clients FOREIGN KEY (ClientId) REFERENCES dbo.Clients(ClientId),

    CONSTRAINT CK_Quotes_Type CHECK (PolicyType IN ('Vida','Automóvil','Salud','Hogar')),
    CONSTRAINT CK_Quotes_Amount CHECK (InsuredAmount > 0),
    CONSTRAINT CK_Quotes_Term CHECK (TermMonths > 0),
    CONSTRAINT CK_Quotes_Premium CHECK (MonthlyPremium > 0),
    CONSTRAINT CK_Quotes_Status CHECK (QuoteStatus IN ('Generada','Enviada','Aceptada','Rechazada','Expirada'))
);
GO

CREATE INDEX IX_Quotes_ClientId_CreatedAt ON dbo.Quotes (ClientId, CreatedAt DESC);
CREATE INDEX IX_Quotes_Status_Type ON dbo.Quotes (QuoteStatus, PolicyType);
GO

CREATE TABLE dbo.PolicyCancellationRequests
(
    RequestId         INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_PolicyCancellationRequests PRIMARY KEY,
    PolicyId          INT NOT NULL,

    RequestedAt       DATETIME2(0) NOT NULL CONSTRAINT DF_CancelReq_RequestedAt DEFAULT SYSUTCDATETIME(),
    RequestedByUserId INT NOT NULL,

    Status            VARCHAR(15) NOT NULL CONSTRAINT DF_CancelReq_Status DEFAULT('Pendiente'),
    ProcessedAt       DATETIME2(0) NULL,
    ProcessedByUserId INT NULL,
    Notes             NVARCHAR(300) NULL,

    CONSTRAINT FK_CancelReq_Policies FOREIGN KEY (PolicyId) REFERENCES dbo.Policies(PolicyId),
    CONSTRAINT FK_CancelReq_RequestedBy FOREIGN KEY (RequestedByUserId) REFERENCES dbo.Users(UserId),
    CONSTRAINT FK_CancelReq_ProcessedBy FOREIGN KEY (ProcessedByUserId) REFERENCES dbo.Users(UserId),
    CONSTRAINT CK_CancelReq_Status CHECK (Status IN ('Pendiente','Aprobada','Rechazada'))
);
GO

CREATE INDEX IX_CancelReq_PolicyId ON dbo.PolicyCancellationRequests (PolicyId);
CREATE INDEX IX_CancelReq_Status ON dbo.PolicyCancellationRequests (Status);
GO


INSERT INTO dbo.Roles (RoleName) VALUES ('Administrador'), ('Cliente');
GO

DECLARE @AdminRoleId  INT = (SELECT TOP 1 RoleId FROM dbo.Roles WHERE RoleName = 'Administrador');
DECLARE @ClientRoleId INT = (SELECT TOP 1 RoleId FROM dbo.Roles WHERE RoleName = 'Cliente');

INSERT INTO dbo.Users
(
    Username, Email, PasswordHash, PasswordSalt,
    RoleId, ClientId, DisplayName, IsActive
)
VALUES
(
    'admin',
    'admin@zurich.com',
    0x719041cb0818c0083f7e9f49784c1f134f893fc8fe713800a020d76e1343e23997b831d1b49391d730e6d81558a3559b1b072690fc7099698b451957e296a009,
    0x5ba3ab9fddff18ac6faf1a2b494e71ccf99d88955519b82d06abea61cf25bc89,
    @AdminRoleId,
    NULL,
    N'Administrador Zurich',
    1
);

INSERT INTO dbo.Clients (IdentificationNumber, FullName, Email, Phone, Address)
VALUES ('0102030405', N'Cliente Demo Uno', 'cliente1@demo.com', '0999999999', N'Av. Demo 123');

DECLARE @ClientIdDemo INT = SCOPE_IDENTITY();

INSERT INTO dbo.Users
(
    Username, Email, PasswordHash, PasswordSalt,
    RoleId, ClientId, DisplayName, IsActive
)
VALUES
(
    'cliente1',
    'cliente1@demo.com',
    0xfae47d6cbee2c4c43c5cdaeafaa8445c294f3f213dd4d9e0424d77d5df88a921ecbe03908545e143d775760f425d9565ca9a4bb571778a48b880e48a71b7ae40,
    0xdf1417552799057a6ff668cc7fdcb2f885c2317187102a2c6bc160b3495f87a5,
    @ClientRoleId,
    @ClientIdDemo,
    N'Cliente Demo Uno',
    1
);
GO
