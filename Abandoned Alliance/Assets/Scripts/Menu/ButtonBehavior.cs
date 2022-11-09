using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    // Scene changer
    public void ChangeScene(string nextScene)
    {
    	SceneManager.LoadScene(nextScene);
    }
    
    // Quit game
    public void QuitGame()
    {
    	Application.Quit();
    	UnityEditor.EditorApplication.isPlaying = false; // Editor version
    }
}
