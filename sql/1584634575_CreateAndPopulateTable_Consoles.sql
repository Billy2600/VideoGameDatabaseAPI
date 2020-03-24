IF (NOT EXISTS (SELECT * 
                FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_SCHEMA = 'dbo' 
                AND  TABLE_NAME = 'Consoles'))
BEGIN
	CREATE TABLE Consoles (
		ConsoleId INT IDENTITY PRIMARY KEY NOT NULL,
		ConsoleName VARCHAR(255) NOT NULL,
		ReleaseDate DATE NULL,
		Manufacturer VARCHAR(255) NULL,
		UnitsSold INT NULL
	);

	--Only abritarily 'notable' consoles have been included
	INSERT INTO Consoles (ConsoleName, ReleaseDate, Manufacturer, UnitsSold) VALUES
		--Second generation
		('Atari 2600','1977-09-11','Atari',30000000),
		('Atari 5200','1982-11-01','Atari',1000000),
		('ColecoVision','1982-08-01','Coleco',2000000),
		('Fairchild Channel F','1976-11-01','Fairchild',250000),
		('Vectrex','1982-11-01','GCE',0),
		('Intellivision','1980-01-01','Mattel',3000000),
		('Magnavox Odyssey²','1978-12-01','Magnavox',30000000),

	--Third generation
		('SG-1000','1983-07-05','Sega',2000000),
		('Nintendo Entertainment System','1983-07-05','Nintendo',61000000),
		('Sega Master System','1985-08-20','Sega',13000000),
		('Family Computer Disk System','1986-02-21', 'Nintendo', 4000000),
		('Atari 7800','1986-05-01','Atari',0),
		('Atari XEGS','1987-01-01','Atari',2000000),

	--Fourth generation
		('Turbografx-16','1987-10-30','NEC',10000000),
		('Sega Genesis','1988-10-29','Sega',35000000),
		('Turbografx-CD','1988-12-04','NEC',0),
		('Super Nintendo Entertainment System','1990-11-21','Nintendo', 49000000),
		('Sega CD','1991-12-12','Sega',2000000),
		('Neo-Geo AES','1990-04-26','SNK',750000),
		('CD-i','1992-12-03','Philips',1500000),

		('Neo-Geo CD','1994-09-09','SNK',0),
		('Sega 32X','1994-11-21','Sega',800000),

		--Fifth generation
		('3DO Interactive Multiplayer','1993-10-04','Panasonic/Sayno/GoldStar',2000000),
		('Atari Jaguar','1993-11-23','Atari',250000),
		('Sega Saturn','1994-11-22','Sega',9260000),
		('PlayStation','1994-12-03','Sony',102490000),
		('Atari Jaguar CD','1995-09-21','Atari',0),
		('Nintendo 64','1996-06-23','Nintendo',32930000),
		('Nintendo 64 DD','1999-12-01','Nintendo',15000),

		--Sixth generation
		('Dreamcast','1998-11-27','Sega', 9130000),
		('PlayStation 2','2000-03-04','Sony',155000000),
		('Nintendo Gamecube','2001-11-14','Nintendo',21740000),
		('Xbox','2001-11-15','Microsoft',24000000),

		--Seventh generation
		('Xbox 360','2005-11-22','Microsoft',83700000),
		('PlayStation 3','2006-11-11','Sony',80000000),
		('Wii','2006-11-19','Nintendo',101630000),

		--Eighth generation
		('Wii U','2012-11-18','Nintendo',13560000),
		('PlayStation 4','2013-11-15','Sony',106000000),
		('Xbox One','2013-11-22','Microsoft',41000000),
		('Nintendo Switch','2017-03-03','Nintendo',52480000)
END