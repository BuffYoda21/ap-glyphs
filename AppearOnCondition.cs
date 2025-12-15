using UnityEngine;

namespace ApGlyphs {
    public class AppearOnCondition : MonoBehaviour {
        public void Update() {
            if (target == null) return;
            if (target.activeInHierarchy) {
                Destroy(target);
                gameObject.SetActive(true);
            }
        }

        public GameObject target;
    }
}