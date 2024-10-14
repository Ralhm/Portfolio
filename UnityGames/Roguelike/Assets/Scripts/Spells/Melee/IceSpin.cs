using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpellData.asset", menuName = "SpellData/Ice")]
public class IceSpin : Spell
{
    public float AngularSpeed = 1;
    public float SpinSpeed;
    public float CircleTime = 1.0f;
    public float damageInterval = 0;
    float timer = 0;
    float currentAngle;
    Vector2 FixedPoint;

    public IceSpin()
    {
        SpellName = "IceSpin";
    }
    public override void ActivateSpell()
    {
        base.ActivateSpell();
        //Debug.Log("Activating Ice");
    }

    public override void Cast()
    {
        base.Cast();

        Debug.Log("Casting Ice");
        player.SetState(Player.State.CastingSpell);
        player.Anim.SetTrigger("Spin");


        player.SetColor(Color.blue);
        player.StartCoroutine(Circle());
        player.StartCoroutine(IceDamage());

    }

    public IEnumerator Circle()
    {
        currentAngle = -Mathf.Atan2(-player.ForwardDir.y, player.ForwardDir.x) + 1.5708f;
        //Debug.Log("Forward Angle:" + Mathf.Atan2(player.ForwardDir.y, player.ForwardDir.x));
        //Debug.Log("Forward Dir:" + player.ForwardDir);
        //Debug.Log("Forward Angle Degrees:" + Mathf.Atan2(player.ForwardDir.y, player.ForwardDir.x) * 180 / 3.141592);

        player.SetInvulnerable(true);

        timer = 0;
        FixedPoint = player.transform.position;
        while (timer < CircleTime)
        {
            currentAngle += AngularSpeed * Time.deltaTime;
            Vector2 Offset = new Vector2(Mathf.Sin(currentAngle) * 2, Mathf.Cos(currentAngle) * 2);
            
            Vector3 cross = Vector3.Cross(new Vector3(Offset.x, Offset.y, 0), Vector3.forward);
            Vector2 right = new Vector2(cross.x, cross.y);
            player.RB.AddForce(right * SpinSpeed);


            //Debug.DrawRay(player.transform.position, Offset, Color.green);
            //Debug.DrawRay(player.transform.position + new Vector3(Offset.x, Offset.y, 0), right, Color.green);
            yield return new WaitForFixedUpdate();
            timer += (1.0f / 60.0f);
        }
        //only set current spell to null when ending the spell naturally

        if (Active)
        {
            Debug.Log("ENding Ice!");
            FinishSpell();
            StopSpell();
        }

    }

    public IEnumerator IceDamage()
    {
        while (Active)
        {
            yield return new WaitForSeconds(damageInterval);
            //Debug.Log("Current time: " + timer);
            //Debug.Log("ICEDAMAGE!");
            CheckAOE(new Vector2(player.transform.position.x, player.transform.position.y));


        }
    }

    public override void CancelSpell()
    {
        base.CancelSpell();
        timer = CircleTime + 50;
        StopSpell();
        //Debug.Log("STOPPING ICE ROUTINE");
    }
    public override void StopSpell()
    {
        Active = false;
        player.StopCoroutine(IceDamage());
        player.StopCoroutine(Circle());
        player.SetInvulnerable(false);
        player.SetColor(Color.white);
        Debug.Log("Ice  Stopped!");
    }
}
