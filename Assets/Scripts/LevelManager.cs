using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    // Singleton instance
    public static LevelManager Instance { get; private set; }

    // Reference to the transition animator
    public Animator transitionAnimator;

    private void Awake()
    {
        // Singleton instance check
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Prevent destruction on scene load
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //transitionAnimator.SetTrigger("EndTransition");
    }


    // Function to load the next level
    public void LoadNextLevel()
    {

        if (SceneManager.GetActiveScene().buildIndex + 1 < SceneManager.sceneCountInBuildSettings)
        {
            StartCoroutine(LoadLevelWithTransition());
        }
        else
        {
            Debug.Log("No more levels to load.");
        }
    }

    // Coroutine to handle the transition and level loading
    private IEnumerator LoadLevelWithTransition()
    {
        yield return new WaitForSeconds(0.5f); // Adjust this duration to match your animation

        // Play the fade-out animation
        transitionAnimator.SetTrigger("End");

        // Wait for the fade-out animation to finish
        yield return new WaitForSeconds(1f); // Adjust this duration to match your animation

        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);

        transitionAnimator.SetTrigger("Start");

    }
}
