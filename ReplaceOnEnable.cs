using UnityEngine;

namespace ApGlyphs {
    public class ReplaceOnEnable : MonoBehaviour {
        public GameObject replacement;

        public void OnEnable() {
            replacement.SetActive(true);
            Destroy(gameObject);
        }
    }
}