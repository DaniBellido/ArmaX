using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    //public GameManager gameManager;
    public GameObject loadingScreen;
    public Slider loadingBar;
    public TMP_Text progressText;


    public void NewGame() 
    {
        LoadSceneAsync(2/*"MissionScene"*/);
    }

    public void LoadGame()
    {
        // Implementar la funcionalidad de carga de juego aquí
    }

    public void Training()
    {
        LoadSceneAsync(3/*"TrainingScene"*/);
    }

    public void Options()
    {
        // Implementar la funcionalidad de opciones aquí
    }

    public void ExitGame() 
    {
        Application.Quit();
    }


    //REMOVE THESE 2 FUNCTIONS FROM HERE AND PLACE THEM IN THE GAMEMANAGER AFTER NARRATIVE DEADLINE.
    public void LoadSceneAsync(int sceneIndex)
    {
        StartCoroutine(LoadAsynchronously(sceneIndex));
    }

    IEnumerator LoadAsynchronously(int sceneIndex)
    {
        

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        loadingScreen.SetActive(true);

        //stop main menu song here
        FindObjectOfType<AudioManager>().Stop("TitleScreen");

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            loadingBar.value = progress;
            progressText.text = progress * 100f + "%";
            yield return null;
        }

        loadingScreen.SetActive(false);
    }
}
