using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HeroMovement : MonoBehaviour
{
    //Thisd is a temp hero movement script to test out fog of war.
    //some function in this file will not be in the beta release
    private Vector2 velocity;
    private Vector3 direction;
    private bool hasMoved;
    public Tilemap fogOfWarTile;
    [SerializeField] private int visionRange;
    void Start()
    {
        FogUpdate();
    }
    // Update is called once per frame
    void Update()
    {
        if(velocity.x == 0)
        {
            hasMoved = false;
        }
        else if(velocity.x != 0 && !hasMoved)
        {
            hasMoved = true;
            MoveByDir();
        }
        velocity = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    //Moving in a certain direction. At the current moment, the user will use keyboard to move,
    //but later on, player would click to a valid position to move.
    private void MoveByDir()
    {
        if(velocity.x < 0) //move left
        {
            if(velocity.y > 0) //move upper left
            {
                direction = new Vector3(-0.5f, 0.5f);
            }
            else if(velocity.y < 0) //move bot left
            {
                direction = new Vector3(-0.5f, -0.5f);
            }
            else
            {
                direction = new Vector3(-1f, 0f); //move left
            }
        }
        else if (velocity.x > 0) //move right
        {
            if(velocity.y > 0)
            {
                direction = new Vector3(0.5f, 0.5f);
            }
            else if (velocity.y < 0)
            {
                direction = new Vector3(0.5f, -0.5f);
            }
            else
            {
                direction = new Vector3(1, 0);
            }
        }

        transform.position += direction;
        FogUpdate();
    }

    //Update the fog as the hero moves
    private void FogUpdate()
    {
        Vector3Int currPlayerPos = fogOfWarTile.WorldToCell(transform.position); // cell position convert to world position
        for(int i = -3; i <= visionRange; i++)
        {
            for (int j = -3; j <= visionRange; j++)
            {
                fogOfWarTile.SetTile(currPlayerPos + new Vector3Int(i, j, 0), null);// remove tile surround the player
            }
        }
    }
}
