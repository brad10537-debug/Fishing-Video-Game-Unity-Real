using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Editor-only helper that builds a visually rich, structured Starter Pond prototype scene.
// Safe to run repeatedly: existing GameObjects are reused/updated, and only missing objects/components are added.
public static class StarterPondSceneBuilder
{
    private const string InteractableLayerName = "Interactable";

    [MenuItem("Tools/Fishing Game/Build Starter Pond Prototype")]
    public static void BuildStarterPondPrototype()
    {
        Debug.Log("Building Wizard Pond Starter Pond Prototype scene setup...");

        int interactableLayer = EnsureLayer(InteractableLayerName);

        // 1. Core Scene Elements
        GameObject ground = BuildGround();
        GameObject pond = BuildPond();
        GameObject dock = BuildDock();

        // 2. Gameplay & Systems
        GameObject player = BuildPlayer(interactableLayer);
        GameObject fishingSystems = BuildFishingSystems();
        GameObject audioFeedback = BuildAudioFeedback();
        PrototypeHud hud = BuildHudCanvas(player);
        GameObject fishingSpot = BuildFishingSpot(interactableLayer);
        GameObject shopkeeper = BuildShopkeeper(interactableLayer);
        GameObject validator = BuildValidator();

        // 3. Decorative & Environmental Polish
        BuildEnvironmentDecorations();
        SetupLighting();

        // 4. Wiring References
        WirePlayerReferences(player, hud, interactableLayer);
        WireHudReferences(hud, player);
        DisableLegacyLeftClickFishingInStarterScene();

        // 5. Final Setup
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        Selection.activeGameObject = player;

        Debug.Log("Starter Pond Prototype setup complete. Created/updated: "
            + ground.name + ", " + pond.name + ", " + dock.name + ", " + player.name + ", " 
            + fishingSystems.name + ", " + audioFeedback.name + ", " + hud.name + ", " + fishingSpot.name + ", " 
            + shopkeeper.name + ", " + validator.name + " with rich prototype decorations.");
        Debug.Log("Starter Pond Prototype ready: walk to Starter Fishing Spot and press E.");
        Debug.Log("Game audio feedback ready: optional clips can be assigned on Game Audio Feedback, or generated placeholder tones will play.");
        Debug.Log("Legacy left-click bobber casting is kept as an alternate/future system, but this builder disables it for the Starter Pond Prototype test path.");
    }

    private static GameObject BuildGround()
    {
        GameObject ground = GetOrCreateRoot("Ground");
        ground.transform.position = Vector3.zero;
        ground.transform.rotation = Quaternion.identity;
        ground.transform.localScale = new Vector3(40f, 1f, 40f);

        // Add MeshFilter and MeshRenderer if they don't exist
        var filter = AddOrGet<MeshFilter>(ground);
        if (filter.sharedMesh == null)
        {
            GameObject tempCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            filter.sharedMesh = tempCube.GetComponent<MeshFilter>().sharedMesh;
            Object.DestroyImmediate(tempCube);
        }

        AddOrGet<MeshRenderer>(ground);
        AddOrGet<BoxCollider>(ground);

        Material grassMat = GetOrCreateMaterial("Proto_Grass", new Color(0.25f, 0.65f, 0.35f));
        SetMaterial(ground, grassMat);

        return ground;
    }

    private static GameObject BuildPond()
    {
        GameObject pond = GetOrCreateRoot("Pond");
        pond.transform.position = new Vector3(0f, 0.51f, 5f);
        pond.transform.rotation = Quaternion.identity;
        pond.transform.localScale = new Vector3(15f, 0.02f, 15f);

        var filter = AddOrGet<MeshFilter>(pond);
        if (filter.sharedMesh == null)
        {
            GameObject tempCylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            filter.sharedMesh = tempCylinder.GetComponent<MeshFilter>().sharedMesh;
            Object.DestroyImmediate(tempCylinder);
        }

        AddOrGet<MeshRenderer>(pond);
        
        // Remove existing collider on the pond so the player doesn't stand on top of water
        var collider = pond.GetComponent<Collider>();
        if (collider != null)
        {
            Object.DestroyImmediate(collider);
        }

        Material waterMat = GetOrCreateMaterial("Proto_Water", new Color(0.1f, 0.5f, 0.85f, 0.65f), 0.1f, 0.2f, true);
        SetMaterial(pond, waterMat);

        return pond;
    }

    private static GameObject BuildDock()
    {
        GameObject dock = GetOrCreateRoot("Wooden Dock");
        dock.transform.position = new Vector3(0f, 0.5f, -1f);
        dock.transform.rotation = Quaternion.identity;
        dock.transform.localScale = Vector3.one;

        // Build simple dock planks using children
        GameObject mainPlank = GetOrCreateChild(dock.transform, "Main Plank");
        mainPlank.transform.localPosition = new Vector3(0f, 0.1f, -1.5f);
        mainPlank.transform.localRotation = Quaternion.identity;
        mainPlank.transform.localScale = new Vector3(2.5f, 0.15f, 5f);
        AddPrimitiveMeshAndCollider(mainPlank, PrimitiveType.Cube);

        GameObject supportLeft = GetOrCreateChild(dock.transform, "Support Left");
        supportLeft.transform.localPosition = new Vector3(-1.1f, -0.4f, -0.5f);
        supportLeft.transform.localScale = new Vector3(0.2f, 1f, 0.2f);
        AddPrimitiveMeshAndCollider(supportLeft, PrimitiveType.Cylinder);

        GameObject supportRight = GetOrCreateChild(dock.transform, "Support Right");
        supportRight.transform.localPosition = new Vector3(1.1f, -0.4f, -0.5f);
        supportRight.transform.localScale = new Vector3(0.2f, 1f, 0.2f);
        AddPrimitiveMeshAndCollider(supportRight, PrimitiveType.Cylinder);

        Material woodMat = GetOrCreateMaterial("Proto_Wood", new Color(0.55f, 0.38f, 0.22f), 0.9f);
        SetMaterial(mainPlank, woodMat);
        SetMaterial(supportLeft, woodMat);
        SetMaterial(supportRight, woodMat);

        return dock;
    }

    private static GameObject BuildPlayer(int interactableLayer)
    {
        GameObject player = GetOrCreateRoot("Player");
        // Spawn the player at the start of the dock facing the pond
        player.transform.position = new Vector3(0f, 1.2f, -3.5f);
        player.transform.rotation = Quaternion.identity;

        CharacterController characterController = AddOrGet<CharacterController>(player);
        characterController.height = 1.8f;
        characterController.radius = 0.35f;
        characterController.center = new Vector3(0f, 0.9f, 0f);

        AddOrGet<FirstPersonPrototypeController>(player);
        AddOrGet<PlayerInteractor>(player);
        LivingEntity entity = AddOrGet<LivingEntity>(player);
        AddOrGet<SimpleInventory>(player);
        AddOrGet<PlayerCurrency>(player);
        AddOrGet<PlayerProgression>(player);
        AddOrGet<PlayerFishingRod>(player);
        AddOrGet<WizardStats>(player);
        AddOrGet<WizardGear>(player);
        FishJournal journal = AddOrGet<FishJournal>(player);
        journal.ResetToStarterEntries();

        CharacterIdentity identity = AddOrGet<CharacterIdentity>(player);
        identity.displayName = "Young Pond Wizard";
        identity.characterType = CharacterType.Player;
        identity.faction = FactionTeam.Player;
        identity.titleOrRole = "Apprentice Angler";

        entity.maxHealth = 100f;
        entity.health = Mathf.Clamp(entity.health, 1f, entity.maxHealth);
        entity.maxStamina = 100f;
        entity.stamina = Mathf.Clamp(entity.stamina, 1f, entity.maxStamina);
        entity.movementSpeed = 5f;
        entity.sprintSpeed = 8f;

        GameObject cameraObject = GetOrCreateChild(player.transform, "Player Camera");
        cameraObject.transform.localPosition = new Vector3(0f, 1.6f, 0f);
        cameraObject.transform.localRotation = Quaternion.identity;
        Camera camera = AddOrGet<Camera>(cameraObject);
        camera.tag = "MainCamera";

        if (cameraObject.GetComponent<AudioListener>() == null)
        {
            AddOrGet<AudioListener>(cameraObject);
        }

        if (interactableLayer >= 0)
        {
            player.layer = LayerMask.NameToLayer("Default");
        }

        return player;
    }

    private static GameObject BuildFishingSystems()
    {
        GameObject systems = GetOrCreateRoot("Fishing Systems");
        AddOrGet<FishingMinigameManager>(systems);
        AddOrGet<FishingDifficultyManager>(systems);
        return systems;
    }

    private static GameObject BuildAudioFeedback()
    {
        GameObject audioObject = GetOrCreateRoot("Game Audio Feedback");
        AudioSource source = AddOrGet<AudioSource>(audioObject);
        source.playOnAwake = false;
        source.spatialBlend = 0f;
        source.volume = 1f;

        GameAudioFeedback feedback = AddOrGet<GameAudioFeedback>(audioObject);
        feedback.volume = 0.35f;
        feedback.useGeneratedPlaceholderTones = true;
        return audioObject;
    }

    private static PrototypeHud BuildHudCanvas(GameObject player)
    {
        GameObject canvasObject = GetOrCreateRoot("HUD Canvas");
        Canvas canvas = AddOrGet<Canvas>(canvasObject);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = AddOrGet<CanvasScaler>(canvasObject);
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        AddOrGet<GraphicRaycaster>(canvasObject);
        PrototypeHud hud = AddOrGet<PrototypeHud>(canvasObject);

        Font font = GetBuiltinFont();
        hud.popupText = CreatePopupText(canvasObject.transform, "Central Popup Text", font);
        hud.popupCanvasGroup = hud.popupText.GetComponent<CanvasGroup>();
        hud.popupRectTransform = hud.popupText.GetComponent<RectTransform>();

        hud.promptText = CreateText(canvasObject.transform, "Prompt Text", "Look at Starter Fishing Spot and press E to fish.", 0, font);
        hud.statusText = CreateText(canvasObject.transform, "Status Text", "Starter Pond Prototype ready. Use E on the fishing spot.", 1, font);
        hud.goldText = CreateText(canvasObject.transform, "Coins Text", "Coins: 0", 2, font);
        hud.playerLevelText = CreateText(canvasObject.transform, "Level Text", "Level: 1", 3, font);
        hud.xpText = CreateText(canvasObject.transform, "XP Text", "XP: 0 / 100", 4, font);
        hud.equippedRodText = CreateText(canvasObject.transform, "Rod Text", "Rod: Lv.1 Twig Wand Rod", 5, font);
        hud.fishCountText = CreateText(canvasObject.transform, "Fish Count Text", "Fish: 0", 6, font);
        hud.lastCatchText = CreateText(canvasObject.transform, "Last Catch Text", "Last Catch: None", 7, font, new Vector2(520f, 110f));
        hud.shopMenuText = CreateText(canvasObject.transform, "Shop Menu Text", "", 12, font, new Vector2(720f, 220f));

        hud.minigameTitleText = CreateText(canvasObject.transform, "Minigame Title Text", "Fishing: Idle", 20, font);
        hud.minigameInstructionsText = CreateText(canvasObject.transform, "Minigame Instructions Text", "", 21, font, new Vector2(720f, 32f));
        hud.minigamePromptText = CreateText(canvasObject.transform, "Minigame Prompt Text", "", 22, font);
        hud.minigameProgressText = CreateText(canvasObject.transform, "Minigame Progress Text", "", 23, font, new Vector2(720f, 60f));
        hud.minigameResultText = CreateText(canvasObject.transform, "Minigame Result Text", "", 25, font, new Vector2(720f, 40f));
        hud.minigameProgressSlider = CreateSlider(canvasObject.transform, "Minigame Progress Slider", 24);

        hud.playerHealthText = CreateText(canvasObject.transform, "Health Text", "Health: 100 / 100", 31, font);
        hud.playerStaminaText = CreateText(canvasObject.transform, "Stamina Text", "Stamina: 100 / 100", 32, font);
        hud.movementStateText = CreateText(canvasObject.transform, "Movement Text", "Movement: Idle", 33, font);
        hud.currentAreaText = CreateText(canvasObject.transform, "Area Text", "Area: Starter Pond", 34, font);
        CreateText(canvasObject.transform, "Help Text", "J: Fish Journal", 35, font, new Vector2(300f, 26f));

        BuildFishJournalUi(canvasObject.transform, font, player);

        return hud;
    }

    private static void BuildFishJournalUi(Transform canvasTransform, Font font, GameObject player)
    {
        GameObject panel = GetOrCreateUiChild(canvasTransform, "Fish Journal Panel");
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(1f, 0.5f);
        panelRect.anchorMax = new Vector2(1f, 0.5f);
        panelRect.pivot = new Vector2(1f, 0.5f);
        panelRect.anchoredPosition = new Vector2(-30f, 0f);
        panelRect.sizeDelta = new Vector2(520f, 720f);

        Image panelImage = AddOrGet<Image>(panel);
        panelImage.color = new Color(0.03f, 0.05f, 0.08f, 0.88f);
        CanvasGroup panelCanvasGroup = AddOrGet<CanvasGroup>(panel);
        panelCanvasGroup.alpha = 1f;
        panelCanvasGroup.interactable = false;
        panelCanvasGroup.blocksRaycasts = false;

        GameObject textObject = GetOrCreateUiChild(panel.transform, "Fish Journal Text");
        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0f, 0f);
        textRect.anchorMax = new Vector2(1f, 1f);
        textRect.offsetMin = new Vector2(24f, 24f);
        textRect.offsetMax = new Vector2(-24f, -24f);

        Text journalText = AddOrGet<Text>(textObject);
        journalText.font = font;
        journalText.fontSize = 18;
        journalText.alignment = TextAnchor.UpperLeft;
        journalText.color = Color.white;
        journalText.horizontalOverflow = HorizontalWrapMode.Wrap;
        journalText.verticalOverflow = VerticalWrapMode.Overflow;
        journalText.raycastTarget = false;

        FishJournalUi journalUi = AddOrGet<FishJournalUi>(canvasTransform.gameObject);
        journalUi.journalPanel = panel;
        journalUi.journalText = journalText;
        journalUi.journalCanvasGroup = panelCanvasGroup;
        journalUi.journalRectTransform = panelRect;
        journalUi.journal = player.GetComponent<FishJournal>();
        panel.SetActive(false);
    }

    private static GameObject BuildFishingSpot(int interactableLayer)
    {
        // Reuse or create the Starter Fishing Spot
        GameObject spot = GetOrCreateRoot("Starter Fishing Spot");
        
        // Position it at the edge of the pond
        spot.transform.position = new Vector3(0f, 0.7f, 1.8f);
        spot.transform.localScale = Vector3.one;

        // Visual presentation for the fishing spot (bright colored floating marker)
        GameObject visual = GetOrCreateChild(spot.transform, "Visual Marker");
        visual.transform.localPosition = Vector3.zero;
        visual.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
        AddPrimitiveMeshAndCollider(visual, PrimitiveType.Cube);
        
        // Disable the collider on the visual so it doesn't block rays, let the parent trigger handle it
        var visualCollider = visual.GetComponent<Collider>();
        if (visualCollider != null) Object.DestroyImmediate(visualCollider);

        Material glowingSpotMat = GetOrCreateMaterial("Proto_GlowingSpot", new Color(0.2f, 0.9f, 1f, 0.8f), 0.1f, 0f, true, new Color(0.1f, 0.45f, 0.5f) * 1.5f);
        SetMaterial(visual, glowingSpotMat);

        // Attach pulse/bob animation for visual feedback
        var motion = AddOrGet<VillageDecorationMotion>(visual);
        motion.spin = true;
        motion.bob = true;
        motion.bobHeight = 0.12f;
        motion.bobSpeed = 2.5f;

        BoxCollider collider = AddOrGet<BoxCollider>(spot);
        collider.size = new Vector3(2f, 1.2f, 2f);
        collider.isTrigger = true;

        BuildStarterFishingSpotLabel(spot);

        FishingSpotInteractable fishingSpot = AddOrGet<FishingSpotInteractable>(spot);
        fishingSpot.spotName = "Starter Pond";
        fishingSpot.fishingTime = 1f;
        fishingSpot.awardCoinsImmediately = true;
        fishingSpot.speciesPool = FishingSpotInteractable.CreateStarterSpeciesPool();
        fishingSpot.feedbackVisual = visual.transform;

        ApplyLayer(spot, interactableLayer);
        return spot;
    }

    private static void BuildStarterFishingSpotLabel(GameObject spot)
    {
        GameObject label = GetOrCreateChild(spot.transform, "Starter Fishing Spot Label");
        label.transform.localPosition = new Vector3(0f, 1.2f, 0f);
        label.transform.localRotation = Quaternion.identity;
        label.transform.localScale = new Vector3(0.14f, 0.14f, 0.14f);

        TextMesh textMesh = AddOrGet<TextMesh>(label);
        textMesh.text = "Starter Fishing Spot\nPress E";
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        textMesh.characterSize = 1f;
        textMesh.fontSize = 48;
        textMesh.color = new Color(0.7f, 1f, 1f, 1f);

        AddOrGet<MeshRenderer>(label);
        BillboardLabel billboard = AddOrGet<BillboardLabel>(label);
        billboard.lockYRotationOnly = true;
    }

    private static GameObject BuildShopkeeper(int interactableLayer)
    {
        GameObject shopkeeper = GetOrCreateRoot("Shopkeeper");
        shopkeeper.transform.position = new Vector3(4.5f, 1f, -2.5f);
        shopkeeper.transform.rotation = Quaternion.Euler(0f, -45f, 0f);

        CapsuleCollider collider = AddOrGet<CapsuleCollider>(shopkeeper);
        collider.height = 2f;
        collider.radius = 0.4f;
        collider.center = new Vector3(0f, 1f, 0f);
        collider.isTrigger = false;

        CharacterIdentity identity = AddOrGet<CharacterIdentity>(shopkeeper);
        identity.displayName = "Mordo the Rodmancer";
        identity.characterType = CharacterType.Shopkeeper;
        identity.faction = FactionTeam.Village;
        identity.titleOrRole = "Rod Upgrade Shopkeeper";

        ShopKeeperInteractable shop = AddOrGet<ShopKeeperInteractable>(shopkeeper);
        shop.shopkeeperName = "Mordo the Rodmancer";

        // Build a physical shop stall using children
        GameObject shopStall = GetOrCreateChild(shopkeeper.transform, "Shop Stall");
        shopStall.transform.localPosition = new Vector3(0f, 0f, 0.8f);
        shopStall.transform.localRotation = Quaternion.identity;
        shopStall.transform.localScale = Vector3.one;

        GameObject counter = GetOrCreateChild(shopStall.transform, "Counter");
        counter.transform.localPosition = new Vector3(0f, 0.5f, 0f);
        counter.transform.localScale = new Vector3(2.2f, 1f, 0.8f);
        AddPrimitiveMeshAndCollider(counter, PrimitiveType.Cube);

        GameObject pillarLeft = GetOrCreateChild(shopStall.transform, "Pillar Left");
        pillarLeft.transform.localPosition = new Vector3(-1f, 1.5f, 0f);
        pillarLeft.transform.localScale = new Vector3(0.12f, 2f, 0.12f);
        AddPrimitiveMeshAndCollider(pillarLeft, PrimitiveType.Cylinder);

        GameObject pillarRight = GetOrCreateChild(shopStall.transform, "Pillar Right");
        pillarRight.transform.localPosition = new Vector3(1f, 1.5f, 0f);
        pillarRight.transform.localScale = new Vector3(0.12f, 2f, 0.12f);
        AddPrimitiveMeshAndCollider(pillarRight, PrimitiveType.Cylinder);

        GameObject roof = GetOrCreateChild(shopStall.transform, "Roof");
        roof.transform.localPosition = new Vector3(0f, 2.5f, 0f);
        roof.transform.localScale = new Vector3(2.4f, 0.15f, 1f);
        AddPrimitiveMeshAndCollider(roof, PrimitiveType.Cube);

        // Materials for shop
        Material counterMat = GetOrCreateMaterial("Proto_ShopCounter", new Color(0.42f, 0.28f, 0.15f));
        Material roofMat = GetOrCreateMaterial("Proto_ShopRoof", new Color(0.85f, 0.25f, 0.3f));
        SetMaterial(counter, counterMat);
        SetMaterial(pillarLeft, counterMat);
        SetMaterial(pillarRight, counterMat);
        SetMaterial(roof, roofMat);

        // Shop items/decorations on counter
        GameObject potion = GetOrCreateChild(counter.transform, "Potion Flask");
        potion.transform.localPosition = new Vector3(-0.25f, 0.65f, 0f);
        potion.transform.localScale = new Vector3(0.15f, 0.3f, 0.15f);
        AddPrimitiveMeshAndCollider(potion, PrimitiveType.Cylinder);
        Material potionMat = GetOrCreateMaterial("Proto_Potion", new Color(0.9f, 0.1f, 0.7f, 0.85f), 0.1f, 0f, true, new Color(0.45f, 0.05f, 0.35f));
        SetMaterial(potion, potionMat);

        GameObject rodItem = GetOrCreateChild(counter.transform, "Upgrade Rod");
        rodItem.transform.localPosition = new Vector3(0.2f, 0.6f, 0f);
        rodItem.transform.localRotation = Quaternion.Euler(0f, 0f, -60f);
        rodItem.transform.localScale = new Vector3(0.06f, 1.2f, 0.06f);
        AddPrimitiveMeshAndCollider(rodItem, PrimitiveType.Cylinder);
        Material rodMat = GetOrCreateMaterial("Proto_ShopRod", new Color(0.85f, 0.65f, 0.2f), 0.2f, 0.5f);
        SetMaterial(rodItem, rodMat);

        // Sign text label "Rod Shop"
        GameObject signBoard = GetOrCreateChild(shopStall.transform, "Rod Shop Sign");
        signBoard.transform.localPosition = new Vector3(0f, 2.1f, -0.55f);
        signBoard.transform.localRotation = Quaternion.identity;
        signBoard.transform.localScale = new Vector3(1.2f, 0.4f, 0.1f);
        AddPrimitiveMeshAndCollider(signBoard, PrimitiveType.Cube);
        SetMaterial(signBoard, counterMat);
        BuildShopkeeperLabel(shopkeeper);

        ApplyLayer(shopkeeper, interactableLayer);
        return shopkeeper;
    }

    private static void BuildShopkeeperLabel(GameObject shopkeeper)
    {
        GameObject label = GetOrCreateChild(shopkeeper.transform, "Rod Shop Label");
        label.transform.localPosition = new Vector3(0f, 2.75f, 0f);
        label.transform.localRotation = Quaternion.identity;
        label.transform.localScale = new Vector3(0.14f, 0.14f, 0.14f);

        TextMesh textMesh = AddOrGet<TextMesh>(label);
        textMesh.text = "Rod Shop\nMordo";
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        textMesh.characterSize = 1f;
        textMesh.fontSize = 48;
        textMesh.color = new Color(1f, 0.9f, 0.35f, 1f);

        AddOrGet<MeshRenderer>(label);
        BillboardLabel billboard = AddOrGet<BillboardLabel>(label);
        billboard.lockYRotationOnly = true;
    }

    private static void BuildEnvironmentDecorations()
    {
        GameObject decoRoot = GetOrCreateRoot("Prototype Decorations");
        decoRoot.transform.position = Vector3.zero;
        decoRoot.transform.rotation = Quaternion.identity;
        decoRoot.transform.localScale = Vector3.one;

        // 1. Lily Pads
        Vector3[] lilyPadPositions = {
            new Vector3(-2f, 0.53f, 3.5f),
            new Vector3(1.8f, 0.53f, 2.5f),
            new Vector3(-0.8f, 0.53f, 5.2f),
            new Vector3(3.2f, 0.53f, 4.8f)
        };
        for (int i = 0; i < lilyPadPositions.Length; i++)
        {
            GameObject lily = GetOrCreateChild(decoRoot.transform, $"Lily Pad {i}");
            lily.transform.position = lilyPadPositions[i];
            lily.transform.localScale = new Vector3(0.8f, 0.01f, 0.8f);
            AddPrimitiveMeshAndCollider(lily, PrimitiveType.Cylinder);
            Material lilyMat = GetOrCreateMaterial("Proto_LilyPad", new Color(0.18f, 0.55f, 0.3f));
            SetMaterial(lily, lilyMat);
        }

        // 2. Glowing Mushrooms
        Vector3[] shroomPositions = {
            new Vector3(-3.5f, 0.5f, -1.5f),
            new Vector3(3.8f, 0.5f, -0.8f),
            new Vector3(5.5f, 0.5f, -4.2f)
        };
        Color[] shroomColors = {
            new Color(0.9f, 0.3f, 0.9f),
            new Color(0.2f, 0.85f, 0.9f),
            new Color(0.95f, 0.45f, 0.2f)
        };
        for (int i = 0; i < shroomPositions.Length; i++)
        {
            GameObject shroom = GetOrCreateChild(decoRoot.transform, $"Glowing Mushroom {i}");
            shroom.transform.position = shroomPositions[i];
            shroom.transform.localScale = Vector3.one;

            GameObject stem = GetOrCreateChild(shroom.transform, "Stem");
            stem.transform.localPosition = new Vector3(0f, 0.25f, 0f);
            stem.transform.localScale = new Vector3(0.15f, 0.5f, 0.15f);
            AddPrimitiveMeshAndCollider(stem, PrimitiveType.Cylinder);
            Material stemMat = GetOrCreateMaterial("Proto_ShroomStem", new Color(0.9f, 0.88f, 0.8f));
            SetMaterial(stem, stemMat);

            GameObject cap = GetOrCreateChild(shroom.transform, "Cap");
            cap.transform.localPosition = new Vector3(0f, 0.5f, 0f);
            cap.transform.localScale = new Vector3(0.45f, 0.3f, 0.45f);
            AddPrimitiveMeshAndCollider(cap, PrimitiveType.Sphere);
            Material capMat = GetOrCreateMaterial($"Proto_ShroomCap_{i}", shroomColors[i], 0.2f, 0f, false, shroomColors[i] * 0.8f);
            SetMaterial(cap, capMat);

            var pulse = AddOrGet<MaterialPulse>(cap);
            pulse.baseColor = shroomColors[i] * 0.7f;
            pulse.pulseColor = shroomColors[i] * 1.3f;
            pulse.pulseSpeed = 1.5f + i * 0.4f;
        }

        // 3. Frog Placeholder
        GameObject frog = GetOrCreateChild(decoRoot.transform, "Frog Placeholder");
        frog.transform.position = new Vector3(-2f, 0.58f, 3.5f);
        frog.transform.localScale = new Vector3(0.25f, 0.15f, 0.25f);
        AddPrimitiveMeshAndCollider(frog, PrimitiveType.Sphere);
        Material frogMat = GetOrCreateMaterial("Proto_Frog", new Color(0.4f, 0.8f, 0.2f));
        SetMaterial(frog, frogMat);

        // 4. Turtle Placeholder
        GameObject turtle = GetOrCreateChild(decoRoot.transform, "Turtle Placeholder");
        turtle.transform.position = new Vector3(1.8f, 0.58f, 2.5f);
        turtle.transform.localScale = new Vector3(0.4f, 0.18f, 0.5f);
        AddPrimitiveMeshAndCollider(turtle, PrimitiveType.Sphere);
        Material turtleMat = GetOrCreateMaterial("Proto_Turtle", new Color(0.25f, 0.45f, 0.2f));
        SetMaterial(turtle, turtleMat);

        // 5. Fishing Sign
        GameObject sign = GetOrCreateChild(decoRoot.transform, "Fishing Sign");
        sign.transform.position = new Vector3(-1.8f, 0.5f, -0.6f);
        sign.transform.rotation = Quaternion.Euler(0f, 45f, 0f);
        sign.transform.localScale = Vector3.one;

        GameObject post = GetOrCreateChild(sign.transform, "Post");
        post.transform.localPosition = new Vector3(0f, 0.6f, 0f);
        post.transform.localScale = new Vector3(0.1f, 1.2f, 0.1f);
        AddPrimitiveMeshAndCollider(post, PrimitiveType.Cylinder);

        GameObject board = GetOrCreateChild(sign.transform, "Board");
        board.transform.localPosition = new Vector3(0f, 1.1f, 0f);
        board.transform.localScale = new Vector3(0.7f, 0.4f, 0.08f);
        AddPrimitiveMeshAndCollider(board, PrimitiveType.Cube);

        Material signWoodMat = GetOrCreateMaterial("Proto_SignWood", new Color(0.4f, 0.25f, 0.1f));
        SetMaterial(post, signWoodMat);
        SetMaterial(board, signWoodMat);
    }

    private static void SetupLighting()
    {
        // Configure main Directional Light for clean cozy low-poly presentation
        GameObject sunObj = GameObject.Find("Directional Light");
        if (sunObj == null)
        {
            sunObj = new GameObject("Directional Light");
            Undo.RegisterCreatedObjectUndo(sunObj, "Create Directional Light");
        }

        Light sun = AddOrGet<Light>(sunObj);
        sun.type = LightType.Directional;
        sun.intensity = 1.1f;
        sun.color = new Color(1f, 0.96f, 0.85f); // Soft warm sunshine
        sun.shadows = LightShadows.Soft;

        // Position/orient sun optimally
        sunObj.transform.position = new Vector3(0f, 20f, 0f);
        sunObj.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

        // Warm cozy point lights near the shop and dock
        GameObject shopLightGo = GetOrCreateChild(sunObj.transform, "Shop Lantern Point Light");
        shopLightGo.transform.position = new Vector3(4.5f, 2.2f, -1.8f);
        Light shopLight = AddOrGet<Light>(shopLightGo);
        shopLight.type = LightType.Point;
        shopLight.intensity = 1.4f;
        shopLight.range = 5f;
        shopLight.color = new Color(1f, 0.7f, 0.25f); // Amber cozy lantern glow
        shopLight.shadows = LightShadows.None;

        GameObject dockLightGo = GetOrCreateChild(sunObj.transform, "Dock Lantern Point Light");
        dockLightGo.transform.position = new Vector3(-1.5f, 1.4f, -1f);
        Light dockLight = AddOrGet<Light>(dockLightGo);
        dockLight.type = LightType.Point;
        dockLight.intensity = 1.2f;
        dockLight.range = 4f;
        dockLight.color = new Color(1f, 0.75f, 0.3f);
        dockLight.shadows = LightShadows.None;

        // Apply global LowPolyPresentationSettings
        GameObject presentationGo = GameObject.Find("Village Presentation");
        if (presentationGo == null)
        {
            presentationGo = new GameObject("Village Presentation");
            Undo.RegisterCreatedObjectUndo(presentationGo, "Create Village Presentation");
        }
        var presentation = AddOrGet<LowPolyPresentationSettings>(presentationGo);
        presentation.mainCamera = Camera.main;
        presentation.sunLight = sun;
        presentation.useFog = true;
        presentation.fogColor = new Color(0.55f, 0.78f, 0.88f);
        presentation.fogDensity = 0.015f;
        presentation.cameraBackground = new Color(0.5f, 0.75f, 0.85f);
        presentation.ApplyPresentationSettings();
    }

    private static GameObject BuildValidator()
    {
        GameObject validator = GetOrCreateRoot("Prototype Scene Validator");
        PrototypeSceneValidator sceneValidator = AddOrGet<PrototypeSceneValidator>(validator);
        sceneValidator.runOnStart = true;
        sceneValidator.checkOptionalObjects = false; // Bypass optional village objects for focused starter loop
        return validator;
    }

    private static void WirePlayerReferences(GameObject player, PrototypeHud hud, int interactableLayer)
    {
        Camera camera = player.GetComponentInChildren<Camera>();
        FirstPersonPrototypeController controller = player.GetComponent<FirstPersonPrototypeController>();
        PlayerInteractor interactor = player.GetComponent<PlayerInteractor>();
        PlayerProgression progression = player.GetComponent<PlayerProgression>();

        if (controller != null)
        {
            controller.playerCamera = camera;
        }

        if (interactor != null)
        {
            interactor.playerCamera = camera;
            interactor.inventory = player.GetComponent<SimpleInventory>();
            interactor.currency = player.GetComponent<PlayerCurrency>();
            interactor.fishingRod = player.GetComponent<PlayerFishingRod>();
            interactor.progression = player.GetComponent<PlayerProgression>();
            interactor.wizardStats = player.GetComponent<WizardStats>();
            interactor.wizardGear = player.GetComponent<WizardGear>();
            interactor.fishJournal = player.GetComponent<FishJournal>();
            interactor.prototypeHud = hud;
            interactor.movementController = controller;

            if (interactableLayer >= 0)
            {
                interactor.interactableLayers = 1 << interactableLayer;
            }
        }

        if (progression != null)
        {
            progression.currency = player.GetComponent<PlayerCurrency>();
            progression.fishingRod = player.GetComponent<PlayerFishingRod>();
            progression.wizardStats = player.GetComponent<WizardStats>();
        }
    }

    private static void WireHudReferences(PrototypeHud hud, GameObject player)
    {
        if (hud == null || player == null)
        {
            return;
        }

        hud.inventory = player.GetComponent<SimpleInventory>();
        hud.currency = player.GetComponent<PlayerCurrency>();
        hud.fishingRod = player.GetComponent<PlayerFishingRod>();
        hud.progression = player.GetComponent<PlayerProgression>();
        hud.wizardStats = player.GetComponent<WizardStats>();
        hud.wizardGear = player.GetComponent<WizardGear>();
        hud.playerEntity = player.GetComponent<LivingEntity>();
    }

    private static void DisableLegacyLeftClickFishingInStarterScene()
    {
        FirstPersonFishingController[] legacyControllers = Object.FindObjectsByType<FirstPersonFishingController>(FindObjectsInactive.Include);
        foreach (FirstPersonFishingController legacyController in legacyControllers)
        {
            if (legacyController.enabled)
            {
                legacyController.enabled = false;
                Debug.Log("Disabled legacy left-click fishing controller on " + legacyController.gameObject.name
                    + ". Starter prototype uses PlayerInteractor + FishingSpotInteractable with E.");
            }

            legacyController.legacyCastingEnabled = false;
            legacyController.showLegacyStartupHint = false;
        }
    }

    private static GameObject GetOrCreateRoot(string objectName)
    {
        GameObject existing = GameObject.Find(objectName);
        if (existing != null)
        {
            Debug.Log("Reusing existing GameObject: " + objectName);
            return existing;
        }

        GameObject created = new GameObject(objectName);
        Undo.RegisterCreatedObjectUndo(created, "Create " + objectName);
        Debug.Log("Created GameObject: " + objectName);
        return created;
    }

    private static GameObject GetOrCreateChild(Transform parent, string objectName)
    {
        Transform existing = parent.Find(objectName);
        if (existing != null)
        {
            Debug.Log("Reusing child GameObject: " + objectName);
            return existing.gameObject;
        }

        GameObject created = new GameObject(objectName);
        Undo.RegisterCreatedObjectUndo(created, "Create " + objectName);
        created.transform.SetParent(parent, false);
        Debug.Log("Created child GameObject: " + objectName);
        return created;
    }

    private static T AddOrGet<T>(GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponent<T>();
        if (component != null)
        {
            return component;
        }

        component = Undo.AddComponent<T>(gameObject);
        Debug.Log("Added " + typeof(T).Name + " to " + gameObject.name);
        return component;
    }

    private static void AddPrimitiveMeshAndCollider(GameObject target, PrimitiveType type)
    {
        var filter = AddOrGet<MeshFilter>(target);
        if (filter.sharedMesh == null)
        {
            GameObject temp = GameObject.CreatePrimitive(type);
            filter.sharedMesh = temp.GetComponent<MeshFilter>().sharedMesh;
            Object.DestroyImmediate(temp);
        }
        AddOrGet<MeshRenderer>(target);

        if (type == PrimitiveType.Cube) AddOrGet<BoxCollider>(target);
        else if (type == PrimitiveType.Sphere) AddOrGet<SphereCollider>(target);
        else if (type == PrimitiveType.Cylinder) AddOrGet<CapsuleCollider>(target);
    }

    private static void SetMaterial(GameObject obj, Material mat)
    {
        var renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.sharedMaterial = mat;
        }
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            SetMaterial(obj.transform.GetChild(i).gameObject, mat);
        }
    }

    private static Material GetOrCreateMaterial(string name, Color color, float roughness = 0.8f, float metallic = 0f, bool isTransparent = false, Color? emissionColor = null)
    {
        string folderPath = "Assets/Materials";
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder("Assets", "Materials");
        }
        string subFolderPath = "Assets/Materials/Prototype";
        if (!AssetDatabase.IsValidFolder(subFolderPath))
        {
            AssetDatabase.CreateFolder("Assets/Materials", "Prototype");
        }

        string assetPath = $"{subFolderPath}/{name}.mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
        if (mat == null)
        {
            mat = new Material(Shader.Find("Standard"));
            AssetDatabase.CreateAsset(mat, assetPath);
            Debug.Log($"Created material asset: {assetPath}");
        }

        mat.color = color;
        mat.SetFloat("_Glossiness", 1f - roughness);
        mat.SetFloat("_Metallic", metallic);

        if (isTransparent)
        {
            mat.SetFloat("_Mode", 3); // Transparent mode
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000;
        }
        else
        {
            mat.SetFloat("_Mode", 0); // Opaque mode
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            mat.SetInt("_ZWrite", 1);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.DisableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = -1;
        }

        if (emissionColor.HasValue)
        {
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", emissionColor.Value);
        }
        else
        {
            mat.DisableKeyword("_EMISSION");
        }

        EditorUtility.SetDirty(mat);
        return mat;
    }

    private static Text CreateText(Transform parent, string objectName, string text, int row, Font font)
    {
        return CreateText(parent, objectName, text, row, font, new Vector2(520f, 26f));
    }

    private static Text CreateText(Transform parent, string objectName, string text, int row, Font font, Vector2 size)
    {
        GameObject textObject = GetOrCreateUiChild(parent, objectName);
        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0f, 1f);
        rectTransform.anchorMax = new Vector2(0f, 1f);
        rectTransform.pivot = new Vector2(0f, 1f);
        rectTransform.anchoredPosition = new Vector2(20f, -20f - row * 28f);
        rectTransform.sizeDelta = size;

        Text uiText = AddOrGet<Text>(textObject);
        uiText.text = text;
        uiText.font = font;
        uiText.fontSize = 18;
        uiText.alignment = TextAnchor.UpperLeft;
        uiText.color = Color.white;
        uiText.raycastTarget = false;
        return uiText;
    }

    private static Text CreatePopupText(Transform parent, string objectName, Font font)
    {
        GameObject textObject = GetOrCreateUiChild(parent, objectName);
        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = new Vector2(0f, 80f);
        rectTransform.sizeDelta = new Vector2(760f, 260f);

        Text uiText = AddOrGet<Text>(textObject);
        uiText.text = "";
        uiText.font = font;
        uiText.fontSize = 38;
        uiText.fontStyle = FontStyle.Bold;
        uiText.alignment = TextAnchor.MiddleCenter;
        uiText.color = new Color(0.7f, 1f, 0.85f);
        uiText.raycastTarget = false;

        CanvasGroup canvasGroup = AddOrGet<CanvasGroup>(textObject);
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        return uiText;
    }

    private static Slider CreateSlider(Transform parent, string objectName, int row)
    {
        GameObject sliderObject = GetOrCreateUiChild(parent, objectName);
        RectTransform rectTransform = sliderObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0f, 1f);
        rectTransform.anchorMax = new Vector2(0f, 1f);
        rectTransform.pivot = new Vector2(0f, 1f);
        rectTransform.anchoredPosition = new Vector2(20f, -20f - row * 28f);
        rectTransform.sizeDelta = new Vector2(320f, 18f);

        Slider slider = AddOrGet<Slider>(sliderObject);
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 0f;
        slider.transition = Selectable.Transition.None;

        GameObject background = GetOrCreateUiChild(sliderObject.transform, "Background");
        Image backgroundImage = AddOrGet<Image>(background);
        backgroundImage.color = new Color(0f, 0f, 0f, 0.55f);
        StretchToParent(background.GetComponent<RectTransform>());

        GameObject fillArea = GetOrCreateUiChild(sliderObject.transform, "Fill Area");
        RectTransform fillAreaRect = fillArea.GetComponent<RectTransform>();
        StretchToParent(fillAreaRect);
        fillAreaRect.offsetMin = new Vector2(3f, 3f);
        fillAreaRect.offsetMax = new Vector2(-3f, -3f);

        GameObject fill = GetOrCreateUiChild(fillArea.transform, "Fill");
        Image fillImage = AddOrGet<Image>(fill);
        fillImage.color = new Color(0.2f, 0.9f, 1f, 0.9f);
        RectTransform fillRect = fill.GetComponent<RectTransform>();
        StretchToParent(fillRect);

        slider.fillRect = fillRect;
        slider.targetGraphic = fillImage;
        return slider;
    }

    private static GameObject GetOrCreateUiChild(Transform parent, string objectName)
    {
        Transform existing = parent.Find(objectName);
        if (existing != null)
        {
            if (existing.GetComponent<RectTransform>() == null)
            {
                string fallbackName = objectName + " UI";
                Transform fallback = parent.Find(fallbackName);
                if (fallback != null && fallback.GetComponent<RectTransform>() != null)
                {
                    Debug.LogWarning(objectName + " exists but is not a UI object. Reusing " + fallbackName + " instead.");
                    return fallback.gameObject;
                }

                GameObject fallbackCreated = new GameObject(fallbackName, typeof(RectTransform));
                Undo.RegisterCreatedObjectUndo(fallbackCreated, "Create " + fallbackName);
                fallbackCreated.transform.SetParent(parent, false);
                Debug.LogWarning(objectName + " exists but is not a UI object. Created " + fallbackName + " instead.");
                return fallbackCreated;
            }

            Debug.Log("Reusing UI child GameObject: " + objectName);
            return existing.gameObject;
        }

        GameObject created = new GameObject(objectName, typeof(RectTransform));
        Undo.RegisterCreatedObjectUndo(created, "Create " + objectName);
        created.transform.SetParent(parent, false);
        Debug.Log("Created UI child GameObject: " + objectName);
        return created;
    }

    private static void StretchToParent(RectTransform rectTransform)
    {
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }

    private static Font GetBuiltinFont()
    {
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null)
        {
            font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        return font;
    }

    private static int EnsureLayer(string layerName)
    {
        int existingLayer = LayerMask.NameToLayer(layerName);
        if (existingLayer >= 0)
        {
            Debug.Log("Layer already exists: " + layerName);
            return existingLayer;
        }

        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty layers = tagManager.FindProperty("layers");

        for (int i = 8; i < layers.arraySize; i++)
        {
            SerializedProperty layer = layers.GetArrayElementAtIndex(i);
            if (string.IsNullOrEmpty(layer.stringValue))
            {
                layer.stringValue = layerName;
                tagManager.ApplyModifiedProperties();
                Debug.Log("Created layer: " + layerName);
                return i;
            }
        }

        Debug.LogWarning("Could not create Interactable layer because all user layers are full. PlayerInteractor will keep its existing layer mask.");
        return -1;
    }

    private static void ApplyLayer(GameObject gameObject, int layer)
    {
        if (layer < 0)
        {
            return;
        }

        gameObject.layer = layer;
        foreach (Transform child in gameObject.GetComponentsInChildren<Transform>(true))
        {
            child.gameObject.layer = layer;
        }
    }
}
