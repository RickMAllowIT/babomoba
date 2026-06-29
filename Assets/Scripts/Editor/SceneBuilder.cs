using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

/// <summary>
/// Editor tool to build the full BaboMOBA sandbox scene programmatically.
/// Run via: Tools > BaboMOBA > Build Scene
/// </summary>
public class SceneBuilder : EditorWindow
{
    [MenuItem("Tools/BaboMOBA/Build Scene")]
    public static void BuildScene()
    {
        // ── Ground ──────────────────────────────────────────────────
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.position = Vector3.zero;
        ground.transform.localScale = new Vector3(2f, 1f, 2f); // 20x20
        Renderer groundRenderer = ground.GetComponent<Renderer>();
        groundRenderer.material.color = Color.gray;

        // ── Directional Light ───────────────────────────────────────
        GameObject lightObj = new GameObject("Directional Light");
        Light light = lightObj.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1f;
        lightObj.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

        // ── Camera ──────────────────────────────────────────────────
        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            GameObject camObj = new GameObject("Main Camera");
            camObj.AddComponent<Camera>();
            camObj.tag = "MainCamera";
            mainCam = camObj.GetComponent<Camera>();
        }
        mainCam.transform.position = new Vector3(0f, 15f, -12f);
        mainCam.transform.rotation = Quaternion.Euler(45f, 0f, 0f);

        // ── Lane Positions ──────────────────────────────────────────
        Vector3 spawnEnd = new Vector3(0f, 0f, 8f);      // far end — minion spawn
        Vector3 towerPos = new Vector3(0f, 0f, 0f);      // midpoint — tower
        Vector3 corePos  = new Vector3(0f, 0f, -8f);     // player's end — core

        // ── Player ──────────────────────────────────────────────────
        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        player.name = "Player";
        player.transform.position = new Vector3(0f, 0.5f, -5f);
        player.transform.localScale = Vector3.one;
        player.GetComponent<Renderer>().material.color = Color.blue;

        Rigidbody rb = player.AddComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
        rb.useGravity = false;

        player.AddComponent<PlayerMovement>();
        player.AddComponent<PlayerAim>();
        player.AddComponent<ManualFire>();
        player.AddComponent<AutoStream>();
        player.AddComponent<AutoStreamToggle>();
        player.AddComponent<AutoStreamSuspension>();
        player.AddComponent<Dash>();
        player.AddComponent<LaserBeam>();
        player.AddComponent<HarvesterFilter>();
        player.AddComponent<TargetHighlight>();
        // Targeting mode default: Harvester
        player.AddComponent<DemolitionMode>();

        // ── Camera Follow ───────────────────────────────────────────
        mainCam.gameObject.AddComponent<CameraFollow>().target = player.transform;

        // ── Spawner ─────────────────────────────────────────────────
        GameObject spawner = new GameObject("MinionSpawner");
        spawner.transform.position = spawnEnd;
        MinionSpawner ms = spawner.AddComponent<MinionSpawner>();

        // ── Tower ───────────────────────────────────────────────────
        GameObject tower = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        tower.name = "Tower";
        tower.transform.position = towerPos + Vector3.up * 1.5f;
        tower.transform.localScale = new Vector3(1.5f, 3f, 1.5f);
        tower.GetComponent<Renderer>().material.color = Color.yellow;
        tower.tag = "Tower";

        Health towerHealth = tower.AddComponent<Health>();
        towerHealth.maxHealth = 500f;
        tower.AddComponent<TowerTargeting>();
        tower.AddComponent<TowerFiring>();
        tower.AddComponent<TowerDestruction>();

        // ── Core ────────────────────────────────────────────────────
        GameObject core = GameObject.CreatePrimitive(PrimitiveType.Cube);
        core.name = "Core";
        core.transform.position = corePos + Vector3.up * 1f;
        core.transform.localScale = new Vector3(3f, 2f, 3f);
        core.GetComponent<Renderer>().material.color = Color.green;
        core.tag = "Core";

        Health coreHealth = core.AddComponent<Health>();
        coreHealth.maxHealth = 1000f;
        core.AddComponent<Core>();

        // ── Game Over Manager ───────────────────────────────────────
        GameObject gameManager = new GameObject("GameManager");
        GameOver go = gameManager.AddComponent<GameOver>();

        // ── Enemy Dummy ─────────────────────────────────────────────
        GameObject dummy = GameObject.CreatePrimitive(PrimitiveType.Cube);
        dummy.name = "EnemyDummy";
        dummy.transform.position = new Vector3(2f, 0.5f, 0f);
        dummy.transform.localScale = new Vector3(1f, 1f, 1f);
        dummy.GetComponent<Renderer>().material.color = Color.red;
        dummy.tag = "Enemy";

        Health dummyHealth = dummy.AddComponent<Health>();
        dummyHealth.maxHealth = 9999f;
        dummy.AddComponent<EnemyDummy>();
        DummyPatrol dp = dummy.AddComponent<DummyPatrol>();

        // ── Waypoints for dummy patrol ──────────────────────────────
        GameObject wpA = new GameObject("Waypoint_A");
        wpA.transform.position = new Vector3(2f, 0f, 2f);
        GameObject wpB = new GameObject("Waypoint_B");
        wpB.transform.position = new Vector3(2f, 0f, -2f);

        // Set waypoints via reflection (serialized fields)
        var wpField = typeof(DummyPatrol).GetField("waypointA",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (wpField != null)
        {
            wpField.SetValue(dp, wpA.transform);
            var wpFieldB = typeof(DummyPatrol).GetField("waypointB",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            wpFieldB?.SetValue(dp, wpB.transform);
        }

        // ── NavMesh ─────────────────────────────────────────────────
        NavMeshSurface surface = ground.AddComponent<NavMeshSurface>();
        // Bake is called separately since it requires Editor mode

        // ── Tags Setup ──────────────────────────────────────────────
        EnsureTag("Minion");
        EnsureTag("Enemy");
        EnsureTag("Tower");
        EnsureTag("Core");

        // ── Minion prefab setup ─────────────────────────────────────
        GameObject minionPrefab = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        minionPrefab.name = "Minion";
        minionPrefab.transform.localScale = Vector3.one * 0.5f;
        minionPrefab.GetComponent<Renderer>().material.color = Color.gray;
        minionPrefab.tag = "Minion";
        minionPrefab.AddComponent<NavMeshAgent>();
        minionPrefab.AddComponent<Health>().maxHealth = 30f;
        minionPrefab.AddComponent<Minion>();

        // Set the prefab reference on spawner
        ms.minionPrefab = minionPrefab;
        ms.spawnPoint = spawnEnd;

        // ── Projectile Prefab ───────────────────────────────────────
        GameObject projPrefab = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        projPrefab.name = "Projectile";
        projPrefab.transform.localScale = Vector3.one * 0.2f;
        projPrefab.GetComponent<Collider>().isTrigger = true;
        projPrefab.AddComponent<Rigidbody>().useGravity = false;
        projPrefab.AddComponent<Projectile>();

        // Set projectile prefab refs
        var mf = player.GetComponent<ManualFire>();
        var mfField = typeof(ManualFire).GetField("projectilePrefab",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        mfField?.SetValue(mf, projPrefab);

        var asField = typeof(AutoStream).GetField("projectilePrefab",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        asField?.SetValue(player.GetComponent<AutoStream>(), projPrefab);

        // ── Tower Projectile Prefab ─────────────────────────────────
        GameObject towerProj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        towerProj.name = "TowerProjectile";
        towerProj.transform.localScale = Vector3.one * 0.3f;
        towerProj.GetComponent<Renderer>().material.color = Color.red;
        towerProj.GetComponent<Collider>().isTrigger = true;
        towerProj.AddComponent<Rigidbody>().useGravity = false;
        towerProj.AddComponent<Projectile>();

        var tf = tower.GetComponent<TowerFiring>();
        var tfField = typeof(TowerFiring).GetField("projectilePrefab",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        tfField?.SetValue(tf, towerProj);

        // ── Link TowerDestruction OnTowerDestroyed → Core.OnTowerDestroyed ──
        // Wire via UnityEvent in inspector; stub here

        Debug.Log("✅ BaboMOBA scene built.  To bake NavMesh: Window > AI > Navigation > Bake.");
        Debug.Log("   Then in Game view, press Play to test.");
    }

    private static void EnsureTag(string tag)
    {
        SerializedObject tagManager = new SerializedObject(
            AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");
        for (int i = 0; i < tagsProp.arraySize; i++)
        {
            if (tagsProp.GetArrayElementAtIndex(i).stringValue == tag)
            {
                return; // already exists
            }
        }
        tagsProp.InsertArrayElementAtIndex(tagsProp.arraySize);
        tagsProp.GetArrayElementAtIndex(tagsProp.arraySize - 1).stringValue = tag;
        tagManager.ApplyModifiedProperties();
    }
}
