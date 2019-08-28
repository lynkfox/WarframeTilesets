select Count(Distinct Runs.Id) as CorpusShipRuns  from Runs
Inner Join MapPoints on runs.ID = MapPoints.RunId
Inner Join Tiles on mapPoints.TileName = Tiles.Name And Tiles.TilesetName = 'CorpusShip'

select * from runs;