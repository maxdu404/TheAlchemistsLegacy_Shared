# The Alchemist's Legacy — Game Design Document

**Version:** Alpha Design v2 · **Date:** 2026-04-22
**Subject:** UTS 31262 / 32003 Game Design Methodologies
**Assessment:** A3 Alpha (Week 10) → A4 Beta (Week 12)
**Submission:** WebGL build to https://itch.io/jam/uts-game-design-subjects-2026

> **Working principle (v2):** design all levels purely from intent — *what does the player experience and learn?* — and only AFTER all level designs are locked do we map them to existing scripts / assets (see §17, currently a stub to be filled later). Each level section below follows the tutor's reference format: design goals → iterations → POI breakdown → known flaws → planned fixes.

---

## 1. Assignment Constraints (non-negotiable)

| Constraint | Value |
|---|---|
| Total playtime | ≤ 20 minutes |
| Level count | 5 (L0 tutorial + L1–L4) — confirmed by teacher |
| Themes (must use exactly 2) | **Set in Stone** + **Power Move** |
| Content rules | No violence, no adult content (−5 group penalty) |
| Level numbering | Must be unambiguous in Level Select (or 0% Individual Level Quality) |
| Pause / exit | ESC must pause and allow return to main menu |
| Asset sources | Full attribution list required (0% Game Quality if missing) |

---

## 2. Premise

> You are the apprentice of a master alchemist who has passed.
> The master left no will of words — only trials **set in stone**: carved runes, sealed pedestals, silent inscriptions.
> You inherit nothing by force. Each trial demands one decisive **power move** — the right artifact, applied at the right place.
> Complete the four trials. Claim the legacy.

The master's voice speaks only through inscriptions the player finds — every scroll and stone tablet is his guidance from beyond.

**Tone:** mystical, instructive, spare. Slightly cryptic but always fair. Never threatening.

---

## 3. Theme Expression

### Set in Stone
- **Literal:** every puzzle is anchored by a stone artifact — runes, pedestals, tablets, engraved door seals, the final relic pedestals.
- **Metaphorical:** the master's trials are immutable. Sequences, symbol matches, and carved instructions cannot be negotiated with — only followed.

### Power Move
- Each level resolves with **one decisive artifact-in-right-place moment**:
  - L0: rune into door slot
  - L1: seal stamp into door recess
  - L2: flame onto torch (in correct order)
  - L3: slab onto altar
  - L4: three relics onto their pedestals
- The player never overpowers anything. They apply the *right power at the right spot* — the opposite of violence, the essence of alchemy.

Both themes combine into a single mechanical identity: **progression = discover the correct artifact → deploy it in the correct location**.

---

## 4. Core Mechanics (Player Verbs)

| Verb | Input | Scope |
|---|---|---|
| Move | WASD | All levels |
| Look | Mouse | All levels |
| Jump | Space | Disabled for this build — the player steps over small obstacles through controller step offset, but Space is not a required verb |
| Pick up / Drop | E | All levels |
| Interact / Use held item | Right-click | All levels |
| Read scroll | Right-click on scroll | Introduced L0 |
| Pause / Exit to menu | ESC | All levels |

**Design rule:** the player should never need a verb the tutorial didn't teach.

---

## 5. Level Arc Summary

| # | Title | New mechanic | Time | Primary theme beat |
|---|---|---|---|---|
| L0 | The Apprentice's Door | Observation + pick-up + place-in-slot | ~1 min | Set in Stone introduced |
| L1 | The Master's Seal | Crafting (combine materials at a station) | ~4 min | Power Move (the seal) |
| L2 | Flame of Memory | Ordered-action sequence with reset | ~4 min | Set in Stone (immutable order) |
| L3 | The Master's Lantern | Multi-zone gathering + environmental light trigger | ~5 min | Power Move (light reveals) |
| L4 | The Final Legacy | Free exploration + symbol-matching (no scroll hints) | ~5 min | Set in Stone (legacy claimed) |

**Total:** ~19 minutes, 1 minute under the 20-min cap.

### 5.1 Level completion and menu flow
For the Beta build, levels are launched from the **Main Menu / Level Select** rather than forced into a single continuous campaign chain. This supports the assessment requirement that playtesters can try every level even if they cannot complete a previous one.

- **Start New Game** begins at **Level 0 — Tutorial**.
- **Level Select** allows direct entry to **Level 0, Level 1, Level 2, Level 3, or Level 4**.
- Completing any level plays that level's short success moment, then fades back to the **Main Menu / Trial Complete** hub.
- The next level is not auto-loaded. The player chooses the next level from Level Select.
- ESC pause always offers **Resume** and **Return to Main Menu**.

---

## 6. Level 0 — The Apprentice's Door

### 6.1 Designer's goals
- Teach **every player verb** used in the rest of the game.
- Keep difficulty at **zero** — failure should not be possible, only delay.
- Establish the **master's voice** and the **Set-in-Stone visual vocabulary** (engraved runes, stone slots).
- Graded directly for "Tutorial Level Quality" — it must feel guided without feeling condescending.

### 6.2 Master's voice — scroll text
> *"My apprentice — read slow, look close.*
> *What is marked in stone cannot be forgotten.*
> *Match the runes, and the path will open."*

### 6.3 Space
A single small bedroom/laboratory room. Bed, desk with scroll, chair, small treasure chest, wardrobe, exit door with 2-slot rune lock. One way in (spawn), one way out (door).

**Current Unity implementation note:** `TheAlchemistsLegacy_Level0.unity` is generated from `Assets/Scenes/Level9.unity` so the room shell, lighting, walls, floor, and decorations are reused as kit-bash material. Old Level9 gameplay objects, UI, pickups, triggers, and manager scripts are removed. The tutorial-specific objects are then placed into the copied room.

### 6.4 Points of Interest (numbered, top-down intent)

| POI | Object | Player learns |
|---|---|---|
| 1 | Spawn point (beside bed) | Orients player facing the desk with scroll on it |
| 2 | Desk + scroll | Right-click to read; master's first message appears full-screen |
| 3 | Rune A (on desk, plain sight) | E to pick up |
| 4 | Treasure chest (beside / beneath desk) | Right-click to open a container; Rune B is inside |
| 5 | Rune B (inside chest) | Reinforces pick-up + rewards looking in containers |
| 6 | Door rune lock (2 slots) | Right-click while holding item to place into slot |
| 7 | Exit door | Opens when both slots filled → short success line → fade to the Trial Complete hub |

### 6.5 Moment-to-moment flow
1. Spawn → prompt "*[RMB] Read*" on scroll.
2. Read scroll → full-screen message → player internalizes goal.
3. Pick up Rune A (visible on desk).
4. Notice treasure chest → right-click → chest opens → Rune B inside.
5. Pick up Rune B.
6. Walk to door → prompt "*[RMB] Place rune*" at each slot.
7. Slot both runes → door opens → master's success line appears → walk through the exit → fade to the `TrailComplete` scene, presented to the player as `Trial Complete`.

### 6.6 Design iterations
- **V0 (Max's original):** scroll + 2 wall symbols + symbol-match lock; no container. ~60 sec, but only teaches reading + matching.
- **V1 (current):** added treasure chest (POI 4–5) so the level teaches *container interaction* too. Adds ~20 sec but broadens verb coverage.
- **V2 (post-Alpha playtest):** TBD — will review whether the chest reads as interactive enough, and whether players read the scroll or skip it.

### 6.7 Known flaws & planned fixes
| Flaw | Severity | Planned fix |
|---|---|---|
| Chest may not read as interactive (looks like static furniture) | High | Subtle emissive highlight + prompt when player looks at it; escalate to an outline if player is idle 15s |
| Player could skip the scroll and still solve by trial-and-error | Low | Acceptable — trial-and-error still teaches pick-up + place; scroll is narrative reinforcement, not puzzle blocker |
| Only 2 runes may feel too short | Low | Keep at 2 — tutorial pedagogy favors brevity; a 3rd would bore returning players |

### 6.8 Theme expression
- **Set in Stone:** runes are literal engraved stone tiles; door lock is a carved recess.
- **Power Move:** slotting the runes is the player's first decisive artifact placement — establishes the pattern every subsequent level echoes.

### 6.9 Time target
60–90 seconds.

### 6.10 Developer build target
Build Level 0 as a **complete vertical slice** of the whole game loop:

`spawn -> read hint -> pick up Rune A -> open chest -> pick up Rune B -> place both runes -> door opens -> fade to Trial Complete hub`

This level should be simple enough that a first-time playtester can finish it without help, but complete enough that later levels can reuse the same interaction pattern.

### 6.11 Adapt-don't-copy rule for existing project files
The current Unity project contains assets and scripts from a different game. These are allowed as **kit-bash material**, but Level 0 must be visibly redesigned for *The Alchemist's Legacy*.

Use existing material only in these ways:
- **Okay to reuse:** room props, door meshes, chest/box models, outline/highlight effects, prompt UI ideas, player controller, pickup logic, simple door animation logic.
- **Must adapt:** object names, puzzle logic, prompt text, item visuals, scene layout, completion flow, and narrative text.
- **Do not copy as-is:** any old puzzle whose objective, item identity, level flow, or story belongs to the previous game.

Every adapted asset must be added to the asset/source list. If the team changes an existing script, rename variables and comments around the new alchemy/rune purpose so the code is understandable to the team.

### 6.12 Unity scene specification
Current scene name: `TheAlchemistsLegacy_Level0`.

| Unity object | Required components / setup | Behaviour |
|---|---|---|
| `Player_L0` | First-person controller, camera, crosshair, pickup/interact raycast, pause support | Spawns beside bed facing desk. Player can move with WASD, look with mouse, pick/drop with E, interact with RMB |
| `UI_HUD` | Crosshair, contextual prompt text, held-item text | Shows only useful prompts; never blocks view |
| `UI_ReadPanel` | Full-screen/semi-full-screen text panel, close action | Opens when player RMBs the scroll; closes with RMB/E/ESC |
| `Scroll_MasterIntro` | Collider, interactable layer/tag, outline-on-look | Prompt: `[RMB] Read`; opens the master's scroll text |
| `Desk_RuneSun` | Small stone tile, collider, pickup tag/layer, rigidbody if needed. Current placeholder: Level3 `FireSphere` | Prompt: `[E] Pick up Sun Rune`; can be carried/dropped |
| `Chest_RuneContainer` | Small treasure chest model, collider, interactable layer/tag, closed/open state | Prompt: `[RMB] Open Chest`; opens once and reveals second rune |
| `Chest_RuneMoon` | Hidden at scene start or placed inside closed chest. Current placeholder: Level3 `WoodSphere` | Becomes visible/pickable after chest opens |
| `Door_L0` | Door mesh, closed/open animation or simple rotate/slide | Opens only after both rune slots are filled |
| `DoorSlot_Sun` | Collider trigger/interactable, symbol decal above/inside slot. Current placeholder: Level3 `Bucket1` | Accepts only `FireSphere`; prompt while holding it: `[RMB] Place Sun Rune` |
| `DoorSlot_Moon` | Collider trigger/interactable, symbol decal above/inside slot. Current placeholder: Level3 `Bucket2` | Accepts only `WoodSphere`; prompt while holding it: `[RMB] Place Moon Rune` |
| `ExitTrigger_ToMainMenu` | Trigger volume behind/inside opened door | Starts fade-out and loads the `TrailComplete` scene, which is visually labelled `Trial Complete` |

Minimum room layout:

```text
┌──────────────────────────────┐
│ Bed / spawn                  │
│   P -> faces desk            │
│                              │
│ Desk: Scroll + Sun Rune      │
│ Chest: Moon Rune             │
│                              │
│          Door + 2 Rune Slots │
└──────────────────────────────┘
```

### 6.13 Interaction rules
- The player can hold only one item at a time.
- Pressing `E` while holding an item drops it safely in front of the player.
- Right-click on the scroll reads; right-click on the chest opens; right-click on a slot places the held rune.
- A wrong rune at a slot should not disappear. It should stay in the player's hand and show a short prompt: `This rune does not match this mark.`
- If the player looks at the door with no rune held, prompt: `Two stone marks wait for their runes.`
- If one rune is placed, the matching slot glows softly and remains filled.
- When both runes are placed, play a short stone-grind sound, open the door, and show the master's closing line: `Good. You remember how to see.`
- After the player walks through the opened doorway, fade out and load the `TrailComplete` scene. For this project, that scene functions as the level-select hub and should be visually labelled `Trial Complete`.

### 6.14 Implementation order for developers
1. Run `Tools > The Alchemist's Legacy > Create Level 0 Scene` in Unity Edit Mode.
2. The generator copies `Level9.unity` into `TheAlchemistsLegacy_Level0.unity` as the room base.
3. The generator keeps the room shell, lighting, walls, floor, and decoration, then removes old gameplay objects.
4. It creates a fresh Player, HUD prompt UI, scroll, treasure chest, tutorial manager, door, and exit marker.
5. It clones Level3 `FireSphere`, `WoodSphere`, `Bucket1`, and `Bucket2` as temporary rune/slot placeholders.
6. Test the complete flow: read scroll, pick up `FireSphere`, open chest, pick up `WoodSphere`, right-click both buckets, door opens, exit loads `TrailComplete`.
7. Replace the placeholder ball/bucket visuals with final rune/stone-slot art after the full flow works.
8. Add glow/outline/audio polish only after the full flow works.

### 6.15 Acceptance checklist
Level 0 is ready for Alpha/Beta testing when:

- A player who knows only WASD + mouse can discover every remaining control from prompts.
- The level can be completed in 60–90 seconds by a new player.
- The player cannot permanently lose either rune.
- The player cannot get stuck inside the chest, door, or room props.
- The door opens only after both correct runes are placed.
- Walking through the opened door fades to the Trial Complete hub (`TrailComplete` scene), not directly to Level 1.
- The Level Select lists this scene as **Level 0 — Tutorial**.
- ESC pauses the level and can return to the main menu.
- The level works in a WebGL build, not only in the Unity editor.
- All reused assets/scripts are listed in the group asset/source list.

---

## 7. Level 1 — The Master's Seal

### 7.1 Designer's goals
- Introduce **crafting** — the first compound power move.
- Teach the pattern *collect → combine → deploy* that L3 will reuse.
- Let the player experience a **visible cause-and-effect chain** (runes ignite furnace; furnace produces seal; seal opens door). This is the "why alchemy?" moment.

### 7.2 Master's voice — wall scroll
> *"Power begins in fire.*
> *Three runes wake the forge; wood and iron become the seal.*
> *Stamp your mark upon the door, and pass."*

### 7.3 Space
Indoor workshop. Single large room: corners hold glowing rune fragments; centered workbench has a 3-slot pedestal; back wall has the unlit furnace; side shelves hold wood and iron; exit door has a circular seal recess.

### 7.4 Points of Interest

| POI | Object | Player learns |
|---|---|---|
| 1 | Spawn (room entry) | Faces the wall scroll — reading happens first |
| 2 | Wall scroll | Delivers hint; reinforces L0's read-to-progress pattern |
| 3–5 | Three rune fragments (corners) | Rewards visual scanning; glowing = collectible |
| 6 | Stone pedestal (3 slots) | Place-into-slot *scales up* (3 at once, not 2) |
| 7 | Furnace | Ignites only when pedestal is complete — visible chain reaction |
| 8 | Wood log (shelf) | Reinforces pick-up with a different object shape |
| 9 | Iron ingot (shelf) | Teaches combining multiple items into one station |
| 10 | Crafting diagram (wall) | Visual-only recipe; no reading required |
| 11 | Seal stamp (spawns from furnace) | New artifact type — the power move |
| 12 | Seal door recess | Final slot; terminating the level |

### 7.5 Moment-to-moment flow
1. Enter workshop → read wall scroll.
2. Scan room → spot 3 glowing rune fragments → collect them.
3. Place each on pedestal → pedestal pulses → **furnace visibly ignites** (ambient fire audio, warm glow).
4. Pick up wood and iron → place both in furnace.
5. 2-second fire animation → seal stamp materializes on furnace output slab.
6. Pick up seal stamp → walk to door → right-click recess → door opens.

### 7.6 Design iterations
- **V0 (Max's original):** 3 rune fragments unlocked a shelf containing wood + iron. Diegetically weak ("why does a rune unlock a shelf?").
- **V1 (current):** 3 rune fragments *power the furnace*. Same player actions, but the causality is natural — the runes are a magical ignition source, not an arbitrary key.
- **V2 (post-Alpha playtest):** TBD — may adjust rune placements if players miss one.

### 7.7 Known flaws & planned fixes
| Flaw | Severity | Planned fix |
|---|---|---|
| Crafting still feels arbitrary without a visible chain | High | **Before:** runes silently enable furnace. **After:** light beam / particle trail visibly travels from pedestal to furnace on completion, then furnace ignites with audio punch |
| Player may try to put wood/iron in furnace before runes are placed | Medium | **Before:** items silently do nothing. **After:** item visibly bounces out + "The forge is cold" prompt |
| Wood/iron blend into the shelf | Medium | Emissive outline on hover |
| 3 rune fragments may be missed in corners | Low | Slight pulse animation draws eye; scroll hint says "three" explicitly |

### 7.8 Theme expression
- **Set in Stone:** rune fragments, engraved wall diagram, stone pedestal, cast-stone seal stamp.
- **Power Move:** the seal stamp is the literal power move — the apprentice presses the master's mark and inherits passage.

### 7.9 Time target
3–4 minutes.

### 7.10 Meaningful choice
Low (linear crafting) — but player can collect runes in any order, read diagram before or after collecting. Different player personas, same outcome.

---

## 8. Level 2 — Flame of Memory

### 8.1 Designer's goals
- Introduce **ordered sequence puzzles** — the first trial that punishes haste.
- Force the player to **slow down, read environmental hints carefully, and execute under reset threat**.
- Establish outdoor space as a hub (foreshadows L3 and L4 which expand it).

### 8.2 Master's voice — central stone tablet
> *"Light the path home, warm the place of rest, guard the long hall —*
> *in that order, and none before."*

### 8.3 Space
Outdoor courtyard hub. Cabin A and Cabin B are open and enterable. Castle ground floor is open (corridor accessible). Cabins C, D and castle upper floor are locked with visible doors (foreshadowing). Main gate at the far end is sealed.

### 8.4 Points of Interest

| POI | Object | Player learns |
|---|---|---|
| 1 | Spawn (courtyard entry) | Faces central stone tablet |
| 2 | Central stone tablet | Delivers the ordered hint — player must memorize |
| 3 | Cabin A interior: flint on table | Required-item pick-up (first tool the game locks behind progression) |
| 4 | Torch 1 — Cabin A doorway ("path home") | First correct sequence step |
| 5 | Torch 2 — inside Cabin B ("place of rest") | Second correct sequence step |
| 6 | Torch 3 — castle corridor ("long hall") | Third correct sequence step |
| 7 | Locked Cabin C / D / upper floor doors | Visible-but-locked = environmental foreshadowing |
| 8 | Main gate | Opens on correct sequence completion |

### 8.5 Moment-to-moment flow
1. Spawn → read central tablet → three phrases, one order.
2. Enter Cabin A → find and pick up flint.
3. Exit to Cabin A doorway → use flint on Torch 1.
4. Walk to Cabin B → light Torch 2.
5. Enter castle → light Torch 3.
6. All three lit → main gate grinds open.

### 8.6 Design iterations
- **V0 (Max's original):** 3 torches lit in order, reset on wrong. Core concept already strong.
- **V1 (current):** added the *environmental language* layer ("path home" = Cabin A, "place of rest" = Cabin B's bed, "long hall" = castle corridor) so the hint becomes *spatially interpretable*, not rote memorization. Player must think, not just remember.
- **V2 (post-Alpha playtest):** TBD — may tune the reset VFX if players find it too punishing.

### 8.7 Known flaws & planned fixes
| Flaw | Severity | Planned fix |
|---|---|---|
| Reset punishes trial-and-error players harshly | High | **Before:** wrong order snuffs all torches silently. **After:** clear red-flash + low bell + short on-screen prompt "*Wrong order — begin again*" so the player knows it's reset, not a bug |
| "Path home / place of rest / long hall" may be ambiguous in English | Medium | Tablet text calibrated; if playtest shows confusion, add secondary environmental cue (e.g. a dim candle already lit at Cabin A indicating "home") |
| Player may miss the flint in Cabin A | Medium | Flint on a lit table with subtle glow; tablet hint implies "spark lies where the apprentice sleeps" |
| Player could light torches in correct order by luck without reading | Low | Acceptable — they still experienced the mechanic |

### 8.8 Theme expression
- **Set in Stone:** the master's order is inscribed — immutable. Wrong order resets because the stone does not bend.
- **Power Move:** each flame application is a decisive act; the *order* is the power move, not the lighting itself.

### 8.9 Time target
3–5 minutes (depends on how carefully player reads).

### 8.10 Meaningful choice
Player chooses reading depth — read carefully once and solve, or skim and reset twice. Different personas, same content.

---

## 9. Level 3 — The Master's Lantern

### 9.1 Designer's goals
- Introduce **multi-zone material gathering** — the player must leave and re-enter multiple spaces.
- Introduce **held-item environmental trigger** — the crafted item is not just a key, it's a *condition of access* (carry it, and the world changes).
- Reinforce crafting (from L1) with a spatial twist.

### 9.2 Master's voice — castle desk note
> *"Three parts make the lamp.*
> *Oil from the hearth where fire was first tamed.*
> *Wick from the place of rest.*
> *Base from the cold room I have not yet seen.*
> *Only the lamp reveals the spire stair."*

### 9.3 Space
Expanded from L2. Castle interior is now fully traversable on the ground floor. Cabin C opens at level start (lamp base inside). Cabin D remains locked (foreshadowing L4). The castle stairwell to the upper floor is visibly dark — impassable without lantern. The courtyard altar is now inscribed with a slab-shaped recess.

### 9.4 Points of Interest

| POI | Object | Player learns |
|---|---|---|
| 1 | Castle ground floor spawn / desk | Faces the master's note — reading first |
| 2 | Desk note | Delivers 3-part hint with spatial cues |
| 3 | Cabin A — oil jar by hearth | "Where fire was first tamed" = L2's first torch location |
| 4 | Cabin B — wick on bedside | "Place of rest" = L2's second torch location (spatial callback) |
| 5 | Cabin C — lamp base on shelf | "Cold room I have not yet seen" = newly opened space |
| 6 | Castle furnace (crafting station) | L1 callback — same crafting loop, different output |
| 7 | Lantern (spawns from furnace) | New artifact: *light source as key* |
| 8 | Dark stairwell | Impassable without lantern → illuminates when player approaches carrying it |
| 9 | Upper floor: stone slab | The goal artifact of the level |
| 10 | Courtyard altar | Receives slab; completes the level and fades back to Main Menu |

### 9.5 Moment-to-moment flow
1. Read desk note.
2. Visit all three cabins (any order) → collect oil, wick, lamp base.
3. Return to castle furnace → combine all three → lantern appears.
4. Carry lantern toward dark stairwell → sconces ignite sequentially as player approaches → upper floor revealed.
5. Climb stairs → pick up stone slab.
6. Descend → carry slab to courtyard altar → right-click → slab seats → master's success line appears → fade back to Main Menu.

### 9.6 Design iterations
- **V0 (Max's original):** 3 materials → lantern → dark stairs illuminate → slab → altar.
- **V1 (current):** tied the cabin locations to L2's memorable landmarks ("the hearth where fire was first tamed" = L2 Torch 1 cabin). Player's memory of L2 becomes a *navigation tool* in L3 — making the game feel continuous, not disjointed.
- **V2 (post-Alpha playtest):** TBD — may tune stairwell lighting if the reveal underwhelms.

### 9.7 Known flaws & planned fixes
| Flaw | Severity | Planned fix |
|---|---|---|
| Player forgets which cabin is which from L2 | High | Desk note uses *spatial callbacks*, not cabin labels — player who remembers L2 has an edge, but any player can explore all 3 cabins |
| Stairwell reveal may feel flat if lighting is weak | High | **Before:** stairwell fades in subtly. **After:** strong sequential sconce ignition with warm audio crescendo as player climbs — moment of wonder |
| Player may not realize lantern is the trigger | Medium | Stairwell entry prompt: "*It is too dark. Something must illuminate this path.*" Disappears when player holds lantern |
| Altar placement ambiguous (courtyard is large) | Medium | Altar has a bright stone glow + prompt when slab is held near |
| Level is long (5 min) — could lose pacing | Medium | Consider shortening gather phase to 2 materials if playtest shows drag |

### 9.8 Theme expression
- **Set in Stone:** stone slab, stone altar, engraved master's note, carved stairwell inscriptions.
- **Power Move:** the lantern is literally a *power* — light — that the player *moves* into darkness to reveal a hidden path. Most literal instance of both themes combined.

### 9.9 Time target
5 minutes.

### 9.10 Meaningful choice
Player chooses cabin order (distinct from L2's strict order). Relief after L2's tight timing. Different from L4 which removes hints entirely.

---

## 10. Level 4 — The Final Legacy

### 10.1 Designer's goals
- **Capstone.** No scroll. Only one stone tablet. The apprentice is no longer being taught — they are being **tested**.
- Combine every prior verb: observation, crafting (via L3's lantern recurrence), navigation, symbol matching.
- Offer the **highest meaningful choice** in the game — player picks order, path, strategy.

### 10.2 Master's voice — central stone tablet (the only hint in the level)
> *"Three relics, each to its mark.*
> *The legacy is set in stone."*

### 10.3 Space
Full estate open: courtyard, Cabins A/B/C/D, castle ground and upper floors. Three stone pedestals arranged in the courtyard center, each bearing a distinct symbol: **○ Circle**, **▢ Square**, **△ Triangle**. A sealed final stone door at the estate's back wall.

### 10.4 Points of Interest

| POI | Object | Player learns |
|---|---|---|
| 1 | Spawn (courtyard center) | Faces the tablet and the 3 pedestals immediately |
| 2 | Central stone tablet | Single minimal hint — player must reason from here |
| 3 | Circle pedestal | Target A |
| 4 | Square pedestal | Target B |
| 5 | Triangle pedestal | Target C |
| 6 | Bronze orb (○) — Cabin C shelf | Relic 1 |
| 7 | Square stone (▢) — castle upper floor | Relic 2 — requires lantern (L3 callback) |
| 8 | Triangle crystal (△) — Cabin D | Relic 3 — in newly accessible cabin |
| 9 | Final stone door (back wall) | Opens when all three pedestals are filled correctly |
| 10 | Ending sequence trigger (past the door) | Master's final voice line + short credits beat + fade back to Main Menu |

### 10.5 Moment-to-moment flow
1. Spawn in courtyard → read tablet → inspect three pedestals, note their carvings.
2. Explore estate freely. Find 3 relics in any order.
3. Access upper floor by re-fetching or re-crafting the lantern (L3 callback).
4. Return to each pedestal → right-click to place matching relic.
5. Each correct placement → stone-grind audio + `AddStone()` trigger + pedestal glows.
6. All 3 placed → final stone door opens → ending sequence → fade back to Main Menu.

### 10.6 Design iterations
- **V0 (Max's original):** 3 relics on 3 pedestals, shape-match, `StoneDoor` opens.
- **V1 (current):** explicit *reuse of L3 lantern skill* for the upper-floor relic — integrates the level into the game's continuity. Also added the narrative ending-sequence beat so the game doesn't just end on a door animation.
- **V2 (post-Alpha playtest):** TBD — likely to adjust hint strength for lost players.

### 10.7 Known flaws & planned fixes
| Flaw | Severity | Planned fix |
|---|---|---|
| Player lost with no guidance (especially playtesters from other labs per A4 brief) | **Critical** | **Before:** only the tablet. **After:** pedestal carvings *large and visible from across the courtyard*; each relic emits a subtle pulsing glow matching its pedestal symbol; if player idle > 3 min, a faint audio cue pulls toward the nearest un-found relic |
| Player forgets the lantern mechanic from L3 | High | Fresh lantern recipe available in the castle (with all 3 materials already visible) — the lesson is reinforced, not re-taught |
| Placing wrong relic on wrong pedestal is silent failure | Medium | **Before:** relic stays in hand. **After:** relic visibly bounces off + low chime + subtle "not your mark" prompt |
| Final door opens into void without closure | High | **Before:** scene transitions to TrailComplete or returns to menu too abruptly. **After:** short ending sequence with master's final line, brief credits beat, then fade back to Main Menu — earns the "Premise / Narrative" grade marks |
| Playtime variance (skilled vs confused players) | Medium | Tune glow-strength on relics; goal is 3–5 min median, 7 min ceiling |

### 10.8 Theme expression
- **Set in Stone:** the pedestals are literally stone. The tablet states it outright. The ending *places the three stones permanently* — the legacy is now set in stone, by the apprentice's hand.
- **Power Move:** the final synthesis — three correct placements that claim the inheritance. This is the game's thesis statement.

### 10.9 Time target
4–5 minutes.

### 10.10 Meaningful choice
**Highest in the game.** Player chooses:
- Which relic to fetch first
- Which zone to clear first
- Whether to re-craft lantern immediately or postpone the upper-floor relic
- How carefully to read environmental cues vs brute-force try placements

---

## 11. Supporting Systems

### 11.1 Main Menu
- Title card: "The Alchemist's Legacy"
- Buttons: **Start New Game** | **Level Select** | **Quit**
- Level Select shows: "Level 0 — Tutorial", "Level 1 — The Master's Seal", ..., "Level 4 — The Final Legacy"
- **Start New Game** loads Level 0.
- **Level Select** lets playtesters launch any level directly.
- After any level is completed, the game fades back to this Main Menu instead of automatically loading the next level.

### 11.2 Pause Menu
- ESC to open, cursor unlocks.
- **Resume** | **Return to Main Menu**

### 11.3 HUD
- Crosshair
- Interact prompt (contextual)
- Held-item name (bottom-right)

### 11.4 Audio layers
- Ambient music: soft, mystical, per-level variation
- Correct action: low warm bell
- Wrong action: muffled thud / soft discord
- Stone grind: doors opening, pedestals accepting relics
- Fire crackle: torches, furnace

---

## 12. Narrative Through-Line

Each level ends with a one-sentence fragment of the master's will, revealed after completion:

| Level | Master's closing line |
|---|---|
| L0 | *"Good. You remember how to see."* |
| L1 | *"Good. You remember how to make."* |
| L2 | *"Good. You remember how to wait."* |
| L3 | *"Good. You remember how to reveal."* |
| L4 | *"Now you remember who you are. The legacy is yours."* |

Four verbs across four trials: **see → make → wait → reveal → (become).**

---

## 13. Open Design Questions

1. **Existing scepter + footsteps puzzle** (`GateSequenceManager.cs` in old castle): cut, keep as optional environmental storytelling in L3, or swap into L4? *(Recommendation: keep as optional flavor in L3 — adds depth without duplicating L2's sequence mechanic.)*
2. **L0 rune count** — 2 or 3? *(Recommendation: 2, for tutorial brevity.)*
3. **Lantern use in L4** — because levels return to Main Menu after completion, should L4 include a fresh lantern setup or skip the lantern requirement? *(Recommendation: include a fresh, simplified lantern setup if the upper-floor relic requires it.)*
4. **Ending sequence scope** — text fade or pre-rendered cinematic? *(Recommendation for Alpha: text fade with voice line.)*
5. **Level ownership** — to be confirmed with team (see §14).

---

## 14. Level Ownership (proposal — confirm with team)

| Level | Owner | Rationale |
|---|---|---|
| L0 Tutorial | Whole group / shared | Brief treats tutorial as group responsibility |
| L1 Workshop | Student A | Crafting mechanic is foundational |
| L2 Courtyard | Student B | Sequence puzzle — mechanically clean |
| L3 Castle | Student C | Largest level — needs confident developer |
| L4 Full Estate | Student D | Capstone — needs best designer |
| — | Student E | **Meta layer:** main menu, level select, pause menu, ending sequence, audio pass, itch.io page, marketing video |

Rationale for Student E: connective tissue + marketing polish. Confirm with tutor.

---

## 15. Alpha Scope (Week 10 deliverable)

For A3 Alpha grading:
- ✅ **L0 fully playable**
- ✅ **L1 fully playable** (demonstrates the core crafting loop)
- ✅ **Main menu + Level Select + Pause menu** scaffolded
- ✅ **Paper designs for L2, L3, L4** (this document)
- ✅ **Asset source list started**
- ⬜ L2, L3, L4 playable — not required for Alpha

**Beta Week 11–12** = build L2, L3, L4 + polish + marketing material.

---

## 16. For the Individual Level Plan & Analysis Report

Each student extracts from their level's section above:
1. **Designer's goals** (§n.1)
2. **Moment-to-moment flow** (§n.5) — convert into a flow diagram
3. **Points of Interest table** (§n.4) — convert into a top-down map with numbered POIs
4. **Design iterations** (§n.6) — show V0 → V1 evolution honestly
5. **Known flaws & planned fixes** (§n.7) — add before/after screenshots once playtested
6. **Theme expression** (§n.8) — directly maps to assignment themes

---

## 17. Existing Asset & Script Reuse Map

**[DEFERRED — to be completed AFTER all level designs are locked]**

Once §§6–10 are signed off, we'll do an asset-mapping pass:
- For each level, list which existing scripts / prefabs / art assets can be reused unchanged
- List which existing scripts need adaptation (and scope of adaptation)
- List what must be built from scratch
- List what needs to be imported from the Unity Asset Store

This deliberately happens **after** design is locked so that existing code does not constrain the design. If an existing script does not serve the design, we rewrite.

Asset inventory is in `assetList.txt` and the `Assets/` directory. Scripts are in `Assets/Scripts/`.

---

## 18. Change Log

- **2026-04-22 v2** — restructured to match tutor's reference format (Ryan's doc). Each level now has explicit Designer's Goals, Points of Interest, Design Iterations, Known Flaws / Planned Fixes. Script/asset discussion removed from level bodies and consolidated into §17 stub (to be filled after design lock).
- **2026-04-22 v1** — initial design locked: themes Set in Stone + Power Move, 5-level arc, narrative spine, alpha scope.
