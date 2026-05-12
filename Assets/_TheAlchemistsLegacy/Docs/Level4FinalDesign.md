# Level 4 Final Design — The Final Legacy

**Status:** Updated design lock, May 12  
**Level role:** Capstone / final trial  
**Core fantasy:** The apprentice proves they understand the master's legacy by walking the castle ritual route, collecting four clearly marked alchemy materials, returning each one to a central alchemy station, and using the completed Legacy Seal to open the castle-wall gate.

---

## 1. Final Design Pillars

1. **Capstone, not new tutorial**  
   Level 4 combines familiar exploration, pickup, placement, HUD prompts, and final-door logic without adding new controls.

2. **Reuse old puzzle language only**  
   Material obstacles must echo mechanics the player already learned: pickup, local placement, key/chest, lantern reveal, ordered activation, and central crafting. Do not add new interaction types such as scales, mirrors, valves, or physics contraptions.

3. **No blind search**  
   Each clue and its material must appear in the same player view. Materials are never merely "nearby"; they sit directly under or beside their matching sign.

4. **No memory tax**  
   The HUD repeats the recipe after each clue is found, and the alchemy station repeats the complete order. The player should not need to remember a clue from several minutes ago.

5. **Icon-first readability**  
   The puzzle uses numbers, simple English, colors, and icons: `1 FIRE`, `2 SALT`, `3 STONE`, `4 DROP`. Text supports the icons, not the other way around.

6. **One-item carry friendly**  
   The player can carry only one item at a time, so the alchemy station must be a central return point. Never ask the player to carry all four materials to the third floor.

7. **Inheritance over possession**  
   The ending tests the theme by asking the player to return the completed Legacy Seal to the castle-wall gate, opening the true exit.

---

## 2. Locked Level Flow

### Phase 1 — Courtyard Framing

**Player starts:** central courtyard, facing the castle, central tablet, central alchemy station, and visible castle-wall exit gate.

**Central Tablet text:**

```text
The final legacy is not taken.
Four signs mark the old recipe.
Find each sign, take its material,
and return each one to the old cauldron.
The completed seal opens the wall gate.
```

**Purpose:** establish the final trial and direct the player toward the castle route.

### Phase 2 — Lantern and Recipe HUD

**Location:** castle ground floor / main hall.  
**Key object:** `Lantern_l4`, placed clearly on a table with a short master note.

The HUD recipe checklist is not gated by the lantern. It appears as soon as the player enters any recipe station trigger, picks up any recipe material, or reaches the alchemy station:

```text
Alchemy Recipe
1 FIRE: -
2 SALT: -
3 STONE: -
4 DROP: -
```

The lantern is a visual helper, not a progress gate. Carrying it can brighten signs, make icons glow, or improve readability, but the player can still read the station, see the HUD clue, and collect the material without it.

### Phase 3 — Castle Ritual Route

The player follows a route through the estate. Each station has one sign, one matching icon, one material in the same view, and one tiny obstacle that reuses earlier level mechanics.

| Order | Location | Scene sign | Old mechanic echoed | Material placement and obstacle | Pickup |
|---|---|---|---|---|---|
| 1 | Windmill side / left yard | `1 FIRE` with black flame icon | Level 1 fire/forge interaction, simplified | Player picks up `forge_tool`, then right-clicks `FireWooForAsh`. `Magic_fire_ash` hides and `Ash_l4` becomes collectible on the plane directly below the sign. | `Ash_l4` |
| 2 | Market stall / right shed | `2 SALT` with white crystal icon | Level 0/3 key-and-chest interaction | `Key_l4_salt` and `Chest_l4_salt` sit in the same stall view. Player uses the key to open the chest and collect `Salt_l4` inside. | `Salt_l4` |
| 3 | Castle lamp room and second floor | `3 STONE` with yellow rock icon | Level 3 lamp choice and door teleport | `scroll_level` on `table_no` gives the lamp clue. `table_2h` has `lamp_2th`; carrying it lets `Door_2th` teleport to `Door_2th_tar`. | `YellowStone_l4` |
| 4 | Castle lamp room and third floor | `4 DROP` with blue droplet icon | Level 3 lamp choice and door teleport | `table_3th` has `lamp_3th`; carrying it lets `Door_3th` teleport to `Door_3th_tar`. `table_no` has `lamp_no`, a broken lamp with no useful door. | `BlueDrop_l4` |

After each pickup, the HUD updates:

```text
Alchemy Recipe
1 FIRE: Ash ✓
2 SALT: -
3 STONE: -
4 DROP: -
```

**Purpose:** make the route feel like a castle pilgrimage while keeping the actual puzzle readable for international players.

**Lamp room rule:** the three table colors are the clue language. The left table `table_3th` holds `lamp_3th`, the right table `table_2h` holds `lamp_2th`, and the middle table `table_no` holds both `lamp_no` and `scroll_level`. `lamp_no` is intentionally useless, but it should not soft-lock the player because they can drop it and pick another lamp.

**Obstacle rule:** every local obstacle must take 10-30 seconds, must be solvable from the same room or same camera view as its clue, and must use prompts the player already understands.

### Phase 4 — Central Alchemy Station

**Location:** courtyard center or castle ground-floor main hall, close to the central tablet and not far from the player spawn.  
**Objects:** `AlchemyWorkbench_l4`, `AlchemyCauldron_l4`, four material slots, and a visible station recipe.

The station repeats the full answer in the world and HUD:

```text
Place materials in order:
1 FIRE -> Ash
2 SALT -> Salt
3 STONE -> Yellow Stone
4 DROP -> Blue Drop
```

Each slot uses the same number, icon, and color as the route signs.

Because the player can carry only one item, this is a hub-and-spoke loop:

```text
Find one material -> return to central alchemy station -> place it -> go find the next material
```

**Correct sequence:** Ash -> Salt -> Yellow Stone -> Blue Drop.  
**Wrong material:** show clear feedback and do not punish:

```text
Wrong slot. Match the number and icon.
```

**Reward:** create `LegacySeal_l4`.

### Phase 5 — Castle-Wall Gate Exit

**Location:** large gate in the castle wall, preferably the main wall gate visible from the courtyard.  
**Objects:** `LegacyGate_l4`, `LegacySealSlot_l4`, true ending trigger.

The player carries the completed `LegacySeal_l4` from the alchemy station to the gate and places it in the gate slot. The castle lights up, the wall gate opens, and the HUD says:

```text
You kept the legacy alive.
```

The player walks through the opened gate to trigger `TrailComplete`.

---

## 3. Required Points of Interest

| POI | Object / Location | Required design intent |
|---|---|---|
| 1 | Central Tablet / Courtyard | Frames the final legacy trial |
| 2 | Lantern_l4 / Castle main hall | Optional readability helper for signs and icons |
| 3 | FIRE Station / Windmill side | First clue and Ash pickup, same view |
| 4 | SALT Station / Market stall | Second clue and Salt pickup, same view |
| 5 | STONE Station / Castle second floor | Third clue and Yellow Stone pickup, same view |
| 6 | DROP Station / Castle third floor | Fourth clue and Blue Drop pickup, same view |
| 7 | Central Alchemy Station | Hub return point; repeats full order and creates Legacy Seal |
| 8 | Castle-Wall Gate | Receives Legacy Seal and opens the unified exit |
| 9 | Gate Exit Trigger | True ending route to `TrailComplete` |

---

## 4. HUD Rules

These are non-negotiable:

- The recipe checklist appears when the player enters any recipe station trigger, picks up any recipe material, or reaches the alchemy station.
- The recipe checklist must not depend on having `Lantern_l4`.
- On entering a station trigger, show a short HUD clue:

```text
Recipe Clue Found
1 FIRE -> Ash
Take the Ash below this sign.
```

- After pickup, update the recipe checklist immediately.
- At the alchemy station, show the complete recipe until the Legacy Seal is created.
- When holding a material, the objective should direct the player back to the central alchemy station.
- When holding `LegacySeal_l4`, the objective should direct the player to the castle-wall gate.
- If a station has a local obstacle, the HUD prompt must name the reused action directly: `Place the fire stamp`, `Open the salt chest`, `Bring the lantern`, or `Press the marks in order`.
- For the lamp room, HUD prompts should say which lamp/door is being used: `Door_2th needs the second-floor lamp`, `Door_3th needs the third-floor lamp`, and `The middle lamp has no path`.
- HUD text must use simple words only: `FIRE`, `SALT`, `STONE`, `DROP`, `Ash`, `Salt`, `Yellow Stone`, `Blue Drop`.
- The HUD should not require long reading during movement.
- `Lantern_l4` may improve sign visibility, but it never hides essential information or blocks pickup progress.

---

## 5. Visual Clarity Rules

- The sign and material must be visible together in one camera view.
- Each sign should use a large number, one simple word, one icon, and one strong color.
- The same number, word, icon, and color must appear on the sign, pickup, HUD checklist, and alchemy slot.
- Do not hide materials in corners, houses, crates, or behind props.
- Material obstacles must reuse earlier level mechanics only.
- Any helper object for a local obstacle must be in the same view as that station, except `Lantern_l4`, which is clearly staged in the main hall.
- `Door_2th` must teleport to `Door_2th_tar` only while holding `lamp_2th`.
- `Door_3th` must teleport to `Door_3th_tar` only while holding `lamp_3th`.
- `lamp_no` must not unlock either door.
- The alchemy station must sit at the central hub and repeat the complete order in-world and in HUD.
- The final exit is the castle-wall gate, not a third-floor chamber.
- Wrong actions should give useful feedback, not just `Not yet.`
- The final gate should be visible early so the player understands the long-term goal.

---

## 6. Staged Asset Mapping

| Puzzle item | Asset path |
|---|---|
| Ash | `Assets/_TheAlchemistsLegacy/Art/Level4PuzzleAssets/Ash/model/model.dae` |
| Salt | `Assets/_TheAlchemistsLegacy/Art/Level4PuzzleAssets/SaltProps/SaltBag.fbx` |
| Yellow Stone | `Assets/_TheAlchemistsLegacy/Art/Level4PuzzleAssets/YellowStone/YellowStone_MeseCrystal.blend` |
| Blue Drop | `Assets/_TheAlchemistsLegacy/Art/Level4PuzzleAssets/BlueDrop/MoonDropBottle.fbx` |
| Alchemy Workbench | `Assets/_TheAlchemistsLegacy/Art/Level4PuzzleAssets/AlchemyStation/AlchemyWorkbench.fbx` |
| Alchemy Cauldron | `Assets/_TheAlchemistsLegacy/Art/Level4PuzzleAssets/AlchemyStation/AlchemyCauldron.fbx` |

---

## 7. Implementation Acceptance Checklist

- [ ] Player can complete the whole flow without debug keys.
- [ ] Central Tablet frames the four-sign recipe trial.
- [ ] `Lantern_l4` is visible and clearly framed by a note or HUD prompt as an optional readability helper.
- [ ] HUD recipe checklist appears when entering any station trigger, picking up any material, or reaching the alchemy station.
- [ ] HUD recipe checklist does not require `Lantern_l4`.
- [ ] FIRE sign and Ash are visible in the same view.
- [ ] FIRE obstacle uses `Level4AshFromFirewood`: hold `forge_tool`, right-click `FireWooForAsh`, hide `Magic_fire_ash`, and reveal `Ash_l4` on the plane.
- [ ] SALT sign and Salt are visible in the same view.
- [ ] SALT obstacle uses `Key_l4_salt` to open `Chest_l4_salt`, revealing `Salt_l4`.
- [ ] STONE sign and Yellow Stone are visible in the same view.
- [ ] STONE path uses `lamp_2th` and `Door_2th -> Door_2th_tar`.
- [ ] DROP sign and Blue Drop are visible in the same view.
- [ ] DROP path uses `lamp_3th` and `Door_3th -> Door_3th_tar`.
- [ ] `lamp_no` is clearly a wrong/broken lamp and does not unlock either teleport door.
- [ ] Each pickup updates the HUD checklist.
- [ ] Alchemy station is placed at the central hub, not on the third floor.
- [ ] While holding a material, HUD points the player back to the central alchemy station.
- [ ] Alchemy station repeats the full order in HUD and in-world.
- [ ] Correct order Ash -> Salt -> Yellow Stone -> Blue Drop creates `LegacySeal_l4`.
- [ ] Wrong alchemy placement gives clear retry feedback.
- [ ] Returning `LegacySeal_l4` to the castle-wall gate opens the unified exit.
- [ ] True ending line appears before or while walking through the opened gate to `TrailComplete`.

---

## 8. Current Repo Gap Notes

`Level4.unity` still needs scene-side placement for the four stations, central alchemy station, castle-wall gate seal slot, HUD trigger logic, and final exit trigger. The existing Level 4 HUD support should be updated from relic-only objectives to the four-step recipe checklist.
