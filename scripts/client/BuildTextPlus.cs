using System;
using System.Collections.Generic;
using Il2Cpp;
using MelonLoader;
using UnityEngine;

namespace ApGlyphs {
    public class BuildTextPlus : MonoBehaviour {
        void Update() {
            if (!sm) sm = GameObject.Find("Manager intro")?.GetComponent<SaveManager>();
            if (triggerfadeout && !fading) {
                fading = true;
                fastfadeout();
            }
            if (placed || sequenceLocked || sm.loading || substrings.Count == 0) return;
            startPosX = transform.localPosition.x;
            if (alignment == Alignment.Center) Center();

            string fullMsg = GetFullText();
            MelonLogger.Msg($"Building text: {fullMsg}");
            foreach (var (text, col) in substrings) {
                for (int i = 0; i < text.Length; i++) {
                    try {
                        if (text[i] != ' ') {
                            char let = text.ToUpper()[i];

                            // dummy prefab object base
                            GameObject obj = Instantiate(Resources.Load<GameObject>("text/A"), new Vector3(transform.position.x + x, transform.position.y + y, transform.position.z), transform.rotation);
                            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
                            GlyphCharacterPlus gc = obj.AddComponent<GlyphCharacterPlus>();

                            GlyphCharacter oldGc = obj.GetComponent<GlyphCharacter>();
                            gc.CopyFrom(oldGc);
                            Destroy(oldGc);

                            string path;
                            if (sprites.TryGetValue(let, out Sprite sprite) && sprite) {
                                sr.sprite = sprite;
                            } else {
                                if (customSpritePaths.TryGetValue(let, out path)) {
                                    sr.sprite = SpriteCache.LoadSpriteFromResources(path, new Vector2(0.5f, 0.5f));
                                    sprites[let] = sr.sprite;
                                } else {
                                    if (vanillaSpritePaths.TryGetValue(let, out path)) {
                                        sr.sprite = Resources.Load<Sprite>(path);
                                        sprites[let] = sr.sprite;
                                    } else {
                                        MelonLogger.Warning("Failed to find sprite for " + let);
                                        Destroy(obj);
                                        continue;
                                    }
                                }
                            }

                            obj.layer = gameObject.layer;
                            if (hover) obj.AddComponent<Hover>();
                            if (flicker) gc.flickeringMode = true;
                            sr.color = col;
                            sr.sortingOrder = order;
                            if (fadein) sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0f);
                            obj.transform.localScale = new Vector3(obj.transform.localScale.x * textsize, obj.transform.localScale.y * textsize, obj.transform.localScale.z);
                            obj.transform.SetParent(transform);
                            obj.name = text[i].ToString();
                            characters.Add(obj);
                            if (proximityMode && proximityTarget)
                                gc.SetProxyGlyph(proximityTarget, proximityDist, maximumTransparency, minimumTransparency, updateproxytransparencymax);
                        }
                    } catch (Exception ex) {
                        MelonLogger.Error($"Failed to create charater for {text[i]}: {ex.Message}");
                    }
                    x += textsize;
                    x += textsize * 0.1f;
                    if (x > width && alignment == Alignment.Left) {
                        x = 0f;
                        y -= textsize * 1.15f;
                    }
                    if (alignment == Alignment.Right)
                        transform.localPosition -= new Vector3(textsize * 1.1f, 0f, 0f);
                }
            }
            placed = true;
        }

        void OnEnable() {
            bool rebuild = false;
            foreach (GameObject character in characters) {
                if (!character) rebuild = true;
            }
            if (rebuild) Reset();
        }

        public void fastfadeout() {
            GlyphCharacter[] componentsInChildren = GetComponentsInChildren<GlyphCharacter>();
            for (int i = 0; i < componentsInChildren.Length; i++) {
                componentsInChildren[i].fastfadeOut();
            }
        }

        public void fadeout() {
            GlyphCharacter[] componentsInChildren = GetComponentsInChildren<GlyphCharacter>();
            for (int i = 0; i < componentsInChildren.Length; i++) {
                componentsInChildren[i].fadeOut();
            }
        }

        public void Center() {
            string text = GetFullText();

            if (text.Length % 2 == 1) {
                x -= textsize * (text.Length / 2);
                x -= textsize * 0.1f * (text.Length / 2);
                if (text.Length == 1) {
                    x = 0f;
                }
            } else {
                x -= textsize * (text.Length / 2f);
                x += 0.5f * textsize;
                x -= textsize * 0.1f * (text.Length / 2);
                x += 0.05f * textsize;
            }
        }

        public void SetText(params (string, Color)[] segs) {
            substrings.Clear();
            foreach (var (text, col) in segs)
                substrings.Add((text, col));
            Reset();
        }

        public void SetText(params string[] segs) {
            substrings.Clear();
            foreach (string text in segs)
                substrings.Add((text, Color.white));
            Reset();
        }

        public string GetFullText() {
            string fullMsg = "";
            for (int i = 0; i < substrings.Count; i++)
                fullMsg += substrings[i].Item1;
            return fullMsg;
        }

        public void CopyFrom(BuildText other) {
            substrings.Clear();
            substrings.Add((other.text, other.col));
            x = other.x;
            y = other.y;
            textsize = other.textsize;
            width = other.width;
            fadeinspeed = other.fadeinspeed;
            proximityDist = other.proximityDist;
            maximumTransparency = other.maximumTransparency;
            minimumTransparency = other.minimumTransparency;
            order = other.order;
            fadein = other.fadein;
            delayfade = other.delayfade;
            sequenceLocked = other.sequenceLocked;
            placed = other.placed;
            if (other.center) alignment = Alignment.Center;
            proximityMode = other.proximityMode;
            updateproxytransparencymax = other.updateproxytransparencymax;
            hover = other.hover;
            flicker = other.flicker;
            triggerfadeout = other.triggerfadeout;
            fading = other.fading;
            proximityTarget = other.proximityTarget;
            characters.Clear();
            foreach (GameObject character in other.characters)
                characters.Add(character);
            sm = other.sm;
        }

        public void Reset() {
            characters.Clear();
            for (int i = transform.childCount - 1; i >= 0; i--)
                Destroy(transform.GetChild(i).gameObject);
            placed = false;
            x = 0f;
            y = 0f;
            transform.localPosition = new Vector3(startPosX, transform.localPosition.y, transform.localPosition.z);
        }

        public Alignment alignment = Alignment.Left;

        private static Dictionary<char, Sprite> sprites = new Dictionary<char, Sprite>();
        private static Dictionary<char, string> customSpritePaths = new Dictionary<char, string>() {
            {'(', "font.parenthesesO"},
            {')', "font.parenthesesC"},
            {'[', "font.bracketO"},
            {']', "font.bracketC"},
            {'{', "font.curlyO"},
            {'}', "font.curlyC"},
            {'@', "font.at"},
            {'#', "font.hash"},
            {'%', "font.percent"},
            {'^', "font.caret"},
            {'=', "font.equal"},
            {'|', "font.bar"},
            {'~', "font.tilde"},
            {'$', "font.dollar"},
            {'&', "font.and"},
            {'*', "font.asterisk"},
            {'\\', "font.bslash"},
            {',', "font.comma"},
            {'-', "font.dash"},
            {'!', "font.exclamation"},
            {'/', "font.fslash"},
            {'.', "font.period"},
            {'\'', "font.quote"},
            {'"', "font.quote2"},
            {';', "font.semicolon"},
            {'_', "font.underscore"},
        };
        private static Dictionary<char, string> vanillaSpritePaths = new Dictionary<char, string>() {
            {'A', "sprites/text/A"},
            {'B', "sprites/text/B"},
            {'C', "sprites/text/C"},
            {'D', "sprites/text/D"},
            {'E', "sprites/text/E"},
            {'F', "sprites/text/F"},
            {'G', "sprites/text/G"},
            {'H', "sprites/text/H"},
            {'I', "sprites/text/I"},
            {'J', "sprites/text/J"},
            {'K', "sprites/text/K"},
            {'L', "sprites/text/L"},
            {'M', "sprites/text/M"},
            {'N', "sprites/text/N"},
            {'O', "sprites/text/O"},
            {'P', "sprites/text/P"},
            {'Q', "sprites/text/Q"},
            {'R', "sprites/text/R"},
            {'S', "sprites/text/S"},
            {'T', "sprites/text/T"},
            {'U', "sprites/text/U"},
            {'V', "sprites/text/V"},
            {'W', "sprites/text/W"},
            {'X', "sprites/text/X"},
            {'Y', "sprites/text/Y"},
            {'Z', "sprites/text/Z"},
            {'0', "sprites/text/0"},
            {'1', "sprites/text/1"},
            {'2', "sprites/text/2"},
            {'3', "sprites/text/3"},
            {'4', "sprites/text/4"},
            {'5', "sprites/text/5"},
            {'6', "sprites/text/6"},
            {'7', "sprites/text/7"},
            {'8', "sprites/text/8"},
            {'9', "sprites/text/9"},
            {'<', "sprites/text/@"},
            {'>', "sprites/text/#"},
            {'©', "sprites/text/©"},
            {'+', "sprites/text/+"},
            {':', "sprites/text/Colon"},
            {'?', "sprites/text/Qmark"},
        };

        public enum Alignment {
            Left,
            Center,
            Right, // does not support new lines
        }

        public List<(string, Color)> substrings = new List<(string, Color)>();
        public float x;
        public float y;
        public float textsize;
        public float width;
        public float fadeinspeed = 1f;
        public float proximityDist = 5f;
        public float maximumTransparency = 0.35f;
        public float minimumTransparency;
        public int order;
        public bool fadein;
        public bool delayfade;
        public bool sequenceLocked;
        public bool placed;
        public bool proximityMode;
        public bool updateproxytransparencymax;
        public bool hover;
        public bool flicker;
        public bool triggerfadeout;
        private bool fading;
        public GameObject proximityTarget;
        private List<GameObject> characters = new List<GameObject>();
        private SaveManager sm;
        private float startPosX = 0f;
    }

    public class GlyphCharacterPlus : MonoBehaviour {
        void Start() {
            bt = transform.parent.GetComponent<BuildTextPlus>();
            delay = Time.time + UnityEngine.Random.Range(0.6f * bt.fadeinspeed, 1.2f * bt.fadeinspeed);
            if (bt.delayfade) {
                fadeinspeed = UnityEngine.Random.Range(0.005f, 0.008f) / bt.fadeinspeed;
            }
            sr = GetComponent<SpriteRenderer>();
            if (flickeringMode) {
                flickerspeed = UnityEngine.Random.Range(0.5f, 0.2f);
                flickerintensity = UnityEngine.Random.Range(0.5f, 0.1f);
            }
        }

        void FixedUpdate() {
            if (flickeringMode) {
                if (flickerprogress >= 1f && Time.time > flickerCD) {
                    flickerspeed = UnityEngine.Random.Range(0.5f, 0.2f);
                    flickerintensity = UnityEngine.Random.Range(0.5f, 0.1f);
                    flickerprogress = 0f;
                    flickerCD = Time.time + UnityEngine.Random.Range(1.5f, 0.5f);
                }
                if (Time.time > flickerCD) {
                    flickerprogress += Time.deltaTime * flickerspeed;
                    float a = Mathf.Sin(MathF.PI * flickerprogress) * flickerintensity;
                    sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, a);
                }
            } else if (bt.fadein && (Time.time > delay || !bt.delayfade) && !proximityMode && (sr.color.a < 1f || (fadeinspeed < 0f && sr.color.a > 0f))) {
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, sr.color.a + fadeinspeed);
            }
        }

        void Update() {
            if (proximityMode && (bool)proximityTarget) {
                UpdateProxyGlyph();
            }
        }

        public void SetProxyGlyph(GameObject target, float dist, float maxT, float minT, bool um) {
            proximityMode = true;
            proximityTarget = target;
            proximityDist = dist;
            minimumTransparency = minT;
            maximumTransparency = maxT;
            updatemax = um;
        }

        public void UpdateProxyGlyph() {
            if (updatemax) {
                maximumTransparency = bt.maximumTransparency;
            }
            float num = proximityDist - Vector3.Distance(new Vector3(base.transform.position.x, base.transform.position.y, 0f), new Vector3(proximityTarget.transform.position.x, proximityTarget.transform.position.y, 0f)) / proximityDist;
            if (num > 0f) {
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, maximumTransparency * num);
            } else if (sr.color.a != minimumTransparency) {
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, minimumTransparency);
            }
        }

        public void fadeOut() => fadeinspeed = UnityEngine.Random.Range(0.005f, 0.008f) / (0f - bt.fadeinspeed);

        public void fastfadeOut() => fadeinspeed = 10f * UnityEngine.Random.Range(0.005f, 0.008f) / (0f - bt.fadeinspeed);

        public void CopyFrom(GlyphCharacter other) {
            proximityMode = other.proximityMode;
            updatemax = other.updatemax;
            flickeringMode = other.flickeringMode;
            delay = other.delay;
            fadeinspeed = other.fadeinspeed;
            proximityDist = other.proximityDist;
            minimumTransparency = other.minimumTransparency;
            maximumTransparency = other.maximumTransparency;
            flickerintensity = other.flickerintensity;
            flickerspeed = other.flickerspeed;
            flickerprogress = other.flickerprogress;
            flickerCD = other.flickerCD;
            proximityTarget = other.proximityTarget;
            sr = other.sr;
        }

        private bool proximityMode;

        private bool updatemax;

        public bool flickeringMode;

        private float delay;

        private float fadeinspeed = 0.01f;

        private float proximityDist;

        private float minimumTransparency;

        private float maximumTransparency;

        private float flickerintensity = 0.5f;

        private float flickerspeed = 1f;

        private float flickerprogress;

        private float flickerCD;

        private GameObject proximityTarget;

        private BuildTextPlus bt;

        private SpriteRenderer sr;
    }
}