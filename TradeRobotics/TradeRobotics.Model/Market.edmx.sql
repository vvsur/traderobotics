
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 10/15/2010 19:23:40
-- Generated from EDMX file: C:\Users\Dima\Documents\My Projects\TradeRobotics\TradeRobotics.Model\Market.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [Market];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[AllBars]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AllBars];
GO
IF OBJECT_ID(N'[dbo].[Quotes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Quotes];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'AllBars'
CREATE TABLE [dbo].[AllBars] (
    [Id] uniqueidentifier  NOT NULL,
    [Open] float  NOT NULL,
    [Low] float  NOT NULL,
    [High] float  NOT NULL,
    [Close] float  NOT NULL,
    [Volume] int  NOT NULL,
    [Time] datetime  NOT NULL,
    [Period] int  NOT NULL,
    [Symbol] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Quotes'
CREATE TABLE [dbo].[Quotes] (
    [Id] uniqueidentifier  NOT NULL,
    [Time] datetime  NOT NULL,
    [Price] float  NOT NULL,
    [Volume] float  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'AllBars'
ALTER TABLE [dbo].[AllBars]
ADD CONSTRAINT [PK_AllBars]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Quotes'
ALTER TABLE [dbo].[Quotes]
ADD CONSTRAINT [PK_Quotes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------