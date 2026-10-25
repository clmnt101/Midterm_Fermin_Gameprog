
using UnityEngine;

public class PlayerController : MonoBehaviour, ISwitchable
{
    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public float shootInterval = 1f;

    [Header("Targeting")]
    public float detectionRange = 10f;
    public float rotationSpeed = 5f;
    public LayerMask enemyLayer = ~0;

    [Header("Color")]
    public ColorState currentState = ColorState.Red;
    public Renderer playerRenderer;

    private float shootTimer;
    private EnemyBase currentTarget;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;

        if (playerRenderer == null)
            playerRenderer = GetComponent<Renderer>();

        UpdateColor();

        Debug.Log("Player ready. Current color: " + currentState);
    }

    private void Update()
    {
        if (GameManager.IsGameOver)
            return;

        if (Input.GetMouseButtonDown(0))
            TrySwitchColorFromClick();

        FindNearestEnemy();
        RotateTowardsTarget();

        shootTimer += Time.deltaTime;

        if (shootTimer >= shootInterval)
        {
            shootTimer = 0f;
            Shoot();
        }
    }

    private void TrySwitchColorFromClick()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;

            if (mainCamera == null)
            {
                Debug.LogError("No MainCamera found!");
                return;
            }
        }

        Ray ray = mainCamera.ScreenPointToRay(
            Input.mousePosition
        );

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
        {
            if (hit.transform == transform ||
                hit.transform.IsChildOf(transform))
            {
                SwitchState();
            }
        }
    }

    public void SwitchState()
    {
        currentState = (ColorState)(
            ((int)currentState + 1) % 5
        );

        UpdateColor();

        BulletProjectile[] bullets =
            FindObjectsOfType<BulletProjectile>();

        foreach (BulletProjectile bullet in bullets)
        {
            if (bullet != null)
                bullet.SetColor(currentState);
        }

        Debug.Log("Player and bullets changed to: " + currentState);
    }

    private void UpdateColor()
    {
        if (playerRenderer != null)
        {
            playerRenderer.material.color =
                GetStateColor(currentState);
        }
        else
        {
            Debug.LogWarning("Player Renderer is not assigned!");
        }
    }

    public static Color GetStateColor(ColorState state)
    {
        switch (state)
        {
            case ColorState.Red:
                return Color.red;
            case ColorState.Blue:
                return Color.blue;
            case ColorState.Green:
                return Color.green;
            case ColorState.Yellow:
                return Color.yellow;
            case ColorState.Purple:
                return new Color(0.6f, 0f, 1f);
            default:
                return Color.white;
        }
    }

    private void Shoot()
    {
        if (bulletPrefab == null || bulletSpawnPoint == null)
            return;

        GameObject bullet = Instantiate(
            bulletPrefab,
            bulletSpawnPoint.position,
            bulletSpawnPoint.rotation
        );

        BulletProjectile projectile =
            bullet.GetComponent<BulletProjectile>();

        if (projectile != null)
        {
            projectile.Initialize(currentState);
        }
        else
        {
            Debug.LogError(
                "Bullet prefab is missing BulletProjectile!",
                bullet
            );

            Destroy(bullet);
        }
    }

    private void FindNearestEnemy()
    {
        currentTarget = null;
        float nearestDistance = detectionRange;

        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            detectionRange,
            enemyLayer
        );

        foreach (Collider hit in hits)
        {
            EnemyBase enemy =
                hit.GetComponentInParent<EnemyBase>();

            if (enemy == null)
                continue;

            float distance = Vector3.Distance(
                transform.position,
                enemy.transform.position
            );

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                currentTarget = enemy;
            }
        }
    }

    private void RotateTowardsTarget()
    {
        if (currentTarget == null)
            return;

        Vector3 direction =
            currentTarget.transform.position - transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }
}
