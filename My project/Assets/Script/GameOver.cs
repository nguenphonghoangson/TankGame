using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOver: MonoBehaviour
{
    public Text gameOverText;
    public Button replayButton;

    private void Start()
    {
        // Ẩn UI khi khởi động
        HideUI();
    }

    public void ShowUI()
    {
        Time.timeScale = 0;
        gameObject.SetActive(true);
    }

    public void HideUI()
    {
        gameObject.SetActive(false);
    }

    public void Replay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
