-- This script assumes you have already created a database called 'VideoGames'
CREATE TABLE IF NOT EXISTS "Consoles" ( -- Table name in quotes as to preserve capitalization
    "ConsoleId" SERIAL PRIMARY KEY NOT NULL,
    "ConsoleName" VARCHAR(255) NOT NULL,
    "ReleaseDate" TIMESTAMP,
    "Manufacturer" VARCHAR(255),
    "UnitsSold" INT
);

CREATE TABLE IF NOT EXISTS "Publishers" (
    "PublisherId" SERIAL PRIMARY KEY NOT NULL,
    "PublisherName" VARCHAR(255) NOT NULL
);

CREATE TABLE IF NOT EXISTS "Genres" (
    "GenreId" SERIAL PRIMARY KEY NOT NULL,
    "GenreName" VARCHAR(255) NOT NULL
);

CREATE TABLE IF NOT EXISTS "GamesGenres" (
    "GameGenreId" SERIAL PRIMARY KEY NOT NULL,
    "GameId" INT NOT NULL,
    "GenreId" INT NOT NULL
);

CREATE TABLE IF NOT EXISTS "Games" (
    "GameId" SERIAL PRIMARY KEY NOT NULL,
    "GameName" VARCHAR(255) NOT NULL,
    "ReleaseDate" TIMESTAMP,
    "PublisherId" INT REFERENCES "Publishers" ("PublisherId"),
    "ConsoleId" INT REFERENCES "Consoles" ("ConsoleId")    
);