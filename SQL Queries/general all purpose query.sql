select * from Users Where Id=1

select * from Tiles; select * from Tilesets; select * from Missions; Select * from MapPoints; select * from Runs;

--truncate table MapPoints;  truncate table Tilesets; truncate table Tiles;  truncate table Missions; truncate table Runs;

Delete from Tiles; Delete From Tilesets; Delete From Missions; Delete From Runs; Delete From MapPoints;
DBCC CHECKIDENT (MapPoints, RESEED, 0); DBCC CHECKIDENT ( Tiles, RESEED,0);DBCC CHECKIDENT ( Tileset, RESEED,0);DBCC CHECKIDENT ( Missions,RESEED, 0);DBCC CHECKIDENT ( Runs,RESEED, 0);