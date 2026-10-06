using UnityEngine;

namespace ApGlyphs {
    public class SeedCounter : MonoBehaviour {
        public void Start() => unhiddenPosition = transform.position;

        public void Update() {
            if (isHidden) return;
            if (InventoryManager.items.TryGetValue("Seeds", out int count) && count >= 10)
                Appear();
            else
                Hide();
        }

        private void Appear() {
            transform.position = unhiddenPosition;
            isHidden = false;
        }

        private void Hide() {
            transform.position = hiddenPosition;
            isHidden = true;
        }

        public Vector3 hiddenPosition;
        public Vector3 unhiddenPosition;
        private bool isHidden = false;
    }
}