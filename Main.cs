using MelonLoader;
using Il2CppInterop.Runtime.Injection;

[assembly: MelonInfo(typeof(ApGlyphs.Main), "ApGlyphs", "1.0.0", "BuffYoda21")]
[assembly: MelonGame("Vortex Bros.", "GLYPHS")]

namespace ApGlyphs {
    public class Main : MelonMod {
        [System.Obsolete]
        public override void OnApplicationStart() {
            if (isInitialized) return;
            var harmony = new HarmonyLib.Harmony("ApGlyphs.Patches");
            harmony.PatchAll();
            // class injection here
            isInitialized = true;
        }
        bool isInitialized = false;
    }
}