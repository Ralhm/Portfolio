using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StandardAttack : Skills
{
    public StandardAttack() : base()
    {
        type = "Standard";
        audioName = "Slap";
        cost = 0;
        action = ActionType.Timing;
        SkillName = "Attack";
        SkillDescription = "Standard Attack. " + cost + "SP";
    }


    public override void Execute(Character Target, Character User, bool Hit)
    {
        if (Hit)
        {
            Debug.Log("Hit!");
            Target.TakeDamage(User, bonus, property, subProperty);
        }
        else
        {
            Debug.Log("Miss...");
            Target.TakeDamage(User, -1, property, subProperty);
        }

        
        Debug.Log("Executing Standard Attack");
    }
}

public class Thunder : Skills
{

    public Thunder() : base()
    {
        statusEffect = true;
        type = "Magic";
        duration = 1;
        bonus = 2;
        cost = 3;
        timing = 1.5F;
        action = ActionType.Aim;
        SkillName = "Thunder";
        SkillDescription = "Drop a Lightning Bolt on an enemy, Low chance to stun." + cost + "SP";
        property = Property.Electric;
        subProperty = Property.Stun;
        successRate = 40;

    }
    

    public override void Execute(Character Target, Character User, bool Hit)
    {
        audioName = "Electric1";
        if (Hit)
        {
            Debug.Log("Hit!");
            Target.Affliction(subProperty, successRate, duration);
            Target.TakeDamage(User, bonus, property, subProperty);
            
        }
        else
        {
            Debug.Log("Miss...");
            
        }
        User.LoseSP(cost);
        Debug.Log("Casting Thunder");
    }
}


public class Fireball : Skills
{

    public Fireball() : base()
    {
        type = "Magic";
        bonus = 5;
        cost = 4;
        timing = 7F;
        action = ActionType.Mash;
        SkillName = "Fireball";
        SkillDescription = "Shoot a ball of flame. " + cost + "SP";
        property = Property.Fire;
    }

    public override void Execute(Character Target, Character User, bool Hit)
    {
        audioName = "Fire1";

        if (Hit)
        {
            Debug.Log("Hit!");
            Target.TakeDamage(User, bonus, property, subProperty);
        }
        else
        {
            Debug.Log("Miss...");
            User.health -= 1;

        }
        User.LoseSP(cost);
        Debug.Log("Casting Fireball");
    }
}

public class IceShatter : Skills
{
    
    public IceShatter() : base()
    {
        singleTarget = false;
        type = "Magic";
        bonus = 0;
        cost = 4;
        timing = 1.6F;
        action = ActionType.Sequence;
        SkillName = "IceShatter";
        SkillDescription = "Pierce All enemies with a frigid shard of ice. " + cost + "SP";
        property = Property.Ice;
    }

    public override void Execute(List<Enemy> Target, Character User, bool Hit)
    {
        audioName = "Ice1";

        if (Hit)
        {
            Debug.Log("Hit!");
            AttackAll(Target, User);
        }
        else
        {
            Debug.Log("Miss...");

        }
        User.LoseSP(cost);
        Debug.Log("Casting IceShatter");
    }
}

public class Lullaby : Skills
{

    public Lullaby() : base()
    {
        statusEffect = true;
        singleTarget = false;
        type = "Sing";
        bonus = 0;
        cost = 1;
        timing = 1F;
        action = ActionType.Timing;
        SkillName = "Lullaby";
        SkillDescription = "Sing a hypnotic tune, High chance to put all targets to sleep. " + cost + "SP";
        subProperty = Property.Sleep;
        successRate = 80;
    }

    public override void Execute(List<Enemy> Target, Character User, bool Hit)
    {
        audioName = "Restore";
        if (Hit)
        {
            AttackAll(Target, User);
        }
        else
        {
            Debug.Log("Miss...");

        }
        User.LoseSP(cost);
        Debug.Log("Executing Lullaby");
    }
}

public class StrongThrust : Skills
{

    public StrongThrust() : base()
    {
        
        type = "Thrust";
        bonus = 4;
        cost = 4;
        action = ActionType.Timing;
        SkillName = "StrongThrust";
        SkillDescription = "Stabbing Motion, deals Stab damage. " + cost + "SP";
        timing = 2F;
        property = Property.Stab;
    }

    public override void Execute(Character Target, Character User, bool Hit)
    {
        audioName = "StrongAttack";
        if (Hit)
        {
            Debug.Log("Hit!");
            Target.TakeDamage(User, bonus, property, subProperty);
        }
        else
        {
            Debug.Log("Miss...");
            Target.TakeDamage(User, -1, property, subProperty);

        }

        User.LoseSP(cost);
        Debug.Log("Executing Thrust");
    }
}

public class ShieldBash : Skills
{

    public ShieldBash() : base()
    {
        statusEffect = true;
        type = "Shield";
        duration = 1;
        bonus = 2;
        cost = 4;
        action = ActionType.Timing;
        SkillName = "ShieldBash";
        SkillDescription = "Slam your shield into the enemy, deals bash damage and has high chance to stun. " + cost + "SP";
        timing = 2F;
        property = Property.Bash;
        subProperty = Property.Stun;
        successRate = 75;
    }

    public override void Execute(Character Target, Character User, bool Hit)
    {
        audioName = "Bash";
        if (Hit)
        {
            Debug.Log("Hit!");
            Target.Affliction(subProperty, successRate, duration);
            Target.TakeDamage(User, bonus, property, subProperty);
            
        }
        else
        {
            Debug.Log("Miss...");
            Target.TakeDamage(User, -1, property, subProperty);

        }

        User.LoseSP(cost);
        Debug.Log("Executing Thrust");
    }
}

public class Taunt : Skills
{
    public Taunt() : base()
    {
        type = "Special";
        cost = 3;
        timing = 1.45F;
        action = ActionType.None;
        SkillName = "Taunt";
        effectAmount = 2;
        SkillDescription = "Raise targets Draw Rate and defense by" + effectAmount + ". " + cost + "SP";
        buffType = BuffType.Defense;
        property = Property.Party;

    }


    public override void Execute(Player Target, Character User, bool Hit)
    {
        audioName = "Buff";
        User.LoseSP(cost);
        if (Hit)
        {
            effectActive = isEffected(Target, this);
            if (!effectActive)
            {
                Debug.Log("Applying the Buff");
                Target.drawRate += 15;
            }

        }


    }
}

public class BroadSlash : Skills
{
    public BroadSlash() : base()
    {
        singleTarget = false;
        type = "Standard";
        timing = 0.5F;
        action = ActionType.Timing;
        SkillName = "BroadSlash";
        SkillDescription = "Slash all foes with a blade. " + cost + "SP";
        property = Property.Slash;
    }

    public override void Execute(List<Enemy> Target, Character User, bool Hit)
    {
        audioName = "Slash2";
        if (Hit)
        {
            Debug.Log("Hit!");
            AttackAll(Target, User);

        }
        else
        {
            Debug.Log("Miss...");


        }
        User.LoseSP(cost);
        Debug.Log("Executing BroadSlash");
    }
}

public class DaggerFlurry : Skills
{

    public DaggerFlurry() : base()
    {
        type = "Standard";
        bonus = 3;
        cost = 3;
        timing = 1.6F;
        action = ActionType.Sequence;
        SkillName = "DaggerFlurry";
        SkillDescription = "Slice foe with daggers. " + cost + "SP";
        property = Property.Slash;
    }

    public override void Execute(Character Target, Character User, bool Hit)
    {
        audioName = "Slash1";
        if (Hit)
        {
            Debug.Log("Hit!");
            Target.TakeDamage(User, bonus, property, subProperty);
        }
        else
        {
            Debug.Log("Miss...");
            Target.TakeDamage(User, -1, property, subProperty);

        }

        User.LoseSP(cost);
        Debug.Log("Executing DaggerFlurry");
    }
}

public class ArrowShot : Skills
{

    public ArrowShot() : base()
    {
        type = "Arrow";
        bonus = 1;
        cost = 2;
        timing = 1.5F;
        action = ActionType.Aim;
        SkillName = "ArrowShot";
        SkillDescription = "Fire an arrow, deals Stab damage. " + cost + "SP";
        property = Property.Stab;
    }

    public override void Execute(Character Target, Character User, bool Hit) 
    {
        audioName = "Stab";
        if (Hit)
        {
            Debug.Log("Hit!");
            Target.TakeDamage(User, bonus, property, subProperty);
        }
        else
        {
            Debug.Log("Miss...");

        }
        User.LoseSP(cost);
        Debug.Log("Executing ArrowShot");
    }
}

public class PoisonArrow : Skills
{

    public PoisonArrow() : base()
    {
        statusEffect = true;
        type = "Arrow";
        bonus = 1;
        cost = 4;
        timing = 1.5F;
        action = ActionType.Aim;
        SkillName = "PoisonArrow";
        SkillDescription = "Fire an arrow with a poisoned tip, deals stab damage and has a high chance to poison. " + cost + "SP";
        property = Property.Stab;
        subProperty = Property.Poison;
        successRate = 80;
    }

    public override void Execute(Character Target, Character User, bool Hit)
    {
        audioName = "Stab";
        if (Hit)
        {
            Debug.Log("Hit!");
            Target.Affliction(subProperty, successRate, duration);
            Target.TakeDamage(User, bonus, property, subProperty);
            
        }
        else
        {
            Debug.Log("Miss...");

        }
        User.LoseSP(cost);
        Debug.Log("Executing PoisonArrow");
    }
}

public class Acupuncture : Skills
{
    public Acupuncture() : base()
    {
        statusEffect = true;
        singleTarget = false;
        cost = 3;
        type = "Standard";
        action = ActionType.Sequence;
        SkillName = "Acupuncture";
        SkillDescription = "Toss needles that deal stab damage at all enemies. Medium chance to paralyze." + cost + "SP"; ;
        property = Property.Stab;
        subProperty = Property.Paralyze;
        successRate = 50;


    }

    public override void Execute(List<Enemy> Target, Character User, bool Hit)
    {
        audioName = "Stab";

        if (Hit)
        {
            Debug.Log("Hit!");
            AttackAll(Target, User);
        }
        else
        {
            Debug.Log("Miss...");

        }
        User.LoseSP(cost);
        Debug.Log("Executing Acupuncture");
    }
}

public class DeliriumBomb : Skills
{
    public DeliriumBomb() : base()
    {
        statusEffect = true;
        singleTarget = false;
        cost = 4;
        type = "Bomb";
        action = ActionType.Sequence;
        SkillName = "Delirium Bomb";
        SkillDescription = "Toss a bomb that deals fire damage to all enemies. Low chance to confuse. " + cost + "SP"; ;
        property = Property.Fire;
        subProperty = Property.Confuse;
        successRate = 35;

    }

    public override void Execute(List<Enemy> Target, Character User, bool Hit)
    {
        audioName = "Fire1";

        if (Hit)
        {
            Debug.Log("Hit!");
            AttackAll(Target, User);
        }
        else
        {
            Debug.Log("Miss...");

        }
        User.LoseSP(cost);
        Debug.Log("Executing Delirium Bomb");
    }
}

public class ToxicBomb : Skills
{
    public ToxicBomb() : base()
    {
        statusEffect = true;
        singleTarget = false;
        cost = 4;
        type = "Bomb";
        action = ActionType.Sequence;
        SkillName = "Toxic Bomb";
        SkillDescription = "Toss a bomb with a High chance to poison all enemies. " + cost + "SP"; ;
        subProperty = Property.Poison;
        successRate = 90;
    }

    public override void Execute(List<Enemy> Target, Character User, bool Hit)
    {
        audioName = "Fire1";

        if (Hit)
        {
            Debug.Log("Hit!");
            AttackAll(Target, User);
        }
        else
        {
            Debug.Log("Miss...");

        }
        User.LoseSP(cost);
        Debug.Log("Executing Toxic Bomb");
    }
}

public class StrangeBrew : Skills
{
    public StrangeBrew() : base()
    {
        cost = 1;
        type = "Potion";
        action = ActionType.None;
        SkillName = "Strange Brew";
        SkillDescription = "Improvise a potion that will heal a random amount up to 10 health. " + cost + "SP"; ;
        property = Property.Party;


    }

    public override void Execute(Player Target, Character User, bool Hit)
    {
        audioName = "Item";
        Target.RestoreHealth(Random.Range(1, 10));
        User.LoseSP(cost);
        Debug.Log("Executing Strange Brew");
    }
}


public class AttackDebuff : Skills
{
    public AttackDebuff() : base()
    {
        type = "Special";
        cost = 1;
        timing = 1.45F;
        action = ActionType.None;
        SkillName = "AttackDebuff";
        effectAmount = 2;
        SkillDescription = "Lower targets Attack by " + effectAmount + ". " + cost + "SP"; 
        buffType = BuffType.Attack;
    } 


    public override void Execute(Character Target, Character User, bool Hit) 
    {
        audioName = "Debuff";
        User.LoseSP(cost);
        if (Hit)
        {
            effectActive = isEffected(Target, this);
            if (!effectActive)
            {
                Debug.Log("Applying the debuff");
                Target.AddDebuff(this);
            }

        }
        

    }
}



public class DefenseDebuff : Skills
{
    public DefenseDebuff() : base()
    {
        type = "Special";
        cost = 1;
        timing = 1.45F;
        action = ActionType.None;
        SkillName = "DefenseDebuff";
        effectAmount = 2;
        SkillDescription = "Lower targets Defense by " + effectAmount + ". " + cost + "SP";
        buffType = BuffType.Defense;
    }


    public override void Execute(Character Target, Character User, bool Hit)
    {
        audioName = "Debuff";
        User.LoseSP(cost);
        if (Hit)
        {
            effectActive = isEffected(Target, this);

            if (!effectActive)
            {
                Debug.Log("Applying the debuff");
                Target.AddDebuff(this);
            }



            
        }
    }
}

public class DefenseBuff : Skills
{
    public DefenseBuff() : base()
    {
        type = "Special";
        cost = 3;
        timing = 1.45F;
        action = ActionType.None;
        SkillName = "DefenseBuff";
        effectAmount = 4;
        SkillDescription = "Increase targets Defense by " + effectAmount + ". " + cost + "SP";
        buffType = BuffType.Defense;
        property = Property.Party;
    }


    public override void Execute(Player Target, Character User, bool Hit)
    {
        audioName = "Buff";
        User.LoseSP(cost);
        if (Hit)
        {
            effectActive = isEffected(Target, this);

            if (!effectActive)
            {
                Debug.Log("Applying the buff");
                Target.AddBuff(this);
            }

        }
    }
}

public class AttackBuff : Skills
{
    public AttackBuff() : base()
    {
        type = "Special";
        cost = 3;
        timing = 1.45F;
        action = ActionType.None;
        SkillName = "AttackBuff";
        effectAmount = 4;
        SkillDescription = "Increase targets Attack by " + effectAmount + ". " + cost + "SP";  
        buffType = BuffType.Attack;
        property = Property.Party;

    }


    public override void Execute(Player Target, Character User, bool Hit)
    {
        audioName = "Buff";
        User.LoseSP(cost);
        if (Hit)
        {
            effectActive = isEffected(Target, this);
            if (!effectActive)
            {
                Debug.Log("Applying the Buff");
                Target.AddBuff(this);
            }

        }


    }
}

public class Heal : Skills
{

    public Heal() : base()
    {
        cost = 3;
        type = "Special";
        action = ActionType.None;
        SkillName = "Heal";
        SkillDescription = "Restore Targets Health. " + cost + "SP";
        property = Property.Party;
    }


    public override void Execute(Player Target, Character User, bool Hit)
    {
        audioName = "Heal";
        User.LoseSP(cost);
        Target.RestoreHealth(20);

        Debug.Log("Casting Heal");
    }
}


public class Resurrect : Skills
{

    public Resurrect() : base()
    {
        cost = 6;
        type = "Special";
        action = ActionType.None;
        SkillName = "Resurrect";
        SkillDescription = "Revives Target with 10 health. " + cost + "SP";
        property = Property.Party;
    }


    public override void Execute(Player Target, Character User, bool Hit)
    {
        audioName = "Restore";
        User.LoseSP(cost);
        Target.Revive(10);

        Debug.Log("Casting Revive");
    }
}



