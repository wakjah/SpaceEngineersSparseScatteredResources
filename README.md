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
