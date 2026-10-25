
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    public float speed = 12f;
    public float lifetime = 5f;
    public Renderer bulletRenderer;

    private ColorState bulletState;
    private Rigidbody rb;
    private bool hitSomething;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (bulletRenderer == null)
            bulletRenderer = GetComponent<Renderer>();
    }

    public void Initialize(ColorState state)
    {
        SetColor(state);

        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = false;

#if UNITY_6000_0_OR_NEWER
            rb.linearVelocity = transform.forward * speed;
#else
            rb.velocity = transform.forward * speed;
#endif
        }

        Destroy(gameObject, lifetime);
    }

    public void SetColor(ColorState state)
    {
        bulletState = state;

        if (bulletRenderer != null)
        {
            bulletRenderer.material.color =
                PlayerController.GetStateColor(bulletState);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hitSomething || GameManager.IsGameOver)
            return;

        IDamageable damageable =
            other.GetComponentInParent<IDamageable>();

        if (damageable == null)
            return;

        hitSomething = true;

        damageable.TakeDamage(bulletState);

        Destroy(gameObject);
    }
}
