using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[CreateAssetMenu(fileName = "SpellData.asset", menuName = "SpellData/Thunder")]
public class Thunder : Spell
{
    public GameObject effect;
    
    public Thunder()
    {
        SpellName = "Thunder";
    }

    public override void ActivateSpell()
    {
        base.ActivateSpell();
        effect.GetComponent<Animator>().ResetTrigger("Lightning");
        
    }
    public override void Cast()
    {
        base.Cast();

        Vector2 origin = new Vector2(player.transform.position.x, player.transform.position.y) + new Vector2(player.ForwardDir.x, player.ForwardDir.y) * 4;
        
        //super quick super dirty
        //effect.transform.position = origin;
        //effect.GetComponent<Animator>().SetBool("Lighting", true);

        
        //effect.GetComponent<Animation>().Play("Lightning");

        CheckAOE(origin);


    }


}
