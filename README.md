# WarframeTilesets

 A comprehensive repository of tileset information for Warframe


## Tilesets

Most of these folders will contain images in regards to the tiles in various procedurally generated maps. Maps of the tile, pictures from the entrances, overviews, and secrets. This is a long game project.

## Webpage

The page that is the centerpiece of this project.

Contains an Upload ability:  upload warframe screenshots, sorted into directories by userID/mapId/ and stored (indefinitely atm)

Ability to view individual tile's information and update the information in the database.

CodeFirst database structure, so anyone can install the database structure anywhere.

Basic Functionality of major points is in place as of 9/26/19. 

## Future Webpage Plans

* Allow user profiles
* Figure out the best way to prevent sql lockups from many users using the page at once. (Concurancy Checks!!!)
		*(this should actually not be that much of an issue, because most users will only be able to insert, not update - but still)*
* make it look much damn better

* have various links to the Tileset indexes, and the Tiles themselves, to show secrets and other information.
* Show statistics of how common tiles from aggregated data from many runs, what mission type it is most common in

* SuperUsers with the ability to Update Tile Pages
* Regular  Users with the ability to request a change, but that needs to be approved by SuperUsers


## Recent Changes

### 8/22
* Rebuilt Database with CodeFirst properties, and proper migration. 
* If Picture is available of Map of tile, show on ProofRead View (need to rename that view!)

#### Upcoming To Do:
* Edit for TileDetails with Drop down lists for Tileset and Tilename (or something? Grid view?)

### 8/23
* Renamed ProcessFiles view to Review
* Submit button on Review now properly filters out 'Unchecked' files before submitting them into the database.


### 9/26
* Files can be uploaded using the Upload button (no longer scanning my pictures/warframe directory)
* Files stored in directory structure userId/mapId
* Tile Information View mostly complete
* Tile Information Edit mostly complete
* Review of Uploaded Tiles shows image uploaded, map of tile, and the ability to add points to that tile (Kuva, Ayatan, Medallion, Mobile Defense, ect)
* All tiles saved in MapPoints now, not just unique ones - flag for potential 'Bad runs' with extra multiples of the same tile or not all the tiles. 
* flag for runs that did not include the extra add points (ayatan, ect)
* Ability to upload Map/View images.

#### Upcoming To Do:
* Add Variant's links to Tile View and Edit Tile
* Index ability to pick out tiles directly.
* Next Tile/Previous Tile options for alphabetical movement through the tiles of a tileset on the view pages
* users
