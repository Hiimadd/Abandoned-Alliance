using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    public Ability activeAbility;
    private int maxHealth;
    private int currHealth;
    private int defense;
    private int moveSpeed;
    private int actionPoints;
    private int damage;
    private int x;
    private int y;
    private int sightRange;
    private string armorType;

    //Pass in the amount the health should change by, positive = increase in health, negative = decrease in health
    public void changeHealth(int change) {currHealth += change;}

    public void updatePosition(int X, int Y) {x = X; y = Y;}

    public int getHealth() {return currHealth;}

    public int getMoveSpeed() {return moveSpeed;}

    public int getSightRange() {return sightRange;}

    public int getDamage() {return damage;}

    public string getArmor() {return armorType;}


    public Hero(int Health,int Defense,int MoveSpeed,int ActionPoints,int Damage,int SightRange,string ArmorType,int StartX,int StartY)
    {
        maxHealth = currHealth = Health;
        defense = Defense;
        moveSpeed = MoveSpeed;
        actionPoints = ActionPoints;
        damage = Damage;
        sightRange = SightRange;
        armorType = ArmorType;
        x = StartX;
        y = StartY;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
