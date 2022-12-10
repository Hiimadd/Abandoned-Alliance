using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseBehavior : MonoBehaviour
{
	public static bool paused;
	MapManager manager;
	GameObject curCan;
	GameObject pMenu;
	GameObject cam;
	
    // Start is called before the first frame update
    void Start()
    {
        paused = false;
        cam = GameObject.Find("Main Camera");
        manager = GameObject.Find("MapManager").GetComponent<MapManager>();
        pMenu = GameObject.Find("PauseMenu");
        pMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
        	if(!paused)
        	{
        		pauseGame();
        	}
        	else
        	{
        		resumeGame();
        	}
        }
    }
    
    private void pauseGame()
    {
    	paused = true;
    	Time.timeScale = 0;
    	curCan = manager.TurnOrder[manager.CurrTurn].transform.Find("Canvas").gameObject;
    	curCan.SetActive(false);
    	pMenu.SetActive(true);
    	cam.GetComponent<CameraMovement>().enabled = false;
    }
    
    public void resumeGame()
    {
    	paused = false;
    	Time.timeScale = 1;
    	pMenu.SetActive(false);
    	curCan.SetActive(true);
    	cam.GetComponent<CameraMovement>().enabled = true;
    }
}
