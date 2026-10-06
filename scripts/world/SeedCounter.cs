using UnityEngine;

namespace ApGlyphs {
    public class SeedCounter : MonoBehaviour {
        public void Start() {
            unhiddenPosition = transform.position;
            transform.position = hiddenPosition;
        }

        public void Update() {
            if (Time.time < nextUpdate) return;
            nextUpdate = Time.time + UPDATE_INTERVAL;
            if (InventoryManager.items.TryGetValue("Seeds", out int count) && count >= 10)
                Appear();
        }

        private void Appear() {
            transform.position = unhiddenPosition;
            Destroy(this);
        }

        public Vector3 hiddenPosition;
        public Vector3 unhiddenPosition;
        private float nextUpdate = -1f;
        private const float UPDATE_INTERVAL = 5f;
    }
}