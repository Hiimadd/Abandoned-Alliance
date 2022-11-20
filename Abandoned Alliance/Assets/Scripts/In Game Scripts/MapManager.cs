using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    List<List<Tile>> map;
    List<Hero> turnOrder;
    int currTurn;
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private MapManager myself;
    [SerializeField] private Hero heroPrefab;
    [SerializeField] private Tilemap fog;
    [SerializeField] private int width = 32;
    [SerializeField] private int height = 32;
    [SerializeField] private float startX;
    [SerializeField] private float startY;
    [SerializeField] private List<Vector3> blockedTiles; //X = Column, Y = bottom row to be blocked, Z = top row to be blocked. Should be least-to greatest.
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
        Hero hero1 = Instantiate(heroPrefab, new Vector3(0,0), Quaternion.identity);
        hero1.init(10, 0, 1, 3, 2, 3, false, map[0][0], this, fog);
        hero1.name = "Knight";
        turnOrder.Add(hero1);
        hero1.transform.Find("Canvas").gameObject.SetActive(true);


        Hero dummy = Instantiate(heroPrefab, new Vector3(5,5), Quaternion.identity);
        dummy.init(10, 0, 1, 3, 2, 3, true, map[3][3], this, fog);
        dummy.name = "Dummy1";
        turnOrder.Add(dummy);

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
            if(currTurn >= turnOrder.Count) {currTurn = 0;}
            int startingPosition = currTurn;
            while(turnOrder[currTurn].checkIsDummy())
            {
                if(currTurn >= turnOrder.Count-1) 
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