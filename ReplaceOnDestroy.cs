using UnityEngine;

namespace ApGlyphs {
    public class ReplaceOnDestroy : MonoBehaviour {
        public void OnDestroy() {
            replacement.SetActive(true);
        }

        public GameObject replacement;
    }
}