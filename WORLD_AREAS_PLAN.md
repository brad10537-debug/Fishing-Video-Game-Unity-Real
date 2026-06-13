# Wizard Pond World Areas Plan

This plan breaks the prototype world into small areas that can be built with primitives and prefabs.

## 1. Wizard Village Pond

### Purpose

The safe starting area and main identity shot for the game. This is where the player first understands that Wizard Pond is cozy, magical, and fishing-focused.

### Key Props

- central glowing pond
- lily pads
- frogs
- turtles
- dock sections
- fishing sign
- fish bucket
- glowing mushrooms
- lanterns
- crooked wizard hut in background
- warm path lights

### Gameplay Function

- player spawn
- starter fishing spot
- run portal nearby
- first NPC conversation
- first visual read of the world

### Visual Mood

Bright, cozy, blue/cyan magical water, green grass, purple wizard accents, warm lanterns.

### Needed Prefabs

- `PlayerPrototype`
- `FishingSpotPrototype`
- `DockSection`
- `FishingSign`
- `LilyPad`
- `Frog`
- `Turtle`
- `FishBucket`
- `VillageDecoration_Lantern`
- `VillageDecoration_Mushroom`
- `VillageDecoration_Crystal`
- `NPCPrototype`

## 2. Shopkeeper Stall

### Purpose

The economy hub where the player sells fish and upgrades rods/items.

### Key Props

- shop counter
- rod display
- potion bottles
- charm strings
- bait jars
- fish bucket
- hanging lantern
- cloth canopy
- small price signs

### Gameplay Function

- sell all fish
- buy/equip rods
- future charms, potions, bait
- player returns here after runs

### Visual Mood

Warm, inviting, useful, slightly cluttered but readable. Gold, teal, purple, wood brown.

### Needed Prefabs

- `ShopkeeperPrototype`
- `ShopCounter`
- `RodDisplay`
- `PotionBottle`
- `FishBucket`
- `VillageDecoration_Lantern`
- `BillboardLabel` sign object

## 3. Training Fishing Dock

### Purpose

A clear place for the player to learn fishing interactions and minigames without pressure.

### Key Props

- short dock
- fishing spot marker
- tutorial sign
- bait bucket
- simple water ripple
- lantern
- training target post

### Gameplay Function

- test fishing minigame
- catch first fish
- teach E interaction
- future tutorial prompts

### Visual Mood

Clean, readable, close to village pond, friendly blue water and warm dock lighting.

### Needed Prefabs

- `FishingSpotPrototype`
- `DockSection`
- `FishingSign`
- `FishBucket`
- `VillageDecoration_Lantern`
- `LilyPad`

## 4. Enchanted River Path

### Purpose

A light transition area between the safe village and more dangerous areas.

### Key Props

- curving river strip
- stepping stones
- mushrooms
- crystals
- broken fence
- small bridge
- goblin camp props
- reeds and rocks

### Gameplay Function

- introduces mob area
- test mob spawning
- test knockback/damage
- future hazards and gathering nodes

### Visual Mood

Still magical, but a little wilder. More shadows, more mushrooms, more purple/green accents.

### Needed Prefabs

- `MobSpawnAreaPrototype`
- `Mob_PondGoblin`
- `VillageDecoration_Mushroom`
- `VillageDecoration_Crystal`
- `DockSection` broken variant
- `Frog`
- `FishingSign` warning variant

## 5. Moonlit Boss Swamp

### Purpose

The darker boss arena where the giant magical Black Drum fish encounter happens.

### Key Props

- dark circular pond
- boss arena marker
- moonlit arch
- glowing rune stones
- broken dock pieces
- blue fog
- swamp bubbles
- dead tree silhouettes
- giant dark fish silhouette marker

### Gameplay Function

- boss fish encounter
- tension/capture minigame
- run climax
- future boss rewards

### Visual Mood

Moonlit, mysterious, darker blue/black water, cyan/violet glow, high contrast boss highlights.

### Needed Prefabs

- `BossFishSpotPrototype`
- `BossArenaMarker`
- `VillageDecoration_Crystal`
- `VillageDecoration_Lantern` cold color variant
- `DockSection` broken variant
- `MaterialPulse` glowing rune stones
- `BillboardLabel` optional boss gate label
