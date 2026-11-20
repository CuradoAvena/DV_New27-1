using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DamageVignette : MonoBehaviour
{
    public PlayerHealth health;
    public Color color = new Color(0.55f, 0f, 0f, 1f);
    [Range(0, 1)] public float maxAlpha = 0.85f;  // opacidad con la vida casi en 0
    [Range(0, 1)] public float hitFlash = 0.4f;   // destello extra al recibir un golpe
    public float flashFade = 2f;
    public float smooth = 4f;

    Image img;
    float alpha, flash;

    void Awake()
    {
        img = GetComponent<Image>();
        img.sprite = MakeVignette(256);
        img.raycastTarget = false;
        img.color = new Color(color.r, color.g, color.b, 0f);
    }

    void OnEnable() { if (health != null) health.Damaged += OnDamaged; }
    void OnDisable() { if (health != null) health.Damaged -= OnDamaged; }
    void OnDamaged(float amount) { flash = hitFlash; }

    void Update()
    {
        float target = (1f - health.Health01) * maxAlpha;   // menos vida = más rojo
        alpha = Mathf.Lerp(alpha, target, smooth * Time.deltaTime);
        flash = Mathf.MoveTowards(flash, 0f, flashFade * Time.deltaTime);

        var c = color;
        c.a = Mathf.Clamp01(alpha + flash);
        img.color = c;
    }

    static Sprite MakeVignette(int size)
    {
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float dx = (x / (size - 1f) - 0.5f) * 2f;
                float dy = (y / (size - 1f) - 0.5f) * 2f;
                float r = Mathf.Sqrt(dx * dx + dy * dy);
                tex.SetPixel(x, y, new Color(1, 1, 1, Mathf.SmoothStep(0.45f, 1.25f, r)));
            }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }
}