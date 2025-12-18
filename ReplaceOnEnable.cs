using UnityEngine;

namespace ApGlyphs {
    public class ReplaceOnEnable : MonoBehaviour {
        public GameObject replacement;

        public void OnEnable() {
            replacement.SetActive(true);
            if (!destroyTarget) Destroy(gameObject);
            else Destroy(destroyTarget);
        }

        public GameObject destroyTarget;
    }
}