# WarframeTilesets

 A comprehensive repository of tileset information for Warframe


## Tilesets

Most of these folders will contain images in regards to the tiles in various procedurally generated maps. Maps of the tile, pictures from the entrances, overviews, and secrets. This is a long game project.

## Webpage

This section contains (Currently) work to figure out just how common various tiles are. It uses an ASP.Net Core MVC application that will read in screenshots from the game that contain metadata, display the meta data, then add it to a sql server.

As of 8/16/19 the webpage works in my personal test environment (that is, the SQL server is only hosted on my computer, and it pulls directly from my Pictures/Warframe folder on my c:drive) There are *many* unique and special cases to the way Digital Extremes titles its map tiles, and as such there is much work to do to figure them out and build exceptions for them, before I release this into the wild for others to upload their images for.

There is a SQL create script for creating a similar database (but this is in the progress of being a CodeFirst application, so you shouldn't need it), and you'll have to adjust the code to look properly for your images if you want to run this yourself.

(Ie: the uploads button 'works' but doesn't actually do anything yet)

## Future Webpage Plans

* Allow user profiles
* Allow users to upload images, and have the meta data pulled from and processed
* Upload files into temp directories that will be destroyed at the end of the process.
* Figure out the best way to prevent sql lockups from many users using the page at once. (Concurancy Checks!!!)
* make it look much damn better

* tie in the Tileset images I'm creating above. Show the map image for each tile during the 'check' process of parsing out the data. 
* have various links to the Tileset indexes, and the Tiles themselves, to show secrets and other information.
* Show statistics of how common tiles from aggregated data from many runs, what mission type it is most common in

* SuperUsers with the ability to Update Tile Pages
* Regular  Users with the ability to request a change, but that needs to be approved by SuperUsers

* Record Type of Special Add To Mission - Ie: Kuva Siphon, Kuva Flood, Syndicate, Void Rift, Nightmare

## Recent Changes

### 8/22
* Rebuilt Database with CodeFirst properties, and proper migration. 
* If Picture is available of Map of tile, show on ProofRead View (need to rename that view!)

#### Upcoming Next:
* Edit for TileDetails with Drop down lists for Tileset and Tilename (or something? Grid view?)

