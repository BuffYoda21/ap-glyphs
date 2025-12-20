using UnityEngine;

namespace ApGlyphs {
    public class ReplaceOnEnable : MonoBehaviour {
        public void OnEnable() {
            replacement.SetActive(true);
            if (doNotDestroy) return;
            if (!destroyTarget) Destroy(gameObject);
            else Destroy(destroyTarget);
        }

        public GameObject replacement;
        public GameObject destroyTarget;
        public bool doNotDestroy;
    }
}