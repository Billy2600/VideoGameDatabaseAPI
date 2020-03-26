-- Create junction table for Games and Genres
IF (NOT EXISTS (SELECT * 
                FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_SCHEMA = 'dbo' 
                AND  TABLE_NAME = 'GamesGenres'))
BEGIN
	CREATE TABLE GamesGenres (
		GameGenreId INT IDENTITY PRIMARY KEY NOT NULL,
		GameId INT NOT NULL,
		GenreId INT NOT NULL
	);
END
