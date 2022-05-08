use master
GO
IF EXISTS (SELECT * FROM SYS.DATABASES WHERE NAME = 'curs_database') 
	DROP DATABASE curs_database
	GO
CREATE DATABASE curs_database

