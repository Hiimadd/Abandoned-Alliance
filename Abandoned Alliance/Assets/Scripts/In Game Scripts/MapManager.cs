using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    [Serializable]
    public struct HeroArgs
    {
        public int Health;
        public int Defense;
        public int MoveSpeed;
        public int ActionPoints;
        public int Damage;
        public int SightRange;
        public bool dummy;
        public Vector2Int startPos;
        public int heroType;
    }


    List<List<Tile>> map;
    public List<Hero> turnOrder;
    public int currTurn;
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private MapManager myself;
    [SerializeField] private Hero heroPrefab;
    [SerializeField] private Tilemap fog;
    [SerializeField] private int width = 32;
    [SerializeField] private int height = 32;
    [SerializeField] private float startX;
    [SerializeField] private float startY;
    [SerializeField] private List<Vector3> blockedTiles; //X = Column, Y = bottom row to be blocked, Z = top row to be blocked. Should be least-to greatest.
    [SerializeField] private List<HeroArgs> toSpawn;
    private double canUseShortcut = 0;
    private double canUseShortcutWait = 0.25;

    public void makeGrid()
    {
        map = new List<List<Tile>>();
        int blockedTilesPos = 0;
        for(int i = 0; i < width; ++i)
        {
            if(blockedTilesPos < blockedTiles.Count)
            {
                while(blockedTiles[blockedTilesPos].x < i) {++blockedTilesPos; if(blockedTilesPos >= blockedTiles.Count) {break;}}
            }
            List<Tile> row = new List<Tile>();
            for(int j = 0; j < height; ++j)
            {
                Tile spawnedTile = Instantiate(tilePrefab, new Vector3(startX+i, startY+j), Quaternion.identity);
                spawnedTile.name = $"Tile {i} {j}";
                if(blockedTilesPos < blockedTiles.Count && blockedTiles[blockedTilesPos].x == i && blockedTiles[blockedTilesPos].y <= j && blockedTiles[blockedTilesPos].z >= j)
                {
                    spawnedTile.init(2, myself, i, j);
                }
                else
                {
                    spawnedTile.init(1, myself, i, j);
                }
                row.Add(spawnedTile);
            }
            map.Add(row);
        }
    }

    public void spawnHeroes()
    {
        turnOrder = new List<Hero>();
        currTurn = 0;
        bool firstPlayer = true;
        for(int i = 0; i < toSpawn.Count; ++i)
        {
            Hero h = Instantiate(heroPrefab, new Vector3(0,0), Quaternion.identity);
            HeroArgs a = toSpawn[i];
            h.init(a.Health, a.Defense, a.MoveSpeed, a.ActionPoints, a.Damage, a.SightRange, a.dummy, map[a.startPos.x][a.startPos.y], this, fog);
            turnOrder.Add(h);
            switch(a.heroType)
            {
                case 1: //knight
                    h.transform.Find("KnightSprite").gameObject.SetActive(true);
                    h.transform.Find("KnightSprite").gameObject.name = "Sprite";
                    h.transform.Find("KnightCanvas").gameObject.name = "Canvas";
                break;
                case 2: //wizard
                    h.transform.Find("WizardSprite").gameObject.SetActive(true);
                    h.transform.Find("WizardSprite").gameObject.name = "Sprite";
                    h.transform.Find("WizardCanvas").gameObject.name = "Canvas";
                break;
                case 3: //ranger
                    h.transform.Find("RangerSprite").gameObject.SetActive(true);
                    h.transform.Find("RangerSprite").gameObject.name = "Sprite";
                    h.transform.Find("RangerCanvas").gameObject.name = "Canvas";
                break;
                default: //monster
                    h.transform.Find("MonsterSprite").gameObject.SetActive(true);
                    h.transform.Find("MonsterSprite").gameObject.name = "Sprite";
                break;
            }
            if(!a.dummy)
            {
                foreach(Transform t in h.transform.Find("Canvas"))
                {
                    Ability toInit = t.gameObject.GetComponent(typeof(Ability)) as Ability;
                    toInit.init();
                }

                if(firstPlayer)
                {
                    h.transform.Find("Canvas").gameObject.SetActive(true);
                    firstPlayer = false;
                }
            }
        }
        /*Hero hero1 = Instantiate(heroPrefab, new Vector3(0,0), Quaternion.identity);
        hero1.init(10, 0, 1, 3, 2, 3, false, map[0][0], this, fog);
        hero1.name = "Knight";
        turnOrder.Add(hero1);
        hero1.transform.Find("Canvas").gameObject.SetActive(true);


        Hero dummy = Instantiate(heroPrefab, new Vector3(5,5), Quaternion.identity);
        dummy.init(10, 0, 1, 3, 2, 3, true, map[3][3], this, fog);
        dummy.name = "Dummy1";
        turnOrder.Add(dummy);*/

    }

    public void mouseHighlight(int X, int Y)
    {
        Hero activeHero = turnOrder[currTurn];
        if(activeHero.activeAbility == null)
        {
            map[X][Y].toggleMouseHighlight();
        }
        else
        {
            map[X][Y].toggleMouseHighlight();
        }
    }

    public void triggerAbility(Tile loc)
    {
        Hero activeHero = turnOrder[currTurn];
        if(activeHero.activeAbility == null) {return;}
        activeHero.activeAbility.UseAbility(loc);
    }

    public Tile getPos(int x, int y)
    {
        if(x > -1 && x < width && y > -1 && y < height)
        {
            return map[x][y];
        }
        return null;
    }

    public void killHero(Hero hero)
    {
        if(turnOrder[currTurn] == hero) //unit manages to kill itself
        {
            advTurn(hero.getAP());
            turnOrder.Remove(hero);
        }
        else
        {
            for(int i = 0; i < currTurn; ++i) {if(turnOrder[i] == hero) {--currTurn; break;}}
            turnOrder.Remove(hero);
        }
        Destroy(hero.gameObject);
    }

    public void advTurn(int apUsed)
    {
        Hero activeHero = turnOrder[currTurn];
        if(activeHero.getAP() == apUsed)
        {
            activeHero.resetAP();
            activeHero.transform.Find("Canvas").gameObject.SetActive(false);
            ++currTurn;
            if(currTurn >= turnOrder.Count-1) {currTurn = 0;}
            int startingPosition = currTurn;
            while(turnOrder[currTurn].checkIsDummy())
            {
                if(currTurn >= turnOrder.Count-2) 
                {
                    if(turnOrder[currTurn+1].checkIsDummy()) {currTurn = 0; continue;}
                }
                ++currTurn;
                if(currTurn == startingPosition) {break;} //case where the full list is dummies, shouldn't happen in demo but might when dummies are replaced with enemies
            }

            turnOrder[currTurn].transform.Find("Canvas").gameObject.SetActive(true);
        }
        else {activeHero.useAP(apUsed);}
    }

    void Start()
    {
        makeGrid();
        spawnHeroes();
    }

    void Update()
    {
        if(turnOrder[currTurn] == map[10][9].getHero())
        {
            Application.Quit();
    	    //UnityEditor.EditorApplication.isPlaying = false; // Editor version
        }
        if(canUseShortcut <= 0)
        {
            if(Input.GetKey("1"))
            {
                if(turnOrder[currTurn].transform.Find("Canvas").childCount >= 1)
                {
                    Ability temp = turnOrder[currTurn].transform.Find("Canvas").GetChild(0).gameObject.GetComponent(typeof(Ability)) as Ability;
                    temp.toggleActive();
                    canUseShortcut = canUseShortcutWait;
                }
            }
            else if(Input.GetKey("2"))
            {
                if(turnOrder[currTurn].transform.Find("Canvas").childCount >= 2)
                {
                    Ability temp = turnOrder[currTurn].transform.Find("Canvas").GetChild(1).gameObject.GetComponent(typeof(Ability)) as Ability;
                    temp.toggleActive();
                    canUseShortcut = canUseShortcutWait;
                }
            }
            else if(Input.GetKey("3"))
            {
                if(turnOrder[currTurn].transform.Find("Canvas").childCount >= 3)
                {
                    Ability temp = turnOrder[currTurn].transform.Find("Canvas").GetChild(2).gameObject.GetComponent(typeof(Ability)) as Ability;
                    temp.toggleActive();
                    canUseShortcut = canUseShortcutWait;
                }
            }
            else if(Input.GetKey("4"))
            {
                if(turnOrder[currTurn].transform.Find("Canvas").childCount >= 4)
                {
                    Ability temp = turnOrder[currTurn].transform.Find("Canvas").GetChild(3).gameObject.GetComponent(typeof(Ability)) as Ability;
                    temp.toggleActive();
                    canUseShortcut = canUseShortcutWait;
                }
            }
            else if(Input.GetKey("5"))
            {
                if(turnOrder[currTurn].transform.Find("Canvas").childCount >= 5)
                {
                    Ability temp = turnOrder[currTurn].transform.Find("Canvas").GetChild(4).gameObject.GetComponent(typeof(Ability)) as Ability;
                    temp.toggleActive();
                    canUseShortcut = canUseShortcutWait;
                }
            }
        }
        else
        {
            canUseShortcut -= Time.deltaTime;
        }
    }
}