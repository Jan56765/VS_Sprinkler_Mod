# THIS MOD IS IN BETA FOR NOW AND WILL PROBABLY RECEIVE SIGNIFICANT UPDATES IN THE NEAR FUTURE

## Description

This mod adds sprinklers of varying tiers to automate watering farmland.

Currently available tiers are Bronze, Iron and Steel. Placing a sprinkler won't do anything immediately but when filled with water they will periodically activate and water all fields in a set radius, depending on their tier. Once they reach an average moisture threshold of about 80% they will stop until watering is necessary again.

Many aspects of the mod were made mostly out of convenience and if you have a suggestion for an improvement, feel free to add a comment and I will then decide which of those I will try to do. Also, please open an issue for any bugs you might encounter while playing with it. As far as the code side goes, this mod shouldn't be able to brick your world. I am currently playing with this mod on a private server to find bugs myself but so far no game breaking stuff was caused by it. So feel free to try it out for yourself and give feedback! (Should anything still break your world, just load in without the mod and remove any sprinklers you placed)

## The Sprinklers

Bronze - Range 2 Blocks, crafted with 1 Bronze Rod, 4 Bronze Nails and Strips, 2 Rusty Gears, 1 Bucket (Any type of Bronze will work)

Iron - Range 4 Blocks, crafted with 1 Iron Rod, 4 Iron Nails and Strips, 3 Rusty Gears, 1 Bucket

Steel - Range 6 Blocks, crafted with 1 Steel Rod, 4 Steel Nails and Strips, 1 Temporal Gear, 1 Bucket

## Other Stuff

### Configuration

xyzSprinklerRange - Determines the watering radius for the corresponding Sprinkler type (default 2, 4, 6 respectively) 

waterConsumption - Multiplies the water consumption per activation. Formula: 0,1 Liters * waterConsumption (default 1) 

minIntervalInMillis - Defines a base value for how often (in milliseconds) the Sprinklers should activate. To prevent lag, each Sprinkler will be added up to 5 seconds of random delay on top of this value. (default 10.000, very much recommended) 

Don't forget to adjust these config values on client and server if you want to change them, otherwise there will be visual issues.

## Known Limitations

- Due to the random nature of the sprinklers activations, there might be a delay of up to one activation between the server and the clients. For now I haven't found a better way, but this shouldn't be much of an issue as the sprinklers are more of a set and forget type of item.
- The sprinklers will NOT work with timeskipping or fastforwarding of any type. Running /time add 10000 WILL kill your crops. Also they won't run while their chunks aren't loaded. Maybe I will adress this in the future.
- I have tried to make this mod as performant as I could. I tested up to 500 sprinklers running at once. Still I wouldn't recommend recklessly adding large amounts sprinklers into your world. At some point it will most likely affect performance.