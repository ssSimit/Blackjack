using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class MenuAndPause : MonoBehaviour
{
    MusicManager musicManager;
    bool isPaused = false;
    bool isMuted = false;
    [SerializeField] TextMeshProUGUI muteText;

    void Start()
    {
        musicManager = MusicManager.Instance;
        isMuted = musicManager.isMuted;
        UpdateMuteText();
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void PauseAndResumeGame()
    {
        if (!isPaused)
        {
            Time.timeScale = 0; // Pause the game
            isPaused = true;
        }
        else
        {
            Time.timeScale = 1; // Resume the game
            isPaused = false;
        }
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1; // Ensure the game is not paused
        SceneManager.LoadScene("MainMenu");
    }

    public void RestartGame()
    {
        Time.timeScale = 1; // Ensure the game is not paused
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ButtonSound()
    {
        musicManager.PlayAudio("btn");
    }

    public void MuteUnmuteMusic()
    {
        isMuted = !isMuted;
        musicManager.isMuted = isMuted;
        AudioListener.volume = isMuted ? 0 : 1;
        UpdateMuteText();
    }

    void UpdateMuteText()
    {
        if (isMuted)
        {
            muteText.text = "Sound: Off";
        }
        else
        {
            muteText.text = "Sound: On";
        }
    }
}
