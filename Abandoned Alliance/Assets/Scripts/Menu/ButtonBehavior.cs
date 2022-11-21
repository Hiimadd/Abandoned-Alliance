using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonBehavior : MonoBehaviour
{
	
    // Start is called before the first frame update
    void Start()
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
    	//UnityEditor.EditorApplication.isPlaying = false; // Editor version
    }
}
