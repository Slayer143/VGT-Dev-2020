USE [master]
GO
/****** Object:  Database [VGTBD]    Script Date: 03.09.2020 11:25:24 ******/
CREATE DATABASE [VGTBD]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'VGTBD', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\DATA\VGTBD.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'VGTBD_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\DATA\VGTBD_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO
ALTER DATABASE [VGTBD] SET COMPATIBILITY_LEVEL = 140
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [VGTBD].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [VGTBD] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [VGTBD] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [VGTBD] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [VGTBD] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [VGTBD] SET ARITHABORT OFF 
GO
ALTER DATABASE [VGTBD] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [VGTBD] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [VGTBD] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [VGTBD] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [VGTBD] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [VGTBD] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [VGTBD] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [VGTBD] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [VGTBD] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [VGTBD] SET  DISABLE_BROKER 
GO
ALTER DATABASE [VGTBD] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [VGTBD] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [VGTBD] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [VGTBD] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [VGTBD] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [VGTBD] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [VGTBD] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [VGTBD] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [VGTBD] SET  MULTI_USER 
GO
ALTER DATABASE [VGTBD] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [VGTBD] SET DB_CHAINING OFF 
GO
ALTER DATABASE [VGTBD] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [VGTBD] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [VGTBD] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [VGTBD] SET QUERY_STORE = OFF
GO
USE [VGTBD]
GO
/****** Object:  Table [dbo].[CardsSuits]    Script Date: 03.09.2020 11:25:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CardsSuits](
	[CardSuitId] [int] NOT NULL,
	[SuitName] [nvarchar](15) NOT NULL,
 CONSTRAINT [CardsSuits_PK] PRIMARY KEY CLUSTERED 
(
	[CardSuitId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Games]    Script Date: 03.09.2020 11:25:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Games](
	[GameId] [uniqueidentifier] NOT NULL,
	[GameName] [nvarchar](20) NOT NULL,
 CONSTRAINT [Games_PK] PRIMARY KEY CLUSTERED 
(
	[GameId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GameSessions]    Script Date: 03.09.2020 11:25:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GameSessions](
	[SessionId] [uniqueidentifier] NOT NULL,
	[StartDate] [datetimeoffset](7) NOT NULL,
	[EndDate] [datetimeoffset](7) NULL,
	[GameId] [uniqueidentifier] NOT NULL,
	[SessionStatusId] [int] NOT NULL,
	[RoomSize] [int] NOT NULL,
 CONSTRAINT [GameSession_PK] PRIMARY KEY CLUSTERED 
(
	[SessionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GameSessionStatuses]    Script Date: 03.09.2020 11:25:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GameSessionStatuses](
	[GameSessionStatusId] [int] NOT NULL,
	[StatusName] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_GameSessionStatuses] PRIMARY KEY CLUSTERED 
(
	[GameSessionStatusId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GameSessionUsers]    Script Date: 03.09.2020 11:25:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GameSessionUsers](
	[SessionId] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[PlayerStatusId] [int] NOT NULL,
	[SeatPlace] [int] NOT NULL,
	[UserRoleId] [int] NOT NULL,
	[StartingChips] [int] NOT NULL,
	[NowChips] [int] NOT NULL,
 CONSTRAINT [GameSessionInfo_PK] PRIMARY KEY CLUSTERED 
(
	[SessionId] ASC,
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PlayingCards]    Script Date: 03.09.2020 11:25:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PlayingCards](
	[CardId] [int] NOT NULL,
	[CardValue] [int] NOT NULL,
	[CardSuitId] [int] NOT NULL,
 CONSTRAINT [PlayingCards_PK] PRIMARY KEY CLUSTERED 
(
	[CardId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PokerPlayerStatuses]    Script Date: 03.09.2020 11:25:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PokerPlayerStatuses](
	[PokerPlayerStatusId] [int] NOT NULL,
	[StatusName] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_PokerPlayerStatuses] PRIMARY KEY CLUSTERED 
(
	[PokerPlayerStatusId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserRoles]    Script Date: 03.09.2020 11:25:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRoles](
	[RoleId] [int] NOT NULL,
	[Role] [nvarchar](20) NULL,
 CONSTRAINT [UserRoles_PK] PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserRolesForPoker]    Script Date: 03.09.2020 11:25:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRolesForPoker](
	[UserRoleId] [int] NOT NULL,
	[RoleName] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_UserRoles] PRIMARY KEY CLUSTERED 
(
	[UserRoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 03.09.2020 11:25:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[UserId] [uniqueidentifier] NOT NULL,
	[Login] [nvarchar](50) NOT NULL,
	[Password] [nvarchar](50) NOT NULL,
	[Email] [nvarchar](50) NOT NULL,
	[Chips] [int] NULL,
	[RoleId] [int] NOT NULL,
 CONSTRAINT [Users_PK] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[CardsSuits] ([CardSuitId], [SuitName]) VALUES (1, N'Diamonds')
INSERT [dbo].[CardsSuits] ([CardSuitId], [SuitName]) VALUES (2, N'Hearts')
INSERT [dbo].[CardsSuits] ([CardSuitId], [SuitName]) VALUES (3, N'Spades')
INSERT [dbo].[CardsSuits] ([CardSuitId], [SuitName]) VALUES (4, N'Clubs')
INSERT [dbo].[Games] ([GameId], [GameName]) VALUES (N'ea255d1d-51e0-43d3-b985-6e2bb779879f', N'Fool')
INSERT [dbo].[GameSessions] ([SessionId], [StartDate], [EndDate], [GameId], [SessionStatusId], [RoomSize]) VALUES (N'5186274c-a4d9-42c8-a17b-779874551ecb', CAST(N'2020-09-02T13:20:28.1847612+03:00' AS DateTimeOffset), NULL, N'ea255d1d-51e0-43d3-b985-6e2bb779879f', 0, 2)
INSERT [dbo].[GameSessions] ([SessionId], [StartDate], [EndDate], [GameId], [SessionStatusId], [RoomSize]) VALUES (N'09dd3c49-ebf3-4d19-b214-a5ffd5a0577b', CAST(N'2020-09-01T18:59:54.8398816+03:00' AS DateTimeOffset), NULL, N'ea255d1d-51e0-43d3-b985-6e2bb779879f', 0, 2)
INSERT [dbo].[GameSessions] ([SessionId], [StartDate], [EndDate], [GameId], [SessionStatusId], [RoomSize]) VALUES (N'8b75ca50-3c22-4d15-abaa-cfeac987cb4c', CAST(N'2020-09-01T19:06:02.9699548+03:00' AS DateTimeOffset), NULL, N'ea255d1d-51e0-43d3-b985-6e2bb779879f', 0, 2)
INSERT [dbo].[GameSessions] ([SessionId], [StartDate], [EndDate], [GameId], [SessionStatusId], [RoomSize]) VALUES (N'368ba517-a092-4c84-9194-fb625e44bdf4', CAST(N'2020-09-01T18:51:43.8655133+03:00' AS DateTimeOffset), NULL, N'ea255d1d-51e0-43d3-b985-6e2bb779879f', 0, 2)
INSERT [dbo].[GameSessionStatuses] ([GameSessionStatusId], [StatusName]) VALUES (0, N'NotStaffed')
INSERT [dbo].[GameSessionStatuses] ([GameSessionStatusId], [StatusName]) VALUES (1, N'Staffed')
INSERT [dbo].[GameSessionStatuses] ([GameSessionStatusId], [StatusName]) VALUES (2, N'InProgress')
INSERT [dbo].[GameSessionStatuses] ([GameSessionStatusId], [StatusName]) VALUES (3, N'Finished')
INSERT [dbo].[GameSessionUsers] ([SessionId], [UserId], [PlayerStatusId], [SeatPlace], [UserRoleId], [StartingChips], [NowChips]) VALUES (N'5186274c-a4d9-42c8-a17b-779874551ecb', N'a1175c61-c7b4-40fd-ad8f-23acb855b1a6', 1, 2, 0, 5000, 5000)
INSERT [dbo].[GameSessionUsers] ([SessionId], [UserId], [PlayerStatusId], [SeatPlace], [UserRoleId], [StartingChips], [NowChips]) VALUES (N'5186274c-a4d9-42c8-a17b-779874551ecb', N'e989eb10-2958-4ea6-9f2e-9164da782345', 0, 1, 1, 1000, 1000)
INSERT [dbo].[PlayingCards] ([CardId], [CardValue], [CardSuitId]) VALUES (0, 2, 1)
INSERT [dbo].[PlayingCards] ([CardId], [CardValue], [CardSuitId]) VALUES (1, 2, 2)
INSERT [dbo].[PlayingCards] ([CardId], [CardValue], [CardSuitId]) VALUES (2, 2, 3)
INSERT [dbo].[PlayingCards] ([CardId], [CardValue], [CardSuitId]) VALUES (3, 2, 4)
INSERT [dbo].[PlayingCards] ([CardId], [CardValue], [CardSuitId]) VALUES (4, 3, 1)
INSERT [dbo].[PlayingCards] ([CardId], [CardValue], [CardSuitId]) VALUES (5, 3, 2)
INSERT [dbo].[PlayingCards] ([CardId], [CardValue], [CardSuitId]) VALUES (6, 3, 3)
INSERT [dbo].[PlayingCards] ([CardId], [CardValue], [CardSuitId]) VALUES (7, 3, 4)
INSERT [dbo].[PlayingCards] ([CardId], [CardValue], [CardSuitId]) VALUES (8, 4, 1)
INSERT [dbo].[PlayingCards] ([CardId], [CardValue], [CardSuitId]) VALUES (9, 4, 2)
INSERT [dbo].[PlayingCards] ([CardId], [CardValue], [CardSuitId]) VALUES (10, 4, 3)
INSERT [dbo].[PlayingCards] ([CardId], [CardValue], [CardSuitId]) VALUES (11, 4, 4)
INSERT [dbo].[PlayingCards] ([CardId], [CardValue], [CardSuitId]) VALUES (12, 5, 1)
INSERT [dbo].[PlayingCards] ([CardId], [CardValue], [CardSuitId]) VALUES (13, 5, 2)
INSERT [dbo].[PlayingCards] ([CardId], [CardValue], [CardSuitId]) VALUES (14, 5, 3)
INSERT [dbo].[PlayingCards] ([CardId], [CardValue], [CardSuitId]) VALUES (15, 5, 4)
INSERT [dbo].[PlayingCards] ([CardId], [CardValue], [CardSuitId]) VALUES (16, 6, 1)
INSERT [dbo].[PlayingCards] ([CardId], [CardValue], [CardSuitId]) VALUES (17, 6, 2)
INSERT [dbo].[PlayingCards] ([CardId], [CardValue], [CardSuitId]) VALUES (18, 6, 3)
INSERT [dbo].[PlayingCards] ([CardId], [CardValue], [CardSuitId]) VALUES (19, 6, 4)
INSERT [dbo].[PlayingCards] ([CardId], [CardValue], [CardSuitId]) VALUES (20, 7, 1)
INSERT [dbo].[PlayingCards] ([CardId], [CardValue], [CardSuitId]) VALUES (21, 7, 2)
INSERT [dbo].[PlayingCards] ([CardId], [CardValue], [CardSuitId]) VALUES (22, 7, 3)
INSERT [dbo].[PlayingCards] ([CardId], [CardValue], [CardSuitId]) VALUES (23, 7, 4)
INSERT [dbo].[PlayingCards] ([CardId], [CardValue], [CardSuitId]) VALUES (24, 8, 1)
INSERT [dbo].[PlayingCards] ([CardId], [CardValue], [CardSuitId]) VALUES (25, 8, 2)
INSERT [dbo].[PlayingCards] ([CardId], [CardValue], [CardSuitId]) VALUES (26, 8, 3)
INSERT [dbo].[PlayingCards] ([CardId], [CardValue], [CardSuitId]) VALUES (27, 8, 4)
INSERT [dbo].[PlayingCards] ([CardId], [CardValue], [CardSuitId]) VALUES (28, 9, 1)
INSERT [dbo].[PlayingCards] ([CardId], [CardValue], [CardSuitId]) VALUES (29, 9, 2)
INSERT [dbo].[PlayingCards] ([CardId], [CardValue], [CardSuitId]) VALUES (30, 9, 3)
INSERT [dbo].[PlayingCards] ([CardId], [CardValue], [CardSuitId]) VALUES (31, 9, 4)
INSERT [dbo].[PlayingCards] ([CardId], [CardValue], [CardSuitId]) VALUES (32, 10, 1)
INSERT [dbo].[PlayingCards] ([CardId], [CardValue], [CardSuitId]) VALUES (33, 10, 2)
INSERT [dbo].[PlayingCards] ([CardId], [CardValue], [CardSuitId]) VALUES (34, 10, 3)
INSERT [dbo].[PlayingCards] ([CardId], [CardValue], [CardSuitId]) VALUES (35, 10, 4)
INSERT [dbo].[PlayingCards] ([CardId], [CardValue], [CardSuitId]) VALUES (36, 11, 1)
INSERT [dbo].[PlayingCards] ([CardId], [CardValue], [CardSuitId]) VALUES (37, 11, 2)
INSERT [dbo].[PlayingCards] ([CardId], [CardValue], [CardSuitId]) VALUES (38, 11, 3)
INSERT [dbo].[PlayingCards] ([CardId], [CardValue], [CardSuitId]) VALUES (39, 11, 4)
INSERT [dbo].[PlayingCards] ([CardId], [CardValue], [CardSuitId]) VALUES (40, 12, 1)
INSERT [dbo].[PlayingCards] ([CardId], [CardValue], [CardSuitId]) VALUES (41, 12, 2)
INSERT [dbo].[PlayingCards] ([CardId], [CardValue], [CardSuitId]) VALUES (42, 12, 3)
INSERT [dbo].[PlayingCards] ([CardId], [CardValue], [CardSuitId]) VALUES (43, 12, 4)
INSERT [dbo].[PlayingCards] ([CardId], [CardValue], [CardSuitId]) VALUES (44, 13, 1)
INSERT [dbo].[PlayingCards] ([CardId], [CardValue], [CardSuitId]) VALUES (45, 13, 2)
INSERT [dbo].[PlayingCards] ([CardId], [CardValue], [CardSuitId]) VALUES (46, 13, 3)
INSERT [dbo].[PlayingCards] ([CardId], [CardValue], [CardSuitId]) VALUES (47, 13, 4)
INSERT [dbo].[PlayingCards] ([CardId], [CardValue], [CardSuitId]) VALUES (48, 14, 1)
INSERT [dbo].[PlayingCards] ([CardId], [CardValue], [CardSuitId]) VALUES (49, 14, 2)
INSERT [dbo].[PlayingCards] ([CardId], [CardValue], [CardSuitId]) VALUES (50, 14, 3)
INSERT [dbo].[PlayingCards] ([CardId], [CardValue], [CardSuitId]) VALUES (51, 14, 4)
INSERT [dbo].[PlayingCards] ([CardId], [CardValue], [CardSuitId]) VALUES (52, 15, 1)
INSERT [dbo].[PlayingCards] ([CardId], [CardValue], [CardSuitId]) VALUES (53, 15, 2)
INSERT [dbo].[PlayingCards] ([CardId], [CardValue], [CardSuitId]) VALUES (54, 15, 3)
INSERT [dbo].[PlayingCards] ([CardId], [CardValue], [CardSuitId]) VALUES (55, 15, 4)
INSERT [dbo].[PokerPlayerStatuses] ([PokerPlayerStatusId], [StatusName]) VALUES (0, N'WaitingForStart')
INSERT [dbo].[PokerPlayerStatuses] ([PokerPlayerStatusId], [StatusName]) VALUES (1, N'ReadyForStart')
INSERT [dbo].[PokerPlayerStatuses] ([PokerPlayerStatusId], [StatusName]) VALUES (2, N'MakesMove')
INSERT [dbo].[PokerPlayerStatuses] ([PokerPlayerStatusId], [StatusName]) VALUES (3, N'WaitingForMove')
INSERT [dbo].[PokerPlayerStatuses] ([PokerPlayerStatusId], [StatusName]) VALUES (4, N'Win')
INSERT [dbo].[PokerPlayerStatuses] ([PokerPlayerStatusId], [StatusName]) VALUES (5, N'Lose')
INSERT [dbo].[UserRoles] ([RoleId], [Role]) VALUES (0, N'Admin')
INSERT [dbo].[UserRoles] ([RoleId], [Role]) VALUES (1, N'CommonUser')
INSERT [dbo].[UserRoles] ([RoleId], [Role]) VALUES (2, N'PremiumUser')
INSERT [dbo].[UserRolesForPoker] ([UserRoleId], [RoleName]) VALUES (0, N'Player')
INSERT [dbo].[UserRolesForPoker] ([UserRoleId], [RoleName]) VALUES (1, N'Stickman')
INSERT [dbo].[Users] ([UserId], [Login], [Password], [Email], [Chips], [RoleId]) VALUES (N'a1175c61-c7b4-40fd-ad8f-23acb855b1a6', N'TestLogin3', N'TestPassword', N'TestEmail3', NULL, 1)
INSERT [dbo].[Users] ([UserId], [Login], [Password], [Email], [Chips], [RoleId]) VALUES (N'0d878804-bc73-4901-8add-45971b0db2d6', N'TestLogin', N'TestPassword', N'TestEmail', NULL, 1)
INSERT [dbo].[Users] ([UserId], [Login], [Password], [Email], [Chips], [RoleId]) VALUES (N'51d25ea3-9341-4da7-846d-6381e624f0df', N'TestLogin2', N'TestPassword', N'TestEmail2', NULL, 1)
INSERT [dbo].[Users] ([UserId], [Login], [Password], [Email], [Chips], [RoleId]) VALUES (N'e989eb10-2958-4ea6-9f2e-9164da782345', N'Alex', N'2025', N'Testmailru', NULL, 0)
INSERT [dbo].[Users] ([UserId], [Login], [Password], [Email], [Chips], [RoleId]) VALUES (N'9aff4fc8-c6ce-4c2a-bbb9-ebd9b85a3277', N'Iregorik', N'2020', N'Testmailru2', NULL, 0)
ALTER TABLE [dbo].[GameSessions]  WITH CHECK ADD  CONSTRAINT [FK_GameSessions_GameSessionStatuses] FOREIGN KEY([SessionStatusId])
REFERENCES [dbo].[GameSessionStatuses] ([GameSessionStatusId])
GO
ALTER TABLE [dbo].[GameSessions] CHECK CONSTRAINT [FK_GameSessions_GameSessionStatuses]
GO
ALTER TABLE [dbo].[GameSessions]  WITH CHECK ADD  CONSTRAINT [GameSession_Games_FK] FOREIGN KEY([GameId])
REFERENCES [dbo].[Games] ([GameId])
GO
ALTER TABLE [dbo].[GameSessions] CHECK CONSTRAINT [GameSession_Games_FK]
GO
ALTER TABLE [dbo].[GameSessionUsers]  WITH CHECK ADD  CONSTRAINT [FK_GameSessionUsers_PokerPlayerStatuses] FOREIGN KEY([PlayerStatusId])
REFERENCES [dbo].[PokerPlayerStatuses] ([PokerPlayerStatusId])
GO
ALTER TABLE [dbo].[GameSessionUsers] CHECK CONSTRAINT [FK_GameSessionUsers_PokerPlayerStatuses]
GO
ALTER TABLE [dbo].[GameSessionUsers]  WITH CHECK ADD  CONSTRAINT [FK_GameSessionUsers_UserRoles] FOREIGN KEY([UserRoleId])
REFERENCES [dbo].[UserRolesForPoker] ([UserRoleId])
GO
ALTER TABLE [dbo].[GameSessionUsers] CHECK CONSTRAINT [FK_GameSessionUsers_UserRoles]
GO
ALTER TABLE [dbo].[GameSessionUsers]  WITH CHECK ADD  CONSTRAINT [GameSessionInfo_GameSession_FK] FOREIGN KEY([SessionId])
REFERENCES [dbo].[GameSessions] ([SessionId])
GO
ALTER TABLE [dbo].[GameSessionUsers] CHECK CONSTRAINT [GameSessionInfo_GameSession_FK]
GO
ALTER TABLE [dbo].[GameSessionUsers]  WITH CHECK ADD  CONSTRAINT [GameSessionInfo_Users_FK] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[GameSessionUsers] CHECK CONSTRAINT [GameSessionInfo_Users_FK]
GO
ALTER TABLE [dbo].[PlayingCards]  WITH CHECK ADD  CONSTRAINT [PlayingCards_CardSuits_FK] FOREIGN KEY([CardSuitId])
REFERENCES [dbo].[CardsSuits] ([CardSuitId])
GO
ALTER TABLE [dbo].[PlayingCards] CHECK CONSTRAINT [PlayingCards_CardSuits_FK]
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_Users_UserRoles] FOREIGN KEY([RoleId])
REFERENCES [dbo].[UserRoles] ([RoleId])
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_Users_UserRoles]
GO
USE [master]
GO
ALTER DATABASE [VGTBD] SET  READ_WRITE 
GO
