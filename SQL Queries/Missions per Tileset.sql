select Count(Distinct Runs.Id) as CorpusShipRuns,
(Select  Count(Distinct Runs.IdentityString) as Capture from Runs Inner Join MapPoints on runs.ID = MapPoints.RunId
Inner Join Tiles on mapPoints.TileName = Tiles.Name And Tiles.TilesetName = 'CorpusShip' where MissionType = 'Exterminate' ) as Exterminate,
(Select  Count(Distinct Runs.IdentityString) as Capture from Runs Inner Join MapPoints on runs.ID = MapPoints.RunId
Inner Join Tiles on mapPoints.TileName = Tiles.Name And Tiles.TilesetName = 'CorpusShip' where MissionType = 'Capture' ) as Capture,
(Select  Count(Distinct Runs.IdentityString) as Capture from Runs Inner Join MapPoints on runs.ID = MapPoints.RunId
Inner Join Tiles on mapPoints.TileName = Tiles.Name And Tiles.TilesetName = 'CorpusShip' where MissionType = 'Mobile Defense' ) as MobileDefense,
(Select  Count(Distinct Runs.IdentityString) as Capture from Runs Inner Join MapPoints on runs.ID = MapPoints.RunId
Inner Join Tiles on mapPoints.TileName = Tiles.Name And Tiles.TilesetName = 'CorpusShip' where MissionType = 'Rescue' ) as Rescue,
(Select  Count(Distinct Runs.IdentityString) as Capture from Runs Inner Join MapPoints on runs.ID = MapPoints.RunId
Inner Join Tiles on mapPoints.TileName = Tiles.Name And Tiles.TilesetName = 'CorpusShip' where MissionType = 'Sabotage (Reactor)' ) as Sabotage,
(Select  Count(Distinct Runs.IdentityString) as Capture from Runs Inner Join MapPoints on runs.ID = MapPoints.RunId
Inner Join Tiles on mapPoints.TileName = Tiles.Name And Tiles.TilesetName = 'CorpusShip' where MissionType = 'Spy' ) as Spy,
(Select  Count(Distinct Runs.IdentityString) as Capture from Runs Inner Join MapPoints on runs.ID = MapPoints.RunId
Inner Join Tiles on mapPoints.TileName = Tiles.Name And Tiles.TilesetName = 'CorpusShip' where MissionType = 'Survival' )  as Survival,
(Select  Count(Distinct Runs.IdentityString) as Capture from Runs Inner Join MapPoints on runs.ID = MapPoints.RunId
Inner Join Tiles on mapPoints.TileName = Tiles.Name And Tiles.TilesetName = 'CorpusShip' where MissionType = 'Assassination (The Sergeant)' ) as Assassination
from Runs
Inner Join MapPoints on runs.ID = MapPoints.RunId
Inner Join Tiles on mapPoints.TileName = Tiles.Name And Tiles.TilesetName = 'CorpusShip';



