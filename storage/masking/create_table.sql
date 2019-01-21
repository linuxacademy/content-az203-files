-- Drop table

-- DROP TABLE laaz200dm.dbo.Users

CREATE TABLE laaz200dm.dbo.Users (
	UserId int NOT NULL,
	AccountCode varchar(50) NOT NULL,
	Pin varchar(10) NOT NULL,
	Name varchar(50) NOT NULL
) GO;
