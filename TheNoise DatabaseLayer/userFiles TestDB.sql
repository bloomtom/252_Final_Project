IF db_id('userFiles') IS NOT NULL
	BEGIN
		DROP DATABASE userFiles -- Because we are testing and want to create a new DB
		DROP TABLE userDirectories
		DROP TABLE musicUsers
		DROP TABLE directories
		DROP PROCEDURE checkMusicUser
		DROP procedure addMusicUser
	END
ELSE 
	PRINT 'Creating new database'

GO -- start a new batch

CREATE DATABASE userFiles
GO

USE userFiles

CREATE TABLE musicUsers
(
	username	varchar(50)	NOT NULL	PRIMARY KEY,
	password	varchar(80) NOT NULL
)

CREATE TABLE directories
(
	d_id	int				NOT NULL PRIMARY KEY,
	path	varchar(255)	NOT NULL
)

CREATE TABLE userDirectories
(
	username	varchar(50) NOT NULL FOREIGN KEY REFERENCES musicUsers(username), 
	d_id	int NOT NULL	FOREIGN KEY REFERENCES directories(d_id),
	PRIMARY KEY (username, d_id)
)



INSERT INTO musicUsers VALUES('Test1', '23d7d2c7b948c6474295a2385b6c35636f51d58e87af29dc59a154e85f3a99c1')
INSERT INTO musicUsers VALUES('Test2', '85dd870a1d91c42bcd3fd878529144a0318a542defccc8250ec6bf56de3edc87')
INSERT INTO musicUsers VALUES('Test3', '94834c730388380185219ea32f1e266f75b6c7736ed7d0ffffeedcb7f8475a4a')

INSERT INTO directories VALUES (0, 'C:\Test\0')
INSERT INTO directories VALUES (1, 'C:\Test\1')
INSERT INTO directories VALUES (2, 'C:\Test\2')
INSERT INTO directories VALUES (3, 'C:\Test\3')

INSERT INTO userDirectories VALUES('Test1', '0')
INSERT INTO userDirectories VALUES('Test1', '1')
INSERT INTO userDirectories VALUES('Test2', '2')
INSERT INTO userDirectories VALUES('Test3', '3')

SELECT * FROM musicUsers
SELECT * FROM directories
SELECT * FROM userDirectories

GO

CREATE PROCEDURE checkMusicUser @uname varchar(50), @passw varchar(80), @retval int output
AS
BEGIN
	--DECLARE @retval int
	IF (SELECT username FROM musicUsers WHERE username = @uname) = @uname
		IF (SELECT password FROM musicUsers WHERE password = @passw) = @passw
			BEGIN
				SELECT @retval = 1
				PRINT @retval
				RETURN @retval
			END
		ELSE
			BEGIN
				SELECT @retval = 0
				PRINT @retval
				RETURN @retval
			END
	ELSE
		BEGIN
			SELECT @retval = 2
			PRINT @retval
			RETURN @retval
		END
END
GO

CREATE PROCEDURE addMusicUser @uname varChar(50), @passw varchar(80)
AS
INSERT INTO musicUsers VALUES(@uname, @passw)
GO

SELECT * FROM musicUsers
GO