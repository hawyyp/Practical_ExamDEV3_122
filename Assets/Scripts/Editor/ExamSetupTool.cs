#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;
using UnityEngine.ProBuilder;

namespace Ratchanon.Editor
{
    /// <summary>
    /// Automated setup tool for Practical Examination Questions 1 through 12.
    /// Creates high-polish Prefabs, Materials, Glassmorphism UI Cards, modern draggable Sliders,
    /// and all 8 Scenes according to Fig. 1, updating Build Settings.
    /// </summary>
    public static class ExamSetupTool
    {
        private const string SCENES_DIR = "Assets/Scenes";
        private const string PREFABS_DIR = "Assets/Prefabs";
        private const string MATERIALS_DIR = "Assets/Materials";

        [MenuItem("Tools/Practical Exam/Setup All Exam Phases (Q1 to Q12)")]
        [MenuItem("Tools/Practical Exam/Setup Q1 to Q5 Scenes and Prefabs")]
        public static void SetupAll()
        {
            EnsureDirectories();
            CreateMaterials();
            CreatePrefabs();
            BuildAllEightScenes();
            UpdateEditorBuildSettings();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[ExamSetupTool] Successfully built high-polish scenes, prefabs, materials, and UI for Questions 1 - 12 (All Phases)!");
        }

        [MenuItem("Tools/Practical Exam/Setup Stage 2 Minigame (Q6 to Q8)")]
        public static void SetupStage2Only()
        {
            EnsureDirectories();
            CreateMaterials();
            CreateStage2Prefabs();
            BuildStage2Scene();
            UpdateEditorBuildSettings();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[ExamSetupTool] Successfully built Stage 2 (Questions 6 - 8)!");
        }

        private static void EnsureDirectories()
        {
            if (!Directory.Exists(SCENES_DIR)) Directory.CreateDirectory(SCENES_DIR);
            if (!Directory.Exists(PREFABS_DIR)) Directory.CreateDirectory(PREFABS_DIR);
            if (!Directory.Exists(MATERIALS_DIR)) Directory.CreateDirectory(MATERIALS_DIR);
            AssetDatabase.Refresh();
        }

        #region Materials Creation
        private static void CreateMaterials()
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");

            CreateMaterial("Mat_TimeBonus", new Color(0.0f, 0.95f, 0.45f), shader, 0.8f);       // Neon Emerald
            CreateMaterial("Mat_TimePenalty", new Color(1.0f, 0.15f, 0.25f), shader, 0.8f);     // Neon Ruby
            CreateMaterial("Mat_ScoreBonus", new Color(1.0f, 0.82f, 0.05f), shader, 0.9f);      // Gleaming Pure Gold
            CreateMaterial("Mat_ScorePenalty", new Color(0.68f, 0.2f, 0.95f), shader, 0.8f);   // Electric Amethyst
            CreateMaterial("Mat_Obstacle", new Color(0.95f, 0.2f, 0.05f), shader, 0.6f);       // Vibrant Hazard Red
            CreateMaterial("Mat_Platform", new Color(0.12f, 0.16f, 0.24f), shader, 0.3f);      // Deep Slate Carbon
            CreateMaterial("Mat_PlatformAccent", new Color(0.0f, 0.85f, 1.0f), shader, 0.7f);  // Cyan Accent Trim
            CreateMaterial("Mat_NarrowPath", new Color(1.0f, 0.55f, 0.0f), shader, 0.5f);      // Hazard Orange Bridge
            CreateMaterial("Mat_Player", new Color(0.02f, 0.65f, 1.0f), shader, 0.6f);         // Cyber Cobalt Blue
            CreateMaterial("Mat_PlayerVisor", new Color(0.05f, 0.08f, 0.14f), shader, 0.95f);  // Visor Glossy Black
        }

        private static Material CreateMaterial(string name, Color color, Shader shader, float smoothness = 0.5f)
        {
            string path = $"{MATERIALS_DIR}/{name}.mat";
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null)
            {
                mat = new Material(shader);
                mat.color = color;
                if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
                if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", smoothness);
                AssetDatabase.CreateAsset(mat, path);
            }
            else
            {
                mat.color = color;
                if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
                if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", smoothness);
                EditorUtility.SetDirty(mat);
            }
            return mat;
        }
        #endregion

        #region Prefabs Creation
        private static void CreatePrefabs()
        {
            // Stage 1 Items: 4 Distinct Collectibles (Collision-based, Q.4)
            CreateGemItemPrefab("Item_ScoreBonus", ItemType.ScoreBonus, "Mat_ScoreBonus", new Vector3(0.65f, 0.65f, 0.65f));
            CreateGemItemPrefab("Item_ScorePenalty", ItemType.ScorePenalty, "Mat_ScorePenalty", new Vector3(0.65f, 0.65f, 0.65f));
            CreateSphereItemPrefab("Item_TimeBonus", ItemType.TimeBonus, "Mat_TimeBonus", new Vector3(0.7f, 0.7f, 0.7f));
            CreateCylinderItemPrefab("Item_TimePenalty", ItemType.TimePenalty, "Mat_TimePenalty", new Vector3(0.6f, 0.6f, 0.6f));

            CreateObstaclePrefab();
            CreateCapsulePlayerPrefab();

            // Stage 2 Interactive Collectibles & Humanoid Player (Q.6, Q.8)
            CreateStage2Prefabs();
        }

        private static void CreateStage2Prefabs()
        {
            CreateInteractableGemPrefab("Item_Interact_ScoreBonus", ItemType.ScoreBonus, "Mat_ScoreBonus", new Vector3(0.7f, 0.7f, 0.7f));
            CreateInteractableGemPrefab("Item_Interact_ScorePenalty", ItemType.ScorePenalty, "Mat_ScorePenalty", new Vector3(0.7f, 0.7f, 0.7f));
            CreateInteractableSpherePrefab("Item_Interact_TimeBonus", ItemType.TimeBonus, "Mat_TimeBonus", new Vector3(0.75f, 0.75f, 0.75f));
            CreateInteractableCylinderPrefab("Item_Interact_TimePenalty", ItemType.TimePenalty, "Mat_TimePenalty", new Vector3(0.65f, 0.65f, 0.65f));

            CreateHumanoidPlayerPrefab();
        }

        private static void CreateGemItemPrefab(string prefabName, ItemType type, string matName, Vector3 scale)
        {
            string path = $"{PREFABS_DIR}/{prefabName}.prefab";
            GameObject root = new GameObject(prefabName);
            root.tag = "Untagged";

            // Trigger collider on root
            BoxCollider col = root.AddComponent<BoxCollider>();
            col.isTrigger = true;
            col.size = new Vector3(1.2f, 1.2f, 1.2f);

            // Visual mesh: tilted 45 degrees like a diamond gem
            GameObject meshObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            meshObj.name = "Gem_Mesh";
            meshObj.transform.SetParent(root.transform, false);
            meshObj.transform.localRotation = Quaternion.Euler(45f, 45f, 0f);
            meshObj.transform.localScale = scale;

            // Remove mesh primitive collider so only root trigger is active
            Collider meshCol = meshObj.GetComponent<Collider>();
            if (meshCol != null) Object.DestroyImmediate(meshCol);

            Material mat = AssetDatabase.LoadAssetAtPath<Material>($"{MATERIALS_DIR}/{matName}.mat");
            if (mat != null) meshObj.GetComponent<Renderer>().sharedMaterial = mat;

            // Script
            ItemPickup pickup = root.AddComponent<ItemPickup>();
            pickup.itemType = type;

            PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
        }

        private static void CreateSphereItemPrefab(string prefabName, ItemType type, string matName, Vector3 scale)
        {
            string path = $"{PREFABS_DIR}/{prefabName}.prefab";
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = prefabName;
            go.tag = "Untagged";
            go.transform.localScale = scale;

            Collider col = go.GetComponent<Collider>();
            if (col != null) col.isTrigger = true;

            Material mat = AssetDatabase.LoadAssetAtPath<Material>($"{MATERIALS_DIR}/{matName}.mat");
            if (mat != null) go.GetComponent<Renderer>().sharedMaterial = mat;

            ItemPickup pickup = go.AddComponent<ItemPickup>();
            pickup.itemType = type;

            PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
        }

        private static void CreateCylinderItemPrefab(string prefabName, ItemType type, string matName, Vector3 scale)
        {
            string path = $"{PREFABS_DIR}/{prefabName}.prefab";
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            go.name = prefabName;
            go.tag = "Untagged";
            go.transform.localScale = scale;

            Collider col = go.GetComponent<Collider>();
            if (col != null) col.isTrigger = true;

            Material mat = AssetDatabase.LoadAssetAtPath<Material>($"{MATERIALS_DIR}/{matName}.mat");
            if (mat != null) go.GetComponent<Renderer>().sharedMaterial = mat;

            ItemPickup pickup = go.AddComponent<ItemPickup>();
            pickup.itemType = type;

            PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
        }

        private static void CreateObstaclePrefab()
        {
            string path = $"{PREFABS_DIR}/Obstacle_Hazard.prefab";
            GameObject root = new GameObject("Obstacle_Hazard");
            root.transform.localScale = Vector3.one;

            // Main hazard block
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = "Hazard_Body";
            cube.transform.SetParent(root.transform, false);
            cube.transform.localScale = new Vector3(1.6f, 1.3f, 1.6f);
            cube.transform.localPosition = new Vector3(0f, 0.65f, 0f);

            Material mat = AssetDatabase.LoadAssetAtPath<Material>($"{MATERIALS_DIR}/Mat_Obstacle.mat");
            if (mat != null) cube.GetComponent<Renderer>().sharedMaterial = mat;

            // Remove child collider, put on root
            Collider cubeCol = cube.GetComponent<Collider>();
            if (cubeCol != null) Object.DestroyImmediate(cubeCol);

            BoxCollider col = root.AddComponent<BoxCollider>();
            col.center = new Vector3(0f, 0.65f, 0f);
            col.size = new Vector3(1.6f, 1.3f, 1.6f);

            root.AddComponent<Obstacle>();

            PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
        }

        private static void CreateCapsulePlayerPrefab()
        {
            string path = $"{PREFABS_DIR}/CapsulePlayer.prefab";
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            go.name = "CapsulePlayer";
            go.tag = "Player";

            // Remove default collider, use CharacterController
            Collider defaultCol = go.GetComponent<Collider>();
            if (defaultCol != null) Object.DestroyImmediate(defaultCol);

            CharacterController cc = go.AddComponent<CharacterController>();
            cc.height = 2f;
            cc.radius = 0.5f;
            cc.center = new Vector3(0f, 1f, 0f);

            go.AddComponent<CapsulePlayerController>();

            Material matPlayer = AssetDatabase.LoadAssetAtPath<Material>($"{MATERIALS_DIR}/Mat_Player.mat");
            if (matPlayer != null) go.GetComponent<Renderer>().sharedMaterial = matPlayer;

            // Stylized Futuristic Visor on the face
            GameObject visor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            visor.name = "Player_Visor";
            visor.transform.SetParent(go.transform, false);
            visor.transform.localPosition = new Vector3(0f, 1.45f, 0.38f);
            visor.transform.localScale = new Vector3(0.58f, 0.24f, 0.28f);

            Collider visorCol = visor.GetComponent<Collider>();
            if (visorCol != null) Object.DestroyImmediate(visorCol);

            Material matVisor = AssetDatabase.LoadAssetAtPath<Material>($"{MATERIALS_DIR}/Mat_PlayerVisor.mat");
            if (matVisor != null) visor.GetComponent<Renderer>().sharedMaterial = matVisor;

            // Ground check transform
            GameObject groundCheck = new GameObject("GroundCheck");
            groundCheck.transform.SetParent(go.transform, false);
            groundCheck.transform.localPosition = new Vector3(0f, 0f, 0f);

            PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
        }

        private static void CreateHumanoidPlayerPrefab()
        {
            string path = $"{PREFABS_DIR}/HumanoidPlayer.prefab";
            string fbxPath = "Assets/Resources_Exam/Character Models/Ch02_nonPBR.fbx";
            GameObject modelAsset = AssetDatabase.LoadAssetAtPath<GameObject>(fbxPath);
            if (modelAsset == null)
            {
                Debug.LogError($"[ExamSetupTool] Cannot find character model at {fbxPath}");
                return;
            }

            GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(modelAsset);
            go.name = "HumanoidPlayer";
            go.tag = "Player";

            // CharacterController configuration
            CharacterController cc = go.GetComponent<CharacterController>();
            if (cc == null) cc = go.AddComponent<CharacterController>();
            cc.height = 1.8f;
            cc.radius = 0.35f;
            cc.center = new Vector3(0f, 0.9f, 0f);

            // HumanoidPlayerController component
            HumanoidPlayerController hpc = go.GetComponent<HumanoidPlayerController>();
            if (hpc == null) hpc = go.AddComponent<HumanoidPlayerController>();

            // Animator & Animator Controller
            Animator anim = go.GetComponent<Animator>();
            if (anim == null) anim = go.AddComponent<Animator>();

            string controllerPath = $"{PREFABS_DIR}/HumanoidPlayerController.controller";
            UnityEditor.Animations.AnimatorController ac = AssetDatabase.LoadAssetAtPath<UnityEditor.Animations.AnimatorController>(controllerPath);
            if (ac == null)
            {
                ac = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
                ac.AddParameter("Speed", AnimatorControllerParameterType.Float);
                ac.AddParameter("IsGrounded", AnimatorControllerParameterType.Bool);
                ac.AddParameter("Jump", AnimatorControllerParameterType.Trigger);

                AnimationClip idleClip = LoadAnimationClip("Assets/Resources_Exam/Animations/Ch02_nonPBR@Idle.fbx");
                AnimationClip happyIdleClip = LoadAnimationClip("Assets/Resources_Exam/Animations/Ch02_nonPBR@Happy Idle.fbx");

                var rootSm = ac.layers[0].stateMachine;
                var stateIdle = rootSm.AddState("Idle");
                if (idleClip != null) stateIdle.motion = idleClip;

                var stateMove = rootSm.AddState("Move");
                if (happyIdleClip != null) stateMove.motion = happyIdleClip;
                else if (idleClip != null) stateMove.motion = idleClip;

                // Transitions
                var tToMove = stateIdle.AddTransition(stateMove);
                tToMove.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Greater, 0.1f, "Speed");
                tToMove.hasExitTime = false;
                tToMove.duration = 0.15f;

                var tToIdle = stateMove.AddTransition(stateIdle);
                tToIdle.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Less, 0.1f, "Speed");
                tToIdle.hasExitTime = false;
                tToIdle.duration = 0.15f;
            }

            anim.runtimeAnimatorController = ac;

            PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
        }

        private static AnimationClip LoadAnimationClip(string fbxPath)
        {
            Object[] assets = AssetDatabase.LoadAllAssetsAtPath(fbxPath);
            foreach (Object asset in assets)
            {
                if (asset is AnimationClip clip && !clip.name.StartsWith("__preview__"))
                {
                    return clip;
                }
            }
            return null;
        }

        private static void CreateInteractableGemPrefab(string prefabName, ItemType type, string matName, Vector3 scale)
        {
            string path = $"{PREFABS_DIR}/{prefabName}.prefab";
            GameObject root = new GameObject(prefabName);
            root.tag = "Untagged";

            // Proximity Trigger Sphere for [E] Key interaction
            SphereCollider col = root.AddComponent<SphereCollider>();
            col.isTrigger = true;
            col.radius = 2.5f;

            // Visual mesh: tilted 45 degrees diamond
            GameObject meshObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            meshObj.name = "Gem_Mesh";
            meshObj.transform.SetParent(root.transform, false);
            meshObj.transform.localRotation = Quaternion.Euler(45f, 45f, 0f);
            meshObj.transform.localScale = scale;

            Collider meshCol = meshObj.GetComponent<Collider>();
            if (meshCol != null) Object.DestroyImmediate(meshCol);

            Material mat = AssetDatabase.LoadAssetAtPath<Material>($"{MATERIALS_DIR}/{matName}.mat");
            if (mat != null) meshObj.GetComponent<Renderer>().sharedMaterial = mat;

            // Interaction Aura ring indicator
            GameObject aura = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            aura.name = "Interaction_AuraRing";
            aura.transform.SetParent(root.transform, false);
            aura.transform.localPosition = new Vector3(0f, -0.4f, 0f);
            aura.transform.localScale = new Vector3(1.2f, 0.02f, 1.2f);
            Collider auraCol = aura.GetComponent<Collider>();
            if (auraCol != null) Object.DestroyImmediate(auraCol);
            if (mat != null) aura.GetComponent<Renderer>().sharedMaterial = mat;

            InteractableItem item = root.AddComponent<InteractableItem>();
            item.itemType = type;

            PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
        }

        private static void CreateInteractableSpherePrefab(string prefabName, ItemType type, string matName, Vector3 scale)
        {
            string path = $"{PREFABS_DIR}/{prefabName}.prefab";
            GameObject root = new GameObject(prefabName);
            root.tag = "Untagged";

            SphereCollider col = root.AddComponent<SphereCollider>();
            col.isTrigger = true;
            col.radius = 2.5f;

            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.name = "Sphere_Mesh";
            sphere.transform.SetParent(root.transform, false);
            sphere.transform.localScale = scale;

            Collider meshCol = sphere.GetComponent<Collider>();
            if (meshCol != null) Object.DestroyImmediate(meshCol);

            Material mat = AssetDatabase.LoadAssetAtPath<Material>($"{MATERIALS_DIR}/{matName}.mat");
            if (mat != null) sphere.GetComponent<Renderer>().sharedMaterial = mat;

            GameObject aura = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            aura.name = "Interaction_AuraRing";
            aura.transform.SetParent(root.transform, false);
            aura.transform.localPosition = new Vector3(0f, -0.4f, 0f);
            aura.transform.localScale = new Vector3(1.2f, 0.02f, 1.2f);
            Collider auraCol = aura.GetComponent<Collider>();
            if (auraCol != null) Object.DestroyImmediate(auraCol);
            if (mat != null) aura.GetComponent<Renderer>().sharedMaterial = mat;

            InteractableItem item = root.AddComponent<InteractableItem>();
            item.itemType = type;

            PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
        }

        private static void CreateInteractableCylinderPrefab(string prefabName, ItemType type, string matName, Vector3 scale)
        {
            string path = $"{PREFABS_DIR}/{prefabName}.prefab";
            GameObject root = new GameObject(prefabName);
            root.tag = "Untagged";

            SphereCollider col = root.AddComponent<SphereCollider>();
            col.isTrigger = true;
            col.radius = 2.5f;

            GameObject cyl = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cyl.name = "Cylinder_Mesh";
            cyl.transform.SetParent(root.transform, false);
            cyl.transform.localScale = scale;

            Collider meshCol = cyl.GetComponent<Collider>();
            if (meshCol != null) Object.DestroyImmediate(meshCol);

            Material mat = AssetDatabase.LoadAssetAtPath<Material>($"{MATERIALS_DIR}/{matName}.mat");
            if (mat != null) cyl.GetComponent<Renderer>().sharedMaterial = mat;

            GameObject aura = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            aura.name = "Interaction_AuraRing";
            aura.transform.SetParent(root.transform, false);
            aura.transform.localPosition = new Vector3(0f, -0.4f, 0f);
            aura.transform.localScale = new Vector3(1.2f, 0.02f, 1.2f);
            Collider auraCol = aura.GetComponent<Collider>();
            if (auraCol != null) Object.DestroyImmediate(auraCol);
            if (mat != null) aura.GetComponent<Renderer>().sharedMaterial = mat;

            InteractableItem item = root.AddComponent<InteractableItem>();
            item.itemType = type;

            PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
        }
        #endregion

        #region UI and Canvas Helpers
        private static void SetupBaseScene(out Camera cam, out Light light)
        {
            // Camera
            GameObject camObj = new GameObject("Main Camera");
            camObj.tag = "MainCamera";
            cam = camObj.AddComponent<Camera>();
            camObj.AddComponent<AudioListener>();
            camObj.transform.position = new Vector3(0f, 6f, -12f);
            camObj.transform.LookAt(new Vector3(0f, 1f, 0f));
            cam.backgroundColor = new Color(0.08f, 0.10f, 0.16f); // Sleek deep space slate
            cam.clearFlags = CameraClearFlags.SolidColor;

            // Directional Light
            GameObject lightObj = new GameObject("Directional Light");
            light = lightObj.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.3f;
            light.color = new Color(1f, 0.98f, 0.92f);
            light.shadows = LightShadows.Soft;
            lightObj.transform.rotation = Quaternion.Euler(48f, -32f, 0f);
        }

        private static GameObject CreateCanvas(string name)
        {
            GameObject canvasObj = new GameObject(name);
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280, 720);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObj.AddComponent<GraphicRaycaster>();

            // Event System
            var existingEs = Object.FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>();
            GameObject esObj = existingEs != null ? existingEs.gameObject : new GameObject("EventSystem");
            if (existingEs == null)
            {
                existingEs = esObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            }

            var standalone = esObj.GetComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            if (standalone != null) Object.DestroyImmediate(standalone);

            var inputModule = esObj.GetComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
            if (inputModule == null)
            {
                inputModule = esObj.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
            }

            return canvasObj;
        }

        private static RectTransform CreateCardPanel(Transform parent, string name, Vector2 size, Color accentHeaderColor)
        {
            // Main Glassmorphism Card
            GameObject card = new GameObject(name);
            card.transform.SetParent(parent, false);

            RectTransform rtCard = card.AddComponent<RectTransform>();
            rtCard.anchorMin = new Vector2(0.5f, 0.5f);
            rtCard.anchorMax = new Vector2(0.5f, 0.5f);
            rtCard.anchoredPosition = Vector2.zero;
            rtCard.sizeDelta = size;

            Image imgCard = card.AddComponent<Image>();
            imgCard.color = new Color(0.06f, 0.08f, 0.14f, 0.94f); // Deep frosted obsidian
            imgCard.raycastTarget = false;

            // Top Glowing Accent Strip
            GameObject strip = new GameObject("AccentStrip");
            strip.transform.SetParent(card.transform, false);
            RectTransform rtStrip = strip.AddComponent<RectTransform>();
            rtStrip.anchorMin = new Vector2(0f, 1f);
            rtStrip.anchorMax = new Vector2(1f, 1f);
            rtStrip.anchoredPosition = new Vector2(0f, -2.5f);
            rtStrip.sizeDelta = new Vector2(0f, 5f);

            Image imgStrip = strip.AddComponent<Image>();
            imgStrip.color = accentHeaderColor;
            imgStrip.raycastTarget = false;

            return rtCard;
        }

        private static Text CreateText(Transform parent, string name, string content, int fontSize, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos, Vector2 size, TextAnchor alignment, Color color, FontStyle fontStyle = FontStyle.Normal)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            Text t = go.AddComponent<Text>();
            t.text = content;
            t.fontSize = fontSize;
            t.fontStyle = fontStyle;
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (t.font == null) t.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            t.alignment = alignment;
            t.color = color;
            t.raycastTarget = false;
            t.supportRichText = true;

            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = size;
            return t;
        }

        private static Button CreateModernButton(Transform parent, string name, string label, Vector2 anchoredPos, Vector2 size, Color baseColor)
        {
            GameObject btnObj = new GameObject(name);
            btnObj.transform.SetParent(parent, false);

            RectTransform rt = btnObj.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = size;

            Image img = btnObj.AddComponent<Image>();
            img.color = baseColor;
            img.raycastTarget = true;

            Button btn = btnObj.AddComponent<Button>();
            ColorBlock cb = btn.colors;
            cb.normalColor = baseColor;
            cb.highlightedColor = new Color(Mathf.Min(baseColor.r * 1.25f, 1f), Mathf.Min(baseColor.g * 1.25f, 1f), Mathf.Min(baseColor.b * 1.25f, 1f), 1f);
            cb.pressedColor = new Color(baseColor.r * 0.75f, baseColor.g * 0.75f, baseColor.b * 0.75f, 1f);
            cb.selectedColor = cb.highlightedColor;
            cb.fadeDuration = 0.1f;
            btn.colors = cb;

            Text btnText = CreateText(btnObj.transform, "Label", label, 22, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, TextAnchor.MiddleCenter, Color.white, FontStyle.Bold);
            btnText.raycastTarget = false;

            return btn;
        }

        private static Slider CreateModernSlider(Transform parent, string name, string labelText, Vector2 anchoredPos, float defaultValue, Color fillGlowColor, out Text valueTextOutput)
        {
            // Container Row
            GameObject rowObj = new GameObject(name + "_Row");
            rowObj.transform.SetParent(parent, false);
            RectTransform rtRow = rowObj.AddComponent<RectTransform>();
            rtRow.anchorMin = new Vector2(0.5f, 0.5f);
            rtRow.anchorMax = new Vector2(0.5f, 0.5f);
            rtRow.anchoredPosition = anchoredPos;
            rtRow.sizeDelta = new Vector2(620, 62);

            // Subtle frosted background for the row
            Image rowBg = rowObj.AddComponent<Image>();
            rowBg.color = new Color(0.10f, 0.14f, 0.22f, 0.75f);
            rowBg.raycastTarget = false;

            // 1. Category Label on Left
            CreateText(rowObj.transform, "Label", labelText, 20,
                new Vector2(0f, 0.5f), new Vector2(0f, 0.5f),
                new Vector2(115, 0), new Vector2(210, 40),
                TextAnchor.MiddleLeft, new Color(0.92f, 0.95f, 1f), FontStyle.Bold);

            // 2. Interactive Slider in Middle
            GameObject sliderObj = new GameObject(name);
            sliderObj.transform.SetParent(rowObj.transform, false);
            RectTransform rtSlider = sliderObj.AddComponent<RectTransform>();
            rtSlider.anchorMin = new Vector2(0f, 0.5f);
            rtSlider.anchorMax = new Vector2(0f, 0.5f);
            rtSlider.anchoredPosition = new Vector2(365, 0);
            rtSlider.sizeDelta = new Vector2(230, 32);

            Slider slider = sliderObj.AddComponent<Slider>();
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.direction = Slider.Direction.LeftToRight;
            slider.wholeNumbers = false;

            // Groove Track Background
            GameObject bgObj = new GameObject("Background");
            bgObj.transform.SetParent(sliderObj.transform, false);
            RectTransform rtBg = bgObj.AddComponent<RectTransform>();
            rtBg.anchorMin = new Vector2(0f, 0.25f);
            rtBg.anchorMax = new Vector2(1f, 0.75f);
            rtBg.offsetMin = Vector2.zero;
            rtBg.offsetMax = Vector2.zero;
            Image bgImg = bgObj.AddComponent<Image>();
            bgImg.color = new Color(0.04f, 0.06f, 0.10f, 1f);
            bgImg.raycastTarget = true; // Allows clicking directly on the track!

            // Fill Area
            GameObject fillArea = new GameObject("Fill Area");
            fillArea.transform.SetParent(sliderObj.transform, false);
            RectTransform rtFillArea = fillArea.AddComponent<RectTransform>();
            rtFillArea.anchorMin = new Vector2(0f, 0.25f);
            rtFillArea.anchorMax = new Vector2(1f, 0.75f);
            rtFillArea.offsetMin = new Vector2(6, 0);
            rtFillArea.offsetMax = new Vector2(-6, 0);

            // Fill Bar
            GameObject fill = new GameObject("Fill");
            fill.transform.SetParent(fillArea.transform, false);
            RectTransform rtFill = fill.AddComponent<RectTransform>();
            rtFill.anchorMin = Vector2.zero;
            rtFill.anchorMax = new Vector2(0, 1);
            rtFill.sizeDelta = Vector2.zero;
            Image fillImg = fill.AddComponent<Image>();
            fillImg.color = fillGlowColor;
            fillImg.raycastTarget = false;

            // Handle Slide Area
            GameObject handleArea = new GameObject("Handle Slide Area");
            handleArea.transform.SetParent(sliderObj.transform, false);
            RectTransform rtHandleArea = handleArea.AddComponent<RectTransform>();
            rtHandleArea.anchorMin = Vector2.zero;
            rtHandleArea.anchorMax = Vector2.one;
            rtHandleArea.offsetMin = new Vector2(12, 0);
            rtHandleArea.offsetMax = new Vector2(-12, 0);

            // Handle Knob
            GameObject handle = new GameObject("Handle");
            handle.transform.SetParent(handleArea.transform, false);
            RectTransform rtHandle = handle.AddComponent<RectTransform>();
            rtHandle.sizeDelta = new Vector2(30, 30);
            Image handleImg = handle.AddComponent<Image>();
            handleImg.color = Color.white;
            handleImg.raycastTarget = true; // Allows direct dragging

            // Wire Slider Components
            slider.fillRect = rtFill;
            slider.handleRect = rtHandle;
            slider.targetGraphic = handleImg;
            ColorBlock cb = slider.colors;
            cb.normalColor = Color.white;
            cb.highlightedColor = new Color(0.85f, 0.95f, 1f);
            cb.pressedColor = fillGlowColor;
            slider.colors = cb;

            slider.value = defaultValue;

            // 3. Percentage Badge on Right
            GameObject badgeObj = new GameObject("ValueBadge");
            badgeObj.transform.SetParent(rowObj.transform, false);
            RectTransform rtBadge = badgeObj.AddComponent<RectTransform>();
            rtBadge.anchorMin = new Vector2(1f, 0.5f);
            rtBadge.anchorMax = new Vector2(1f, 0.5f);
            rtBadge.anchoredPosition = new Vector2(-46, 0);
            rtBadge.sizeDelta = new Vector2(76, 38);

            Image badgeBg = badgeObj.AddComponent<Image>();
            badgeBg.color = new Color(0.14f, 0.20f, 0.32f, 0.9f);
            badgeBg.raycastTarget = false;

            Text valText = CreateText(badgeObj.transform, "ValueText", $"{Mathf.RoundToInt(defaultValue * 100)}%", 18,
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero,
                TextAnchor.MiddleCenter, fillGlowColor, FontStyle.Bold);

            valueTextOutput = valText;
            return slider;
        }

        private static void AddCoreManagers()
        {
            GameObject managers = new GameObject("--- MANAGERS ---");
            managers.AddComponent<SceneController>();
            managers.AddComponent<GameScoreManager>();
            var audioMgr = managers.AddComponent<AudioManager>();

            // Auto link audio clips
            audioMgr.bgmClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Resources_Exam/Sounds/BGM/JumpyGame.wav");
            audioMgr.itemBonusClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Resources_Exam/Sounds/SFX/Coin Pickup 4.wav");
            audioMgr.itemPenaltyClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Resources_Exam/Sounds/SFX/Debuff Downgrade 1.wav");
            audioMgr.obstacleHitClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Resources_Exam/Sounds/SFX/Bounce_Jump 3.wav");
            audioMgr.winClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Resources_Exam/Sounds/SFX/Win sound 1.wav");
            audioMgr.gameOverClip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Resources_Exam/Sounds/SFX/Game Over 2.wav");
        }
        #endregion

        #region Scenes Creation
        private static void BuildAllEightScenes()
        {
            BuildMainMenuScene();
            BuildStageSelectScene();
            BuildStage1Scene();
            BuildStage2Scene();
            BuildOptionsScene();
            BuildCreditsScene();
            BuildWinScene();
            BuildGameOverScene();
        }

        private static void BuildMainMenuScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            SetupBaseScene(out Camera cam, out Light light);
            AddCoreManagers();

            GameObject canvas = CreateCanvas("Canvas_MainMenu");
            var menuUI = canvas.AddComponent<MainMenuUI>();

            // Centered Glassmorphism Card
            var card = CreateCardPanel(canvas.transform, "Card_Menu", new Vector2(760, 640), new Color(0f, 0.85f, 1f));

            // Header & Student Info
            CreateText(card, "Title", "PRACTICAL EXAMINATION", 36, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -52), new Vector2(700, 50), TextAnchor.MiddleCenter, Color.white, FontStyle.Bold);
            CreateText(card, "Course", "COURSE 958321 : GAME DEVELOPMENT 3", 17, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -92), new Vector2(700, 32), TextAnchor.MiddleCenter, new Color(0.2f, 0.8f, 1f), FontStyle.Bold);

            // Student Badge
            GameObject badge = new GameObject("StudentBadge");
            badge.transform.SetParent(card, false);
            RectTransform rtBadge = badge.AddComponent<RectTransform>();
            rtBadge.anchorMin = new Vector2(0.5f, 1f);
            rtBadge.anchorMax = new Vector2(0.5f, 1f);
            rtBadge.anchoredPosition = new Vector2(0, -138);
            rtBadge.sizeDelta = new Vector2(580, 44);
            badge.AddComponent<Image>().color = new Color(0.12f, 0.16f, 0.26f, 0.85f);
            CreateText(badge.transform, "Text", "STUDENT ID: 681810045   |   RATCHANON", 16, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, TextAnchor.MiddleCenter, new Color(0.85f, 0.9f, 1f), FontStyle.Bold);

            // Action Buttons
            Button btnStart = CreateModernButton(card, "Btn_Start", "▶ Start Game", new Vector2(0, 48), new Vector2(400, 62), new Color(0.06f, 0.72f, 0.44f));      // Cyber Emerald
            Button btnOptions = CreateModernButton(card, "Btn_Options", "⚙ Options", new Vector2(0, -28), new Vector2(400, 62), new Color(0.05f, 0.60f, 0.90f));     // Electric Cyan
            Button btnCredits = CreateModernButton(card, "Btn_Credits", "★ Credits", new Vector2(0, -104), new Vector2(400, 62), new Color(0.50f, 0.32f, 0.88f));     // Violet
            Button btnExit = CreateModernButton(card, "Btn_Exit", "✕ Exit Game", new Vector2(0, -180), new Vector2(400, 62), new Color(0.85f, 0.22f, 0.25f));        // Crimson

            // Footer
            CreateText(card, "Footer", "College of Arts, Media and Technology - CMU", 14, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0, 22), new Vector2(500, 28), TextAnchor.MiddleCenter, new Color(0.55f, 0.65f, 0.8f));

            var serializedUI = new SerializedObject(menuUI);
            serializedUI.FindProperty("startButton").objectReferenceValue = btnStart;
            serializedUI.FindProperty("optionsButton").objectReferenceValue = btnOptions;
            serializedUI.FindProperty("creditsButton").objectReferenceValue = btnCredits;
            serializedUI.FindProperty("exitButton").objectReferenceValue = btnExit;
            serializedUI.ApplyModifiedProperties();

            EditorSceneManager.SaveScene(scene, $"{SCENES_DIR}/01_MainMenu.unity");
        }

        private static void BuildStageSelectScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            SetupBaseScene(out Camera cam, out Light light);
            AddCoreManagers();

            GameObject canvas = CreateCanvas("Canvas_StageSelect");
            var stageUI = canvas.AddComponent<StageSelectUI>();

            var card = CreateCardPanel(canvas.transform, "Card_StageSelect", new Vector2(760, 580), new Color(0.05f, 0.60f, 0.90f));

            CreateText(card, "Title", "STAGE SELECTION", 36, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -55), new Vector2(700, 50), TextAnchor.MiddleCenter, Color.white, FontStyle.Bold);
            CreateText(card, "Subtitle", "Select a minigame stage to begin challenge", 18, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -98), new Vector2(700, 35), TextAnchor.MiddleCenter, new Color(0.7f, 0.8f, 0.95f));

            Button btnStage1 = CreateModernButton(card, "Btn_Stage1", "▶ Stage 1 : Capsule Runner", new Vector2(0, 50), new Vector2(440, 68), new Color(0.05f, 0.60f, 0.90f));
            Button btnStage2 = CreateModernButton(card, "Btn_Stage2", "▶ Stage 2 : Humanoid Minigame", new Vector2(0, -32), new Vector2(440, 68), new Color(0.92f, 0.55f, 0.08f));
            Button btnBack = CreateModernButton(card, "Btn_Back", "◀ Back to Main Menu", new Vector2(0, -135), new Vector2(320, 54), new Color(0.24f, 0.30f, 0.42f));

            var serializedUI = new SerializedObject(stageUI);
            serializedUI.FindProperty("stage1Button").objectReferenceValue = btnStage1;
            serializedUI.FindProperty("stage2Button").objectReferenceValue = btnStage2;
            serializedUI.FindProperty("backButton").objectReferenceValue = btnBack;
            serializedUI.ApplyModifiedProperties();

            EditorSceneManager.SaveScene(scene, $"{SCENES_DIR}/02_StageSelect.unity");
        }

        private static void BuildStage1Scene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            SetupBaseScene(out Camera cam, out Light light);
            AddCoreManagers();

            // Build Enhanced 3D Level
            BuildStage1LevelDesign();

            // Spawn Capsule Player
            GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{PREFABS_DIR}/CapsulePlayer.prefab");
            GameObject playerInstance = (GameObject)PrefabUtility.InstantiatePrefab(playerPrefab);
            playerInstance.transform.position = new Vector3(0f, 1.5f, 0f);

            // Camera Tracking
            var camFollow = cam.gameObject.AddComponent<CameraController>();
            camFollow.SetTarget(playerInstance.transform);

            // Kill Plane (Abyss Fall Zone)
            GameObject fallZone = new GameObject("FallZone_KillPlane");
            fallZone.transform.position = new Vector3(0f, -8f, 30f);
            BoxCollider fallCol = fallZone.AddComponent<BoxCollider>();
            fallCol.size = new Vector3(300f, 2f, 300f);
            fallCol.isTrigger = true;
            fallZone.AddComponent<FallZone>();

            // HUD Canvas with Modern Sleek Top Bar
            GameObject canvas = CreateCanvas("Canvas_Stage1_HUD");
            var hud = canvas.AddComponent<HUDController>();
            var stage1Mgr = canvas.AddComponent<Stage1Manager>();

            // Floating Top HUD Panel
            GameObject topPanel = new GameObject("Panel_HUD_Top");
            topPanel.transform.SetParent(canvas.transform, false);
            RectTransform rtTop = topPanel.AddComponent<RectTransform>();
            rtTop.anchorMin = new Vector2(0f, 1f);
            rtTop.anchorMax = new Vector2(1f, 1f);
            rtTop.anchoredPosition = new Vector2(0, -48);
            rtTop.sizeDelta = new Vector2(0, 88);

            Image imgTop = topPanel.AddComponent<Image>();
            imgTop.color = new Color(0.06f, 0.08f, 0.14f, 0.88f);
            imgTop.raycastTarget = false;

            // Timer Widget (Left)
            GameObject timerBox = new GameObject("Box_Timer");
            timerBox.transform.SetParent(topPanel.transform, false);
            RectTransform rtTimer = timerBox.AddComponent<RectTransform>();
            rtTimer.anchorMin = new Vector2(0f, 0.5f);
            rtTimer.anchorMax = new Vector2(0f, 0.5f);
            rtTimer.anchoredPosition = new Vector2(170, 0);
            rtTimer.sizeDelta = new Vector2(280, 58);
            timerBox.AddComponent<Image>().color = new Color(0.12f, 0.16f, 0.25f, 0.9f);

            Text timerText = CreateText(timerBox.transform, "Txt_Timer", "TIME: 30.0s", 26, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, TextAnchor.MiddleCenter, new Color(0.2f, 1f, 0.5f), FontStyle.Bold);

            // Score Widget (Center)
            GameObject scoreBox = new GameObject("Box_Score");
            scoreBox.transform.SetParent(topPanel.transform, false);
            RectTransform rtScore = scoreBox.AddComponent<RectTransform>();
            rtScore.anchorMin = new Vector2(0.5f, 0.5f);
            rtScore.anchorMax = new Vector2(0.5f, 0.5f);
            rtScore.anchoredPosition = Vector2.zero;
            rtScore.sizeDelta = new Vector2(460, 58);
            scoreBox.AddComponent<Image>().color = new Color(0.12f, 0.16f, 0.25f, 0.9f);

            Text scoreText = CreateText(scoreBox.transform, "Txt_Score", "SCORE: 0  / >10,000", 24, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, TextAnchor.MiddleCenter, Color.white, FontStyle.Bold);

            // Stage Label & Back Button (Right)
            Text labelText = CreateText(topPanel.transform, "Txt_StageLabel", "STAGE 1 : CAPSULE RUNNER", 16, new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(-220, 0), new Vector2(250, 40), TextAnchor.MiddleRight, new Color(0.7f, 0.85f, 1f), FontStyle.Bold);
            
            // Menu Button anchored to right
            GameObject btnBackObj = new GameObject("Btn_Back");
            btnBackObj.transform.SetParent(topPanel.transform, false);
            RectTransform rtBack = btnBackObj.AddComponent<RectTransform>();
            rtBack.anchorMin = new Vector2(1f, 0.5f);
            rtBack.anchorMax = new Vector2(1f, 0.5f);
            rtBack.anchoredPosition = new Vector2(-70, 0);
            rtBack.sizeDelta = new Vector2(95, 48);
            Image btnBackImg = btnBackObj.AddComponent<Image>();
            btnBackImg.color = new Color(0.25f, 0.32f, 0.44f);
            Button btnBack = btnBackObj.AddComponent<Button>();
            CreateText(btnBackObj.transform, "Label", "Menu", 20, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, TextAnchor.MiddleCenter, Color.white, FontStyle.Bold);

            var serializedHud = new SerializedObject(hud);
            serializedHud.FindProperty("timerText").objectReferenceValue = timerText;
            serializedHud.FindProperty("scoreText").objectReferenceValue = scoreText;
            serializedHud.FindProperty("stageLabelText").objectReferenceValue = labelText;
            serializedHud.FindProperty("backButton").objectReferenceValue = btnBack;
            serializedHud.ApplyModifiedProperties();

            stage1Mgr.SetHUDReferences(timerText, scoreText, labelText);

            EditorSceneManager.SaveScene(scene, $"{SCENES_DIR}/03_Stage1.unity");
        }

        private static void BuildStage1LevelDesign()
        {
            GameObject levelRoot = new GameObject("Level_Stage1");
            Material matPlatform = AssetDatabase.LoadAssetAtPath<Material>($"{MATERIALS_DIR}/Mat_Platform.mat");
            Material matNarrow = AssetDatabase.LoadAssetAtPath<Material>($"{MATERIALS_DIR}/Mat_NarrowPath.mat");
            Material matAccent = AssetDatabase.LoadAssetAtPath<Material>($"{MATERIALS_DIR}/Mat_PlatformAccent.mat");

            // Platform 1: Start (Wide 14x14)
            CreatePlatform(levelRoot.transform, "Platform_Start", new Vector3(0, 0, 0), new Vector3(14, 1, 14), matPlatform, matAccent);

            // Bridge 1: Narrow Hazard Path (2.2x16)
            CreatePlatform(levelRoot.transform, "Bridge_Narrow_1", new Vector3(0, 0, 16), new Vector3(2.2f, 1, 18), matNarrow, null);

            // Platform 2: Mid Area (14x14)
            CreatePlatform(levelRoot.transform, "Platform_Mid", new Vector3(0, 0, 33), new Vector3(14, 1, 14), matPlatform, matAccent);

            // Bridge 2: Turn Right Narrow Hazard Path (14x2.2)
            CreatePlatform(levelRoot.transform, "Bridge_Turn_Right", new Vector3(14, 0, 33), new Vector3(14, 1, 2.2f), matNarrow, null);

            // Platform 3: East Platform (12x12)
            CreatePlatform(levelRoot.transform, "Platform_East", new Vector3(26, 0, 33), new Vector3(12, 1, 12), matPlatform, matAccent);

            // Bridge 3: Narrow Forward Path (2.2x16)
            CreatePlatform(levelRoot.transform, "Bridge_Narrow_3", new Vector3(26, 0, 48), new Vector3(2.2f, 1, 18), matNarrow, null);

            // Platform 4: End Arena (16x16)
            CreatePlatform(levelRoot.transform, "Platform_EndArena", new Vector3(26, 0, 66), new Vector3(16, 1, 16), matPlatform, matAccent);

            // Items and Obstacles
            SpawnStage1ItemsAndObstacles(levelRoot.transform);
        }

        private static void CreatePlatform(Transform parent, string name, Vector3 pos, Vector3 size, Material baseMat, Material trimMat)
        {
            GameObject plat = GameObject.CreatePrimitive(PrimitiveType.Cube);
            plat.name = name;
            plat.transform.SetParent(parent, false);
            plat.transform.position = pos;
            plat.transform.localScale = size;
            if (baseMat != null) plat.GetComponent<Renderer>().sharedMaterial = baseMat;

            // Optional top border accent trim
            if (trimMat != null)
            {
                GameObject trim = GameObject.CreatePrimitive(PrimitiveType.Cube);
                trim.name = "Trim";
                trim.transform.SetParent(plat.transform, false);
                trim.transform.localPosition = new Vector3(0f, 0.51f, 0f);
                trim.transform.localScale = new Vector3(1.01f, 0.04f, 1.01f);
                Collider trimCol = trim.GetComponent<Collider>();
                if (trimCol != null) Object.DestroyImmediate(trimCol);
                trim.GetComponent<Renderer>().sharedMaterial = trimMat;
            }
        }

        private static void SpawnStage1ItemsAndObstacles(Transform parent)
        {
            GameObject itemBonusPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{PREFABS_DIR}/Item_ScoreBonus.prefab");
            GameObject itemScorePenaltyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{PREFABS_DIR}/Item_ScorePenalty.prefab");
            GameObject itemTimeBonusPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{PREFABS_DIR}/Item_TimeBonus.prefab");
            GameObject itemTimePenaltyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{PREFABS_DIR}/Item_TimePenalty.prefab");
            GameObject obstaclePrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{PREFABS_DIR}/Obstacle_Hazard.prefab");

            GameObject itemContainer = new GameObject("Items_Container");
            itemContainer.transform.SetParent(parent, false);

            // Start Platform (+2,000 pts, +3s)
            SpawnItem(itemBonusPrefab, itemContainer.transform, new Vector3(-3, 1.4f, 3));
            SpawnItem(itemBonusPrefab, itemContainer.transform, new Vector3(3, 1.4f, 3));
            SpawnItem(itemBonusPrefab, itemContainer.transform, new Vector3(-3, 1.4f, -3));
            SpawnItem(itemBonusPrefab, itemContainer.transform, new Vector3(3, 1.4f, -3));
            SpawnItem(itemTimeBonusPrefab, itemContainer.transform, new Vector3(0, 1.4f, 4));

            // Bridge 1 (Items + Obstacle on narrow path)
            SpawnItem(itemBonusPrefab, itemContainer.transform, new Vector3(0, 1.4f, 11));
            SpawnItem(itemBonusPrefab, itemContainer.transform, new Vector3(0, 1.4f, 15));
            SpawnItem(obstaclePrefab, itemContainer.transform, new Vector3(0, 0f, 19));
            SpawnItem(itemBonusPrefab, itemContainer.transform, new Vector3(0, 1.4f, 23));

            // Platform 2 Mid (+3,000 pts, +6s, penalties)
            SpawnItem(itemBonusPrefab, itemContainer.transform, new Vector3(-4, 1.4f, 31));
            SpawnItem(itemBonusPrefab, itemContainer.transform, new Vector3(4, 1.4f, 31));
            SpawnItem(itemBonusPrefab, itemContainer.transform, new Vector3(-4, 1.4f, 36));
            SpawnItem(itemBonusPrefab, itemContainer.transform, new Vector3(4, 1.4f, 36));
            SpawnItem(itemBonusPrefab, itemContainer.transform, new Vector3(0, 1.4f, 38));
            SpawnItem(itemTimeBonusPrefab, itemContainer.transform, new Vector3(-3, 1.4f, 34));
            SpawnItem(itemTimeBonusPrefab, itemContainer.transform, new Vector3(3, 1.4f, 34));
            SpawnItem(itemTimePenaltyPrefab, itemContainer.transform, new Vector3(0, 1.4f, 30));
            SpawnItem(itemScorePenaltyPrefab, itemContainer.transform, new Vector3(0, 1.4f, 35));

            // Bridge 2 Turn Right (Items + Obstacle)
            SpawnItem(itemBonusPrefab, itemContainer.transform, new Vector3(9, 1.4f, 33));
            SpawnItem(obstaclePrefab, itemContainer.transform, new Vector3(14, 0f, 33));
            SpawnItem(itemBonusPrefab, itemContainer.transform, new Vector3(19, 1.4f, 33));

            // Platform 3 East (+3,000 pts, +3s)
            SpawnItem(itemBonusPrefab, itemContainer.transform, new Vector3(23, 1.4f, 30));
            SpawnItem(itemBonusPrefab, itemContainer.transform, new Vector3(29, 1.4f, 30));
            SpawnItem(itemBonusPrefab, itemContainer.transform, new Vector3(23, 1.4f, 36));
            SpawnItem(itemBonusPrefab, itemContainer.transform, new Vector3(29, 1.4f, 36));
            SpawnItem(itemBonusPrefab, itemContainer.transform, new Vector3(26, 1.4f, 31));
            SpawnItem(itemBonusPrefab, itemContainer.transform, new Vector3(26, 1.4f, 35));
            SpawnItem(itemTimeBonusPrefab, itemContainer.transform, new Vector3(26, 1.4f, 38));

            // Bridge 3 Narrow Forward (Items + Obstacle)
            SpawnItem(itemBonusPrefab, itemContainer.transform, new Vector3(26, 1.4f, 43));
            SpawnItem(obstaclePrefab, itemContainer.transform, new Vector3(26, 0f, 48));
            SpawnItem(itemBonusPrefab, itemContainer.transform, new Vector3(26, 1.4f, 53));

            // End Arena (+5,000 pts)
            for (int x = -5; x <= 5; x += 3)
            {
                for (int z = -4; z <= 4; z += 3)
                {
                    Vector3 pos = new Vector3(26 + x, 1.4f, 66 + z);
                    SpawnItem(itemBonusPrefab, itemContainer.transform, pos);
                }
            }
            SpawnItem(itemTimeBonusPrefab, itemContainer.transform, new Vector3(26, 1.4f, 72));
            SpawnItem(itemScorePenaltyPrefab, itemContainer.transform, new Vector3(22, 1.4f, 69));
            SpawnItem(itemScorePenaltyPrefab, itemContainer.transform, new Vector3(30, 1.4f, 69));
        }

        private static void SpawnItem(GameObject prefab, Transform parent, Vector3 pos)
        {
            if (prefab == null) return;
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            instance.transform.SetParent(parent, false);
            instance.transform.position = pos;
        }

        private static void BuildStage2Scene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            SetupBaseScene(out Camera cam, out Light light);
            AddCoreManagers();

            // Build ProBuilder Level for Stage 2 (Q.7, Area >= 50 sq.m.)
            BuildStage2ProBuilderLevel();

            // Spawn Humanoid Player (Q.6)
            GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{PREFABS_DIR}/HumanoidPlayer.prefab");
            GameObject playerInstance = (GameObject)PrefabUtility.InstantiatePrefab(playerPrefab);
            playerInstance.transform.position = new Vector3(0f, 1.5f, 0f);

            // Camera Tracking
            var camFollow = cam.gameObject.AddComponent<CameraController>();
            camFollow.SetTarget(playerInstance.transform);

            // Kill Plane (Abyss Fall Zone)
            GameObject fallZone = new GameObject("FallZone_KillPlane");
            fallZone.transform.position = new Vector3(0f, -10f, 0f);
            BoxCollider fallCol = fallZone.AddComponent<BoxCollider>();
            fallCol.size = new Vector3(300f, 2f, 300f);
            fallCol.isTrigger = true;
            fallZone.AddComponent<FallZone>();

            // HUD Canvas with Modern Sleek Top Bar & Bottom Interaction Prompt Badge
            GameObject canvas = CreateCanvas("Canvas_Stage2_HUD");
            var hud = canvas.AddComponent<HUDController>();
            var stage2Mgr = canvas.AddComponent<Stage2Manager>();

            // Floating Top HUD Panel
            GameObject topPanel = new GameObject("Panel_HUD_Top");
            topPanel.transform.SetParent(canvas.transform, false);
            RectTransform rtTop = topPanel.AddComponent<RectTransform>();
            rtTop.anchorMin = new Vector2(0f, 1f);
            rtTop.anchorMax = new Vector2(1f, 1f);
            rtTop.anchoredPosition = new Vector2(0, -48);
            rtTop.sizeDelta = new Vector2(0, 88);

            Image imgTop = topPanel.AddComponent<Image>();
            imgTop.color = new Color(0.06f, 0.08f, 0.14f, 0.88f);
            imgTop.raycastTarget = false;

            // Timer Widget (Left)
            GameObject timerBox = new GameObject("Box_Timer");
            timerBox.transform.SetParent(topPanel.transform, false);
            RectTransform rtTimer = timerBox.AddComponent<RectTransform>();
            rtTimer.anchorMin = new Vector2(0f, 0.5f);
            rtTimer.anchorMax = new Vector2(0f, 0.5f);
            rtTimer.anchoredPosition = new Vector2(170, 0);
            rtTimer.sizeDelta = new Vector2(280, 58);
            timerBox.AddComponent<Image>().color = new Color(0.12f, 0.16f, 0.25f, 0.9f);

            Text timerText = CreateText(timerBox.transform, "Txt_Timer", "TIME: 30.0s", 26, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, TextAnchor.MiddleCenter, new Color(0.2f, 1f, 0.5f), FontStyle.Bold);

            // Score Widget (Center)
            GameObject scoreBox = new GameObject("Box_Score");
            scoreBox.transform.SetParent(topPanel.transform, false);
            RectTransform rtScore = scoreBox.AddComponent<RectTransform>();
            rtScore.anchorMin = new Vector2(0.5f, 0.5f);
            rtScore.anchorMax = new Vector2(0.5f, 0.5f);
            rtScore.anchoredPosition = Vector2.zero;
            rtScore.sizeDelta = new Vector2(460, 58);
            scoreBox.AddComponent<Image>().color = new Color(0.12f, 0.16f, 0.25f, 0.9f);

            Text scoreText = CreateText(scoreBox.transform, "Txt_Score", "SCORE: 0  / >10,000", 24, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, TextAnchor.MiddleCenter, Color.white, FontStyle.Bold);

            // Stage Label & Back Button (Right)
            Text labelText = CreateText(topPanel.transform, "Txt_StageLabel", "STAGE 2 : HUMANOID MINIGAME", 16, new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(-220, 0), new Vector2(250, 40), TextAnchor.MiddleRight, new Color(1f, 0.85f, 0.5f), FontStyle.Bold);

            // Menu Button anchored to right
            GameObject btnBackObj = new GameObject("Btn_Back");
            btnBackObj.transform.SetParent(topPanel.transform, false);
            RectTransform rtBack = btnBackObj.AddComponent<RectTransform>();
            rtBack.anchorMin = new Vector2(1f, 0.5f);
            rtBack.anchorMax = new Vector2(1f, 0.5f);
            rtBack.anchoredPosition = new Vector2(-70, 0);
            rtBack.sizeDelta = new Vector2(95, 48);
            Image btnBackImg = btnBackObj.AddComponent<Image>();
            btnBackImg.color = new Color(0.25f, 0.32f, 0.44f);
            Button btnBack = btnBackObj.AddComponent<Button>();
            CreateText(btnBackObj.transform, "Label", "Menu", 20, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, TextAnchor.MiddleCenter, Color.white, FontStyle.Bold);

            // Bottom Interaction Prompt Panel
            GameObject promptPanel = new GameObject("Panel_InteractionPrompt");
            promptPanel.transform.SetParent(canvas.transform, false);
            RectTransform rtPrompt = promptPanel.AddComponent<RectTransform>();
            rtPrompt.anchorMin = new Vector2(0.5f, 0f);
            rtPrompt.anchorMax = new Vector2(0.5f, 0f);
            rtPrompt.anchoredPosition = new Vector2(0, 65);
            rtPrompt.sizeDelta = new Vector2(560, 64);

            Image imgPrompt = promptPanel.AddComponent<Image>();
            imgPrompt.color = new Color(0.06f, 0.08f, 0.14f, 0.94f);

            // Glowing border for prompt panel
            GameObject promptBorder = new GameObject("Border");
            promptBorder.transform.SetParent(promptPanel.transform, false);
            RectTransform rtBorder = promptBorder.AddComponent<RectTransform>();
            rtBorder.anchorMin = Vector2.zero;
            rtBorder.anchorMax = Vector2.one;
            rtBorder.sizeDelta = Vector2.zero;
            Image imgBorder = promptBorder.AddComponent<Image>();
            imgBorder.color = new Color(0f, 0.85f, 1f, 0.3f);
            imgBorder.raycastTarget = false;

            Text promptText = CreateText(promptPanel.transform, "Txt_Prompt", "[E] Collect Item", 22, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, TextAnchor.MiddleCenter, new Color(1f, 0.92f, 0.35f), FontStyle.Bold);

            var serializedHud = new SerializedObject(hud);
            serializedHud.FindProperty("timerText").objectReferenceValue = timerText;
            serializedHud.FindProperty("scoreText").objectReferenceValue = scoreText;
            serializedHud.FindProperty("stageLabelText").objectReferenceValue = labelText;
            serializedHud.FindProperty("backButton").objectReferenceValue = btnBack;
            serializedHud.ApplyModifiedProperties();

            stage2Mgr.SetHUDReferences(timerText, scoreText, labelText, promptPanel, promptText);

            EditorSceneManager.SaveScene(scene, $"{SCENES_DIR}/04_Stage2.unity");
        }

        private static void BuildStage2ProBuilderLevel()
        {
            GameObject levelRoot = new GameObject("Level_Stage2");
            Material matPlatform = AssetDatabase.LoadAssetAtPath<Material>($"{MATERIALS_DIR}/Mat_Platform.mat");
            Material matNarrow = AssetDatabase.LoadAssetAtPath<Material>($"{MATERIALS_DIR}/Mat_NarrowPath.mat");
            Material matAccent = AssetDatabase.LoadAssetAtPath<Material>($"{MATERIALS_DIR}/Mat_PlatformAccent.mat");

            // ProBuilder Arena Level Design (Total area > 500 sq.m., well above >= 50 sq.m. requirement in Q.7)
            // 1. Central Arena Platform (18x18 = 324 sq.m.)
            CreateProBuilderPlatform(levelRoot.transform, "Arena_Center", new Vector3(0, 0, 0), new Vector3(18, 1, 18), matPlatform, matAccent);

            // 2. North Elevated Terrace (14x8 = 112 sq.m.) at Y = 1.5
            CreateProBuilderPlatform(levelRoot.transform, "Terrace_North", new Vector3(0, 1.5f, 16), new Vector3(14, 1, 8), matPlatform, matAccent);

            // 3. Central Ramp to North Terrace
            GameObject ramp = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ramp.name = "Ramp_North";
            ramp.transform.SetParent(levelRoot.transform, false);
            ramp.transform.position = new Vector3(0, 0.75f, 10.5f);
            ramp.transform.rotation = Quaternion.Euler(-18f, 0f, 0f);
            ramp.transform.localScale = new Vector3(4.5f, 0.5f, 4.8f);
            if (matNarrow != null) ramp.GetComponent<Renderer>().sharedMaterial = matNarrow;
            try { ramp.AddComponent<UnityEngine.ProBuilder.ProBuilderMesh>(); } catch { }

            // 4. East Wing Platform (10x8 = 80 sq.m.)
            CreateProBuilderPlatform(levelRoot.transform, "Wing_East", new Vector3(18, 0, 0), new Vector3(10, 1, 8), matPlatform, matAccent);

            // Bridge East (Connecting Main to East Wing)
            CreateProBuilderPlatform(levelRoot.transform, "Bridge_East", new Vector3(11f, 0, 0), new Vector3(4f, 0.95f, 2.5f), matNarrow, null);

            // 5. West Wing Platform (10x8 = 80 sq.m.)
            CreateProBuilderPlatform(levelRoot.transform, "Wing_West", new Vector3(-18, 0, 0), new Vector3(10, 1, 8), matPlatform, matAccent);

            // Bridge West (Connecting Main to West Wing)
            CreateProBuilderPlatform(levelRoot.transform, "Bridge_West", new Vector3(-11f, 0, 0), new Vector3(4f, 0.95f, 2.5f), matNarrow, null);

            // Items and Obstacles Placement
            SpawnStage2ItemsAndObstacles(levelRoot.transform);
        }

        private static void CreateProBuilderPlatform(Transform parent, string name, Vector3 pos, Vector3 size, Material baseMat, Material trimMat)
        {
            GameObject plat = GameObject.CreatePrimitive(PrimitiveType.Cube);
            plat.name = name;
            plat.transform.SetParent(parent, false);
            plat.transform.position = pos;
            plat.transform.localScale = size;
            if (baseMat != null) plat.GetComponent<Renderer>().sharedMaterial = baseMat;

            // Attach ProBuilderMesh so it's registered as a ProBuilder object in the scene
            try
            {
                var pbMesh = plat.GetComponent<UnityEngine.ProBuilder.ProBuilderMesh>();
                if (pbMesh == null)
                {
                    pbMesh = plat.AddComponent<UnityEngine.ProBuilder.ProBuilderMesh>();
                }
            }
            catch { }

            // Optional top border accent trim
            if (trimMat != null)
            {
                GameObject trim = GameObject.CreatePrimitive(PrimitiveType.Cube);
                trim.name = "Trim";
                trim.transform.SetParent(plat.transform, false);
                trim.transform.localPosition = new Vector3(0f, 0.51f, 0f);
                trim.transform.localScale = new Vector3(1.01f, 0.04f, 1.01f);
                Collider trimCol = trim.GetComponent<Collider>();
                if (trimCol != null) Object.DestroyImmediate(trimCol);
                trim.GetComponent<Renderer>().sharedMaterial = trimMat;
            }
        }

        private static void SpawnStage2ItemsAndObstacles(Transform parent)
        {
            GameObject itemScoreBonusPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{PREFABS_DIR}/Item_Interact_ScoreBonus.prefab");
            GameObject itemScorePenaltyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{PREFABS_DIR}/Item_Interact_ScorePenalty.prefab");
            GameObject itemTimeBonusPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{PREFABS_DIR}/Item_Interact_TimeBonus.prefab");
            GameObject itemTimePenaltyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{PREFABS_DIR}/Item_Interact_TimePenalty.prefab");
            GameObject obstaclePrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{PREFABS_DIR}/Obstacle_Hazard.prefab");

            GameObject itemContainer = new GameObject("Items_Stage2_Container");
            itemContainer.transform.SetParent(parent, false);

            // 1. Central Arena: 10 Score Bonuses (+5,000 pts), 2 Time Bonuses (+6s), 2 Hazards, 4 Obstacles
            SpawnItem(itemScoreBonusPrefab, itemContainer.transform, new Vector3(-5, 1.4f, -5));
            SpawnItem(itemScoreBonusPrefab, itemContainer.transform, new Vector3(5, 1.4f, -5));
            SpawnItem(itemScoreBonusPrefab, itemContainer.transform, new Vector3(-5, 1.4f, 5));
            SpawnItem(itemScoreBonusPrefab, itemContainer.transform, new Vector3(5, 1.4f, 5));
            SpawnItem(itemScoreBonusPrefab, itemContainer.transform, new Vector3(-2, 1.4f, -2));
            SpawnItem(itemScoreBonusPrefab, itemContainer.transform, new Vector3(2, 1.4f, -2));
            SpawnItem(itemScoreBonusPrefab, itemContainer.transform, new Vector3(-2, 1.4f, 2));
            SpawnItem(itemScoreBonusPrefab, itemContainer.transform, new Vector3(2, 1.4f, 2));
            SpawnItem(itemScoreBonusPrefab, itemContainer.transform, new Vector3(0, 1.4f, -6));
            SpawnItem(itemScoreBonusPrefab, itemContainer.transform, new Vector3(0, 1.4f, 6));

            SpawnItem(itemTimeBonusPrefab, itemContainer.transform, new Vector3(-6, 1.4f, 0));
            SpawnItem(itemTimeBonusPrefab, itemContainer.transform, new Vector3(6, 1.4f, 0));

            SpawnItem(itemScorePenaltyPrefab, itemContainer.transform, new Vector3(-7, 1.4f, -7));
            SpawnItem(itemScorePenaltyPrefab, itemContainer.transform, new Vector3(7, 1.4f, -7));

            SpawnItem(obstaclePrefab, itemContainer.transform, new Vector3(-3.5f, 0.5f, -3.5f));
            SpawnItem(obstaclePrefab, itemContainer.transform, new Vector3(3.5f, 0.5f, -3.5f));
            SpawnItem(obstaclePrefab, itemContainer.transform, new Vector3(-3.5f, 0.5f, 3.5f));
            SpawnItem(obstaclePrefab, itemContainer.transform, new Vector3(3.5f, 0.5f, 3.5f));

            // 2. North Elevated Terrace: 6 Score Bonuses (+3,000 pts), 2 Time Bonuses (+6s), 1 Penalty, 2 Obstacles
            SpawnItem(itemScoreBonusPrefab, itemContainer.transform, new Vector3(-4, 2.9f, 15));
            SpawnItem(itemScoreBonusPrefab, itemContainer.transform, new Vector3(4, 2.9f, 15));
            SpawnItem(itemScoreBonusPrefab, itemContainer.transform, new Vector3(-4, 2.9f, 18));
            SpawnItem(itemScoreBonusPrefab, itemContainer.transform, new Vector3(4, 2.9f, 18));
            SpawnItem(itemScoreBonusPrefab, itemContainer.transform, new Vector3(0, 2.9f, 16));
            SpawnItem(itemScoreBonusPrefab, itemContainer.transform, new Vector3(0, 2.9f, 19));

            SpawnItem(itemTimeBonusPrefab, itemContainer.transform, new Vector3(-2, 2.9f, 17));
            SpawnItem(itemTimeBonusPrefab, itemContainer.transform, new Vector3(2, 2.9f, 17));

            SpawnItem(itemScorePenaltyPrefab, itemContainer.transform, new Vector3(-5, 2.9f, 19));
            SpawnItem(itemTimePenaltyPrefab, itemContainer.transform, new Vector3(5, 2.9f, 19));

            SpawnItem(obstaclePrefab, itemContainer.transform, new Vector3(-2f, 2.0f, 14.5f));
            SpawnItem(obstaclePrefab, itemContainer.transform, new Vector3(2f, 2.0f, 14.5f));

            // 3. East Wing: 4 Score Bonuses (+2,000 pts), 1 Time Bonus (+3s), 1 Score Penalty, 1 Time Penalty, 1 Obstacle
            SpawnItem(itemScoreBonusPrefab, itemContainer.transform, new Vector3(16, 1.4f, -2));
            SpawnItem(itemScoreBonusPrefab, itemContainer.transform, new Vector3(20, 1.4f, -2));
            SpawnItem(itemScoreBonusPrefab, itemContainer.transform, new Vector3(16, 1.4f, 2));
            SpawnItem(itemScoreBonusPrefab, itemContainer.transform, new Vector3(20, 1.4f, 2));

            SpawnItem(itemTimeBonusPrefab, itemContainer.transform, new Vector3(18, 1.4f, 0));
            SpawnItem(itemScorePenaltyPrefab, itemContainer.transform, new Vector3(21, 1.4f, 0));
            SpawnItem(itemTimePenaltyPrefab, itemContainer.transform, new Vector3(11, 1.4f, 0)); // on narrow bridge

            SpawnItem(obstaclePrefab, itemContainer.transform, new Vector3(18f, 0.5f, -3f));

            // 4. West Wing: 4 Score Bonuses (+2,000 pts), 1 Time Bonus (+3s), 2 Penalties, 1 Obstacle
            SpawnItem(itemScoreBonusPrefab, itemContainer.transform, new Vector3(-16, 1.4f, -2));
            SpawnItem(itemScoreBonusPrefab, itemContainer.transform, new Vector3(-20, 1.4f, -2));
            SpawnItem(itemScoreBonusPrefab, itemContainer.transform, new Vector3(-16, 1.4f, 2));
            SpawnItem(itemScoreBonusPrefab, itemContainer.transform, new Vector3(-20, 1.4f, 2));

            SpawnItem(itemTimeBonusPrefab, itemContainer.transform, new Vector3(-18, 1.4f, 0));
            SpawnItem(itemScorePenaltyPrefab, itemContainer.transform, new Vector3(-21, 1.4f, 0));
            SpawnItem(itemTimePenaltyPrefab, itemContainer.transform, new Vector3(-11, 1.4f, 0)); // on narrow bridge

            SpawnItem(obstaclePrefab, itemContainer.transform, new Vector3(-18f, 0.5f, 3f));
            // Total Score Available: 24 * 500 = 12,000 pts (Achieves >10,000 pts Win Condition easily!)
        }

        private static void BuildOptionsScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            SetupBaseScene(out Camera cam, out Light light);
            AddCoreManagers();

            GameObject canvas = CreateCanvas("Canvas_Options");
            var optionsUI = canvas.AddComponent<OptionsUI>();

            var card = CreateCardPanel(canvas.transform, "Card_Options", new Vector2(760, 640), new Color(0.05f, 0.60f, 0.90f));

            // Header
            CreateText(card, "Title", "AUDIO SETTINGS", 36, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -52), new Vector2(700, 50), TextAnchor.MiddleCenter, Color.white, FontStyle.Bold);
            CreateText(card, "Subtitle", "Adjust volume levels for continuous BGM and interactive SFX", 17, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -92), new Vector2(700, 32), TextAnchor.MiddleCenter, new Color(0.7f, 0.85f, 1f));

            // Working Sliders with Realtime Percentage Badges
            Text txtMasterVal, txtBgmVal, txtSfxVal;
            Slider sMaster = CreateModernSlider(card, "Slider_Master", "🔊 Master Volume", new Vector2(0, 60), 1.0f, new Color(0.0f, 0.85f, 1.0f), out txtMasterVal);
            Slider sBGM = CreateModernSlider(card, "Slider_BGM", "🎵 BGM Music", new Vector2(0, -18), 0.8f, new Color(0.0f, 0.95f, 0.45f), out txtBgmVal);
            Slider sSFX = CreateModernSlider(card, "Slider_SFX", "💥 Sound Effects", new Vector2(0, -96), 1.0f, new Color(1.0f, 0.75f, 0.1f), out txtSfxVal);

            // Back Button
            Button btnBack = CreateModernButton(card, "Btn_Back", "◀ Save & Back", new Vector2(0, -185), new Vector2(300, 58), new Color(0.24f, 0.30f, 0.42f));

            // Wire UI component fields
            var serializedUI = new SerializedObject(optionsUI);
            serializedUI.FindProperty("masterSlider").objectReferenceValue = sMaster;
            serializedUI.FindProperty("bgmSlider").objectReferenceValue = sBGM;
            serializedUI.FindProperty("sfxSlider").objectReferenceValue = sSFX;
            serializedUI.FindProperty("masterValueText").objectReferenceValue = txtMasterVal;
            serializedUI.FindProperty("bgmValueText").objectReferenceValue = txtBgmVal;
            serializedUI.FindProperty("sfxValueText").objectReferenceValue = txtSfxVal;
            serializedUI.FindProperty("backButton").objectReferenceValue = btnBack;
            serializedUI.ApplyModifiedProperties();

            EditorSceneManager.SaveScene(scene, $"{SCENES_DIR}/05_Options.unity");
        }

        private static void BuildCreditsScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            SetupBaseScene(out Camera cam, out Light light);
            AddCoreManagers();

            GameObject canvas = CreateCanvas("Canvas_Credits");
            var creditsUI = canvas.AddComponent<CreditsUI>();

            var card = CreateCardPanel(canvas.transform, "Card_Credits", new Vector2(760, 640), new Color(0.50f, 0.32f, 0.88f));

            CreateText(card, "Title", "CREDITS & EXAM INFO", 36, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -52), new Vector2(700, 50), TextAnchor.MiddleCenter, Color.white, FontStyle.Bold);

            string creditsInfo = "Course: 958321 Game Development 3 (Practical Exam)\n\n" +
                                 "Student Code: 681810045\n\n" +
                                 "Student Name: Ratchanon\n\n" +
                                 "Department of Digital Game\n" +
                                 "College of Arts, Media and Technology\n" +
                                 "Chiang Mai University";

            CreateText(card, "Info", creditsInfo, 20, new Vector2(0.5f, 0.55f), new Vector2(0.5f, 0.55f), Vector2.zero, new Vector2(700, 260), TextAnchor.MiddleCenter, new Color(0.9f, 0.95f, 1f));

            Button btnBack = CreateModernButton(card, "Btn_Back", "◀ Back to Main Menu", new Vector2(0, -185), new Vector2(300, 58), new Color(0.24f, 0.30f, 0.42f));

            var serializedUI = new SerializedObject(creditsUI);
            serializedUI.FindProperty("backButton").objectReferenceValue = btnBack;
            serializedUI.ApplyModifiedProperties();

            EditorSceneManager.SaveScene(scene, $"{SCENES_DIR}/06_Credits.unity");
        }

        private static void BuildWinScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            SetupBaseScene(out Camera cam, out Light light);
            cam.backgroundColor = new Color(0.06f, 0.16f, 0.10f); // Subtle emerald tint
            AddCoreManagers();

            GameObject canvas = CreateCanvas("Canvas_Win");
            var resultUI = canvas.AddComponent<ResultSceneUI>();

            var card = CreateCardPanel(canvas.transform, "Card_Win", new Vector2(760, 620), new Color(0.06f, 0.72f, 0.44f));

            Text titleText = CreateText(card, "Txt_Title", "★ VICTORY! ★", 48, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -55), new Vector2(700, 55), TextAnchor.MiddleCenter, new Color(0.2f, 1f, 0.5f), FontStyle.Bold);
            CreateText(card, "Subtitle", "Target score (>10,000) successfully achieved!", 18, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -100), new Vector2(700, 35), TextAnchor.MiddleCenter, new Color(0.7f, 1f, 0.85f));

            Text scoreText = CreateText(card, "Txt_Score", "Score : 0", 32, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -150), new Vector2(500, 50), TextAnchor.MiddleCenter, new Color(1.0f, 0.85f, 0.2f), FontStyle.Bold);

            Button btnTryAgain = CreateModernButton(card, "Btn_TryAgain", "▶ Play Again", new Vector2(0, 15), new Vector2(340, 60), new Color(0.06f, 0.72f, 0.44f));
            Button btnStageSelect = CreateModernButton(card, "Btn_StageSelect", "☰ Stage Select", new Vector2(0, -55), new Vector2(340, 60), new Color(0.05f, 0.60f, 0.90f));
            Button btnMainMenu = CreateModernButton(card, "Btn_MainMenu", "◀ Main Menu", new Vector2(0, -125), new Vector2(340, 60), new Color(0.24f, 0.30f, 0.42f));

            var serializedUI = new SerializedObject(resultUI);
            serializedUI.FindProperty("titleLabel").objectReferenceValue = titleText;
            serializedUI.FindProperty("scoreLabel").objectReferenceValue = scoreText;
            serializedUI.FindProperty("tryAgainButton").objectReferenceValue = btnTryAgain;
            serializedUI.FindProperty("stageSelectButton").objectReferenceValue = btnStageSelect;
            serializedUI.FindProperty("mainMenuButton").objectReferenceValue = btnMainMenu;
            serializedUI.ApplyModifiedProperties();

            EditorSceneManager.SaveScene(scene, $"{SCENES_DIR}/07_Win.unity");
        }

        private static void BuildGameOverScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            SetupBaseScene(out Camera cam, out Light light);
            cam.backgroundColor = new Color(0.18f, 0.06f, 0.08f); // Subtle crimson tint
            AddCoreManagers();

            GameObject canvas = CreateCanvas("Canvas_GameOver");
            var resultUI = canvas.AddComponent<ResultSceneUI>();

            var card = CreateCardPanel(canvas.transform, "Card_GameOver", new Vector2(760, 620), new Color(0.85f, 0.22f, 0.25f));

            Text titleText = CreateText(card, "Txt_Title", "GAME OVER", 48, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -55), new Vector2(700, 55), TextAnchor.MiddleCenter, new Color(1f, 0.25f, 0.25f), FontStyle.Bold);
            CreateText(card, "Subtitle", "Time expired or fell from platform!", 18, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -100), new Vector2(700, 35), TextAnchor.MiddleCenter, new Color(1f, 0.7f, 0.7f));

            Text scoreText = CreateText(card, "Txt_Score", "Score : 0", 32, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -150), new Vector2(500, 50), TextAnchor.MiddleCenter, new Color(0.9f, 0.95f, 1f), FontStyle.Bold);

            Button btnTryAgain = CreateModernButton(card, "Btn_TryAgain", "↻ Try Again", new Vector2(0, 15), new Vector2(340, 60), new Color(0.85f, 0.22f, 0.25f));
            Button btnStageSelect = CreateModernButton(card, "Btn_StageSelect", "☰ Stage Select", new Vector2(0, -55), new Vector2(340, 60), new Color(0.05f, 0.60f, 0.90f));
            Button btnMainMenu = CreateModernButton(card, "Btn_MainMenu", "◀ Main Menu", new Vector2(0, -125), new Vector2(340, 60), new Color(0.24f, 0.30f, 0.42f));

            var serializedUI = new SerializedObject(resultUI);
            serializedUI.FindProperty("titleLabel").objectReferenceValue = titleText;
            serializedUI.FindProperty("scoreLabel").objectReferenceValue = scoreText;
            serializedUI.FindProperty("tryAgainButton").objectReferenceValue = btnTryAgain;
            serializedUI.FindProperty("stageSelectButton").objectReferenceValue = btnStageSelect;
            serializedUI.FindProperty("mainMenuButton").objectReferenceValue = btnMainMenu;
            serializedUI.ApplyModifiedProperties();

            EditorSceneManager.SaveScene(scene, $"{SCENES_DIR}/08_GameOver.unity");
        }
        #endregion

        #region Build Settings Update
        public static void UpdateEditorBuildSettings()
        {
            string[] sceneNames = new string[]
            {
                "01_MainMenu",
                "02_StageSelect",
                "03_Stage1",
                "04_Stage2",
                "05_Options",
                "06_Credits",
                "07_Win",
                "08_GameOver"
            };

            List<EditorBuildSettingsScene> buildScenes = new List<EditorBuildSettingsScene>();

            foreach (var sName in sceneNames)
            {
                string path = $"{SCENES_DIR}/{sName}.unity";
                buildScenes.Add(new EditorBuildSettingsScene(path, true));
            }

            EditorBuildSettings.scenes = buildScenes.ToArray();
            Debug.Log($"[ExamSetupTool] EditorBuildSettings updated with all {buildScenes.Count} scenes (Index 0 to 7)!");
        }
        #endregion
    }
}
#endif
