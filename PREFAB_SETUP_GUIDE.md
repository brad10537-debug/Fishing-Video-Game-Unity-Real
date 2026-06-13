# Wizard Pond Prefab Setup Guide

This guide turns the current prototype objects into reusable prefabs. None of these prefabs should depend on hardcoded scene object names. Use Inspector references when a prefab needs to talk to scene objects such as the HUD, Run Manager, or UI sliders.

## Shared Rules

- Create an `Interactable` layer.
- Put interactable prefabs on the `Interactable` layer.
- Keep player-owned systems on the player: inventory, gold, rods, stats, gear, XP.
- Keep scene-wide systems in the scene: HUD Canvas, Run Manager, Fishing Systems.
- Let scripts auto-find references for quick testing, but drag references manually when possible.
- Use simple primitive meshes and flat materials first.

## 1. PlayerPrototype

### GameObject Structure

- `PlayerPrototype`
  - `Player Camera`

### Required Components

On `PlayerPrototype`:

- `CharacterController`
- `FirstPersonPrototypeController`
- `PlayerInteractor`
- `PlayerAreaTracker`
- `CharacterIdentity`
- `LivingEntity`
- `SimpleInventory`
- `PlayerCurrency`
- `PlayerFishingRod`
- `PlayerProgression`
- `WizardStats`
- `WizardGear`

On `Player Camera`:

- `Camera`
- `AudioListener`

### Collider / Rigidbody

- Use `CharacterController`.
- Do not add Rigidbody to the player for this prototype.

### Tags / Layers

- Tag: `Player` recommended.
- Layer: `Default`.

### Inspector Values

`CharacterIdentity`:

- Display Name: `Young Pond Wizard`
- Character Type: `Player`
- Faction: `Player`
- Title Or Role: `Apprentice Angler`

`LivingEntity`:

- Max Health: `100`
- Max Stamina: `100`
- Movement Speed: `5`
- Sprint Speed: `8`
- Stamina Regen Rate: `15`

`FirstPersonPrototypeController`:

- Player Camera: drag `Player Camera`
- Sprint Key: `Left Shift`
- Jump Key: `Space`

`PlayerInteractor`:

- Player Camera: drag `Player Camera`
- Movement Controller: drag `PlayerPrototype`
- Interactable Layers: include `Interactable`
- Other player components can auto-fill, but dragging them is safer.

`PlayerAreaTracker`:

- Prototype Hud: drag scene `HUD Canvas` with `PrototypeHud`
- Fallback Area Name: `Village`

### Test

Press Play. Confirm WASD movement, mouse look, Shift sprint, Space jump, E interaction, stamina HUD, and prompts.

## 2. FishingSpotPrototype

### GameObject Structure

- `FishingSpotPrototype`
  - Optional visual marker: rock, sign, glowing ring, pond ripple

### Required Components

- `Collider`
- `FishingSpotInteractable`

### Collider / Rigidbody

- Use a Box Collider or Sphere Collider.
- Set `Is Trigger` either on or off; `PlayerInteractor` supports trigger hits.
- No Rigidbody needed.

### Tags / Layers

- Layer: `Interactable`.

### Inspector Values

`FishingSpotInteractable`:

- Spot Name: `Funky Wizard Pond`
- Fishing Time: `2`
- Keep default species/rarity settings for first test.

### UI References

None on the prefab. The fishing spot talks through `PlayerInteractor` and `FishingMinigameManager`.

### Test

Look at the fishing spot, press E, wait for the minigame, complete it, and confirm a fish appears in HUD/status.

## 3. ShopkeeperPrototype

### GameObject Structure

- `ShopkeeperPrototype`
  - Optional hat/robe primitives

### Required Components

- `Collider`
- `CharacterIdentity`
- `ShopKeeperInteractable`
- Optional `NPCInteractable`
- Optional `SimpleNPCIdleMotion`

### Collider / Rigidbody

- Capsule Collider recommended.
- No Rigidbody needed.

### Tags / Layers

- Layer: `Interactable`.

### Inspector Values

`CharacterIdentity`:

- Display Name: `Froggin the Bait Merchant`
- Character Type: `Shopkeeper`
- Faction: `Village`
- Title Or Role: `Bait Merchant`

`ShopKeeperInteractable`:

- Shopkeeper Name: `Froggin`

### UI References

None on the prefab. Shop UI goes through the player `PrototypeHud`.

### Test

Press E on the shopkeeper. Press `1` to sell fish. Press `2+` to buy/equip rods.

## 4. NPCPrototype

### GameObject Structure

- `NPCPrototype`
  - Optional hat/robe primitives

### Required Components

- `Collider`
- `CharacterIdentity`
- `NPCInteractable`
- Optional `SimpleNPCIdleMotion`

### Collider / Rigidbody

- Capsule Collider recommended.
- No Rigidbody needed.

### Tags / Layers

- Layer: `Interactable`.

### Inspector Values

Example:

- Display Name: `Shellbert the Turtle Sage`
- Character Type: `Trainer`
- Faction: `Village`
- Title Or Role: `Fishing Sage`
- Dialogue Line: `Slow casts catch strange fish.`
- Hangout Area: `MainPond`

### UI References

None.

### Test

Look at NPC, confirm name appears, press E, confirm dialogue/status appears.

## 5. Mob_PondGoblin

### GameObject Structure

- `Mob_PondGoblin`
  - Optional low-poly body/hat primitives
  - Optional `World Space Health Bar`

### Required Components

- `Collider`
- `Rigidbody`
- `CharacterIdentity`
- `LivingEntity`
- `MobController`
- `KnockbackReceiver`
- `KnockbackTestInteractable`

### Collider / Rigidbody

- Capsule Collider recommended.
- Rigidbody:
  - Use Gravity: on
  - Is Kinematic: off
  - Collision Detection: Discrete
  - Freeze Rotation X/Z if you want less tipping

### Tags / Layers

- Layer: `Interactable` if player should target/test it with E.

### Inspector Values

`CharacterIdentity`:

- Display Name: `Pond Goblin`
- Character Type: `Mob`
- Faction: `PondMobs`
- Title Or Role: `Wild Edge Pest`

`LivingEntity`:

- Max Health: `50`
- Health: `50`
- Movement Speed: `2`

`MobController`:

- Wander Radius: `3`
- Wander Speed: `1.2`

`KnockbackReceiver`:

- Default Knockback Force: `8`
- Upward Force: `2`
- Stun Duration: `1.25`

`KnockbackTestInteractable`:

- Display Name: `Pond Goblin`
- Damage Amount: `25`
- Receiver: drag `KnockbackReceiver`

### UI References

Optional floating health bar:

- Add child world-space Canvas.
- Add Text for name.
- Add Slider for health.
- Add `FloatingHealthBar`.
- Assign target `LivingEntity`, name Text, and health Slider.

### Test

Press Play. Look at mob and confirm target health HUD. Press E twice if health is 50 and damage is 25. Confirm defeat message.

## 6. MobSpawnAreaPrototype

### GameObject Structure

- `MobSpawnAreaPrototype`

### Required Components

- `MobSpawnArea`

### Collider / Rigidbody

- None required.

### Tags / Layers

- Layer: `Default`.

### Inspector Values

`MobSpawnArea`:

- Area Name: `Wild Edge`
- Mob Prefab: drag `Mob_PondGoblin`
- Spawn Count: `3`
- Spawn Radius: `6`
- Spawn On Start: on

### UI References

None.

### Test

Press Play. Confirm mobs spawn inside the gizmo radius and begin wandering.

## 7. SitPoint_Bench

### GameObject Structure

- `SitPoint_Bench`
  - `Sit Point`

### Required Components

- `Collider`
- `SitPointInteractable`

### Collider / Rigidbody

- Box Collider recommended.
- No Rigidbody needed.

### Tags / Layers

- Layer: `Interactable`.

### Inspector Values

`SitPointInteractable`:

- Display Name: `Bench`
- Sit Point: drag child `Sit Point`
- Sitting Message: `Sitting`

### UI References

None.

### Test

Look at bench, press E, confirm movement pauses and HUD/status says sitting. Press E or move to stand.

## 8. BossFishSpotPrototype

### GameObject Structure

- `BossFishSpotPrototype`
  - Optional magical pond marker / arch / crystals

### Required Components

- `Collider`
- `BossFishEncounter`

### Collider / Rigidbody

- Box Collider or Sphere Collider.
- No Rigidbody needed.

### Tags / Layers

- Layer: `Interactable`.

### Inspector Values

`BossFishEncounter`:

- Keep default boss pool for first test.
- Max Tension: `100`
- Max Capture: `100`
- Tension Slider: drag boss tension UI Slider
- Capture Slider: drag boss capture UI Slider

### UI References

Requires boss tension/capture sliders if you want meters visible.

### Test

Start a run, complete required rounds, then interact with the boss spot. Confirm boss prompts and meters update.

## 9. VillageDecoration_Crystal

### GameObject Structure

- `VillageDecoration_Crystal`

### Required Components

- Mesh primitives: cone/cube/sphere
- `VillageDecorationMotion`

### Collider / Rigidbody

- Collider optional.
- No Rigidbody.

### Tags / Layers

- Layer: `Default`.

### Inspector Values

`VillageDecorationMotion`:

- Spin: on
- Spin Speed: `30`
- Bob: on
- Bob Height: `0.15`

### UI References

None.

### Test

Press Play. Crystal should spin/bob.

## 10. VillageDecoration_Mushroom

### GameObject Structure

- `VillageDecoration_Mushroom`
  - Stem cylinder
  - Cap sphere/capsule

### Required Components

- Optional `VillageDecorationMotion`

### Collider / Rigidbody

- Collider optional.
- No Rigidbody.

### Tags / Layers

- Layer: `Default`.

### Inspector Values

Use simple bright materials:

- Stem: pale cream/gray
- Cap: purple, red, blue, or green

### UI References

None.

### Test

Place several around pond/wild edge. Confirm they read clearly from player distance.

## 11. VillageDecoration_Lantern

### GameObject Structure

- `VillageDecoration_Lantern`
  - Post cylinder
  - Lantern cube/sphere
  - Optional Point Light

### Required Components

- Optional `VillageDecorationMotion`
- Optional `Light`

### Collider / Rigidbody

- Collider optional.
- No Rigidbody.

### Tags / Layers

- Layer: `Default`.

### Inspector Values

`Light`:

- Type: Point
- Range: `4`
- Intensity: `1`
- Color: warm yellow/gold

### UI References

None.

### Test

Place near dock/shop paths. Confirm it gives readable warm accents without washing out the scene.

## Visual Building Prefabs

These prefabs are mostly visual placeholders. They make the village feel like a real place while keeping the project beginner-friendly. Use primitive shapes, flat colors, and optional helper scripts.

## 12. WizardHut

### GameObject Structure

- `WizardHut`
  - `Body` cube, slightly scaled and rotated
  - `Roof` cone or pyramid-like primitive
  - `Door` thin cube
  - Optional `WindowGlow` cube/sphere

### Required Components

- Mesh primitives only.
- Optional `MaterialPulse` on `WindowGlow`.

### Collider / Rigidbody

- Box Collider on the main hut body if the player should bump into it.
- No Rigidbody.

### Tags / Layers

- Layer: `Default`.

### Inspector Values

- Use warm brown walls, purple/blue roof, and yellow/cyan window glow.
- If using `MaterialPulse`, set base color to warm yellow and pulse color to cyan or violet.

### Test

Walk around it in Play Mode and confirm it blocks the player without trapping them.

## 13. DockSection

### GameObject Structure

- `DockSection`
  - `Plank` cubes
  - `Post` cylinders

### Required Components

- Mesh primitives.

### Collider / Rigidbody

- Box Collider on the main walking surface.
- No Rigidbody.

### Tags / Layers

- Layer: `Default`.

### Inspector Values

- Use warm brown wood.
- Keep sections small so you can snap several together.

### Test

Place multiple sections, press Play, and confirm the player can walk across them without snagging.

## 14. FishingSign

### GameObject Structure

- `FishingSign`
  - `Post` cylinder
  - `SignBoard` cube
  - Optional child world-space text label

### Required Components

- Optional `BillboardLabel` on the text label so it faces the player camera.

### Collider / Rigidbody

- Collider optional.
- No Rigidbody.

### Tags / Layers

- Layer: `Default`, or `Interactable` only if you later add a sign-reading interactable.

### Inspector Values

- Label ideas: `Training Dock`, `Rare Fish Ahead`, `Boss Swamp`.

### Test

Walk around the sign and confirm any label remains readable.

## 15. LilyPad

### GameObject Structure

- `LilyPad`
  - flattened cylinder or flattened sphere

### Required Components

- Mesh primitive.
- Optional `VillageDecorationMotion` with slow bob.

### Collider / Rigidbody

- Collider optional.
- No Rigidbody.

### Tags / Layers

- Layer: `Default`.

### Inspector Values

- Use soft green.
- Bob Height: `0.03`
- Bob Speed: `0.6`

### Test

Place on the pond and confirm it adds detail without blocking fishing.

## 16. Turtle

### GameObject Structure

- `Turtle`
  - shell flattened sphere
  - body/head small spheres
  - feet small cubes/spheres

### Required Components

- Mesh primitives.
- Optional `SimpleNPCIdleMotion` if you want tiny idle movement.

### Collider / Rigidbody

- Collider optional.
- No Rigidbody.

### Tags / Layers

- Layer: `Default`, or `Interactable` if you later add turtle dialogue.

### Inspector Values

- Shell: teal/brown.
- Body: green.

### Test

Place near rocks and confirm it reads as a turtle from player distance.

## 17. Frog

### GameObject Structure

- `Frog`
  - body sphere
  - eyes small spheres
  - feet small flattened spheres/cubes

### Required Components

- Mesh primitives.
- Optional `VillageDecorationMotion` with gentle bob.

### Collider / Rigidbody

- Collider optional.
- No Rigidbody.

### Tags / Layers

- Layer: `Default`.

### Inspector Values

- Body: bright green or teal.
- Eyes: pale yellow/white.

### Test

Place on lily pads or pond rocks and confirm it creates a cozy pond identity.

## 18. FishBucket

### GameObject Structure

- `FishBucket`
  - bucket cylinder
  - handle torus or curved-looking cube/cylinder pieces
  - optional fish shapes inside

### Required Components

- Mesh primitives only.

### Collider / Rigidbody

- Collider optional.
- No Rigidbody.

### Tags / Layers

- Layer: `Default`.

### Inspector Values

- Bucket: dark wood or metal gray.
- Fish accents: cyan, purple, green.

### Test

Place near fishing spots and shop. Confirm it helps communicate fishing without needing UI text.

## 19. ShopCounter

### GameObject Structure

- `ShopCounter`
  - counter cube
  - display top cube
  - small price signs

### Required Components

- Mesh primitives.

### Collider / Rigidbody

- Box Collider recommended so the player cannot walk through it.
- No Rigidbody.

### Tags / Layers

- Layer: `Default`.

### Inspector Values

- Wood brown counter.
- Teal/purple cloth strip.
- Optional warm lantern nearby.

### Test

Place `ShopkeeperPrototype` behind it and confirm the player can interact from the front.

## 20. RodDisplay

### GameObject Structure

- `RodDisplay`
  - rack cube/cylinders
  - 3 to 5 thin rod cylinders
  - glowing rod tips

### Required Components

- Optional `MaterialPulse` on rod tips.

### Collider / Rigidbody

- Collider optional.
- No Rigidbody.

### Tags / Layers

- Layer: `Default`.

### Inspector Values

- Make rods different colors for upgrade identity:
  - Twig Wand Rod: brown
  - Copper Moon Rod: copper/orange
  - Crystal Tide Rod: cyan
  - Neon Wizard Rod: magenta/cyan
  - Ancient Star Rod: gold/violet

### Test

Place behind shop counter and confirm each rod color is readable.

## 21. PotionBottle

### GameObject Structure

- `PotionBottle`
  - bottle cylinder
  - neck small cylinder
  - cork small cube

### Required Components

- Optional `MaterialPulse` on bottle liquid.

### Collider / Rigidbody

- Collider optional.
- No Rigidbody.

### Tags / Layers

- Layer: `Default`.

### Inspector Values

- Use bright potion colors: pink, cyan, green, gold.

### Test

Duplicate several bottles on the shop counter and confirm they do not distract from the interact prompt.

## 22. BossArenaMarker

### GameObject Structure

- `BossArenaMarker`
  - circular marker made from torus/cylinders/cubes
  - rune stones
  - optional large dark fish silhouette marker

### Required Components

- Optional `MaterialPulse` on rune stones.
- Optional `VillageDecorationMotion` on floating crystals.

### Collider / Rigidbody

- Collider optional unless using it as an interactable gate.
- No Rigidbody.

### Tags / Layers

- Layer: `Default`, or `Interactable` if combined with a boss gate interactable.

### Inspector Values

- Base color: dark blue/black-green.
- Glow colors: violet, cyan, cold blue.

### Test

Place in the Moonlit Boss Swamp and confirm it clearly reads as the boss area entrance or center.

## 23. GoblinPlaceholder

### GameObject Structure

- `GoblinPlaceholder`
  - capsule body
  - small cube/sphere head
  - hat/nose/ear primitives

### Required Components

- Use `Mob_PondGoblin` scripts if it should be a working mob.
- For a visual-only goblin prop, use mesh primitives only.

### Collider / Rigidbody

- Working mob: same Rigidbody and Collider settings as `Mob_PondGoblin`.
- Visual-only prop: collider optional, no Rigidbody.

### Tags / Layers

- Working mob: `Interactable` if E should target it.
- Visual-only prop: `Default`.

### Inspector Values

- Use swamp green body, purple hat, cyan eyes, or orange nose.

### Test

For a working mob, confirm it spawns from `MobSpawnAreaPrototype`, wanders, takes damage, and can be defeated.

## Play Mode Prefab Test Checklist

1. Place one prefab of each type in the scene.
2. Confirm there are no missing script warnings in the Inspector.
3. Press Play.
4. Move, sprint, and jump with `PlayerPrototype`.
5. Interact with `NPCPrototype`.
6. Fish at `FishingSpotPrototype`.
7. Sell fish and buy/equip a rod at `ShopkeeperPrototype`.
8. Sit on `SitPoint_Bench`.
9. Spawn mobs from `MobSpawnAreaPrototype`.
10. Knock back and defeat `Mob_PondGoblin`.
11. Complete or abandon a run.
12. Start boss encounter from `BossFishSpotPrototype`.

## Known Limitations

- Prefabs still depend on scene-level systems like `HUD Canvas`, `Run Manager`, and `Fishing Systems`.
- Shop UI is text/key based.
- Combat is only a knockback/damage test.
- Boss encounter is text/slider based.
- Wardrobe and decoration prefabs are visual/placeholder only.
