using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonBehavior : MonoBehaviour
{
	public string mapName;
	private Dropdown mapDrop;
	
    // Start is called before the first frame update
    void Start()
    {
        mapName = "Map1";
        mapDrop = GameObject.Find("Dropdown").GetComponent<Dropdown>();
    }
    
    public void SetMap()
    {
    	mapName = mapDrop.options[mapDrop.value].text;
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
