using UnityEngine;
using TMPro;

namespace CustomPlatformManager.buttons
{
    internal class ButtonManager : GorillaPressableButton
    {
        public static float red, green, blue;
        private TextMeshPro isPlats, redText, greenText, blueText;

        public override void Start()
        {
            isPlats = Plugin.Instance.manager.transform.Find("Main/TextObjects/Other/isPlatsEnabled")?.GetComponent<TextMeshPro>();
            redText = Plugin.Instance.manager.transform.Find("Main/TextObjects/Colors/Red/Value")?.GetComponent<TextMeshPro>();
            greenText = Plugin.Instance.manager.transform.Find("Main/TextObjects/Colors/Green/Value")?.GetComponent<TextMeshPro>();
            blueText = Plugin.Instance.manager.transform.Find("Main/TextObjects/Colors/Blue/Value")?.GetComponent<TextMeshPro>();

            GetComponent<BoxCollider>().isTrigger = true;
            gameObject.layer = 18;
            onPressButton.AddListener(ButtonActivation);
            UpdatePlatformColor();
            UpdateColorDisplay();
        }

        public override void ButtonActivation()
        {
            isOn = !isOn;
            if (char.IsDigit(gameObject.name[1]))
                UpdateColor(gameObject.name[0], float.Parse(gameObject.name[1].ToString()) / 8f);
            if (gameObject.name.StartsWith("Enable")) { isPlats.text = "PLATFORMS: ENABLED"; Plugin.Instance.isPlatsEnabled = true; }
            if (gameObject.name.StartsWith("Disable")) { isPlats.text = "PLATFORMS: DISABLED"; Plugin.Instance.isPlatsEnabled = false; }
            UpdatePlatformColor();
            UpdateColorDisplay();
        }

        void UpdateColor(char colorComponent, float value) { if (colorComponent == 'R') red = value; else if (colorComponent == 'G') green = value; else if (colorComponent == 'B') blue = value; }
        void UpdatePlatformColor() { Plugin.Instance.platformMaterial.color = new Color(red, green, blue); }
        void UpdateColorDisplay() { redText.text = $"RED: {red * 8}"; greenText.text = $"GREEN: {green * 8}"; blueText.text = $"BLUE: {blue * 8}"; }
    }
}