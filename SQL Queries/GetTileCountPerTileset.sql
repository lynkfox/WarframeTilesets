select 
 count(*) as TotalTiles,
 sum(case when TilesetName = 'CorpusGasCity' then 1 else 0 end) GasCity,
  sum(case when TilesetName = 'CorpusIcePlanet' then 1 else 0 end) CorpusIcePlanet,
  sum(case when TilesetName = 'CorpusOutpost' then 1 else 0 end) CorpusOutpost,
  sum(case when TilesetName = 'CorpusShip' then 1 else 0 end) CorpusShip,
  sum(case when TilesetName = 'CorpusToGrineer' then 1 else 0 end) CorpusToGrineer,
  sum(case when TilesetName = 'GrineerArchwing' then 1 else 0 end) GriArchwing,
  sum(case when TilesetName = 'GrineerAsteroid' then 1 else 0 end) GrineerAsteroid,
  sum(case when TilesetName = 'GrineerForest' then 1 else 0 end) GrineerForest,
  sum(case when TilesetName = 'GrineerFortress' then 1 else 0 end) GrineerFortress,
  sum(case when TilesetName = 'GrineerGalleon' then 1 else 0 end) GrineerGalleon,
  sum(case when TilesetName = 'GrineerOcean' then 1 else 0 end) GrineerOcean,
  sum(case when TilesetName = 'GrineerSettlement' then 1 else 0 end) GrineerSettlement,
  sum(case when TilesetName = 'GrineerShipyards' then 1 else 0 end) GrineerShipyards,
  sum(case when TilesetName = 'GrineerToCorpus' then 1 else 0 end) GrineerToCorpus,
  sum(case when TilesetName = 'InfestedCorpusShip' then 1 else 0 end) InfestedCorpusShip,
  sum(case when TilesetName = 'OrokinMoon' then 1 else 0 end) OrokinMoon,
  sum(case when TilesetName = 'OrokinTower' then 1 else 0 end) OrokinTower,
  sum(case when TilesetName = 'OrokinTowerDerelict' then 1 else 0 end) OrokinTowerDerelict,
  sum(case when TilesetName = 'CorpusArchwing' then 1 else 0 end) CorpusArchwing
 from Tiles






