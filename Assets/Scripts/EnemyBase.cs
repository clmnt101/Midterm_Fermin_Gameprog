
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour, IDamageable
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public float rotationSpeed = 5f;

    [Tooltip("Distance between enemy and player that causes Game Over.")]
    public float reachDistance = 1f;

    [Header("Color")]
    public ColorState enemyState = ColorState.Red;
    public Renderer enemyRenderer;

    private Transform player;
    private bool isDead;

    protected virtual void Awake()
    {
        if (enemyRenderer == null)
            enemyRenderer = GetComponent<Renderer>();

        UpdateEnemyColor();
    }

    protected virtual void Start()
    {
        PlayerController controller =
            FindObjectOfType<PlayerController>();

        if (controller != null)
            player = controller.transform;
    }

    protected virtual void Update()
    {
        if (isDead || GameManager.IsGameOver)
            return;

        if (player == null)
        {
            PlayerController controller =
                FindObjectOfType<PlayerController>();

            if (controller != null)
                player = controller.transform;

            if (player == null)
                return;
        }

        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        float distance = direction.magnitude;

        if (distance <= reachDistance)
        {
            Debug.Log("Enemy reached the player!");

            if (GameManager.Instance != null)
                GameManager.Instance.TriggerGameOver();
            else
                Debug.LogError("No GameManager exists in the scene!");

            return;
        }

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            transform.position +=
                direction.normalized *
                moveSpeed *
                Time.deltaTime;
        }
    }

    public void SetColor(ColorState state)
    {
        enemyState = state;
        UpdateEnemyColor();
    }

    private void UpdateEnemyColor()
    {
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color =
                PlayerController.GetStateColor(enemyState);
        }
    }

    public bool TakeDamage(ColorState bulletColor)
    {
        if (isDead || GameManager.IsGameOver)
            return false;

        if (bulletColor != enemyState)
            return false;

        isDead = true;
        Destroy(gameObject);

        return true;
    }
}
