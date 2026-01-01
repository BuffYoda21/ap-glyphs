using UnityEngine;

namespace ApGlyphs {
    public class BombHatPickupReplacer : ReplaceOnEnable {
        public new void OnEnable() {
            return;
        }

        public void Update() {
            if (destroyTarget) return;
            if (!destroyTarget) destroyTarget = GameObject.Find("World/Escape Sequence/bombHat");
            if (destroyTarget) replacement.transform.position = destroyTarget.transform.position;
            if (!destroyTarget) return;
            base.OnEnable();
        }
    }
}