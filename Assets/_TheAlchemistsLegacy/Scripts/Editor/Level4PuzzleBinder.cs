using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public static class Level4PuzzleBinder
{
    private const float DefaultInteractionRange = 3.0f;
    private const float DefaultAimRadius = 0.22f;

    [MenuItem("Tools/The Alchemists Legacy/Bind Level4 Puzzle Objects")]
    public static void BindLevel4PuzzleObjects()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (!scene.IsValid() || !scene.name.Equals("Level4", StringComparison.OrdinalIgnoreCase))
        {
            Debug.LogWarning("[Level4PuzzleBinder] Open the Level4 scene before running this binder.");
        }

        int changed = 0;
        changed += BindAshPuzzle();
        changed += BindSaltChest();
        changed += BindLampDoors();
        changed += BindItemSlots();
        changed += BindCastleEntryGate();
        changed += PrepareLevel4Pickups();
        changed += RemoveKnownMissingScripts();

        EditorSceneManager.MarkSceneDirty(scene);
        Debug.Log("[Level4PuzzleBinder] Level4 puzzle binding complete. Updated " + changed + " objects. Save the scene.");
    }

    private static int BindAshPuzzle()
    {
        GameObject firewood = FindSceneObjectByName("FireWooForAsh");
        if (firewood == null)
        {
            WarnMissing("FireWooForAsh");
            return 0;
        }

        Level4AshFromFirewood ashPuzzle = EnsureComponent<Level4AshFromFirewood>(firewood);
        GameObject ash = FindSceneObjectByName("Ash_l4");
        GameObject magicFire = FindSceneObjectByName("Magic_fire_ash");

        SerializedObject serialized = new SerializedObject(ashPuzzle);
        SetString(serialized, "requiredToolName", "forge_tool");
        SetBool(serialized, "consumeToolOnUse", true);
        SetObject(serialized, "ashObject", ash);
        SetString(serialized, "ashObjectName", "Ash_l4");
        SetBool(serialized, "hideAshUntilBurned", true);
        SetBool(serialized, "markAshAsPickup", true);
        SetBool(serialized, "hideFirewoodAfterBurn", false);
        SetObject(serialized, "magicFireObject", magicFire);
        SetString(serialized, "magicFireObjectName", "Magic_fire_ash");
        SetBool(serialized, "hideMagicFireAfterBurn", true);
        SetFloat(serialized, "interactionRange", DefaultInteractionRange);
        SetFloat(serialized, "interactionAimRadius", DefaultAimRadius);
        serialized.ApplyModifiedPropertiesWithoutUndo();

        SetActiveIfPresent(ash, false);
        SetActiveIfPresent(magicFire, true);
        EnsureUsableCollider(firewood);
        EditorUtility.SetDirty(ashPuzzle);
        return 1;
    }

    private static int BindSaltChest()
    {
        GameObject chest = FindSceneObjectByName("Chest_l4_salt");
        if (chest == null)
        {
            WarnMissing("Chest_l4_salt");
            return 0;
        }

        KeyChestController keyChest = EnsureComponent<KeyChestController>(chest);
        GameObject salt = FindSceneObjectByName("Salt_l4");

        SerializedObject serialized = new SerializedObject(keyChest);
        SetString(serialized, "keyItemName", "Key_l4_salt");
        SetBool(serialized, "autoConfigureLevel4SaltChest", true);
        SetBool(serialized, "consumeKeyOnOpen", true);
        SetObjectArray(serialized, "contentsToReveal", salt);
        SetBool(serialized, "hideContentsUntilOpen", true);
        SetFloat(serialized, "interactionRange", DefaultInteractionRange);
        SetFloat(serialized, "interactionAimRadius", DefaultAimRadius);
        serialized.ApplyModifiedPropertiesWithoutUndo();

        SetActiveIfPresent(salt, false);
        EnsureUsableCollider(chest);
        EditorUtility.SetDirty(keyChest);
        return 1;
    }

    private static int BindLampDoors()
    {
        int changed = 0;
        changed += BindLampDoor("Door_2th", "Door_2th_tar", "lamp_2th");
        changed += BindLampDoor("Door_3th", "Door_3th_tar", "lamp_3th");
        return changed;
    }

    private static int BindLampDoor(string doorName, string targetName, string lampName)
    {
        GameObject door = FindSceneObjectByName(doorName);
        if (door == null)
        {
            WarnMissing(doorName);
            return 0;
        }

        GameObject target = FindSceneObjectByName(targetName);
        door.SetActive(true);

        Level4LampDoorTeleporter teleporter = EnsureComponent<Level4LampDoorTeleporter>(door);
        SerializedObject serialized = new SerializedObject(teleporter);
        SetObject(serialized, "targetDoor", target != null ? target.transform : null);
        SetString(serialized, "targetDoorName", targetName);
        SetString(serialized, "requiredHeldItemName", lampName);
        SetFloat(serialized, "interactionRange", DefaultInteractionRange);
        SetFloat(serialized, "interactionAimRadius", DefaultAimRadius);
        serialized.ApplyModifiedPropertiesWithoutUndo();

        EnsureUsableCollider(door);
        EditorUtility.SetDirty(door);
        EditorUtility.SetDirty(teleporter);

        if (target == null)
        {
            WarnMissing(targetName);
        }

        return 1;
    }

    private static int BindItemSlots()
    {
        int changed = 0;
        changed += BindSlot(new[] { "Sign_Ash_l4", "AshFlag", "stone_quarter_wallAsh" }, "Ash_l4", "Ash_l4_Placed");
        changed += BindSlot(new[] { "Sign_Salt", "SaltFlag", "stone_quarter_wallSalt" }, "Salt_l4", "Salt_Placed");
        changed += BindSlot(new[] { "Sign_YellowStone", "FlagStone", "YellowStoneFlag" }, "YellowStone_l4", "YellowStone_l4_Placed");
        changed += BindSlot(new[] { "Sign_BlueDrop", "BlueDropFlag", "stone_quarter_wallBlueDrop" }, "BlueDrop_l4", "BlueDrop_Placed");
        return changed;
    }

    private static int BindCastleEntryGate()
    {
        GameObject controllerObject = FindSceneObjectByName("Level4_CastleEntryGateController");
        if (controllerObject == null)
        {
            controllerObject = new GameObject("Level4_CastleEntryGateController");
            Undo.RegisterCreatedObjectUndo(controllerObject, "Create Level4 castle entry gate controller");
        }

        Level4CastleEntryGateController controller = EnsureComponent<Level4CastleEntryGateController>(controllerObject);
        SerializedObject serialized = new SerializedObject(controller);
        SetStringArray(serialized, "requiredSlotNames", "Sign_Ash_l4", "Sign_Salt");
        SetStringArray(serialized, "requiredPlacedVisualNames", "Ash_l4_Placed", "Salt_Placed");
        SetBool(serialized, "usePlacedVisualFallbackIfSlotMissing", false);
        SetStringArray(serialized, "gateObjectNames", "Door_Middle_1_th", "Door_Middle_1th2");
        SetBool(serialized, "closeGateUntilRequirementsMet", true);
        serialized.ApplyModifiedPropertiesWithoutUndo();

        GameObject firstGate = FindSceneObjectByName("Door_Middle_1_th");
        GameObject secondGate = FindSceneObjectByName("Door_Middle_1th2");
        SetActiveIfPresent(firstGate, true);
        SetActiveIfPresent(secondGate, true);

        EditorUtility.SetDirty(controllerObject);
        EditorUtility.SetDirty(controller);

        if (firstGate == null)
        {
            WarnMissing("Door_Middle_1_th");
        }

        if (secondGate == null)
        {
            WarnMissing("Door_Middle_1th2");
        }

        return 1;
    }

    private static int BindSlot(string[] slotNames, string acceptedItemName, string placedVisualName)
    {
        GameObject slot = FindFirstSceneObject(slotNames);
        if (slot == null)
        {
            WarnMissing(string.Join("/", slotNames));
            return 0;
        }

        GameObject placedVisual = FindSceneObjectByName(placedVisualName);
        ItemSlotController slotController = EnsureComponent<ItemSlotController>(slot);

        SerializedObject serialized = new SerializedObject(slotController);
        SetString(serialized, "acceptedItemName", acceptedItemName);
        SetBool(serialized, "autoConfigureFromSlotName", true);
        SetObject(serialized, "placedVisual", placedVisual);
        SetObject(serialized, "placedPoint", null);
        SetBool(serialized, "destroyHeldItemOnPlace", true);
        SetFloat(serialized, "interactionRange", DefaultInteractionRange);
        SetFloat(serialized, "interactionAimRadius", DefaultAimRadius);
        serialized.ApplyModifiedPropertiesWithoutUndo();

        SetActiveIfPresent(placedVisual, false);
        EnsureUsableCollider(slot);
        EditorUtility.SetDirty(slot);
        EditorUtility.SetDirty(slotController);
        return 1;
    }

    private static int PrepareLevel4Pickups()
    {
        int changed = 0;
        string[] pickupNames =
        {
            "forge_tool",
            "Key_l4_salt",
            "Salt_l4",
            "Ash_l4",
            "YellowStone_l4",
            "BlueDrop_l4",
            "lamp_2th",
            "lamp_3th",
            "lamp_no"
        };

        foreach (string pickupName in pickupNames)
        {
            GameObject pickup = FindSceneObjectByName(pickupName);
            if (pickup == null)
            {
                continue;
            }

            TrySetTag(pickup, "Pickup");
            EnsureUsableCollider(pickup);
            EnsureRigidbody(pickup);
            EditorUtility.SetDirty(pickup);
            changed++;
        }

        return changed;
    }

    private static int RemoveKnownMissingScripts()
    {
        int changed = 0;
        GameObject exitCubeDuplicate = FindSceneObjectByName("ExitCube (1)");
        if (exitCubeDuplicate != null)
        {
            changed += GameObjectUtility.RemoveMonoBehavioursWithMissingScript(exitCubeDuplicate);
        }

        return changed;
    }

    private static T EnsureComponent<T>(GameObject target) where T : Component
    {
        T component = target.GetComponent<T>();
        if (component == null)
        {
            component = Undo.AddComponent<T>(target);
        }

        return component;
    }

    private static void EnsureRigidbody(GameObject target)
    {
        Rigidbody body = target.GetComponent<Rigidbody>();
        if (body == null)
        {
            body = Undo.AddComponent<Rigidbody>(target);
        }

        body.useGravity = true;
        body.isKinematic = !target.activeInHierarchy;
        body.collisionDetectionMode = CollisionDetectionMode.Discrete;
    }

    private static void EnsureUsableCollider(GameObject target)
    {
        if (target == null)
        {
            return;
        }

        foreach (MeshCollider meshCollider in target.GetComponentsInChildren<MeshCollider>(true))
        {
            if (!meshCollider.convex && meshCollider.GetComponentInParent<Rigidbody>() != null)
            {
                Undo.RecordObject(meshCollider, "Make Level4 pickup mesh collider convex");
                meshCollider.convex = true;
                EditorUtility.SetDirty(meshCollider);
            }
        }

        BoxCollider boxCollider = target.GetComponent<BoxCollider>();
        if (boxCollider == null)
        {
            boxCollider = Undo.AddComponent<BoxCollider>(target);
        }

        Bounds bounds;
        if (TryGetRendererBounds(target, out bounds))
        {
            Vector3 localCenter = target.transform.InverseTransformPoint(bounds.center);
            Vector3 lossyScale = target.transform.lossyScale;
            Vector3 localSize = new Vector3(
                SafeDivide(bounds.size.x, Mathf.Abs(lossyScale.x)),
                SafeDivide(bounds.size.y, Mathf.Abs(lossyScale.y)),
                SafeDivide(bounds.size.z, Mathf.Abs(lossyScale.z)));

            Undo.RecordObject(boxCollider, "Resize Level4 usable collider");
            boxCollider.center = localCenter;
            boxCollider.size = new Vector3(
                Mathf.Max(localSize.x, 0.25f),
                Mathf.Max(localSize.y, 0.25f),
                Mathf.Max(localSize.z, 0.25f));
            boxCollider.isTrigger = false;
            EditorUtility.SetDirty(boxCollider);
        }
    }

    private static bool TryGetRendererBounds(GameObject target, out Bounds bounds)
    {
        Renderer[] renderers = target.GetComponentsInChildren<Renderer>(true);
        bounds = new Bounds(target.transform.position, Vector3.one);
        bool found = false;

        foreach (Renderer renderer in renderers)
        {
            if (renderer == null)
            {
                continue;
            }

            if (!found)
            {
                bounds = renderer.bounds;
                found = true;
            }
            else
            {
                bounds.Encapsulate(renderer.bounds);
            }
        }

        return found;
    }

    private static float SafeDivide(float value, float divisor)
    {
        return divisor > 0.0001f ? value / divisor : value;
    }

    private static GameObject FindFirstSceneObject(string[] names)
    {
        foreach (string objectName in names)
        {
            GameObject match = FindSceneObjectByName(objectName);
            if (match != null)
            {
                return match;
            }
        }

        return null;
    }

    private static GameObject FindSceneObjectByName(string objectName)
    {
        if (string.IsNullOrWhiteSpace(objectName))
        {
            return null;
        }

        string expectedName = NormalizeName(objectName);
        Scene activeScene = SceneManager.GetActiveScene();
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (GameObject candidate in allObjects)
        {
            if (candidate == null || EditorUtility.IsPersistent(candidate) || candidate.scene != activeScene)
            {
                continue;
            }

            if (NormalizeName(candidate.name).Equals(expectedName, StringComparison.OrdinalIgnoreCase))
            {
                return candidate;
            }
        }

        return null;
    }

    private static string NormalizeName(string value)
    {
        return value.Trim().Replace("(Clone)", "").Trim();
    }

    private static void SetActiveIfPresent(GameObject target, bool active)
    {
        if (target == null)
        {
            return;
        }

        if (target.activeSelf != active)
        {
            Undo.RecordObject(target, "Set Level4 object active");
            target.SetActive(active);
            EditorUtility.SetDirty(target);
        }
    }

    private static void TrySetTag(GameObject target, string tagName)
    {
        try
        {
            target.tag = tagName;
        }
        catch (UnityException)
        {
            Debug.LogWarning("[Level4PuzzleBinder] Tag '" + tagName + "' does not exist. Add it in Project Settings > Tags and Layers.");
        }
    }

    private static void WarnMissing(string objectName)
    {
        Debug.LogWarning("[Level4PuzzleBinder] Could not find scene object: " + objectName);
    }

    private static void SetString(SerializedObject serialized, string propertyName, string value)
    {
        SerializedProperty property = serialized.FindProperty(propertyName);
        if (property != null)
        {
            property.stringValue = value;
        }
    }

    private static void SetBool(SerializedObject serialized, string propertyName, bool value)
    {
        SerializedProperty property = serialized.FindProperty(propertyName);
        if (property != null)
        {
            property.boolValue = value;
        }
    }

    private static void SetFloat(SerializedObject serialized, string propertyName, float value)
    {
        SerializedProperty property = serialized.FindProperty(propertyName);
        if (property != null)
        {
            property.floatValue = value;
        }
    }

    private static void SetObject(SerializedObject serialized, string propertyName, Object value)
    {
        SerializedProperty property = serialized.FindProperty(propertyName);
        if (property != null)
        {
            property.objectReferenceValue = value;
        }
    }

    private static void SetObjectArray(SerializedObject serialized, string propertyName, params Object[] values)
    {
        SerializedProperty property = serialized.FindProperty(propertyName);
        if (property == null || !property.isArray)
        {
            return;
        }

        int count = 0;
        foreach (Object value in values)
        {
            if (value != null)
            {
                count++;
            }
        }

        property.arraySize = count;
        int index = 0;
        foreach (Object value in values)
        {
            if (value == null)
            {
                continue;
            }

            property.GetArrayElementAtIndex(index).objectReferenceValue = value;
            index++;
        }
    }

    private static void SetStringArray(SerializedObject serialized, string propertyName, params string[] values)
    {
        SerializedProperty property = serialized.FindProperty(propertyName);
        if (property == null || !property.isArray)
        {
            return;
        }

        property.arraySize = values.Length;
        for (int i = 0; i < values.Length; i++)
        {
            property.GetArrayElementAtIndex(i).stringValue = values[i];
        }
    }
}
