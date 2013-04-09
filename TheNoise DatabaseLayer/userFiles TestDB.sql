USE master

CREATE DATABASE userFiles
GO

CREATE TABLE users
(
	username	varchar(50)	NOT NULL	PRIMARY KEY,
	password	varchar(50) NOT NULL
)

CREATE TABLE directories
(
	d_id	int				NOT NULL PRIMARY KEY,
	path	varchar(255)	NOT NULL
)

CREATE TABLE userDirectories
(
	username	varchar(50) NOT NULL FOREIGN KEY REFERENCES users(username), 
	d_id	int NOT NULL	FOREIGN KEY REFERENCES directories(d_id),
	PRIMARY KEY (username, d_id)
)

INSERT INTO users VALUES('Test1', 'Test1PW')
INSERT INTO users VALUES('Test2', 'Test2PW')
INSERT INTO users VALUES('Test3', 'Test3PW')

INSERT INTO directories VALUES (0, 'C:\Test\0')
INSERT INTO directories VALUES (1, 'C:\Test\1')
INSERT INTO directories VALUES (2, 'C:\Test\2')
INSERT INTO directories VALUES (3, 'C:\Test\3')

INSERT INTO userDirectories VALUES('Test1', '0')
INSERT INTO userDirectories VALUES('Test1', '1')
INSERT INTO userDirectories VALUES('Test2', '2')
INSERT INTO userDirectories VALUES('Test3', '3')

SELECT * FROM users
SELECT * FROM directories
SELECT * FROM userDirectories