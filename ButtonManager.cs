using UnityEngine;
using TMPro;

namespace CustomPlatformManager.buttons
{
    internal class ButtonManager : GorillaPressableButton
    {
        public static float red;
        public static float green;
        public static float blue;
        public TextMeshPro redText = Plugin.Instance.manager.transform.Find("Handle/Main/TextObjects/Colors/Red/Value")?.GetComponent<TextMeshPro>();
        public TextMeshPro greenText = Plugin.Instance.manager.transform.Find("Handle/Main/TextObjects/Colors/Green/Value")?.GetComponent<TextMeshPro>();
        public TextMeshPro blueText = Plugin.Instance.manager.transform.Find("Handle/Main/TextObjects/Colors/Blue/Value")?.GetComponent<TextMeshPro>();

        public override void Start()
        {
            BoxCollider boxCollider = GetComponent<BoxCollider>();
            boxCollider.isTrigger = true;
            gameObject.layer = 18;
            onPressButton = new UnityEngine.Events.UnityEvent();
            onPressButton.AddListener(new UnityEngine.Events.UnityAction(ButtonActivation));
            Plugin.Instance.platformMaterial.color = new Color(red, green, blue);
        }

        public override void ButtonActivation()
        {
            isOn = !isOn;
            string gameObjectName = gameObject.name;

            char colorComponent = gameObjectName[0];
            float value = float.Parse(gameObjectName[1].ToString()) / 8f;

            switch (colorComponent)
            {
                case 'R':
                    red = value;
                    redText.text = $"RED: {value * 8}";
                    break;
                case 'G':
                    green = value;
                    greenText.text = $"GREEN: {value * 8}";
                    break;
                case 'B':
                    blue = value;
                    blueText.text = $"BLUE: {value * 8}";
                    break;
            }

            Plugin.Instance.platformMaterial.color = new Color(red, green, blue);
            Plugin.Instance.platformL.GetComponent<MeshRenderer>().material = Plugin.Instance.platformMaterial;
            Plugin.Instance.platformR.GetComponent<MeshRenderer>().material = Plugin.Instance.platformMaterial;
        }
    }
}