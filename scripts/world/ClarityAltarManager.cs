using System.Collections;
using Il2Cpp;
using MelonLoader;
using UnityEngine;

namespace ApGlyphs {
    public static class ClarityAltarManager {
        public static void CheckAltarActivation() {
            Transform roomParent = SceneSearcher.Find("World/Region2/Lab/(R18G) (Clarity Altar)");
            if (!roomParent) return;

            bool altarActivationSuccess = false;
            int cubes;
            foreach (DisappearOnSave conditional in roomParent.GetComponentsInChildren<DisappearOnSave>(true)) {
                switch (conditional.gameObject.name) {
                    case "Disappear on Save":
                        if (InventoryManager.items.TryGetValue("Rune Cube", out cubes) && cubes >= 1)
                            conditional.booltargetval = true;
                        break;
                    case "Disappear on Save (1)":
                        if (InventoryManager.items.TryGetValue("Rune Cube", out cubes) && cubes >= 2)
                            conditional.booltargetval = true;
                        break;
                    default:
                        if (!conditional.gameObject.name.Contains("Disappear on Save (2)") && !conditional.gameObject.name.Contains("cube")) continue;
                        if (InventoryManager.items.TryGetValue("Rune Cube", out cubes) && cubes >= 3) {
                            conditional.booltargetval = true;
                            if (conditional.transform.name == "cube1?") altarActivationSuccess = true;
                            if (conditional.transform.GetChild(0)?.name == "R18G ACTIVE")
                                lights = conditional.transform.GetChild(0)?.gameObject;
                        }
                        break;
                }
            }

            if (!altarActivationSuccess && InventoryManager.items.TryGetValue("Rune Cube", out cubes) && cubes >= 3) {
                MelonCoroutines.Start(ScheduleCheck());
            } else if (altarActivationSuccess) {
                MelonCoroutines.Start(SafetyCheck());
            }
        }

        private static IEnumerator ScheduleCheck() {
            yield return new WaitForSeconds(0.25f);
            CheckAltarActivation();
        }

        private static IEnumerator SafetyCheck() {
            yield return new WaitForSeconds(0.25f);
            if (!lights || !lights.activeInHierarchy) {
                CheckAltarActivation();
            }
        }

        private static GameObject lights;
    }
}