using System;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using CustomPlatformManager.buttons;
using TMPro;
using UnityEngine;
using Newtilla;
using HarmonyLib;

namespace CustomPlatformManager
{
    [BepInDependency("Lofiat.Newtilla", "1.0.1")]
    [BepInPlugin("zaynethedev.CustomPlatformManager", "CustomPlatformManager", "1.1.0")]
    public class Plugin : BaseUnityPlugin
    {
        private static Harmony instance;
        public static bool IsPatched { get; private set; }
        public const string InstanceId = "zaynethedev.CustomPlatformManager";
        public bool isFlip = false, inRoom, platSetL, platSetR, isPlatsEnabled;
        public string platformColorR = "8", platformColorG = "8", platformColorB = "8";
        public GameObject manager, platformL, platformR, setupusemanager;
        public Transform platformTransformL, platformTransformR;
        public TextMeshPro redText, greenText, blueText, infoText;
        public Vector3 platformOffsetL = new Vector3(0f, -0.025f, 0f);
        public Vector3 platformOffsetR = new Vector3(0f, -0.025f, 0f);
        public Color platformColor = new Color(1f, 1f, 1f) * 255 / 8f;
        public Material platformMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        public static Plugin Instance;
        public GameObject[] textPages;
        public GameObject[] buttonPages;
        private int currentPage = 0;

        void Start()
        {
            Instance = this;
            var bundle = LoadAssetBundle("CustomPlatformManager.Resources.resources");
            manager = bundle.LoadAsset<GameObject>("CustomPlatformManager");
            GorillaTagger.OnPlayerSpawned(OnGameInitialized);
            Newtilla.Newtilla.OnJoinModded += OnModdedJoined;
            Newtilla.Newtilla.OnLeaveModded += OnModdedLeft;
        }

        void OnGameInitialized()
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
            isPlatsEnabled = true;
            if (manager == null)
                return;

            manager.SetActive(true);
            manager.name = "CPM";
            Transform CPMT = manager.transform;

            infoText = CPMT.Find("Main/TextObjects/Other/Version")?.GetComponent<TextMeshPro>();
            redText = CPMT.Find("Main/TextObjects/Colors/Red/Value")?.GetComponent<TextMeshPro>();
            greenText = CPMT.Find("Main/TextObjects/Colors/Green/Value")?.GetComponent<TextMeshPro>();
            blueText = CPMT.Find("Main/TextObjects/Colors/Blue/Value")?.GetComponent<TextMeshPro>();

            redText.text = "RED: 0";
            greenText.text = "GREEN: 0";
            blueText.text = "BLUE: 0";
            platformColorR = redText.text.ToString();
            platformColorG = greenText.text.ToString();
            platformColorB = blueText.text.ToString();

            manager.transform.localPosition = new Vector3(-69.2f, 12f, -83.8f);
            manager.transform.localScale = new Vector3(0.16f, 0.16f, 0.16f);
            manager.transform.rotation = Quaternion.Euler(0f, 335f, 0f);
            infoText.text = "v1.1.0";

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

            textPages = new GameObject[] {
                CPMT.Find("Main/TextObjects/Sizes").gameObject,
                CPMT.Find("Main/TextObjects/Colors").gameObject
            };

            buttonPages = new GameObject[] {
                CPMT.Find("Main/Buttons/Sizes").gameObject,
                CPMT.Find("Main/Buttons/Colors").gameObject
            };

            foreach (Transform child in CPMT.Find("Main/Buttons/Other"))
            {
                child.gameObject.AddComponent<ButtonManager>();
            }

            foreach (Transform child in CPMT.Find("Main/Buttons/Colors/Red"))
            {
                child.gameObject.AddComponent<ButtonManager>();
            }

            foreach (Transform child in CPMT.Find("Main/Buttons/Colors/Green"))
            {
                child.gameObject.AddComponent<ButtonManager>();
            }

            foreach (Transform child in CPMT.Find("Main/Buttons/Colors/Blue"))
            {
                child.gameObject.AddComponent<ButtonManager>();
            }

            foreach (Transform child in CPMT.Find("Main/Buttons/Sizes/L"))
            {
                child.gameObject.AddComponent<ButtonManager>();
            }

            foreach (Transform child in CPMT.Find("Main/Buttons/Sizes/R"))
            {
                child.gameObject.AddComponent<ButtonManager>();
            }
        }

        public void cleanup()
        {
            manager.SetActive(false);
            platformL.transform.position = Vector3.zero; platformR.transform.position = Vector3.zero;
            platformL.SetActive(false); platformR.SetActive(false);
        }

        void OnEnable()
        {
            setup();
            if (!IsPatched)
            {
                if (instance == null)
                {
                    instance = new Harmony(InstanceId);
                }

                instance.PatchAll(Assembly.GetExecutingAssembly());
                IsPatched = true;
            }
        }

        void OnDisable()
        {
            cleanup();
            if (instance != null && IsPatched)
            {
                instance.UnpatchSelf();
                IsPatched = false;
            }
        }

        void Update()
        {
            if (base.enabled && inRoom)
            {
                if (isPlatsEnabled)
                {
                    if (ControllerInputPoller.instance.rightControllerGripFloat >= 0.5)
                    {
                        if (!platSetR)
                        {
                            platSetR = true;
                            platformTransformR.position = GorillaLocomotion.Player.Instance.rightControllerTransform.position + new Vector3(0, -0.1f, 0);
                            platformTransformR.rotation = Quaternion.Euler(0, -90, 0);
                            platformTransformR.Translate(platformOffsetR);
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
                            platformTransformL.Translate(platformOffsetL);
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
        }

        void OnModdedJoined(string modeName)
        {
            setup();
            inRoom = true;
        }

        void OnModdedLeft(string modeName)
        {
            cleanup();
            inRoom = false;
        }

        public AssetBundle LoadAssetBundle(string path)
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            AssetBundle bundle = AssetBundle.LoadFromStream(stream);
            stream.Close();
            return bundle;
        }

        public void NextPage()
        {
            if (currentPage < textPages.Length - 1 && buttonPages != null && textPages != null)
            {
                textPages[currentPage].SetActive(false);
                buttonPages[currentPage].SetActive(false);
                currentPage++;
                textPages[currentPage].SetActive(true);
                buttonPages[currentPage].SetActive(true);
            }
        }

        public void PreviousPage()
        {
            if (currentPage > 0 && buttonPages != null && textPages != null)
            {
                textPages[currentPage].SetActive(false);
                buttonPages[currentPage].SetActive(false);
                currentPage--;
                textPages[currentPage].SetActive(true);
                buttonPages[currentPage].SetActive(true);
            }
        }
    }
}