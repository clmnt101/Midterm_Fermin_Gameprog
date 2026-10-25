
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static bool IsGameOver { get; private set; }

    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public Text gameOverText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        IsGameOver = false;
        Time.timeScale = 1f;
    }

    private void Start()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        else
        {
            Debug.LogError(
                "Assign GameOverPanel in the GameManager Inspector!",
                this
            );
        }
    }

    public void TriggerGameOver()
    {
        if (IsGameOver)
            return;

        IsGameOver = true;

        Debug.Log("GAME OVER: activating screen.");

        PlayerController player =
            FindObjectOfType<PlayerController>();

        if (player != null)
            player.enabled = false;

        EnemySpawner spawner =
            FindObjectOfType<EnemySpawner>();

        if (spawner != null)
            spawner.StopSpawning();

        if (gameOverText != null)
            gameOverText.text = "GAME OVER";

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("Game Over Panel reference is missing!");
        }

        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        IsGameOver = false;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}
