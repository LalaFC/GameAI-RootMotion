using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject StartPanel;
    [SerializeField] GameObject StartCam;
    [SerializeField] GameObject EndPanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        StartPanel.SetActive(true);
        StartCam.SetActive(true);
        EndPanel.SetActive(false);
    }

    public void StartGame()
    {
        StartPanel.SetActive(false);
        StartCam.SetActive(false);
    }
    public void GameFinished()
    {
        EndPanel.SetActive(true);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
}
