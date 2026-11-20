using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public static readonly List<Interactable> All = new List<Interactable>();

    [Header("Interacción")]
    public float range = 3f;
    public UnityEvent onInteract;

    [Header("Indicador (world space)")]
    public Transform indicator;   // el Canvas
    public GameObject dot;        // círculo: en rango
    public GameObject keyE;       // tecla E: en rango y mirándolo

    Transform cam;

    void OnEnable() { All.Add(this); SetState(false, false); }
    void OnDisable() { All.Remove(this); }

    void Start()
    {
        if (Camera.main != null) cam = Camera.main.transform;
    }

    void LateUpdate()
    {
        // el indicador siempre mira a la cámara
        if (indicator != null && cam != null) indicator.rotation = cam.rotation;
    }

    public void SetState(bool inRange, bool focused)
    {
        if (dot != null) dot.SetActive(inRange && !focused);
        if (keyE != null) keyE.SetActive(inRange && focused);
    }

    public virtual void Interact(GameObject who)
    {
        onInteract.Invoke();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
