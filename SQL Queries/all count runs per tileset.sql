select Top 1 (select Count(Distinct Runs.Id) from Runs
Inner Join MapPoints on runs.ID = MapPoints.RunId
Inner Join Tiles on mapPoints.TileName = Tiles.Name And Tiles.TilesetName = 'CorpusShip') As CorpusShipRuns,
(select Count(Distinct Runs.Id) from Runs
Inner Join MapPoints on runs.ID = MapPoints.RunId
Inner Join Tiles on mapPoints.TileName = Tiles.Name And Tiles.TilesetName = 'CorpusGasCity') As GasCityRuns,
(select Count(Distinct Runs.Id) from Runs
Inner Join MapPoints on runs.ID = MapPoints.RunId
Inner Join Tiles on mapPoints.TileName = Tiles.Name And Tiles.TilesetName = 'CorpusIcePlanet') As IcePlanetRuns,
(select Count(Distinct Runs.Id) from Runs
Inner Join MapPoints on runs.ID = MapPoints.RunId
Inner Join Tiles on mapPoints.TileName = Tiles.Name And Tiles.TilesetName = 'CorpusOutpost') As CorpusOutpostRuns,
(select Count(Distinct Runs.Id) from Runs
Inner Join MapPoints on runs.ID = MapPoints.RunId
Inner Join Tiles on mapPoints.TileName = Tiles.Name And Tiles.TilesetName = 'GrineerAsteroid') As GrineerAsteroidRuns,
(select Count(Distinct Runs.Id) from Runs
Inner Join MapPoints on runs.ID = MapPoints.RunId
Inner Join Tiles on mapPoints.TileName = Tiles.Name And Tiles.TilesetName = 'GrineerForest') As GrineerForestRuns,
(select Count(Distinct Runs.Id) from Runs
Inner Join MapPoints on runs.ID = MapPoints.RunId
Inner Join Tiles on mapPoints.TileName = Tiles.Name And Tiles.TilesetName = 'GrineerFortress') As GrineerFortressRuns,
(select Count(Distinct Runs.Id) from Runs
Inner Join MapPoints on runs.ID = MapPoints.RunId
Inner Join Tiles on mapPoints.TileName = Tiles.Name And Tiles.TilesetName = 'GrineerGalleon') As GrineerGalleonRuns,
(select Count(Distinct Runs.Id) from Runs
Inner Join MapPoints on runs.ID = MapPoints.RunId
Inner Join Tiles on mapPoints.TileName = Tiles.Name And Tiles.TilesetName = 'GrineerOcean') As GrineerOceanRuns,
(select Count(Distinct Runs.Id) from Runs
Inner Join MapPoints on runs.ID = MapPoints.RunId
Inner Join Tiles on mapPoints.TileName = Tiles.Name And Tiles.TilesetName = 'GrineerSettlement') As GrineerSettlementRuns,
(select Count(Distinct Runs.Id) from Runs
Inner Join MapPoints on runs.ID = MapPoints.RunId
Inner Join Tiles on mapPoints.TileName = Tiles.Name And Tiles.TilesetName = 'GrineerShipyards') As GrineerShipyardsRuns,
(select Count(Distinct Runs.Id) from Runs
Inner Join MapPoints on runs.ID = MapPoints.RunId
Inner Join Tiles on mapPoints.TileName = Tiles.Name And Tiles.TilesetName = 'OrokinMoon') As OrokinMoonRuns,
(select Count(Distinct Runs.Id) from Runs
Inner Join MapPoints on runs.ID = MapPoints.RunId
Inner Join Tiles on mapPoints.TileName = Tiles.Name And Tiles.TilesetName = 'OrokinTower') As OrokinTowerRuns,
(select Count(Distinct Runs.Id) from Runs
Inner Join MapPoints on runs.ID = MapPoints.RunId
Inner Join Tiles on mapPoints.TileName = Tiles.Name And Tiles.TilesetName = 'OrokinTowerDerelict') As OrokinTowerDerelictRuns,
(select Count(Distinct Runs.Id) from Runs
Inner Join MapPoints on runs.ID = MapPoints.RunId
Inner Join Tiles on mapPoints.TileName = Tiles.Name And Tiles.TilesetName = 'InfestedCorpusShip') As InfestedCorpusShipRuns,
(select Count(Distinct Runs.Id) from Runs
Inner Join MapPoints on runs.ID = MapPoints.RunId
Inner Join Tiles on mapPoints.TileName = Tiles.Name And Tiles.TilesetName = 'GrineerToCorpus') As GrineerToCorpusRuns,
(select Count(Distinct Runs.Id) from Runs
Inner Join MapPoints on runs.ID = MapPoints.RunId
Inner Join Tiles on mapPoints.TileName = Tiles.Name And Tiles.TilesetName = 'CorpusToGrineer') As CorpusToGrineerRuns,
(select Count(Distinct Runs.Id) from Runs
Inner Join MapPoints on runs.ID = MapPoints.RunId
Inner Join Tiles on mapPoints.TileName = Tiles.Name And Tiles.TilesetName = 'GrineerArchwing') As GrineerArchwingRuns,
(select Count(Distinct Runs.Id) from Runs
Inner Join MapPoints on runs.ID = MapPoints.RunId
Inner Join Tiles on mapPoints.TileName = Tiles.Name And Tiles.TilesetName = 'CorpusArchwing') As CorpusArchwingRuns
from Runs
Inner Join MapPoints on runs.ID = MapPoints.RunId
Inner Join Tiles on mapPoints.TileName = Tiles.Name