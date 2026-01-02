using UnityEngine;

namespace ApGlyphs {
    public class SeedCounter : MonoBehaviour {
        public void Start() {
            inventory = SceneSearcher.Find("Manager intro")?.GetComponent<InventoryManager>();
            unhiddenPosition = transform.position;
        }

        public void Update() {
            if (!inventory) return;
            if (inventory.items.ContainsKey("Seeds") && inventory.items["Seeds"] >= 10 && isHidden)
                Appear();
            else if (!inventory.items.ContainsKey("Seeds") || inventory.items["Seeds"] < 10 && !isHidden)
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

        private InventoryManager inventory;
        public Vector3 hiddenPosition;
        public Vector3 unhiddenPosition;
        private bool isHidden = false;
    }
}