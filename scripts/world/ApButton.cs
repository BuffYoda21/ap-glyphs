using System.Collections.Generic;
using Il2Cpp;
using UnityEngine;
using static ApGlyphs.ButtonManager;

namespace ApGlyphs {
    public class ApButton : MonoBehaviour {
        void Start() {
            gameObject.GetComponent<SpriteRenderer>().color = colorIndex[color];
            if (!buttonObj) buttonObj = gameObject.GetComponent<ButtonObj>();
            if (buttonObj) buttonObj.type = typeIndex[color];
        }

        void OnEnable() => Register(buttonObj);

        void OnDisable() => Unregister(this);

        public int id = -1;
        public ButtonColor color = ButtonColor.RED;
        public ButtonObj buttonObj;
        public string path = "";

        public static readonly Dictionary<ButtonColor, Color> colorIndex = new Dictionary<ButtonColor, Color>() {
            {ButtonColor.SAVE, new Color(0.9986f, 1f, 0f, 1f)},
            {ButtonColor.RED, new Color(1f, 0f, 0f, 1f)},
            {ButtonColor.BLUE, new Color(0f, 0.6059f, 1f, 1f)},
            {ButtonColor.GREEN, new Color(0.0769f, 0.7642f, 0.0936f, 1f)},
            {ButtonColor.YELLOW, new Color(0.8396f, 0.7524f, 0.1545f, 1f)},
            {ButtonColor.PINK, new Color(1f, 0f, 0.6905f, 1f)},
            {ButtonColor.BLACK, new Color(0.1604f, 0.1604f, 0.1604f, 1f)},
        };

        private Dictionary<ButtonColor, string> typeIndex = new Dictionary<ButtonColor, string>() {
            {ButtonColor.SAVE, ""},
            {ButtonColor.RED, ""},
            {ButtonColor.BLUE, "dash"},
            {ButtonColor.GREEN, "attack"},
            {ButtonColor.YELLOW, "dashattack"},
            {ButtonColor.PINK, "parry"},
            {ButtonColor.BLACK, "enemy"},
        };
    }
}