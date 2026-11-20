using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CanvasGroup))]
public class DeathScreen : MonoBehaviour
{
    public float delay = 2f;      // espera a que termine la animación de morir
    public float fadeTime = 1f;

    CanvasGroup group;

    void Awake()
    {
        group = GetComponent<CanvasGroup>();
        group.alpha = 0f;
        group.interactable = false;
        group.blocksRaycasts = false;
    }

    void Update()
    {
        // atajo: R reinicia cuando la pantalla ya está visible
        var kb = Keyboard.current;
        if (group.interactable && kb != null && kb.rKey.wasPressedThisFrame) Restart();
    }

    public void Show() => StartCoroutine(ShowRoutine());

    IEnumerator ShowRoutine()
    {
        yield return new WaitForSeconds(delay);

        Cursor.lockState = CursorLockMode.None;   // libera el mouse para el botón
        Cursor.visible = true;

        for (float t = 0f; t < 1f; t += Time.deltaTime / fadeTime)
        {
            group.alpha = t;
            yield return null;
        }

        group.alpha = 1f;
        group.interactable = true;
        group.blocksRaycasts = true;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
