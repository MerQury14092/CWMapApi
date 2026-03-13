# CWMapApi – Custom Map API for Content Warning

A lightweight, dependency-free API that allows modders to add fully functional custom maps (moons) to **Content Warning**


## How to Create a Custom Map

1. Create your map in Unity
2. Build it as an **AssetBundle** containing exactly one scene
3. In your mod, load the AssetBundle and the scene yourself (via `AssetBundle.LoadFromFile`)
4. Create a class that inherits from `CWMapApi.Api.CustomMap`
5. Register an instance of your class with `CustomMapApi.RegisterCustomMap(yourMapInstance)`

### Required Scene Setup

| Object                | Purpose                                                                                           | Important Notes                                                                                             |
|-----------------------|---------------------------------------------------------------------------------------------------|-------------------------------------------------------------------------------------------------------------|
| **PatrolPoints**      | Empty parent → child Transforms = positions where monsters patrol and where scrap/items can spawn | This GameObject will be **destroyed** after initialization. Only its child positions are used.            |
| **DiveBellSpawns**    | Empty parent → child Transforms = possible dive-bell spawn locations                              | This GameObject will be **destroyed** after initialization. Only positions matter.                        |
| **Navmesh**           | GameObject containing your baked NavMesh                                                          | Required for normal AI movement                                                                            |
| **NavmeshWide**       | GameObject containing your baked NavMesh for large enemies (BigSlap, Toolkit, Fire, etc.)         |     Required for normal AI movement                                  |
| **Level**             | Empty root object that holds all your map geometry and props                                      | Game logic (lighting, audio, etc.) will parent under this object during initialization                     |

**Critical:** All objects that have colliders **must** be on Unity Layer index **10**.

### Example Implementation

```csharp
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using CWMapApi.Api;

public class MyCustomMoon : CustomMap
{
    public override string GetMapName() => "Abandoned Factory";
    public override string GetSceneName() => "AbandonedFactoryScene"; // exact scene name inside your AssetBundle

    public override GameObject GetPatrolPoints()    => GameObject.Find("PatrolPoints");
    public override GameObject GetDiveBells()       => GameObject.Find("DiveBellSpawns");
    public override GameObject GetNavmesh()         => GameObject.Find("Navmesh");
    public override GameObject GetNavmeshWide()     => GameObject.Find("NavmeshWide"); // or return GetNavmesh()
    public override GameObject GetLevel()           => GameObject.Find("Level");

    // Highly recommended for compatibility with ConfigureMap and other map-related mods
    public override string GetMapIconPath()
    {
        return Path.Combine(PluginInfo.PLUGIN_GUID.Directory, "icon.png"); // 256×256 PNG
    }

    // Runs right after your scene is loaded, but BEFORE game logic (players, dive-bell, etc.) is initialized
    public override IEnumerator Prefix()
    {
        SetDefaultSkybox();                                      // removes skybox, sets black ambient (vanilla look)
        SetDefaultWorldMaterialRecursive(GetLevel());           // applies the dark default material to everything
        yield return null;
    }

    // Runs AFTER full map initialization (dive-bell placed, players spawned, etc.)
    public override IEnumerator Postfix()
    {
        // Spawn extra props, play music, modify lighting, etc.
        yield return null;
    }
}
```
## Random Map Pool Control

By default, CWMapApi automatically adds every registered custom moon to the random rotation.

If your mod (or another mod such as ConfigureMap) wants full control over which moon is selected, disable the automatic injection:

`CustomMapApi.EnableMapPoolPatch(false);`

Call with `true` to restore default behavior.

## Included Helper

The mod package contains **DiveBell.unitypackage** – import it into Unity to get an exact 1:1 model of the dive bell for easier placement of spawn points.

## Static API Reference

| Method                                            | Description                                                                 |
|---------------------------------------------------|-----------------------------------------------------------------------------|
| `RegisterCustomMap(CustomMap map)`                | Registers your moon (throws if a moon with the same name already exists)   |
| `UnregisterCustomMap(CustomMap map)`              | Removes a previously registered moon                                        |
| `EnableMapPoolPatch(bool enable)`                 | Turns automatic random-pool injection on/off (default: true)               |
| `IReadOnlyList<CustomMap> GetRegisteredCustomMaps()` | Returns a read-only list of all registered moons                         |

## Common Issues & Fixes

- Map looks too bright → call `SetDefaultWorldMaterialRecursive(GetLevel())` in `Prefix()`
- Colliders don’t work → all geometry colliders must be on **Layer 10**
- No icon in ConfigureMap → implement `GetMapIconPath()`
- Monsters get stuck or dive bell spawns inside walls → re-bake NavMesh and double-check spawn point placement

Happy map-building!