using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpellData.asset", menuName = "SpellData/Projectile")]
public class Projectile : Spell
{

    public SpellBullet BulletClass;
    public float FireRate;
    public bool DestroyOnContact;


    public override void Cast()
    {
        base.Cast();



        SpellBullet bullet = (SpellBullet)player.GetPool().GetObjectInPool(BulletClass, player.transform);
        bullet.SetVelocity(player.ForwardDir);
        bullet.SetSpellData(this);
        bullet.StartCoroutine(bullet.DestroySelf());
    }


}
