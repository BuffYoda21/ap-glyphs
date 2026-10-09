using System.Collections.Generic;
using Il2Cpp;
using MelonLoader;
using UnityEngine;
using static ApGlyphs.ButtonManager;

namespace ApGlyphs {
    public class ApButton : MonoBehaviour {
        void Start() {
            isBroken = IsBroken(id);
            SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
            sr.color = colorIndex[color];
            if (isBroken) sr.sprite = Resources.Load<Sprite>("sprites/platforming/Button Broken");
            if (!buttonObj) buttonObj = gameObject.GetComponent<ButtonObj>();
            if (buttonObj) buttonObj.type = typeIndex[color];
            buttonObj.broken = isBroken;
            hasUncollectedItem = NetworkClient.session.Locations.AllMissingLocations.Contains(id + 10000);

            if (isBroken || hasUncollectedItem) {
                GameObject tagObj = new GameObject("ID");
                tagObj.transform.SetParent(gameObject.transform, false);
                tagObj.transform.localPosition = new Vector3(0f, 1.5f, 0f);
                tagObj.transform.rotation = Quaternion.identity;
                idTag = tagObj.AddComponent<BuildText>();
                idTag.text = id.ToString();
                idTag.textsize = 0.5f;
                idTag.center = true;
                idTag.col = Color.white;
                idTag.normaltext = true;
            }
        }

        void Update() {
            if (!hasUncollectedItem || !buttonObj || !buttonObj.pressed) return;
            OnPress();
        }

        void OnEnable() => Register(buttonObj);

        void OnDisable() => Unregister(this);

        private void OnPress() {
            NetworkClient.CollectItem(id + 10000); // CollectItem handlies invalid IDs and already collected locations but should probably rework this anyway
            hasUncollectedItem = false;
            if (idTag) Destroy(idTag.gameObject); // isBroken can be assumed false since that would have prevented the press
        }

        public void Fix() {
            if (!isBroken) return;
            isBroken = false;
            buttonObj.broken = false;
            gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprites/platforming/Button");
            if (!hasUncollectedItem) Destroy(idTag.gameObject);
        }

        public int id = -1;
        public ButtonColor color = ButtonColor.RED;
        private bool isBroken = false;
        private bool hasUncollectedItem = false;
        public ButtonObj buttonObj;
        public string path = "";
        private BuildText idTag;

        public static readonly Dictionary<ButtonColor, Color> colorIndex = new Dictionary<ButtonColor, Color>() {
            {ButtonColor.SAVE, new Color(0.9986f, 1f, 0f, 1f)},
            {ButtonColor.RED, new Color(1f, 0f, 0f, 1f)},
            {ButtonColor.BLUE, new Color(0f, 0.6059f, 1f, 1f)},
            {ButtonColor.GREEN, new Color(0.0769f, 0.7642f, 0.0936f, 1f)},
            {ButtonColor.YELLOW, new Color(0.8396f, 0.7524f, 0.1545f, 1f)},
            {ButtonColor.PINK, new Color(1f, 0f, 0.6905f, 1f)},
            {ButtonColor.BLACK, new Color(0.1604f, 0.1604f, 0.1604f, 1f)},
        };

        private static readonly Dictionary<ButtonColor, string> typeIndex = new Dictionary<ButtonColor, string>() {
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