# Wizard Pond Prototype

Wizard Pond is a beginner-friendly Unity prototype for a stylized low-poly / pixel-inspired wizard fishing roguelite.

The player is a funky wizard in a cozy magical village. They talk to named NPCs, explore a pond lobby, start timed fishing runs, catch magical fish through quick-time minigames, face boss fish, return to the village, sell fish, buy rods, level up, and improve wizard stats/gear.

## Start Here

Use these docs in order:

1. `README.md` - project overview, current loop, systems, and test checklist.
2. `VISUAL_STYLE_GUIDE.md` - the target cozy low-poly / pixel-stylized look.
3. `WORLD_AREAS_PLAN.md` - the first village, shop, dock, river, and boss swamp areas.
4. `UNITY_SCENE_SETUP_GUIDE.md` - exact Unity Editor scene setup.
5. `PREFAB_SETUP_GUIDE.md` - how to turn scene objects into reusable prefabs.
6. `CORE_GAMEPLAY_SETUP.md` - smallest playable fishing, reward, XP, coins, and rod-upgrade loop.

If something is broken in Play Mode, check `UNITY_SCENE_SETUP_GUIDE.md > Common Mistakes And Fixes` first.

## Fast Starter Scene Setup

Recommended in Unity:

`Tools > Fishing Game > Build Starter Pond Prototype`

This Editor menu builds a rich, fully populated 3D visual test layout in the `StarterPondPrototype` scene using standard primitives and flat colors:

*   **Pond**: A blue semi-transparent cylinder (`Proto_Water` material), centered in front of the wooden dock.
*   **Dock**: A rustic brown wooden dock where the player starts, facing the water.
*   **Starter Fishing Spot**: Positioned at the water's edge, featuring a glowing blue spinning/bobbing cube on the `Interactable` layer.
*   **Shopkeeper Area**: Features a detailed shop stall with a counter, support pillars, red canvas roof, display items (potion bottle, golden upgrade rod), and a "Rod Shop" signboard, with shopkeeper Mordo standing behind it.
*   **Decorations**: Disperses glowing mushrooms (with interactive pulsing colors), green floating lily pads, spherical frog and turtle placeholders, and a post-mounted wooden fishing sign.
*   **Cozy Lighting**: Sets up a warm evening sun Directional Light, an amber lantern Point Light on the shop stall, an amber lantern Point Light near the dock, and subtle cyan/blue depth fog.
*   **System Wiring**: Automatically wires all HUD connections, interactor layers, cameras, audio feedback, and system objects with zero validation warnings.

The intended starter control path is: look at `Starter Fishing Spot` and press `E`.

The older `FirstPersonFishingController` left-click bobber casting script is kept as a legacy alternate/future experiment, but the starter scene builder disables it for this prototype path.

To test that legacy alternate later, enable `FirstPersonFishingController` and turn on `Legacy Casting Enabled` in its Inspector.

The manual setup docs are still useful as a backup and for understanding what the builder created.

## Current Playable Loop

1. Explore the village lobby.
2. Talk to named NPCs.
3. Sit at benches/logs/campfires.
4. Sprint, jump, and manage stamina.
5. Look at the glowing `Starter Fishing Spot` and press `E`.
6. Follow the fishing minigame instructions.
7. Catch or lose fish with clear center-screen popup feedback.
8. Gain coins and XP from successful starter pond catches.
9. Level up and improve fishing skill.
10. Talk to Mordo, then press `0` to buy a Rod Level Upgrade.
11. Press `J` to open/close the Fish Journal and review collection progress.
12. Start longer fishing runs and boss tests when the starter loop feels good.

Expected in-game feedback:

- Catch popup: `Caught!`, fish name, rarity, weight, coins, XP, `New Journal Entry!`, and `Level Up!` when relevant.
- Failure popup: `The fish got away!` plus a reason when known.
- Shop popup: current rod level, upgrade cost, `Press 0 to upgrade rod`, purchase success, or not enough coins.
- Minigame HUD: hooked fish, button prompt, goal, timer/progress, and result.
- Fish Journal: `J` opens/closes discovered fish, catch counts, best weights, and undiscovered `???` entries.
- Sound/game-feel: optional generated tones for prompts, fishing start, hook, success/failure, catch rewards, coins, XP, level up, rod upgrade, shop open/close, and Fish Journal open/close.
- Visual polish: the fishing spot grows slightly when looked at, pulses when fishing starts, reward popups pulse harder for rarer fish, and shop/journal feedback feels more responsive.

Fish behavior styles:

- `Timing`: press `Space` inside the safe zone.
- `Rapid Tap`: tap `Space` quickly.
- `Hold Balance`: hold/release `Space` to manage tension.
- `Reaction Sequence`: press prompted `WASD` keys.
- `Erratic`: behavior changes between simple challenge styles.

Starter species behavior:

- `Pond Minnow`: Timing
- `Bluegill Sprite`: Rapid Tap
- `Moon Carp`: Hold Balance
- `Crystal Trout`: Reaction Sequence
- `Goblin Catfish`: Erratic
- `Ancient Boss Koi`: Erratic boss-preview

## Folder Structure

- `Assets/Scripts/Player`: first-person movement, stamina sprinting, jumping, sitting, movement states, player progression.
- `Assets/Scripts/Characters`: identity, character type, faction/team, health, stamina, shared living entity data.
- `Assets/Scripts/NPCs`: named NPC interaction and simple idle/wander motion.
- `Assets/Scripts/Mobs`: mob wandering and mob spawn areas.
- `Assets/Scripts/Combat`: reserved for future combat; current knockback lives in `Physics`.
- `Assets/Scripts/Physics`: knockback, dropped physics items, simple test interactions.
- `Assets/Scripts/World`: village area markers, area tracking, decoration motion, low-poly presentation settings.
- `Assets/Scripts/UI`: HUD and floating health bars.
- `Assets/Scripts/Fishing`: fish data, fishing spots, rods, fish generation, legacy fishing experiments.
- `Assets/Scripts/Minigames`: fishing encounter types, catch quality, difficulty scaling, minigame manager.
- `Assets/Scripts/Runs`: lobby/run/boss/run-complete state flow and run portal/boss gate interactables.
- `Assets/Scripts/Bosses`: boss fish data, phases, encounter result, boss encounter controller.
- `Assets/Scripts/Inventory`: simple fish/item inventory.
- `Assets/Scripts/Journal`: Play Mode fish collection journal and uGUI journal panel.
- `Assets/Scripts/Economy`: gold and shopkeeper logic.
- `Assets/Scripts/Gear`: gear slots, affixes, random rolls, equipped gear bonuses.
- `Assets/Scripts/Stats`: wizard stat types and stat point spending.
- `Assets/Scripts/Audio`: safe prototype sound feedback helpers.
- `Assets/Scripts/Customization`: reserved for future wizard outfit systems.

## Major Systems

- `FirstPersonPrototypeController`: owns walking, sprinting with stamina, jumping, sitting, and movement state labels.
- `PlayerInteractor`: owns the center-screen raycast, interaction prompt, looked-at character display, and targeted mob/boss health display.
- `PrototypeHud`: displays only readable prototype state: health, stamina, movement, area, target, prompt, fishing/minigame info, run info, gold, and level.
- `GameAudioFeedback`: plays optional assigned AudioClips, or simple generated placeholder tones when clips are missing.
- `CharacterIdentity`: stores display name, character type, faction, role/title, and flavor text.
- `LivingEntity`: shared health/stamina/movement stats for player, NPCs, mobs, and bosses.
- `NPCInteractable`: simple named NPC dialogue through the existing `IInteractable` system.
- `MobController`: simple local wandering for wild-edge mobs.
- `MobSpawnArea`: spawns simple mob prefabs during Play Mode.
- `FishingSpotInteractable`: generates fish, starts minigames, applies catch quality, and sends catches to run/inventory systems.
- `FishJournal`: tracks discovered fish, catch counts, best weights, and collection progress during Play Mode.
- `FishJournalUi`: opens/closes the uGUI Fish Journal panel with `J` or `Esc`.
- `RunManager`: controls lobby, fishing rounds, round results, boss encounter, and run complete states.
- `ShopKeeperInteractable`: text shop for selling fish and buying/equipping rods.
- `WizardStats` and `WizardGear`: prototype stat points and gear bonuses.
- `KnockbackTestInteractable`: simple vertical-slice mob test interaction that applies knockback and damage through `LivingEntity`.
- `LowPolyPresentationSettings`: simple camera, light, ambient, and fog setup for a bright low-poly look.
- `BillboardLabel`: optional helper for floating world labels that face the player camera.
- `MaterialPulse`: optional helper for pulsing magical mushrooms, crystals, lanterns, rod tips, and boss markers.
- `PrototypeSceneValidator`: optional Play Mode setup checker that logs missing core scene objects and references.

## Sound And Game-Feel Setup

Run the starter builder:

`Tools > Fishing Game > Build Starter Pond Prototype`

It creates `Game Audio Feedback` automatically. You do not need audio assets for the prototype because `GameAudioFeedback` can play generated placeholder tones.

Optional later setup:

- Select `Game Audio Feedback`.
- Drag short clips into fields such as `Fishing Cast Clip`, `Catch Reward Clip`, `Coin Gain Clip`, `Level Up Clip`, `Rod Upgrade Clip`, `Shop Open Clip`, or `Journal Open Clip`.
- Keep `Use Generated Placeholder Tones` enabled while clip fields are empty.
- Lower `Volume` if the tones are too loud.

If no sound plays, confirm `Game Audio Feedback` has both `AudioSource` and `GameAudioFeedback`, the `Player Camera` has exactly one `AudioListener`, and Unity/Game view audio is not muted.

## Cleanup Notes

Current overlap review:

- Player movement: active controller is `FirstPersonPrototypeController`. `FirstPersonFishingController` is legacy and should not be on the player.
- Stamina/health: shared data lives in `LivingEntity`; player movement reads stamina from there.
- HUD: display logic lives in `PrototypeHud`; gameplay systems should call HUD setter methods, not own UI decisions.
- Interaction: all interactable scene objects should implement `IInteractable` and be reached through `PlayerInteractor`.
- Fishing: active flow is `FishingSpotInteractable` plus `FishingMinigameManager`. `FishingSpot`, `FishingBobber`, and `FirstPersonFishingController` are legacy experiments.
- Inventory/economy: fish/item storage lives in `SimpleInventory`; gold lives in `PlayerCurrency`.
- Run state: `RunManager` owns lobby/round/boss/run-complete state.
- Mobs: health is `LivingEntity`; simple movement is `MobController`; playful knockback is `KnockbackReceiver`.
- Simple mob damage: the current test path is `KnockbackTestInteractable -> KnockbackReceiver -> LivingEntity.Damage`.

No files were deleted in this pass. The legacy fishing scripts are kept because they may still be attached in an experimental scene, but they are documented as not part of the main Wizard Pond loop.

## Required Scene Setup

For the complete step-by-step Unity Editor setup, see `UNITY_SCENE_SETUP_GUIDE.md`.

### Player

Create or confirm a `Player` GameObject with:

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
- Child `Camera`

Set `CharacterIdentity`:

- Display Name: `Young Pond Wizard`
- Character Type: `Player`
- Faction: `Player`
- Title Or Role: `Apprentice Angler`

Set `LivingEntity`:

- Max Health: `100`
- Max Stamina: `100`
- Movement Speed: `5`
- Sprint Speed: `8`
- Stamina Regen Rate: `15`

### Required Layers

Create an `Interactable` layer if it does not exist.

Assign `Interactable` to:

- NPCs
- Shopkeeper
- Fishing run portal
- Fishing spots
- Boss fishing spot
- Sit points
- Mob/test dummy objects that should respond to E

Make sure `PlayerInteractor > Interactable Layers` includes `Interactable`.

### HUD Canvas

Create a `HUD Canvas` with `PrototypeHud`.

Recommended visible HUD fields:

- Player health text + slider
- Player stamina text + slider
- Movement state text
- Current area text
- Looked-at character text
- Interaction prompt text
- Target health text + slider
- Fishing/minigame texts + slider
- Run timer/state text
- Gold text
- Level/XP text

Optional or collapsible/debug fields:

- Full inventory text
- Gear summary text
- Wizard stats text
- Shop menu text
- Full run value/fish count text

### Village Areas

Create area parent objects with `VillageAreaMarker`:

- `Area - Main Pond`
- `Area - Wizard Village`
- `Area - Wild Edge`
- `Area - Boss Gate`

For area HUD detection, add a trigger collider around each area marker. The player needs `PlayerAreaTracker`.

### Vertical Slice Scene Objects

Minimum required objects for one complete test:

- `Player`
- `HUD Canvas`
- `Prototype Scene Validator`
- `Run Manager`
- `Fishing Systems`
- `Fishing Run Portal`
- `Fishing Spot`
- `Boss Fishing Spot`
- `Shopkeeper`
- `NPCPrototype`
- `MobSpawnArea`
- `Mob_PondGoblin` prefab or scene object
- `Village Presentation`

For quick testing, set `RunManager > Round Duration` to `20` and optionally `Normal Round Count` to `1`. Set it back to `3` when you want the intended run shape.

### Presentation

Create `Village Presentation` with `LowPolyPresentationSettings`.

- Assign the main camera.
- Assign the directional light.
- Use flat materials and bold colors.
- If using pixel textures, set texture Filter Mode to `Point`.
- Optional later: use a low-resolution Render Texture for stronger pixelation.

## Prefab Setup Guide

For the full prefab-by-prefab setup plan, see `PREFAB_SETUP_GUIDE.md`.

Recommended prefabs:

- `PlayerPrototype`
- `FishingSpotPrototype`
- `ShopkeeperPrototype`
- `NPCPrototype`
- `Mob_PondGoblin`
- `MobSpawnAreaPrototype`
- `SitPoint_Bench`
- `BossFishSpotPrototype`
- `VillageDecoration_Crystal`
- `VillageDecoration_Mushroom`
- `VillageDecoration_Lantern`
- `WizardHut`
- `DockSection`
- `FishingSign`
- `LilyPad`
- `Turtle`
- `Frog`
- `FishBucket`
- `ShopCounter`
- `RodDisplay`
- `PotionBottle`
- `BossArenaMarker`
- `GoblinPlaceholder`

## Visual Prototype Checklist

Build this in Unity before worrying about polished art:

1. Create a flat grass ground plane and a simple starter pond.
2. Add `DockSection` pieces leading to the water.
3. Add one `FishingSpotPrototype` at the end of the dock.
4. Add a crooked `WizardHut` and a simple shop stall near the pond.
5. Add one `ShopkeeperPrototype` placeholder behind the stall.
6. Add glowing mushrooms and lanterns around the pond path.
7. Add frog, turtle, lily pad, fish bucket, and fishing sign decorations.
8. Add a `Prototype Scene Validator` object.
9. Press Play and fix warnings in the Console before expanding the scene.

Use `VISUAL_STYLE_GUIDE.md` for colors and mood, and `WORLD_AREAS_PLAN.md` for where each piece belongs.

### PlayerPrototype

- Scripts: `FirstPersonPrototypeController`, `PlayerInteractor`, `PlayerAreaTracker`, `CharacterIdentity`, `LivingEntity`, `SimpleInventory`, `PlayerCurrency`, `PlayerFishingRod`, `PlayerProgression`, `WizardStats`, `WizardGear`.
- Components: `CharacterController`, child `Camera`.
- Rigidbody: none.
- Layer/Tag: tag `Player` if you use tags; default layer is fine.
- References: assign child camera to controller/interactor; assign HUD if not using auto-find.
- Test: walk, sprint, jump, sit, interact, fish.

### NPCPrototype

- Scripts: `CharacterIdentity`, `NPCInteractable`, optional `SimpleNPCIdleMotion`.
- Components: capsule/cube mesh, collider.
- Rigidbody: none unless you want physics pushing.
- Layer: `Interactable`.
- References: `NPCInteractable > Identity` can auto-find.
- Test: look at NPC, confirm name/prompt/dialogue.

### ShopkeeperPrototype

- Scripts: `CharacterIdentity`, `NPCInteractable`, `ShopKeeperInteractable`, optional `SimpleNPCIdleMotion`.
- Components: capsule/cube mesh, collider.
- Rigidbody: none.
- Layer: `Interactable`.
- References: assign inventory/currency/rod if auto-find does not find them.
- Test: talk, sell fish, buy/equip rod.

### Mob_PondGoblin

- Scripts: `CharacterIdentity`, `LivingEntity`, `MobController`, `KnockbackReceiver`, `KnockbackTestInteractable`.
- Components: capsule mesh, collider.
- Rigidbody: required for knockback. Use non-kinematic Rigidbody. Freeze X/Z rotation if you want it to stay mostly upright.
- Layer: `Interactable` only if you want E targeting/test knockback.
- References: `MobController` can auto-find identity/entity. `KnockbackTestInteractable` can auto-find `KnockbackReceiver`.
- Test: spawn it, confirm it wanders, target health appears when looked at, and repeated E presses damage/defeat it.

### Mob_MushroomImp

- Same as `Mob_PondGoblin`, but use a sphere/capsule/cube mushroom silhouette.
- Set `CharacterIdentity > Display Name` to `Mushroom Imp`.
- Test: confirm it spawns and wanders separately from pond goblins.

### FishingSpotPrototype

- Scripts: `FishingSpotInteractable`.
- Components: visible pond marker/rock/sign, collider.
- Rigidbody: none.
- Layer: `Interactable`.
- References: default fish pools can stay as-is.
- Test: look at spot, press E, complete minigame, receive fish.

### WardrobePrototype

- Scripts: `CharacterIdentity`, `NPCInteractable` for now.
- Components: tent/stall primitives, collider.
- Rigidbody: none.
- Layer: `Interactable`.
- References: no wardrobe system yet.
- Test: press E and confirm placeholder dialogue.

### SitPoint_Bench

- Scripts: `SitPointInteractable`.
- Components: bench mesh, collider, child empty `Sit Point`.
- Rigidbody: none.
- Layer: `Interactable`.
- References: assign child `Sit Point`.
- Test: press E to sit, press E or move to stand.

### MobSpawnAreaPrototype

- Scripts: `MobSpawnArea`.
- Components: empty GameObject; optional gizmo only.
- Rigidbody: none.
- Layer: default.
- References: assign `Mob_PondGoblin` or `Mob_MushroomImp` prefab.
- Test: press Play, confirm mobs spawn in radius.

### VillageDecoration_Lantern

- Scripts: optional `VillageDecorationMotion`.
- Components: cylinders/cubes/sphere light shape.
- Rigidbody: none.
- Layer: default.
- References: none.
- Test: confirm subtle spin/bob if enabled.

### VillageDecoration_Crystal

- Scripts: `VillageDecorationMotion`.
- Components: cone/cube crystal shapes.
- Rigidbody: none.
- Layer: default.
- References: none.
- Test: confirm crystal spins/bobs.

### VillageDecoration_Mushroom

- Scripts: optional `VillageDecorationMotion`.
- Components: cylinder stem, sphere/capsule cap.
- Rigidbody: none.
- Layer: default.
- References: none.
- Test: confirm it reads clearly from a distance.

## Play Mode Test Checklist

1. Confirm Unity recompiles with no Console errors.
2. Confirm `Prototype Scene Validator` does not report missing required objects.
3. Move with WASD, sprint with Shift, jump with Space.
4. Confirm health, stamina, movement state, area, gold, and level display clearly.
5. Walk into each area trigger and confirm the area HUD updates.
6. Look at NPCs and confirm name/prompt/dialogue.
7. Look at mobs/boss placeholders and confirm target health appears.
8. Sit at a bench, then stand with E or movement.
9. Use knockback test object if present.
10. Start a fishing run.
11. Confirm run timer appears only during the run.
12. Fish and complete a minigame.
13. Confirm caught fish have rarity, stats, XP, and sell value.
14. Press `X` during a run to abandon and return to lobby with current run fish banked, or complete the run normally.
15. Sell fish at the shop with `1`.
16. Buy/equip a rod with number keys `2` and up.
17. Complete rounds and confirm boss gate status changes during boss state.
18. Return to lobby and sell fish at the shop.

## Known Limitations

- No saving/loading yet.
- No real scene transitions yet.
- Combat is not implemented; only knockback/testing physics exists.
- Mobs wander only and do not attack. The current defeat path is repeated E presses on `KnockbackTestInteractable`.
- NPC dialogue is one-line text only.
- Wardrobe is a placeholder interactable only.
- Boss gate is a status marker, not a teleport.
- HUD uses Unity `Text`; a future pass could move to TextMeshPro.
- Pixelation is setup guidance only; no custom shader added.
- Legacy fishing scripts still exist but should not be used in the main loop.

## Next Recommended Features

1. Add spawn points and move the player between village, run, and boss spaces.
2. Add simple NPC dialogue choices and floating speech bubbles.
3. Add a basic wardrobe prototype with robe color, hat shape, and outfit unlocks.
