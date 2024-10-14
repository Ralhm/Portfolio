using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

[CreateAssetMenu(fileName = "SpellData.asset", menuName = "SpellData/FireDash")]
public class FireDash : Spell
{
    public float DashSpeed = 5;
    public float dashTime = 1.0f;
    public float damageInterval = 0;
    float timer = 0;

    public FireDash()
    {
        SpellName = "FireDash";
    }
    public override void ActivateSpell()
    {
        base.ActivateSpell();
        
    }

    public override void Cast()
    {
        base.Cast();
        Debug.Log("Casting Fire");
        player.SetState(Player.State.CastingSpell);
        player.Anim.SetTrigger("Spin");

        player.SetColor(Color.red);
        player.GetRB().AddForce(player.ForwardDir * DashSpeed);
        player.StartCoroutine(Dash());
        player.StartCoroutine(DashDamage());

    }

    public IEnumerator Dash()
    {
        player.SetInvulnerable(true);
        timer = 0;
        while (timer < dashTime)
        {
            player.GetRB().AddForce(player.ForwardDir * DashSpeed);
            yield return new WaitForFixedUpdate();
            timer += (1.0f / 60.0f);
        }

        if (Active)
        {
            Debug.Log("ENding Fire Dash!");
            FinishSpell();
            StopSpell();
        }

    }

    public IEnumerator DashDamage()
    {
        while (Active)
        {
            //Debug.Log("Fire Damage!!!");
            yield return new WaitForSeconds(damageInterval);
            CheckAOE(new Vector2(player.transform.position.x, player.transform.position.y));
        }
    }

    public override void CancelSpell()
    {
        base.CancelSpell();
        timer = dashTime;

        Debug.Log("Cancelling Fire Dash!");

        StopSpell();
    }

    public override void StopSpell()
    {
        Debug.Log("Stopping Fire Dash!");
        Active = false;
        player.StopCoroutine(DashDamage());
        player.StopCoroutine(Dash());
        player.SetInvulnerable(false);
        player.SetColor(Color.white);
    }
}
