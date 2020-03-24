IF (NOT EXISTS (SELECT * 
                FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_SCHEMA = 'dbo' 
                AND  TABLE_NAME = 'Genres'))
BEGIN
	CREATE TABLE Genres (
		GenreId INT IDENTITY PRIMARY KEY NOT NULL,
		GenreName VARCHAR(255) NOT NULL
	);
END

IF (NOT EXISTS (SELECT * 
                FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_SCHEMA = 'dbo' 
                AND  TABLE_NAME = 'Publishers'))
BEGIN
	CREATE TABLE Publishers (
		PublisherId INT IDENTITY PRIMARY KEY NOT NULL,
		PublisherName VARCHAR(255) NOT NULL
	);
END

IF (NOT EXISTS (SELECT * 
                FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_SCHEMA = 'dbo' 
                AND  TABLE_NAME = 'Games'))
BEGIN
	CREATE TABLE Games (
		GameId INT IDENTITY PRIMARY KEY NOT NULL,
		GameName VARCHAR (255) NOT NULL,
		ReleaseDate DATE NULL,
		PublisherId INT NULL,
		GenreId INT NULL,
		CONSTRAINT FK_Publisher FOREIGN KEY (PublisherId) REFERENCES Publishers(PublisherId),
		CONSTRAINT FK_Genre FOREIGN KEY (GenreId) REFERENCES Genres(GenreId)
	);
END