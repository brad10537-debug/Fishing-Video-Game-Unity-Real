using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// Editor-only tool that scans the open scene and attaches VillageAreaMarker to likely area GameObjects.
// Safe to run repeatedly: objects that already have a VillageAreaMarker are never overwritten.
public static class AreaMarkerSetupTool
{
    [MenuItem("Fishing Tools/Setup Area Markers")]
    public static void SetupAreaMarkers()
    {
        int added = 0;
        int skipped = 0;
        var log = new StringBuilder();
        log.AppendLine("[AreaMarkerSetupTool] Scanning open scene for area GameObjects...");

        // Include inactive so trigger volumes or disabled area roots are not missed.
        Transform[] all = Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (Transform t in all)
        {
            GameObject go = t.gameObject;

            int zone = ClassifyByName(go.name, out VillageAreaType areaType, out string displayName, out string classifyNote);
            if (zone == 0)
                continue;

            if (go.GetComponent<VillageAreaMarker>() != null)
            {
                log.AppendLine($"  SKIP  [{go.name}] — VillageAreaMarker already present.");
                skipped++;
                continue;
            }

            VillageAreaMarker marker = Undo.AddComponent<VillageAreaMarker>(go);
            marker.areaType    = areaType;
            marker.zoneLevel   = zone;
            marker.areaDisplayName = displayName;
            EditorUtility.SetDirty(go);

            string noteStr = string.IsNullOrEmpty(classifyNote) ? "" : $" ({classifyNote})";
            log.AppendLine($"  ADDED [{go.name}] → areaType={areaType}, zoneLevel={zone}, displayName=\"{displayName}\"{noteStr}");
            added++;
        }

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

        log.AppendLine($"[AreaMarkerSetupTool] Done. Added: {added}  Skipped (already had marker): {skipped}");
        Debug.Log(log.ToString());
    }

    // Returns zone 1/2/3 when the name matches a known area pattern, or 0 for no match.
    // Zone 3 is tested first so "Boss Gate" is never accidentally caught by zone-2 "Edge" keywords.
    private static int ClassifyByName(string name, out VillageAreaType areaType, out string displayName, out string note)
    {
        string lower = name.ToLowerInvariant();
        note = "";

        // --- Zone 3: boss encounters, portals, run gates ---
        if (ContainsAny(lower, "boss", "bossgate", "encounter", "portal"))
        {
            areaType    = VillageAreaType.BossGate;
            displayName = "Boss Gate";
            return 3;
        }

        // --- Zone 2: wild / harder fishing areas ---
        if (ContainsAny(lower, "wild", "edge", "outer", "hard", "risk", "advanced"))
        {
            areaType    = VillageAreaType.WildEdge;
            displayName = "Wild Edge";
            return 2;
        }

        // --- Zone 1: starter / safe areas ---
        // "pond", "starter", "lobby", and "safe" map cleanly to MainPond.
        // "village" objects are set to MainPond (zoneLevel 1) per task spec.
        // VillageAreaType.WizardVillage also exists — if this object represents the
        // wizard village hub rather than a basic fishing area, update areaType manually.
        if (ContainsAny(lower, "pond", "starter", "lobby", "safe", "village"))
        {
            areaType    = VillageAreaType.MainPond;
            displayName = "Main Pond";
            if (lower.Contains("village"))
                note = "named 'Village' — assigned MainPond/zoneLevel 1 per spec; consider WizardVillage enum value if this is the village hub";
            return 1;
        }

        areaType    = VillageAreaType.MainPond;
        displayName = "";
        return 0;
    }

    private static bool ContainsAny(string haystack, params string[] needles)
    {
        foreach (string needle in needles)
        {
            if (haystack.Contains(needle))
                return true;
        }
        return false;
    }
}
