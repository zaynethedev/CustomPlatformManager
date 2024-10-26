using System;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using CustomPlatformManager.buttons;
using DevHoldableEngine;
using TMPro;
using UnityEngine;
using Utilla;

namespace CustomPlatformManager
{
    [ModdedGamemode]
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
    [BepInPlugin("zaynethedev.CustomPlatformManager", "CustomPlatformManager", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        public bool isFlip = false;
        public bool isEnabled;
        public bool inRoom;
        public bool platSetL;
        public bool platSetR;
        public string platformType = "Sticky";
        public string platformShape = "Square";
        public string platformSize = "2";
        public string platformColorR = "8";
        public string platformColorG = "8";
        public string platformColorB = "8";
        public GameObject manager;
        public GameObject platformL;
        public GameObject platformR;
        public GameObject setupusemanager;
        public Transform platformTransformL;
        public Transform platformTransformR;
        public Vector3 platformOffset = new Vector3(0f, 0f, 0f);
        public TextMeshPro redText;
        public TextMeshPro greenText;
        public TextMeshPro blueText;
        public TextMeshPro infoText;
        public static Plugin Instance;
        public Color platformColor = new Color(1f, 1f, 1f) * 255 / 8f;
        public Material platformMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));

        void Start()
        {
            Instance = this;
            var bundle = LoadAssetBundle("CustomPlatformManager.Resources.resources");
            manager = bundle.LoadAsset<GameObject>("CustomPlatformManager");
            Utilla.Events.GameInitialized += OnGameInitialized;
        }

        void OnGameInitialized(object sender, EventArgs e)
        {
            if (base.enabled && manager != null)
            {
                manager = Instantiate(manager);
                setup();
                cleanup();
            }
        }

        public void setup()
        {
            if (manager == null)
                return;

            manager.SetActive(true);
            manager.name = "CPM";

            var handle = manager.transform.Find("Handle");
            if (handle == null)
                return;

            handle.gameObject.AddComponent<DevHoldable>();

            infoText = handle.Find("Main/TextObjects/Other/Version")?.GetComponent<TextMeshPro>();
            redText = handle.Find("Main/TextObjects/Colors/Red/Value")?.GetComponent<TextMeshPro>();
            greenText = handle.Find("Main/TextObjects/Colors/Green/Value")?.GetComponent<TextMeshPro>();
            blueText = handle.Find("Main/TextObjects/Colors/Blue/Value")?.GetComponent<TextMeshPro>();

            redText.text = "RED: 0";
            greenText.text = "GREEN: 0";
            blueText.text = "BLUE: 0";
            platformColorR = redText.text.ToString();
            platformColorG = greenText.text.ToString();
            platformColorB = blueText.text.ToString();

            manager.transform.localPosition = new Vector3(-65.5614f, 11.8f, -81.3375f);
            manager.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
            manager.transform.rotation = Quaternion.Euler(0f, 306f, 0);
            infoText.text = "v1.0.0";

            platformL = GameObject.CreatePrimitive(PrimitiveType.Cube);
            platformL.AddComponent<GorillaSurfaceOverride>();
            platformL.GetComponent<MeshRenderer>().material = platformMaterial;
            platformL.name = "GorillaLeftPlatform";
            platformL.transform.position = Vector3.zero;
            platformL.transform.localScale = new Vector3(0.3f, 0.06f, 0.3f);

            platformR = GameObject.CreatePrimitive(PrimitiveType.Cube);
            platformR.AddComponent<GorillaSurfaceOverride>();
            platformR.GetComponent<MeshRenderer>().material = platformMaterial;
            platformR.name = "GorillaRightPlatform";
            platformR.transform.position = Vector3.zero;
            platformR.transform.localScale = new Vector3(0.3f, 0.06f, 0.3f);

            platformTransformL = platformL.transform;
            platformTransformR = platformR.transform;

            foreach (Transform child in handle.Find("Main/Buttons/Colors/Red"))
            {
                child.gameObject.AddComponent<ButtonManager>();
            }

            foreach (Transform child in handle.Find("Main/Buttons/Colors/Green"))
            {
                child.gameObject.AddComponent<ButtonManager>();
            }

            foreach (Transform child in handle.Find("Main/Buttons/Colors/Blue"))
            {
                child.gameObject.AddComponent<ButtonManager>();
            }
        }

        public void cleanup()
        {
            isEnabled = false;
            manager.SetActive(false);
            foreach (GameObject child in new GameObject[] { platformL, platformR })
            {
                child.transform.position = Vector3.zero;
                Destroy(child);
            }
        }

        void OnEnable()
        {
            setup();
            HarmonyPatches.ApplyHarmonyPatches();
        }

        void OnDisable()
        {
            cleanup();
            HarmonyPatches.RemoveHarmonyPatches();
        }

        void Update()
        {
            if (base.enabled && inRoom)
            {
                if (ControllerInputPoller.instance.rightControllerGripFloat >= 0.5)
                {
                    if (!platSetR)
                    {
                        platSetR = true;
                        platformTransformR.position = GorillaLocomotion.Player.Instance.rightControllerTransform.position + new Vector3(0, -0.1f, 0);
                        platformTransformR.rotation = Quaternion.Euler(0, -90, 0);
                        platformTransformR.Translate(platformOffset);
                    }
                }
                else
                {
                    platSetR = false;
                    platformTransformR.position = Vector3.zero;
                    platformTransformR.rotation = Quaternion.identity;
                }
                if (ControllerInputPoller.instance.leftControllerGripFloat >= 0.5)
                {
                    if (!platSetL)
                    {
                        platSetL = true;
                        platformTransformL.position = GorillaLocomotion.Player.Instance.leftControllerTransform.position + new Vector3(0, -0.1f, 0);
                        platformTransformL.rotation = Quaternion.Euler(0, -90, 0);
                        platformTransformL.Translate(platformOffset);
                    }
                }
                else
                {
                    platSetL = false;
                    platformTransformL.position = Vector3.zero;
                    platformTransformL.rotation = Quaternion.identity;
                }
            }
        }

        [ModdedGamemodeJoin]
        public void OnJoin(string gamemode)
        {
            if (base.enabled)
            {
                setup();
            }
            inRoom = true;
        }

        [ModdedGamemodeLeave]
        public void OnLeave(string gamemode)
        {
            if (base.enabled)
            {
                cleanup();
            }
            inRoom = false;
        }

        public AssetBundle LoadAssetBundle(string path)
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            AssetBundle bundle = AssetBundle.LoadFromStream(stream);
            stream.Close();
            return bundle;
        }
    }
}