using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private Hero currUnit;
    private int tileType;
    private MapManager mapManager;
    private int X;
    private int Y;
    [SerializeField] private Color transparent, highlight, abilityHighlight, invalidAbilityHighlight;
    [SerializeField] private SpriteRenderer Renderer;
    private Color defaultColor;

    //Roughly equivelent to a constructor, this is called on Tile creation to set the properties of the generated Tile.
    public void init(int type, MapManager mm, int x, int y)
    {
        currUnit = null;
        tileType = type;
        mapManager = mm;
        Renderer.color = defaultColor = transparent;
        X = x;
        Y = y;
    }

    //Variables to store/return the position of this tile relitive to the other tiles that have been generated.
    //Useful if you want to find adjacent tiles to a given position
    public int getX() {return X;}
    public int getY() {return Y;}

    //Update the tracking of the Hero which is currently on this tile
    public void setHero(Hero hero)
    {
        currUnit = hero;
    }

    //Return the Hero currently standing on this tile, for use with abilities or more general checks within a MapManager.
    public Hero getHero()
    {
        return currUnit;
    }

    //Returns an int indicating if the tile is walkable (1) or not (any other number).
    //May be useful to eventually to create more tile type definitions, but for now walkable or not is sufficient.
    public int getType()
    {
        return tileType;
    }

    //Indended use is to indicate that an ability can be used on this tile if it is clicked
    //Functionally all it does is change the "resting" (not moused over) state of a tile to be abilityHighlight.
    public void toggleAbilityHighlight()
    {
        if(defaultColor != abilityHighlight)
        {
            defaultColor = abilityHighlight;
        }
        else
        {
            defaultColor = transparent;
        }
        Renderer.color = defaultColor;
    }

    //Indended use is to indicate that an ability could be used on this tile, if not for some factor that makes it invalid, such as a presence/lack of units.
    //Functionally all it does is change the "resting" (not moused over) state of a tile to be invalidAbilityHighlight.
    public void toggleInvalidAbilityHighlight()
    {
        if(defaultColor != invalidAbilityHighlight)
        {
            defaultColor = invalidAbilityHighlight;
        }
        else
        {
            defaultColor = transparent;
        }
        Renderer.color = defaultColor;
    }

    //Part of a more complex mouseover highlight system that simply highlights the tile the user is hovering over normally,
    //But should also show the effect radius of an area attack once those are implemented correctly.
    public void toggleMouseHighlight(bool shouldHighlight)
    {
        if(shouldHighlight)
        {
            Renderer.color = highlight;
        }
        else
        {
            Renderer.color = defaultColor;
        }
    }

    //Part of the mouseover highlight system, this indicates that the user is hovering over the tile.
    void OnMouseEnter()
    {
        if(tileType == 1)
        {
            mapManager.mouseHighlight(X, Y, true);
        }
    }

    //Part of the mouseover highlight system, this indicates that the user has stopped hovering over the tile and that it therefore should no longer be highlighted.
    void OnMouseExit()
    {
        if(tileType == 1)
        {
            mapManager.mouseHighlight(X, Y, false);
        }
    }

    //Captures click events on tiles. Essential for ability activation, as they need to know where the user is attempting to use a given ability.
    void OnMouseDown()
    {
        //Debug.Log($"X: {X}, Y: {Y}");
        mapManager.triggerAbility(this);
    }
}