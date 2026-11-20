using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    public Transform cam;                              // Main Camera
    [Range(0.5f, 1f)] public float lookThreshold = 0.9f;   // 1 = justo al centro
    public int collected;

    void Start()
    {
        if (cam == null && Camera.main != null) cam = Camera.main.transform;
    }

    void Update()
    {
        var all = Interactable.All;
        Interactable best = null;
        float bestDot = lookThreshold;

        // 1. De los que están en rango, el que más está al centro de la cámara
        for (int i = 0; i < all.Count; i++)
        {
            var it = all[i];
            if (!InRange(it)) continue;

            Vector3 dir = (it.transform.position - cam.position).normalized;
            float d = Vector3.Dot(cam.forward, dir);
            if (d > bestDot) { bestDot = d; best = it; }
        }

        // 2. Actualiza los indicadores: círculo en rango, E solo en el enfocado
        for (int i = 0; i < all.Count; i++)
            all[i].SetState(InRange(all[i]), all[i] == best);

        // 3. Interactuar
        var kb = Keyboard.current;
        if (best != null && kb != null && kb.eKey.wasPressedThisFrame)
            best.Interact(gameObject);
    }

    bool InRange(Interactable it)
    {
        return Vector3.Distance(transform.position, it.transform.position) <= it.range;
    }

    public void AddItem(string itemName)
    {
        collected++;
        Debug.Log("Recogiste: " + itemName + " (total " + collected + ")");
    }
}
