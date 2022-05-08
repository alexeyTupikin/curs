use curs_database
go
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_CATALOG='curs_database' AND TABLE_NAME='users') 
	DROP TABLE users
	GO
CREATE TABLE users (
	[user_id] int IDENTITY(1,1) PRIMARY KEY,
	[login] nvarchar(50) not null,
	[pass] nvarchar(50) not null,
	[access_level] int not null
	)
insert into users ([login],[pass],[access_level])
values ('admin1@mgutu.ru','XbfahF3f',1),
('n.mukhortova@mgutu.ru','yalushiyuchitel',0),
('a.glusker@mgutu.ru','assemblerissimple',0),
('asmin2@mgutu.ru','HwwdiHr3Hc',1)