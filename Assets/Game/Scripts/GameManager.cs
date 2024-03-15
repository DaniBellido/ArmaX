using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject originalPlayer;
    public GameObject animationPlayer;
    public PlayableDirector cutscene1;

    // Start is called before the first frame update
    void Start()
    {
        // Deactivate the original player and activate the animation player
        originalPlayer.SetActive(false);
        animationPlayer.SetActive(true);

        // Play the animation
        cutscene1.Play();

    }

    // Update is called once per frame
    void Update()
    {
        // If the timeline has ended, destroy the animation player and activate the original player
        if (cutscene1.state == PlayState.Paused)
        {
            Destroy(animationPlayer);
            originalPlayer.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Joystick1Button1)) 
        {
            cutscene1.Pause();
        }

    }


    //THIS SHOULD BE ACTIVE AND NOT IN THE MainMenu script,
    //it's done this way to patch the issue in order to meet narrative deadline

    //public void LoadSceneAsync(int sceneIndex) 
    //{
    //    StartCoroutine(LoadAsynchronously(sceneIndex));
    //}

    //IEnumerator LoadAsynchronously(int sceneIndex) 
    //{
    //    AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

    //    while (!operation.isDone)
    //    {
    //        Debug.Log(operation.progress);

    //        yield return null;
    //    }
    //}
}
