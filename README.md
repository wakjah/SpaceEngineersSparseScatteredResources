# SpaceEngineersSparseScatteredResources
Scatters resources about the solar system and makes the ore spawning much sparser.

This mod is inspired by the mods "Scarce Resources" and "Procedurally Generated Ore".

Like Scarce Resources, each ore is only available in certain places. However, this mod does not have basic resources (iron, nickel, silicon, cobalt) on all the planets and moons.

Instead, the resources are distributed according to the following:

- Moons: Silver and Gold only
- Earth: Iron and Cobalt only
- Mars: Nickel and Platinum only
- Alien: Silicon and Uranium only
- Triton: Silicon and Magnesium only
- Pertam: Silicon only

Ice and boulders are still available in most places.

Asteroids have silver and iron, just like Scarce Resources. 

Update: Asteroid ore spawning is configurable as of v0.2. Edit `%AppData%\SpaceEngineers\Saves\<YOUR_SAVE>\Sparse Scattered Resources_SparseScatteredResourcesMod\Settings.xml` after loading a world with the mod enabled.

Similar to Procedurally Generated Ore, resource patches spawn differently to vanilla. Instead of completely replacing the vanilla spawning, however, this mod takes a different approach.

This mod takes the vanilla ore spawning locations and patch geometries, and removes about 70% of them. 

The pattern used to pick which patches to remove follows a Perlin noise pattern, so there will be resource rich regions and barren wastelands.

# Running the Manipulator Script

The game's resource generation and planet terrain is based on images describing properties across the surface of the planet. This mod's source contains a few scripts used to manipulate these images to achieve its aim of deleting resources. 

You can use these scripts to integrate this mod with custom worlds, or if you just want custom resource settings, or to mess with the resource deletion algorithm. 

NOTE: You will need to be familiar with modding Space Engineers in general in order to be able to use the output of these scripts. It is not the job of this README to explain everything about how to mod the game. A good place to get started with modding Space Engineers is the Keen Software Discord server. 

- `copy_source_images.sh`: Copies base game planet data files into a subdirectory in the source directory.
- `process_images.sh`: Runs the manipulator Python script on the images copied in the previous step, producing a new directory with the processed images. Requires an installation of Anaconda in the expected location; you will probably have to tweak the script for your system if you want to run this.
- `src/manipulator/remove_resources.py`: The actual resource manipulator script.

`remove_resources.py` can be run from an Anaconda prompt:

```
python3 remove_resources.py SOURCE [DEST]
```

SOURCE is either a file or a directory. 

DEST is an optional argument that specifies output location. If input location was a file, output location must be a file; if input was a directory, output must be a directory. If DEST is not supplied, a `matplotlib` plot of the resources after removal is shown instead.

Note that the randomization used in this script is seeded by the hash of the filename of the source file. This means that the same output should be generated for the same input file time and again.
