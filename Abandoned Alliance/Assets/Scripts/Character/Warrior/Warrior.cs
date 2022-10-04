using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warrior : HeroBase
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /* Warrior's basic attack is a single attack, therefore can just call method from base class */
    public void attack()
    {
        base.basicAttack(1);
    }


    /* Warrior's 2nd ability
     * to be implement
     * 
     * Description: "Fight On Soldier!", select one ally to buff defense and attack for 2 turns.
     * Properties: Select a target ally to buff.
     * 
     * 
     * 
     */
    public override void ability2()
    {
        Debug.Log("Used ultimatenon-target ability");
    }

    /* Warrior's ultimate ability
     * to be implement
     * 
     * Description: Attack with a massive shield, advanced 2 tile in front and if hit an enemy, stun the target for 1 turns.
     * Properties: 1 tile attack, inflict stun, AP cost: TBD, cooldown: 6 turns based, fully upgrade will reduce it down to 5 turns.
     * 
     * 
     * 
     */
    public void ultimateAbility()
    {
        Debug.Log("Used ultimate ability");
    }

}
