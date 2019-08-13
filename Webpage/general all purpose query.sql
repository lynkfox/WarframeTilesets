select * from Users Where Id=1

select * from Tiles; select * from Tilesets; select * from Missions; Select * from MapPoints; select * from Runs;

truncate table MapPoints;  truncate table Tilesets; truncate table Tiles;  truncate table Missions; truncate table Runs;

Delete from Tiles; Delete From Tilesets; Delete From Missions; Delete From Runs; Delete From MapPoints;
DBCC CHECKIDENT (MapPoints, RESEED, 1); DBCC CHECKIDENT ( Tiles, RESEED,1);DBCC CHECKIDENT ( Tileset, RESEED,1);DBCC CHECKIDENT ( Missions,RESEED, 1);DBCC CHECKIDENT ( Runs,RESEED, 1);