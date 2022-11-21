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

    public void init(int type, MapManager mm, int x, int y)
    {
        currUnit = null;
        tileType = type;
        mapManager = mm;
        Renderer.color = defaultColor = transparent;
        X = x;
        Y = y;
    }

    public int getX() {return X;}
    public int getY() {return Y;}

    public void setHero(Hero hero)
    {
        currUnit = hero;
    }

    public Hero getHero()
    {
        return currUnit;
    }

    public int getType()
    {
        return tileType;
    }

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

    void OnMouseEnter()
    {
        if(tileType == 1)
        {
            Renderer.color = highlight;
        }
    }

    void OnMouseExit()
    {
        if(tileType == 1)
        {
            Renderer.color = defaultColor;
        }
    }

    void OnMouseDown()
    {
        //Debug.Log($"X: {X}, Y: {Y}");
        mapManager.triggerAbility(this);
    }
}