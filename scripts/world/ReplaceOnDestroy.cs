using UnityEngine;

namespace ApGlyphs {
    public class ReplaceOnDestroy : MonoBehaviour {
        public void OnDestroy() {
            if (replacement) replacement.SetActive(true);
        }

        public GameObject replacement;
    }
}