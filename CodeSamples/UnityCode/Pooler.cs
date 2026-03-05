using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//Primary Pool Manager
//Keep frequently spawned game objects such as money, crystals, and damage numbers in a pool to minimize instantiation and destruction
//Though some of this may be redundant, splitting these into multiple distinct functions and lists makes for more easily readable/usable code

public class Pooler : MonoBehaviour
{
    public List<Money> PooledMoney;
    public List<Crystal> PooledCrystals;
    public List<HealthPickup> PooledHealth;
    public List<ParticleText> PooledDamage;

    public Money MoneyRef;
    public Crystal CrystalRef;
    public HealthPickup HealthRef;
    public ParticleText ParticleRef;

    public Money GetMoneyInPool(Transform transform)
    {
        for (int i = 0; i < PooledMoney.Count; i++)
        {

            if (!PooledMoney[i].gameObject.activeInHierarchy)
            {
                PooledMoney[i].gameObject.SetActive(true);
                PooledMoney[i].transform.position = transform.position;
                PooledMoney[i].RandomForce();
                return PooledMoney[i];
            }
        }
        return SpawnMoneyToPool(transform);
    }


    Money SpawnMoneyToPool(Transform transform)
    {
        Money instance = Instantiate(MoneyRef, transform.position, transform.rotation);
        PooledMoney.Add(instance);
        return instance;

    }

    public Crystal GetCrystalsInPool(Transform transform)
    {
        for (int i = 0; i < PooledCrystals.Count; i++)
        {

            if (!PooledCrystals[i].gameObject.activeInHierarchy)
            {
                PooledCrystals[i].gameObject.SetActive(true);
                PooledCrystals[i].transform.position = transform.position;
                PooledCrystals[i].RandomForce();
                //Debug.Log("Returning Pre-Exisitng crystal!");
                return PooledCrystals[i];
            }
        }
        return SpawnCrystalsToPool(transform);
    }


    Crystal SpawnCrystalsToPool(Transform transform)
    {
        //Debug.Log("Spawning new crystal!");
        Crystal instance = Instantiate(CrystalRef, transform.position, transform.rotation);
        PooledCrystals.Add(instance);
        return instance;

    }

    public HealthPickup GetHealthInPool(Transform transform)
    {
        for (int i = 0; i < PooledHealth.Count; i++)
        {

            if (!PooledHealth[i].gameObject.activeInHierarchy)
            {
                PooledHealth[i].gameObject.SetActive(true);
                PooledHealth[i].transform.position = transform.position;
                PooledHealth[i].RandomForce();
                return PooledHealth[i];
            }
        }
        return SpawnHealthToPool(transform);
    }


    HealthPickup SpawnHealthToPool(Transform transform)
    {
        HealthPickup instance = Instantiate(HealthRef, transform.position, transform.rotation);
        PooledHealth.Add(instance);
        return instance;

    }

    public ParticleText GetDamageInPool(Transform transform, int damage)
    {
        for (int i = 0; i < PooledDamage.Count; i++)
        {

            if (!PooledDamage[i].gameObject.activeInHierarchy)
            {
                PooledDamage[i].gameObject.SetActive(true);
                PooledDamage[i].SetDamage(transform.position, damage);
                return PooledDamage[i];
            }
        }
        return SpawnDamageToPool(transform, damage);
    }


    ParticleText SpawnDamageToPool(Transform transform, int damage)
    {
        ParticleText instance = Instantiate(ParticleRef, transform.position, new Quaternion(0, 0, 0, 0));
        PooledDamage.Add(instance);

        instance.SetDamage(transform.position, damage);
        return instance;

    }

}
