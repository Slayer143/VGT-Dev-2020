use VGTBD

create table Games
(
	GameId uniqueidentifier not null,
	GameName nvarchar(20) not null,
	constraint Games_PK primary key clustered (GameId)
)

create table GameSession
(
	SessionId uniqueidentifier not null,
	StartDate datetimeoffset not null,
	EndDate datetimeoffset not null,
	GameId uniqueidentifier not null,
	Winner uniqueidentifier not null,
	Size int not null,
	constraint GameSession_PK primary key clustered (Sessionid),
	constraint GameSession_Games_FK foreign key (GameId)
	references Games(GameId)
)

create table GameSessionInfo
(
	SessionId uniqueidentifier not null,
	UserId uniqueidentifier not null,
	GameSessionStatus int not null,
	constraint GameSessionInfo_PK primary key clustered (Sessionid, UserId),
	constraint GameSessionInfo_GameSession_FK foreign key (SessionId)
	references GameSession(SessionId),
	constraint GameSessionInfo_Users_FK foreign key (UserId)
	references Users(UserId)
)

create table CardsSuits
(
	CardSuitId int not null,
	SuitName nvarchar(15) not null,
	constraint CardsSuits_PK primary key clustered (CardSuitId)
)

create table PlayingCards
(
	CardId int not null,
	CardValue int not null,
	CardSuitId int not null,
	constraint PlayingCards_PK primary key clustered (CardId),
	constraint PlayingCards_CardSuits_FK foreign key (CardSuitId)
	references CardsSuits(CardSuitId)
)