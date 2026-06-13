# Wizard Pond Unity Scene Setup Guide

This is the exact manual scene setup for the current Wizard Pond prototype. Follow this before making prefabs or adding new gameplay.

## Setup Order

1. Create layers.
2. Create scene-level systems.
3. Create the player.
4. Create HUD UI.
5. Create interaction objects.
6. Create mob test area.
7. Create boss/run objects.
8. Test in Play Mode.

## Required Layer

Create one layer:

- `Interactable`

Assign `Interactable` to:

- NPCs
- Shopkeeper
- Fishing run portal
- Fishing spot
- Boss fish spot
- Sit points
- Mobs/test dummies that should respond to E

On the player, make sure `PlayerInteractor > Interactable Layers` includes `Interactable`.

## 1. Player

### GameObject Hierarchy

Create:

- `Player`
  - `Player Camera`

### Player Components

On `Player`, add:

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

Do not add a Rigidbody to the player. The player uses `CharacterController`.

### Player Camera Components

On `Player Camera`, add:

- `Camera`
- `AudioListener`

### Player Inspector Setup

`CharacterIdentity`:

- Display Name: `Young Pond Wizard`
- Character Type: `Player`
- Faction: `Player`
- Title Or Role: `Apprentice Angler`

`LivingEntity`:

- Max Health: `100`
- Health: `100`
- Max Stamina: `100`
- Stamina: `100`
- Stamina Regen Rate: `15`
- Movement Speed: `5`
- Sprint Speed: `8`
- Sprint Stamina Cost Per Second: `20`
- Level: `1`
- Faction: `Player`

`FirstPersonPrototypeController`:

- Player Camera: drag `Player Camera`
- Move Speed: `5`
- Sprint Key: `Left Shift`
- Jump Key: `Space`
- Jump Height: `1.2`
- Gravity: `-20`
- Mouse Sensitivity: `2`

`PlayerInteractor`:

- Player Camera: drag `Player Camera`
- Inventory: drag `Player`
- Currency: drag `Player`
- Fishing Rod: drag `Player`
- Progression: drag `Player`
- Wizard Stats: drag `Player`
- Wizard Gear: drag `Player`
- Prototype Hud: drag `HUD Canvas`
- Movement Controller: drag `Player`
- Interact Distance: `4`
- Interact Key: `E`
- Interactable Layers: include `Interactable`

`PlayerAreaTracker`:

- Prototype Hud: drag `HUD Canvas`
- Fallback Area Name: `Village`

## 2. HUD / UI

### GameObject Hierarchy

Create:

- `HUD Canvas`
  - `Prompt Text`
  - `Status Text`
  - `Look Target Text`
  - `Target Health Text`
  - `Target Health Slider`
  - `Current Area Text`
  - `Movement State Text`
  - `Player Health Text`
  - `Player Health Slider`
  - `Player Stamina Text`
  - `Player Stamina Slider`
  - `Gold Text`
  - `Level Text`
  - `XP Text`
  - `Equipped Rod Text`
  - `Fish Count Text`
  - `Last Catch Text`
  - `Inventory Text`
  - `Shop Menu Text`
  - `Run State Text`
  - `Run Timer Text`
  - `Run Round Text`
  - `Run Fish Count Text`
  - `Run Value Text`
  - `Boss Status Text`
  - `Minigame Title Text`
  - `Minigame Instructions Text`
  - `Minigame Prompt Text`
  - `Minigame Progress Text`
  - `Minigame Progress Slider`
  - `Minigame Result Text`
  - `Wizard Stats Text`
  - `Gear Text`

Use Unity UI `Text` and `Slider` components for now. The project currently uses `UnityEngine.UI.Text`, not TextMeshPro.

### HUD Canvas Components

On `HUD Canvas`, add:

- `Canvas`
- `CanvasScaler`
- `GraphicRaycaster`
- `PrototypeHud`

### PrototypeHud Inspector References

Drag each UI object into its matching field:

- Prompt Text -> `promptText`
- Status Text -> `statusText`
- Inventory Text -> `inventoryText`
- Gold Text -> `goldText`
- Equipped Rod Text -> `equippedRodText`
- Last Catch Text -> `lastCatchText`
- Fish Count Text -> `fishCountText`
- Shop Menu Text -> `shopMenuText`
- Run State Text -> `runStateText`
- Run Timer Text -> `runTimerText`
- Run Round Text -> `runRoundText`
- Run Fish Count Text -> `runFishCountText`
- Run Value Text -> `runValueText`
- Level Text -> `playerLevelText`
- XP Text -> `xpText`
- Boss Status Text -> `bossStatusText`
- Minigame Title Text -> `minigameTitleText`
- Minigame Instructions Text -> `minigameInstructionsText`
- Minigame Prompt Text -> `minigamePromptText`
- Minigame Progress Text -> `minigameProgressText`
- Minigame Result Text -> `minigameResultText`
- Wizard Stats Text -> `wizardStatsText`
- Gear Text -> `gearText`
- Look Target Text -> `lookTargetText`
- Target Health Text -> `targetHealthText`
- Current Area Text -> `currentAreaText`
- Player Health Text -> `playerHealthText`
- Player Stamina Text -> `playerStaminaText`
- Movement State Text -> `movementStateText`
- Target Health Slider -> `targetHealthSlider`
- Player Health Slider -> `playerHealthSlider`
- Player Stamina Slider -> `playerStaminaSlider`
- Minigame Progress Slider -> `minigameProgressSlider`

Drag `Player` into:

- Inventory
- Currency
- Fishing Rod
- Progression
- Wizard Stats
- Wizard Gear
- Player Entity

The HUD can auto-find these references, but dragging them makes testing clearer.

## 3. Scene Systems

### Prototype Scene Validator

Create:

- `Prototype Scene Validator`

Add:

- `PrototypeSceneValidator`

Inspector:

- Run On Start: on
- Check Optional Objects: on

Purpose:

- Press Play and read the Console.
- The validator reports missing player, HUD, run, fishing, shop, NPC, mob, boss, and area setup.
- It does not control gameplay. It only helps catch setup mistakes.

### Fishing Systems

Create:

- `Fishing Systems`

Add:

- `FishingMinigameManager`
- `FishingDifficultyManager`

Inspector:

- `FishingMinigameManager > Base Duration`: `6`
- `FishingMinigameManager > Action Key`: `Space`
- `FishingDifficultyManager > Zone Level`: `1`

### Run Manager

Create:

- `Run Manager`

Add:

- `RunManager`

Inspector:

- Normal Round Count: `3`
- Round Duration: `45`
- Abandon Run Key: `X`
- Prototype Hud: drag `HUD Canvas`
- Player Currency: drag `Player`
- Player Progression: drag `Player`
- Player Inventory: drag `Player`
- Boss Encounter: drag `Boss Fish Spot`

For quick testing, temporarily set:

- Normal Round Count: `1`
- Round Duration: `20`

## 4. Fishing Spot

### GameObject

Create:

- `Fishing Spot`

Recommended visual children:

- `Fishing Spot Marker`
- `Pond Ripple`
- `Small Sign`

### Components

On `Fishing Spot`, add:

- `Box Collider` or `Sphere Collider`
- `FishingSpotInteractable`

Collider:

- Is Trigger: optional
- Layer: `Interactable`

Inspector:

- Spot Name: `Funky Wizard Pond`
- Fishing Time: `2`
- Keep default fish species and rarity settings for first test.

References:

- None required. It uses `PlayerInteractor`, `RunManager`, `FishingMinigameManager`, and `FishingDifficultyManager`.

## 5. Fishing Run Portal

### GameObject

Create:

- `Fishing Run Portal`

Recommended visual children:

- `Dock Planks`
- `Glow Ring`
- `Portal Crystal`

### Components

On `Fishing Run Portal`, add:

- `Box Collider`
- `FishingRunPortalInteractable`

Collider:

- Layer: `Interactable`

Inspector:

- Portal Name: `Moonlit Fishing Dock`
- Run Manager: drag `Run Manager`

Test:

- Look at portal.
- Press E.
- HUD should show `FishingRound`.

## 6. Shopkeeper

### GameObject

Create:

- `Shopkeeper`

Recommended visual children:

- `Body`
- `Hat`
- `Shop Counter`

### Components

On `Shopkeeper`, add:

- `Capsule Collider`
- `CharacterIdentity`
- `ShopKeeperInteractable`
- Optional `NPCInteractable`
- Optional `SimpleNPCIdleMotion`

Collider:

- Layer: `Interactable`

Inspector:

`CharacterIdentity`:

- Display Name: `Froggin the Bait Merchant`
- Character Type: `Shopkeeper`
- Faction: `Village`
- Title Or Role: `Bait Merchant`

`ShopKeeperInteractable`:

- Shopkeeper Name: `Froggin`

References:

- None required. It uses the interacting player's inventory, currency, and rod components.

Test:

- Press E.
- Press `1` to sell all fish.
- Press `2` or higher to buy/equip rods.
- Press Escape to close shop.

## 7. NPC

### GameObject

Create:

- `NPC_Shellbert`

### Components

On `NPC_Shellbert`, add:

- `Capsule Collider`
- `CharacterIdentity`
- `NPCInteractable`
- Optional `SimpleNPCIdleMotion`

Collider:

- Layer: `Interactable`

Inspector:

`CharacterIdentity`:

- Display Name: `Shellbert the Turtle Sage`
- Character Type: `Trainer`
- Faction: `Village`
- Title Or Role: `Fishing Sage`

`NPCInteractable`:

- Identity: drag `NPC_Shellbert`
- Hangout Area: `MainPond`
- Dialogue Line: `Slow casts catch strange fish.`
- Show Area In Dialogue: on

Test:

- Look at NPC.
- HUD should show name and role.
- Press E.
- Status text should show dialogue.

## 8. Mob

### GameObject

Create:

- `Mob_PondGoblin`
  - `World Space Health Bar` optional

### Components

On `Mob_PondGoblin`, add:

- `Capsule Collider`
- `Rigidbody`
- `CharacterIdentity`
- `LivingEntity`
- `MobController`
- `KnockbackReceiver`
- `KnockbackTestInteractable`

Collider:

- Layer: `Interactable`

Rigidbody:

- Use Gravity: on
- Is Kinematic: off
- Collision Detection: Discrete
- Freeze Rotation X/Z: optional, recommended for cleaner testing

Inspector:

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

- Identity: drag `Mob_PondGoblin`
- Living Entity: drag `Mob_PondGoblin`
- Knockback Receiver: drag `Mob_PondGoblin`
- Wander Radius: `3`
- Wander Speed: `1.2`

`KnockbackReceiver`:

- Living Entity: drag `Mob_PondGoblin`
- Default Knockback Force: `8`
- Upward Force: `2`
- Stun Duration: `1.25`

`KnockbackTestInteractable`:

- Display Name: `Pond Goblin`
- Receiver: drag `Mob_PondGoblin`
- Knockback Force: `8`
- Damage Amount: `25`

### Floating Health Bar

Optional hierarchy:

- `Mob_PondGoblin`
  - `World Space Health Bar`
    - `Name Text`
    - `Health Slider`

On `World Space Health Bar`, add:

- `Canvas`
- `FloatingHealthBar`

Canvas:

- Render Mode: `World Space`

`FloatingHealthBar`:

- Target: drag `Mob_PondGoblin`
- Identity: drag `Mob_PondGoblin`
- Name Text: drag `Name Text`
- Health Slider: drag `Health Slider`

Test:

- Look at mob and confirm target health HUD appears.
- Press E twice and confirm the mob is defeated.

## 9. Mob Spawn Area

### GameObject

Create:

- `Mob Spawn Area`

### Components

On `Mob Spawn Area`, add:

- `MobSpawnArea`

Inspector:

- Area Name: `Wild Edge`
- Mob Prefab: drag `Mob_PondGoblin` prefab
- Spawn Count: `3`
- Spawn Radius: `6`
- Spawn On Start: on

Test:

- Press Play.
- Mobs should spawn near the area and wander.

## 10. Boss Fish Spot

### GameObject

Create:

- `Boss Fish Spot`

Recommended visual children:

- `Boss Pond Marker`
- `Boss Crystal`
- `Boss Arch`

### Components

On `Boss Fish Spot`, add:

- `Box Collider` or `Sphere Collider`
- `BossFishEncounter`

Collider:

- Layer: `Interactable`

Inspector:

`BossFishEncounter`:

- Max Tension: `100`
- Max Capture: `100`
- Tension Slider: drag boss tension UI slider
- Capture Slider: drag boss capture UI slider
- Keep default boss pool for first test.

Test:

- Start a run.
- Complete normal rounds.
- Interact with boss spot.
- Boss HUD should show phase, tension, capture, and prompts.

## 11. Boss Gate

### GameObject

Create:

- `Boss Gate`

### Components

On `Boss Gate`, add:

- `Box Collider`
- `BossGateInteractable`

Collider:

- Layer: `Interactable`

Inspector:

- Gate Name: `Boss Gate`
- Keep default locked/ready messages.

Test:

- Before boss state, gate says locked/inactive.
- During boss state, gate says boss encounter is ready.

## 12. Sit Point

### GameObject

Create:

- `SitPoint_Bench`
  - `Sit Point`

### Components

On `SitPoint_Bench`, add:

- `Box Collider`
- `SitPointInteractable`

Collider:

- Layer: `Interactable`

Inspector:

- Display Name: `Bench`
- Sit Point: drag child `Sit Point`
- Sitting Message: `Sitting`

Test:

- Press E to sit.
- Press E or move to stand.

## 13. Village Areas

Create:

- `Area - Main Pond`
- `Area - Wizard Village`
- `Area - Wild Edge`
- `Area - Boss Gate`

On each area object, add:

- `Box Collider`
- `VillageAreaMarker`

Collider:

- Is Trigger: on

Inspector:

`Area - Main Pond`:

- Area Type: `MainPond`
- Area Display Name: `Main Pond`

`Area - Wizard Village`:

- Area Type: `WizardVillage`
- Area Display Name: `Wizard Village`

`Area - Wild Edge`:

- Area Type: `WildEdge`
- Area Display Name: `Wild Edge`

`Area - Boss Gate`:

- Area Type: `BossGate`
- Area Display Name: `Boss Gate`

Test:

- Walk through each trigger.
- HUD should update current area text.

## 14. Materials / Low-Poly Placeholders

Recommended simple materials:

- Grass: bright moss green
- Pond Water: cyan / blue, slightly transparent if desired
- Dock Wood: warm brown
- Wizard Hut: purple walls, teal roof
- Shop Stall: yellow/gold cloth, brown frame
- Crystals: neon cyan, violet, magenta
- Mushrooms: pale stem, red/purple cap
- Lantern Light: warm yellow
- Boss Gate: dark stone, gold and neon accents
- Player Robe: bold blue, purple, or green
- NPC Robes: distinct bright colors
- Mobs: swamp green, muddy blue, purple accents

Clean low-poly look:

- Use primitives first.
- Keep silhouettes chunky.
- Avoid tiny details.
- Use flat colors.
- Use `LowPolyPresentationSettings` on a `Village Presentation` object.
- If using pixel textures, set texture Filter Mode to `Point`.

## 15. Play Mode Test Checklist

1. Confirm Unity Console has no compile errors.
2. Confirm `Prototype Scene Validator` reports no critical missing objects.
3. Press Play.
4. Move with WASD.
5. Sprint with Shift and confirm stamina drains.
6. Stop sprinting and confirm stamina regenerates.
7. Jump with Space.
8. Look at NPC and press E.
9. Look at shopkeeper and press E.
10. Look at fishing run portal and press E.
11. Confirm HUD shows run state and timer.
12. Look at fishing spot and press E.
13. Complete fishing minigame.
14. Confirm fish catch shows rarity, stats, XP, and sell value.
15. Press X to abandon run and bank fish, or finish the run normally.
16. Return to shopkeeper.
17. Press `1` to sell fish.
18. Press `2` or higher to buy/equip rod if you have enough gold.
19. Walk to mob area.
20. Look at mob and confirm target health appears.
21. Press E to knock back/damage mob.
22. Repeat until defeated.
23. Sit on bench and stand up.
24. Walk through area triggers and confirm area HUD changes.
25. Complete rounds and test boss fish spot if desired.

## 16. Common Mistakes And Fixes

### No interaction prompt appears

- Make sure the object is on the `Interactable` layer.
- Make sure it has a Collider.
- Make sure `PlayerInteractor > Interactable Layers` includes `Interactable`.
- Make sure the object has a script implementing `IInteractable`.

### Player cannot move

- Make sure the player has `CharacterController`.
- Make sure the player has `FirstPersonPrototypeController`.
- Make sure the player is not stuck in the `Sitting` or `Fishing` state.
- Press Escape if the mouse is unlocked, then click Game view again.

### HUD does not update

- Make sure `HUD Canvas` has `PrototypeHud`.
- Drag player references into `PrototypeHud`.
- Drag Text/Slider UI objects into matching fields.
- Make sure the player has `LivingEntity`, `PlayerCurrency`, and `PlayerProgression`.

### Fishing does not start

- Make sure `Fishing Systems` exists.
- Add `FishingMinigameManager`.
- Add `FishingDifficultyManager`.
- Make sure fishing spot has `FishingSpotInteractable`.
- Make sure fishing spot has Collider and `Interactable` layer.

### Shop does not sell fish

- Make sure the player has `SimpleInventory`.
- Make sure fish were banked into inventory.
- If inside a run, press `X` to abandon and bank fish, or finish the run.
- Press `1` while the shop menu is open.

### Rod buying does not work

- Make sure the player has `PlayerFishingRod`.
- Make sure the player has `PlayerCurrency`.
- Make sure you have enough gold.
- Press number keys `2` and up while shop is open.

### Mob cannot be damaged

- Make sure mob has `LivingEntity`.
- Make sure mob has `Rigidbody`.
- Make sure mob has `KnockbackReceiver`.
- Make sure mob has `KnockbackTestInteractable`.
- Make sure `KnockbackTestInteractable > Receiver` is assigned or on the same object.

### Boss encounter does not start

- Make sure `RunManager > Boss Encounter` references `Boss Fish Spot`.
- Complete the required normal rounds first.
- For testing, set `Normal Round Count` to `1`.
- Make sure `Boss Fish Spot` has `BossFishEncounter` and Collider.

### Area name does not change

- Make sure player has `PlayerAreaTracker`.
- Make sure area object has `VillageAreaMarker`.
- Make sure area object has Trigger Collider.
- Make sure the player passes through the trigger volume.
