# The Alchemist's Legacy

The Alchemist's Legacy is a first-person puzzle adventure built in Unity. The player is an apprentice completing a sequence of trials left by their master. Each level teaches or remixes a simple alchemical idea: observe, make, wait, reveal, and finally restore the legacy.

The game focuses on readable environmental puzzles. Players explore, pick up one item at a time, read short notes, use items with right click, and follow HUD prompts toward the next step.

## Unity Version

Use Unity `2021.3.42f1`.

## How To Play

1. Open the project in Unity Hub.
2. Open a scene from `Assets/_TheAlchemistsLegacy/Scenes`.
3. Press Play.
4. Test each level from start to finish.

## Controls

| Action | Input |
|---|---|
| Move | WASD |
| Look | Mouse |
| Sprint | Shift |
| Pick up or drop item | E |
| Use, place, open, or read | Right Mouse Button |
| Recall last available item | R |
| Close note panel | Right Mouse Button, E, or Escape |

## Game Flow

### Level 0 - Seeing

The apprentice wakes in a workshop trial. The player reads the master's note, finds two marked objects, places them in the matching spots, clears the exit, and leaves.

This level introduces the core language of the game: read, observe, pick up, match, and exit.

### Level 1 - Making

The player places three stamps to wake the forge. Once the forge is active, they gather the required materials, create the final forged object, and sacrifice it at the old door to clear the path.

This level focuses on crafting through a clear multi-step sequence.

### Level 2 - Waiting

The player takes a torch and lights three torches in the correct order. The clue explains the order, and the exit opens when the final torch is lit.

This level focuses on patience, sequence, and carrying a tool through the space.

### Level 3 - Revealing

The player uses a special flame to reveal hidden progress, gathers parts for a lantern, crafts the lantern, and uses it to reach the exit key. Placing the key opens the final route.

This level focuses on revealing hidden objects and combining several earlier mechanics.

### Level 4 - The Final Legacy

The final trial asks the player to restore four offerings: Ash, Salt, Yellow Stone, and Blue Drop. Each offering is found through a familiar interaction from earlier levels. Once all four offerings are returned, the final exit opens and the player reaches the completion scene.

This level is the capstone. It should feel like the player is proving they understand the whole game, not learning a new system at the last minute.

## Current Status

- Five playable levels are included.
- The main interaction loop is consistent across the game.
- HUD prompts guide the player through objectives and right-click interactions.
- The player can hold one item at a time.
- Items can be picked up, dropped, and recalled when still available.
- The final level no longer uses a separate final seal step. Restoring the four offerings opens the final exit.

## Playtest Checklist

Before submitting or presenting the game, test every level from a fresh scene load:

- The player can finish the level without debug keys.
- Every required item can be picked up.
- Every required right-click interaction responds.
- HUD prompts appear for important objects.
- Dropping and recalling items does not break progression.
- The exit opens only after the level goal is complete.
- The level transitions to the completion scene correctly.

## Project Structure

| Path | Purpose |
|---|---|
| `Assets/_TheAlchemistsLegacy/Scenes` | Main game scenes |
| `Assets/_TheAlchemistsLegacy/Scripts` | Gameplay scripts |
| `Assets/_TheAlchemistsLegacy/Art` | Project-specific art staging |
| `Assets` | Imported assets and shared materials |

## Team Notes

- Keep gameplay scripts inside `Assets/_TheAlchemistsLegacy/Scripts`.
- Keep playable scenes inside `Assets/_TheAlchemistsLegacy/Scenes`.
- Avoid committing Unity-generated folders such as `Library`, `Temp`, `Logs`, and `UserSettings`.
- Coordinate before multiple people edit the same scene, because Unity scene files are difficult to merge.
