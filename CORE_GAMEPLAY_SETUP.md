# Wizard Pond Core Gameplay Setup

This guide sets up the smallest playable loop:

Player walks to pond -> presses E -> plays fishing minigame -> catches or loses fish -> gains coins/XP -> levels up -> buys a rod upgrade from the shopkeeper.

The intended Starter Pond Prototype control path is:

`Walk to Starter Fishing Spot -> look at it -> press E -> fishing minigame starts`

The old left-click bobber casting script, `FirstPersonFishingController`, is kept in the project as a legacy alternate/future casting experiment. The scene builder disables it for this starter test scene so it does not conflict with the E-to-interact loop.

If you intentionally want to test that legacy alternate later, enable the component and turn on `Legacy Casting Enabled` in its Inspector.

## Recommended One-Click Setup

Use the Unity menu:

`Tools > Fishing Game > Build Starter Pond Prototype`

This creates or updates a complete visual prototype environment utilizing standard Unity primitives and cozy stylized colors/materials:

*   **Ground**: Large moss green plane (`Proto_Grass`) for the terrain.
*   **Pond**: Flat semi-transparent blue circular pond marker (`Proto_Water`) placed in front of the player.
*   **Dock**: Simple wooden dock made of brown planks and support pillars. The player begins standing on or near it, facing the pond.
*   **Starter Fishing Spot**: Positioned at the edge of the pond. Includes a cyan glowing cube visual marker that spins and bobs automatically (`VillageDecorationMotion`) on the `Interactable` layer.
*   **Shopkeeper Area**: Located to the right of the pond, featuring a physical shop stall with a rustic wooden counter, pillars, a red canvas roof, placeholder items (a glowing potion flask, a display upgrade rod), and a sign-board labeled "Rod Shop".
*   **Prototype Decorations**: Dispersed naturally around the pond, including:
    *   *Glowing Mushrooms*: Complete with white stems and pulsing color caps (`MaterialPulse` script) in purple, teal, and orange.
    *   *Lily Pads*: Green floating circular pads on the water surface.
    *   *Frog/Turtle*: Spherical placeholders indicating swamp wildlife.
    *   *Fishing Sign*: A rustic signpost marking the angling zone.
*   **Lighting**: Includes a warm evening Directional Light, a cozy amber Point Light over Mordo's shop counter, and a second lantern Point Light near the dock. It also configures `LowPolyPresentationSettings` for clean solid-color skybox rendering and subtle depth fog.
*   **HUD Canvas**: Fully generated and wired to player systems, with customized uGUI text alignments and a progress bar slider for the minigames.
*   **Game Audio Feedback**: Safe generated placeholder tones for prompts, fishing, rewards, coins, XP, level ups, shop actions, and the Fish Journal. Optional AudioClip fields can replace the tones later.
*   **Prototype Scene Validator**: Added with `checkOptionalObjects = false` to guarantee the starter scene loop passes validation with zero console errors.

You can safely run the menu item more than once. It reuses existing objects, updates positions and materials, and only adds missing pieces.

After it runs, the Console should include:

`Starter Pond Prototype ready: walk to Starter Fishing Spot and press E.`

`Game audio feedback ready: optional clips can be assigned on Game Audio Feedback, or generated placeholder tones will play.`

## Required Scene Objects

Use this manual setup section as a backup if the menu builder misses something or you want to understand what it created.

## 1. Player

Create or confirm a GameObject named `Player`.

Required components:

- `CharacterController`
- `FirstPersonPrototypeController`
- `PlayerInteractor`
- `LivingEntity`
- `SimpleInventory`
- `PlayerCurrency`
- `PlayerProgression`
- `PlayerFishingRod`
- `WizardStats`
- `WizardGear`

Child object:

- `Player Camera` with `Camera` and `AudioListener`

Inspector references:

- `FirstPersonPrototypeController > Player Camera`: drag `Player Camera`
- `PlayerInteractor > Player Camera`: drag `Player Camera`
- `PlayerInteractor > Interactable Layers`: include the `Interactable` layer
- `PlayerProgression > Currency`: drag `Player`
- `PlayerProgression > Fishing Rod`: drag `Player`

Beginner values:

- `PlayerCurrency > Gold`: `0` or `50` if you want to test the first upgrade quickly
- `PlayerProgression > XP To Next Level`: `100`
- `PlayerFishingRod > Rod Level`: `1`
- `PlayerFishingRod > Base Rod Level Upgrade Cost`: `50`

## 2. HUD Canvas

Create a Canvas named `HUD Canvas`.

Add:

- `PrototypeHud`

Recommended child UI objects:

- `Prompt Text`
- `Status Text`
- `Coins Text`
- `Level Text`
- `XP Text`
- `Rod Text`
- `Last Catch Text`
- `Fish Count Text`
- `Shop Menu Text`
- `Minigame Title Text`
- `Minigame Instructions Text`
- `Minigame Prompt Text`
- `Minigame Progress Text`
- `Minigame Result Text`
- `Minigame Progress Slider`

Inspector references:

- Drag each UI Text/Slider into the matching `PrototypeHud` field.
- Drag `Player` into `Inventory`, `Currency`, `Fishing Rod`, and `Progression`.

Notes:

- The old field is still called `goldText` in code for compatibility, but the HUD now displays `Coins`.
- `Minigame Result Text` can also act as the simple reward popup.

## 3. Fishing Systems

Create an empty GameObject named `Fishing Systems`.

Required components:

- `FishingMinigameManager`
- `FishingDifficultyManager`

Suggested values:

- `FishingMinigameManager > Base Duration`: `6`
- `FishingMinigameManager > Action Key`: `Space`
- `FishingDifficultyManager > Zone Level`: `1`

## 3B. Game Audio Feedback

The recommended builder creates a GameObject named `Game Audio Feedback`.

Required components:

- `AudioSource`
- `GameAudioFeedback`

Beginner values:

- `AudioSource > Play On Awake`: disabled
- `AudioSource > Spatial Blend`: `0`
- `GameAudioFeedback > Use Generated Placeholder Tones`: enabled
- `GameAudioFeedback > Volume`: `0.35`

No audio clips are required. With placeholder tones enabled, the game plays simple generated beeps for interaction prompts, fishing cast/start, fish hook, minigame success/failure, catch rewards, coin gain, XP gain, level up, rod upgrade, shop open/close, shop denied/not enough coins, and Fish Journal open/close.

To replace the placeholder tones later, drag short clips into the matching optional clip fields on `GameAudioFeedback`.

## 4. Fishing Spot

Create a GameObject named `Starter Fishing Spot`.

Required components:

- Box Collider or Sphere Collider
- `FishingSpotInteractable`

Layer:

- `Interactable`

Inspector values:

- `Spot Name`: `Starter Pond`
- `Fishing Time`: `0.5` to `1.5`
- `Award Coins Immediately`: enabled
- Leave `Species Pool` as the default starter fish list for first testing.

When `Award Coins Immediately` is enabled, caught fish are kept in the inventory as collected fish, but they are not sellable again. This keeps the starter loop simple and prevents duplicate coin rewards.

Optional ScriptableObject fish:

1. In the Project window, right-click.
2. Choose `Create > Wizard Pond > Fish Species`.
3. Make assets for:
   - `Pond Minnow`
   - `Bluegill Sprite`
   - `Moon Carp`
   - `Crystal Trout`
   - `Goblin Catfish`
   - `Ancient Boss Koi`
4. Drag those assets into `FishingSpotInteractable > Fish Species Assets`.

If `Fish Species Assets` has entries, the fishing spot uses those assets. If it is empty, it uses the built-in starter fish list.

## 5. Shopkeeper

Create a GameObject named `Shopkeeper`.

Required components:

- Capsule Collider
- `CharacterIdentity`
- `ShopKeeperInteractable`

Layer:

- `Interactable`

Inspector values:

- `CharacterIdentity > Display Name`: `Mordo the Rodmancer`
- `CharacterIdentity > Character Type`: `Shopkeeper`
- `ShopKeeperInteractable > Shopkeeper Name`: `Mordo the Rodmancer`

Shop controls in Play Mode:

- `E`: open shop while looking at shopkeeper
- `1`: sell all fish
- `0`: buy Rod Level Upgrade
- `2+`: buy/equip named rods from `PlayerFishingRod`
- `Esc`: close shop

## 6. Prototype Scene Validator

Create an empty GameObject named `Prototype Scene Validator`.

Required component:

- `PrototypeSceneValidator`

Suggested values:

- `Run On Start`: enabled
- `Check Optional Objects`: disabled

Press Play and read the Console. Fix validator warnings before expanding the scene.

## Play Mode Test Checklist

## Fish Behavior Types

Starter fish now have distinct behavior styles:

- `Pond Minnow`: Timing, very easy. Press `Space` in the safe zone.
- `Bluegill Sprite`: Rapid Tap, easy. Tap `Space` quickly to fill the meter.
- `Moon Carp`: Hold Balance, medium. Hold/release `Space` to balance tension.
- `Crystal Trout`: Reaction Sequence, medium-hard. Press prompted `WASD` keys.
- `Goblin Catfish`: Erratic, hard. Watch for changing prompts.
- `Ancient Boss Koi`: Erratic boss-preview, very hard/future-facing.

Rod Level makes these challenges easier by reducing final fishing difficulty. Higher rod level should make safe zones and timing feel more forgiving.

1. Press Play.
2. Confirm no red Console errors appear.
3. Confirm HUD shows Coins, Level, XP, Rod, prompt/status, and minigame fields.
4. Walk to `Starter Fishing Spot`.
5. Look at the glowing marker and confirm it grows slightly. The HUD should say `Press E to fish at Starter Pond.`
6. Press `E` to start fishing. Confirm the marker pulses and a cast/start sound plays.
7. Follow the minigame instruction:
   - Timing Window: press `Space` when the moving bar is inside the safe zone.
   - Rapid Tap: tap `Space` repeatedly until the meter fills.
   - Hold Balance: hold `Space` to raise the meter and release it to lower the meter.
   - Precision Stop: press `Space` when the moving bar is closest to the center.
   - Sequence/Reaction: press the prompted `WASD` keys.
8. On success, confirm:
   - a large center popup appears for a few seconds
   - a catch/reward sound plays
   - popup says `Caught!`
   - fish name appears
   - rarity appears
   - weight appears
   - coins increase
   - XP increases
   - last catch text updates
   - `New Journal Entry!` appears the first time you catch that species
   - level up message appears if XP reaches the next level
   - rarer fish use stronger popup pulses and stay visible longer
   - fish count/inventory updates
9. On failure, confirm a large popup says `The fish got away!` and shows a reason such as `Missed timing`, `Wrong input`, `Tension dropped`, or `Cancelled`.
10. Confirm a failure sound plays and no coins or XP are awarded on failure.
11. Catch fish until you have enough coins for the rod upgrade.
12. Walk to `Shopkeeper`.
13. Look at Mordo and confirm the HUD says `Press E to talk to Mordo the Rodmancer.`
14. Press `E` to open shop and confirm a shop sound plays.
15. Confirm the shop popup/menu shows current coins, current rod level, upgrade cost, and `Press 0 to upgrade rod`.
16. Press `0` to buy a Rod Level Upgrade.
17. Confirm a shop popup says `Rod upgraded to Level X!`, pulses, and plays a rod-upgrade sound.
18. Confirm Rod text updates to the new level.
19. Fish again and confirm the minigame feels slightly easier.
20. Press `J` to open the Fish Journal. Confirm the panel pops in and a journal sound plays.
21. Confirm discovered fish show real names and stats, while undiscovered fish show `???`.
22. Catch the same species again, press `J`, and confirm `Caught` count increases.
23. Catch a heavier fish of the same species and confirm `Best Weight` updates.
24. Press `J` or `Esc` to close the Fish Journal.

## Behavior Playtest Checklist

1. Catch fish until you see `Pond Minnow`; confirm the HUD describes a Timing behavior.
2. Catch fish until you see `Bluegill Sprite`; confirm the HUD asks you to tap `Space`.
3. Catch fish until you see `Moon Carp`; confirm the HUD asks you to hold/release `Space`.
4. Catch fish until you see `Crystal Trout`; confirm the HUD asks for prompted `WASD` keys.
5. Catch fish until you see `Goblin Catfish`; confirm the HUD warns about changing/erratic prompts.
6. Open the Fish Journal with `J` and confirm discovered fish show their behavior style.

## Common Mistakes

- Console says left-click casting: check the `Player` or old fishing objects for `FirstPersonFishingController`. It is a legacy alternate/future casting experiment and should be disabled for this starter scene.

- No prompt appears: put the fishing spot/shopkeeper on the `Interactable` layer and include that layer in `PlayerInteractor`.
- Minigame UI does not update: add `FishingMinigameManager` to the scene and assign HUD minigame fields.
- Popup does not appear: rerun `Tools > Fishing Game > Build Starter Pond Prototype` so the builder creates and wires `Central Popup Text`.
- Popup text appears but does not fade: confirm `Central Popup Text` has a `CanvasGroup` and is assigned to `PrototypeHud > Popup Canvas Group`.
- Fish Journal does not open: rerun the scene builder so it adds `FishJournal` to the player and `FishJournalUi` to `HUD Canvas`.
- Fish Journal opens but is blank: confirm `FishJournalUi > Journal Text` points at `Fish Journal Text`.
- No sound plays: confirm `Game Audio Feedback` exists, has `AudioSource` and `GameAudioFeedback`, and `Use Generated Placeholder Tones` is enabled if no clips are assigned.
- Sound still does not play: confirm the `Player Camera` has one `AudioListener` and Unity/Game view audio is not muted.
- Coins do not update: make sure the player has `PlayerCurrency` and HUD has the `goldText` field assigned to your Coins text.
- XP does not update: make sure the player has `PlayerProgression`.
- Shop keys do nothing: keep looking at the shopkeeper after pressing `E`, or check that the shop menu text is assigned.
