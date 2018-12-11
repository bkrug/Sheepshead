
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 12/10/2018 22:15:46
-- Generated from EDMX file: F:\Users\bjkrug\Documents\Visual Studio 2017\Projects\Sheepshead\Sheepshead.Model\Models\SheepsheadModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [Sheepshead];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_Hand_Game]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Hands] DROP CONSTRAINT [FK_Hand_Game];
GO
IF OBJECT_ID(N'[dbo].[FK_Player_Game]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Players] DROP CONSTRAINT [FK_Player_Game];
GO
IF OBJECT_ID(N'[dbo].[FK_Hand_Partner]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Hands] DROP CONSTRAINT [FK_Hand_Partner];
GO
IF OBJECT_ID(N'[dbo].[FK_Hand_Picker]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Hands] DROP CONSTRAINT [FK_Hand_Picker];
GO
IF OBJECT_ID(N'[dbo].[FK_Hand_StartingPlayer]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Hands] DROP CONSTRAINT [FK_Hand_StartingPlayer];
GO
IF OBJECT_ID(N'[dbo].[FK_Trick_Hand]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Tricks] DROP CONSTRAINT [FK_Trick_Hand];
GO
IF OBJECT_ID(N'[dbo].[FK_Trick_Player]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Tricks] DROP CONSTRAINT [FK_Trick_Player];
GO
IF OBJECT_ID(N'[dbo].[FK_CardsPlayed_Player]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CardsPlayed] DROP CONSTRAINT [FK_CardsPlayed_Player];
GO
IF OBJECT_ID(N'[dbo].[FK_CardsPlayed_Trick]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[CardsPlayed] DROP CONSTRAINT [FK_CardsPlayed_Trick];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Games]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Games];
GO
IF OBJECT_ID(N'[dbo].[Hands]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Hands];
GO
IF OBJECT_ID(N'[dbo].[Players]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Players];
GO
IF OBJECT_ID(N'[dbo].[Tricks]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Tricks];
GO
IF OBJECT_ID(N'[dbo].[CardsPlayed]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CardsPlayed];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Games'
CREATE TABLE [dbo].[Games] (
    [Id] uniqueidentifier  NOT NULL,
    [LeastersEnabled] bit  NOT NULL,
    [PartnerMethod] char(1)  NOT NULL
);
GO

-- Creating table 'Hands'
CREATE TABLE [dbo].[Hands] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Blinds] char(6)  NOT NULL,
    [Buried] char(6)  NOT NULL,
    [RefusingPick] varchar(max)  NOT NULL,
    [StartingParticipantId] int  NOT NULL,
    [PickerId] int  NULL,
    [PartnerId] int  NULL,
    [PartnerCard] char(2)  NULL,
    [GameId] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'Participant'
CREATE TABLE [dbo].[Participant] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] varchar(max)  NOT NULL,
    [Cards] char(36)  NOT NULL,
    [GameId] uniqueidentifier  NOT NULL,
    [Type] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Tricks'
CREATE TABLE [dbo].[Tricks] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [HandId] int  NOT NULL,
    [StartingParticipantId] int  NOT NULL
);
GO

-- Creating table 'CardsPlayed'
CREATE TABLE [dbo].[CardsPlayed] (
    [ParticipantId] int  NOT NULL,
    [TrickId] int  NOT NULL,
    [Card] char(2)  NOT NULL,
    [SortOrder] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Games'
ALTER TABLE [dbo].[Games]
ADD CONSTRAINT [PK_Games]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Hands'
ALTER TABLE [dbo].[Hands]
ADD CONSTRAINT [PK_Hands]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Participant'
ALTER TABLE [dbo].[Participant]
ADD CONSTRAINT [PK_Participant]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Tricks'
ALTER TABLE [dbo].[Tricks]
ADD CONSTRAINT [PK_Tricks]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [ParticipantId], [TrickId] in table 'CardsPlayed'
ALTER TABLE [dbo].[CardsPlayed]
ADD CONSTRAINT [PK_CardsPlayed]
    PRIMARY KEY CLUSTERED ([ParticipantId], [TrickId] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [GameId] in table 'Hands'
ALTER TABLE [dbo].[Hands]
ADD CONSTRAINT [FK_Hand_Game]
    FOREIGN KEY ([GameId])
    REFERENCES [dbo].[Games]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Hand_Game'
CREATE INDEX [IX_FK_Hand_Game]
ON [dbo].[Hands]
    ([GameId]);
GO

-- Creating foreign key on [GameId] in table 'Participant'
ALTER TABLE [dbo].[Participant]
ADD CONSTRAINT [FK_Player_Game]
    FOREIGN KEY ([GameId])
    REFERENCES [dbo].[Games]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Player_Game'
CREATE INDEX [IX_FK_Player_Game]
ON [dbo].[Participant]
    ([GameId]);
GO

-- Creating foreign key on [PartnerId] in table 'Hands'
ALTER TABLE [dbo].[Hands]
ADD CONSTRAINT [FK_Hand_Partner]
    FOREIGN KEY ([PartnerId])
    REFERENCES [dbo].[Participant]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Hand_Partner'
CREATE INDEX [IX_FK_Hand_Partner]
ON [dbo].[Hands]
    ([PartnerId]);
GO

-- Creating foreign key on [PickerId] in table 'Hands'
ALTER TABLE [dbo].[Hands]
ADD CONSTRAINT [FK_Hand_Picker]
    FOREIGN KEY ([PickerId])
    REFERENCES [dbo].[Participant]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Hand_Picker'
CREATE INDEX [IX_FK_Hand_Picker]
ON [dbo].[Hands]
    ([PickerId]);
GO

-- Creating foreign key on [StartingParticipantId] in table 'Hands'
ALTER TABLE [dbo].[Hands]
ADD CONSTRAINT [FK_Hand_StartingPlayer]
    FOREIGN KEY ([StartingParticipantId])
    REFERENCES [dbo].[Participant]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Hand_StartingPlayer'
CREATE INDEX [IX_FK_Hand_StartingPlayer]
ON [dbo].[Hands]
    ([StartingParticipantId]);
GO

-- Creating foreign key on [HandId] in table 'Tricks'
ALTER TABLE [dbo].[Tricks]
ADD CONSTRAINT [FK_Trick_Hand]
    FOREIGN KEY ([HandId])
    REFERENCES [dbo].[Hands]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Trick_Hand'
CREATE INDEX [IX_FK_Trick_Hand]
ON [dbo].[Tricks]
    ([HandId]);
GO

-- Creating foreign key on [StartingParticipantId] in table 'Tricks'
ALTER TABLE [dbo].[Tricks]
ADD CONSTRAINT [FK_Trick_Player]
    FOREIGN KEY ([StartingParticipantId])
    REFERENCES [dbo].[Participant]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Trick_Player'
CREATE INDEX [IX_FK_Trick_Player]
ON [dbo].[Tricks]
    ([StartingParticipantId]);
GO

-- Creating foreign key on [ParticipantId] in table 'CardsPlayed'
ALTER TABLE [dbo].[CardsPlayed]
ADD CONSTRAINT [FK_CardsPlayed_Player]
    FOREIGN KEY ([ParticipantId])
    REFERENCES [dbo].[Participant]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [TrickId] in table 'CardsPlayed'
ALTER TABLE [dbo].[CardsPlayed]
ADD CONSTRAINT [FK_CardsPlayed_Trick]
    FOREIGN KEY ([TrickId])
    REFERENCES [dbo].[Tricks]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_CardsPlayed_Trick'
CREATE INDEX [IX_FK_CardsPlayed_Trick]
ON [dbo].[CardsPlayed]
    ([TrickId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------