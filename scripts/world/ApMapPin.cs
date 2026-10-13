using System;
using Il2Cpp;
using UnityEngine;

namespace ApGlyphs {
    public class ApMapPin : MonoBehaviour {
        void Start() {
            if (!cc) cc = SceneSearcher.Find("Main Camera Parent/Main Camera")?.GetComponent<CameraController>();
            sr = GetComponent<SpriteRenderer>();
        }

        void Update() {
            if (!cc || !Input.GetKeyDown(pinToggleKey) || !string.Equals(cc.mode, "map", StringComparison.Ordinal)) return; // why the hell does the base game use string comparison instead of enums?
            visible = !visible;
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, visible ? 1f : 0f);
        }

        private bool visible = true;
        private SpriteRenderer sr;
        private static CameraController cc;
        public static KeyCode pinToggleKey = KeyCode.Tab;
    }
}