using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerHealth : MonoBehaviour
{
    [Header("Vida")]
    public float maxHealth = 100f;
    public float regenDelay = 5f;   // segundos sin recibir daño para empezar a curar
    public float regenRate = 25f;   // vida por segundo

    [Header("Testing")]
    public float testDamage = 30f;  // daño al presionar K

    [Header("Muerte")]
    public Animator animator;                  // si lo dejas vacío, lo busca solo
    public MonoBehaviour[] disableOnDeath;     // scripts que se apagan al morir
    public UnityEvent onDeath;

    public event Action<float> Damaged;        // lo escucha el efecto de pantalla

    public float Health { get; private set; }
    public float Health01 => Health / maxHealth;
    public bool IsDead { get; private set; }

    static readonly int DieHash = Animator.StringToHash("Die");

    float sinceHit;

    void Awake()
    {
        Health = maxHealth;
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (IsDead) return;

        // TEST: borra estas 2 líneas cuando ya haya enemigos
        var kb = Keyboard.current;
        if (kb != null && kb.kKey.wasPressedThisFrame) TakeDamage(testDamage);

        sinceHit += Time.deltaTime;
        if (sinceHit >= regenDelay && Health < maxHealth)
            Health = Mathf.MoveTowards(Health, maxHealth, regenRate * Time.deltaTime);
    }

    public void TakeDamage(float amount)
    {
        if (IsDead) return;

        Health = Mathf.Max(0f, Health - amount);
        sinceHit = 0f;                 // reinicia la cuenta de la regeneración
        Damaged?.Invoke(amount);

        if (Health <= 0f) Die();
    }

    void Die()
    {
        IsDead = true;

        var tp = GetComponent<TPController>();
        if (tp != null) tp.enabled = false;

        foreach (var s in disableOnDeath)
            if (s != null) s.enabled = false;

        if (animator != null)
        {
            // quita la pose de apuntar de los brazos
            int upper = animator.GetLayerIndex("UpperBody");
            if (upper >= 0) animator.SetLayerWeight(upper, 0f);

            animator.SetTrigger(DieHash);
        }

        onDeath.Invoke();
    }
}