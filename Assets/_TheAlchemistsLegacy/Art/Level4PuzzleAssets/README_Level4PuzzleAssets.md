# Level 4 Puzzle Asset Staging

This folder stages the downloaded free assets for the Level 4 alchemy sequence.

## Puzzle Mapping

| Puzzle item | In-game label | Suggested staged asset | Suggested placement |
|---|---|---|---|
| Ash | FIRE / Ash | `Ash/model/model.dae` with `Ash/Ash_l4.mat` | Windmill side, beside an extinguished brazier or cold fire pit |
| Salt | SALT / Salt | `SaltProps/SaltBag.fbx` with `SaltProps/SaltCrateEmpty.fbx` | Market stall or right-side shed |
| Yellow Stone | STONE / Yellow Stone | `YellowStone/YellowStone_MeseCrystal.blend` | Castle second floor, near a sunlit window or tower interior |
| Blue Drop | DROP / Blue Drop | `BlueDrop/MoonDropBottle.fbx` | Castle third floor or tower-top basin |

## Alchemy Station

Use these as final ritual props:

- `AlchemyStation/AlchemyWorkbench.fbx`
- `AlchemyStation/AlchemyCauldron.fbx`
- `AlchemyStation/SmallBottle.fbx`
- `AlchemyStation/ShelfSmallBottles.fbx`

## Visual Language

Keep the player-facing puzzle simple:

1. `FIRE` with a black flame icon -> Ash
2. `SALT` with a white crystal/bag icon -> Salt
3. `STONE` with a yellow rock icon -> Yellow Stone
4. `DROP` with a blue droplet icon -> Blue Drop

The wall sign, pickup object, inventory text, and alchemy station slot should use the same color and icon.

## Import Notes

- Quaternius assets are CC0. The shared textures are staged in `CommonTextures`.
- The OpenGameArt potion bottle is CC0 and is staged as `MoonDropBottle`.
- The Mese crystal source is a `.blend` file. If Unity cannot import it, convert it to FBX in Blender or replace it with a yellow-tinted low-poly crystal/stone.
- The ash decal source is CC Attribution. Add credit for `Aartee - Pile of ash decal` if it ships in the project.
