IF COL_LENGTH('dbo.Games', 'ConsoleId') IS NULL
BEGIN
	ALTER TABLE Games
	ADD
		ConsoleId INT NULL,
		CONSTRAINT FK_Console FOREIGN KEY (ConsoleId) REFERENCES Consoles(ConsoleId)
END