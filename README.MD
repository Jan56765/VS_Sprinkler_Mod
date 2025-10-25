## Description

This mod adds sprinklers of varying tiers to automate watering farmland. 

Currently available tiers 1 through 4. Placing a sprinkler won't do anything immediately but when filled with water they will periodically activate and water all fields in a set radius, depending on their tier. Once they reach an average moisture threshold of about 95% they will stop until watering is necessary again (that is when falling below 85%). 

Please open an Github issue for any bugs you might encounter while playing with it. 

You can submit feedback here: https://www.vintagestory.at/forums/topic/17909-sprinklers-mod-feature-requests/ 

To use this mod, it needs to be installed on server and client side. For now this mod will most likely be compatible with each subversion of 1.21. 

## The Sprinklers

The sprinklers are split into 4 Tiers (with the last one being more of a bonus for people who want to go the extra mile), each with 6 variants. Those are: Copper, Tin Bronze, Bismuth Bronze, Black Bronze, Brass and Lead. Crafting a sprinkler as one of the variants will require the upgraded Tiers to also use the same material. The Variants offer no advantage over each other and exist just to add visual variety. To be able to craft the Brass and Lead sprinklers, rods, as well as nails and strips have been added for the corresponding material. 

Tier 1 - Range 2 Blocks, Tank size 20 Liters, crafted with 1 Rod, 8 Nails and Strips, 2 Rusty Gears, 1 Bucket 

Tier 2 - Range 4 Blocks, Tank size 40 Liters, crafted with 1 T1 sprinkler, 12 Nails and Strips, 4 Rusty Gears, 1 Bucket 

Tier 3 - Range 6 Blocks, Tank size 60 Liters, crafted with 1 T2 sprinkler, 12 Nails and Strips, 1 Temporal Gear, 1 Bucket 

Tier 4 - Range 8 Blocks, Tank size 100 Liters, crafted with 1 T3 sprinkler, 8 Metal Parts, 3 distinct Jonas parts*, 1 Barrel 

*Those parts being: Jonas sub-assembly (A difference gear assembly), Jonas part (parallel transfer vessel), Jonas sub-assembly (Finely balanced Oscillator) 

There's also downwards facing sprinklers for your fancy industrial looking greenhouses. Those are made by putting any sprinkler into the crafting grid. Once upside-down they can also be reverted to normal by putting them in again. Putting them on a ceiling, you can adjust their vertical range by right clicking on them with your hands. This will toggle their range from 3-6 Blocks below them. This choice was made as switching from a square to a cube area would significantly reduce performance per sprinkler placed. 

## Other Stuff
### Configuration

tXSprinklerRange - Determines the watering radius for the corresponding Sprinkler tier (default 2, 4, 6, 8 respectively) 
tXSprinklerVolume - Determines the Tank size foor the corresponding Sprinkler tier (default 20, 40, 60, 100 respectively) 
waterConsumption - Multiplies the water consumption per activation. Formula: 0,1 Liters * waterConsumption (default 1) 
minIntervalInMillis - Defines a base value for how often (in milliseconds) the Sprinklers should activate. To prevent lag, each Sprinkler will be added up to 5 seconds of random delay on top of this value. (default 10.000, very much recommended) 

Don't forget to adjust these config values on client and server if you want to change them, otherwise there will be visual issues. 

### Known Limitations

- Due to the random nature of the sprinklers activations, there might be a delay of up to one activation between the server and the clients. For now I haven't found a better way, but this shouldn't be much of an issue as the sprinklers are more of a set and forget type of item. 
- The sprinklers will NOT work with timeskipping or fastforwarding of any type. Running /time add 10000 WILL kill your crops. Also they won't run while their chunks aren't loaded. Maybe I will adress this in the future. 
- I have tried to make this mod as performant as I could. I tested up to 500 sprinklers running at once. Still I wouldn't recommend recklessly adding large amounts sprinklers into your world. At some point it will most likely affect performance. 

### FAQ

Q: After an update of the mod the game randomly crashes. 

A: This might happen because of removed sprinklers which try to initialize animations even though they don't exist anymore in the world. In that case, uninstall the mod, load your world once, save and quit and then reinstall the newest version. 