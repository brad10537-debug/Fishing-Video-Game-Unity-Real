# Wizard Pond Visual Style Guide

Wizard Pond is a cozy low-poly / slightly pixel-stylized 3D fantasy fishing roguelite. The goal is not realism. The goal is readable shapes, warm atmosphere, magical color, and a playful wizard fishing identity.

## Overall Art Direction

- Low-poly 3D forms with chunky silhouettes.
- Pixel-inspired color blocking and simple materials.
- Cozy fantasy village mood with magical fishing weirdness.
- Bright starter areas, moodier boss areas.
- Props should be readable from a distance.
- Avoid tiny realism details until the prototype loop feels good.

Visual keywords:

- cozy
- magical pond
- funky wizard village
- low-poly
- pixel-inspired
- colorful
- readable
- playful
- slightly spooky for boss areas

## Color Palette Notes

Starter pond / village:

- Grass: moss green, spring green, soft teal
- Water: cyan, blue, turquoise
- Wood: warm brown, amber
- Magic accents: violet, blue, cyan, magenta
- Lanterns: warm yellow/gold
- Mushrooms: purple, red, teal, pale cream stems
- Wizard clothing: bold purple, blue, green, gold trim

Shop/vendor area:

- Cloth: golden yellow, wine red, teal
- Wood: brown, dark oak
- Item accents: copper, crystal blue, potion pink, glowing green

Boss swamp:

- Water: dark blue, black-green, moonlit cyan
- Fog: muted blue-gray
- Magic accents: violet, cold blue, ghostly cyan
- Boss fish: dark charcoal/black body, silver-blue highlights, glowing eyes, purple/cyan runes

Enemy/hazard areas:

- Ground: muddy green, dull blue, dark moss
- Creatures: swamp green, purple, sickly cyan accents
- Hazards: warning orange, red-magenta, toxic green

## Lighting Direction

Starter village:

- Soft daylight or golden evening.
- Warm lantern pools near paths and shop.
- Slight blue/cyan glow near fishing water.
- Low contrast; friendly and readable.

Fishing run:

- Slightly stronger magical water glow.
- Clear silhouettes for fishing spots and interactables.

Boss swamp:

- Moonlit directional light.
- Low fog.
- Darker background with glowing arena markers.
- Boss should have readable highlights so it does not vanish into the water.

Unity implementation:

- Use one Directional Light.
- Use simple ambient color through `LowPolyPresentationSettings`.
- Add Point Lights only to major lanterns/portals.
- Keep shadows soft or disabled for performance.

## Environment Prop List

Starter pond:

- round pond plane or flattened cylinder
- lily pads
- frogs
- turtles
- cattails/reeds
- fish bucket
- fishing signs
- dock sections
- small rowboat placeholder
- glowing pond stones

Wizard village:

- crooked wizard hut
- shop stall
- wardrobe tent placeholder
- upgrade table
- notice board
- lantern posts
- potion crates
- rod racks
- mushroom clusters
- crystal clusters

River/path:

- curved dirt path
- stepping stones
- broken fence
- small bridge
- river rocks
- glowing mushrooms
- simple goblin camp props

Boss swamp:

- dark pond arena
- crooked dead trees
- glowing swamp bubbles
- moonlit arch
- boss arena marker
- broken dock
- rune stones
- large fish silhouette marker

## UI / HUD Style Direction

The HUD should be readable and calm.

- Use simple rectangular panels with dark translucent backgrounds.
- Use white or pale yellow text.
- Use cyan/purple accents for magical information.
- Use gold accents for currency/reward values.
- Use rarity colors for fish catches:
  - Common: soft white/gray
  - Uncommon: green
  - Rare: blue/cyan
  - Epic: purple/magenta
  - Legendary: orange/gold
  - Mythic: rainbow/neon/glowing

HUD priority:

1. health
2. stamina
3. prompt
4. current area
5. run timer
6. minigame instructions
7. catch reward popup
8. gold and level

Catch popup direction:

- Show fish name large.
- Show rarity in color.
- Show weight, magic power, XP, and sell value.
- Keep flavor text short and fun.

## Character / NPC Style

Player wizard:

- capsule body for now
- wide hat
- bold robe color
- simple belt/sash
- fishing rod on side or in hand later

NPCs:

- one clear color theme per NPC
- exaggerated hats or silhouettes
- simple floating name label optional
- shopkeeper should have counter, bait bucket, rods, and warm lantern

NPC examples:

- Froggin: green/yellow bait merchant near pond dock
- Shellbert: teal/brown turtle sage near rocks
- Mira Moonhook: blue/purple rod upgrader
- Glimmer Gob: magenta/cyan cosmetic vendor
- Old Man Ripple: gray/blue lore NPC near campfire

## Fish Style

Normal magical fish:

- simple low-poly body
- exaggerated fins
- bright color accents
- rarity glow color
- optional sparkle or rune marks later

Prototype first:

- Use text rewards first.
- Use colored fish icons or simple primitives later.
- Avoid full fish models until minigames and economy feel good.

Fish examples:

- Moonfin: blue/cyan, crescent fin
- Crystal Carp: transparent blue/purple crystal accents
- Frogscale Bass: green spots, chunky body
- Turtleback Trout: shell-like back ridge
- Rainbow Rune Eel: long neon body with rune marks

## Boss Fish Style

Boss fish should feel huge, magical, and readable.

Black Drum boss direction:

- giant dark charcoal fish
- heavy rounded body
- silver-blue moonlit edge highlights
- glowing violet/cyan eyes
- magical runes or spots along the side
- creates ripples, bubbles, and fog in arena

Boss arena:

- darker water
- moon reflection
- glowing crystals/rune stones
- broken dock pieces
- a big circular arena marker

Prototype first:

- Use a large dark fish-shaped placeholder or capsule/sphere shape.
- Use `MaterialPulse` for glowing marks.
- Use boss UI sliders for tension/capture.

## Shop / Vendor Style

Shop area should feel warm, useful, and cozy.

Props:

- shop counter
- rod display
- fish bucket
- bait jars
- potion bottles
- charms hanging from string
- lanterns
- small price signs

Color:

- warm yellow lantern light
- wood brown
- teal/purple magical cloth
- gold highlights for shop value

Prototype first:

- Use cubes for counters.
- Use cylinders for potion bottles.
- Use thin cylinders/cubes for rods.
- Use small signs with `BillboardLabel` if needed.

## Beginner-Friendly Unity Implementation Notes

Use primitives first:

- cubes for huts, counters, docks, signs
- cylinders for posts, bottles, mushrooms, lanterns
- spheres for frogs, turtles, bubbles, crystals
- planes or flattened cubes for water/ground

Helpful scripts:

- `VillageDecorationMotion`: spin/bob crystals, lanterns, portal bits.
- `MaterialPulse`: pulse glowing mushrooms/crystals/arena markers.
- `BillboardLabel`: make world-space labels face the camera.
- `LowPolyPresentationSettings`: simple camera background, fog, ambient color, sun intensity.

Material tips:

- Use flat colors.
- Avoid detailed textures early.
- If using pixel textures, set Filter Mode to Point.
- Use emission only on important magical props.

Do not overbuild yet:

- no asset store dependency
- no complex shaders
- no huge terrain system
- no advanced animation controller
- no procedural world generation
