using System.Collections;
using UnityEngine;

//serialization
//Quick save system, quick save at designated points, quit upon quicksaving and delete upon loading quicksave
//save things like level progress, runs done, etc between runs (later)

//if you want to have a character that can have a second spell slot, just replace whichever spell you currently have equipped
//if you have an empty slot, fill that in
//otherwise replace whichever spell you currently have active
//make a copy of the spell you dropped so you can pick it back up in case you don't like it

//Damage    : 11111
//HeatLoss  : 1111
//ManaCost  : 11111
//MaxHealth : 111111
//MaxMana   : 1111111
//Speed     : 111111
//MaxHeat   : 11111
//Recharge  : 11111
//FireRate  : 11111

//---More unique spells

//Enemy Ideas
//Guy who accelerates/charges at you
//Guys who slowly walk at you and shoot at you
//Guy who sits still and shoots at you
//Guy who walks at you and does big club slam in 4 cardinal directions when near

//Add instruments when in active combat


//---unique assets
//Bullets

//---MiniMap
//--Rework/streamline spell casting
//--Dialogue with portraits and text effects
//-reworks door sprites
//-Floor script, dictates parameters of current floor/scene
//prevent same item from spawning multiple times? (remove from a pool on collecting)
//differentiate between magic treasue rooms and normal item treasure rooms?
//make player damage numbers red
//fine tune crystal spawn rates and prices
//balance spells and heat and mana


public class Player : MonoBehaviour
{
    // Start is called before the first frame update

    public enum State
    {
        Normal, CastingSpell
    }

    float FireRate;
    bool ChargingSpell;
    bool canShoot = true;
    bool isFiring;
    float horizInput;
    float verticalInput;
    float Mana;
    float CurrentHeat;
    bool Invulnerable;
    bool LosingHealth;

    private static Player instance;
    public static Player Instance { get { return instance; } }

    public State PlayerState = State.Normal;

    [Header("References")]
    public BulletPool SpellBulletPool;
    public Animator Anim;

    [Header("Data")]
    public PlayerData PlayerData;


    //The currently active spell. should not be set for projectiles
    //Should only be set for the kind of spell that can be cancelled mid cast
    public Spell CurrentSpell;

    [Header("AnimStuff")]
    public float UpThreshold;
    public float SideThreshold;



    [Header("BaseData")]
    public float ManaRechargeRate = 0.1f;
    public float ManaRechargeAmount = 0.1f;
    public float HeatHealthLossRate = 1.2f;
    public int HeatHealthLossAmount = 1;
    public float HeatDispelAmount;
    public float HeatLossRate = 1.2f;
    public float HeatLossAmount = 2.0f;
    public float HeatMultiplier;



    public Rigidbody2D RB;
    public Vector2 ForwardDir;
    public Vector2 MousePos;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    void Start()
    {

        //PlayerData = GameManager.instance.LoadPlayer();

        PlayerData.ProjectileSpell.SetOwner(this);
        FireRate = ((Projectile)PlayerData.ProjectileSpell).FireRate;
        PlayerData.MeleeSpell.SetOwner(this);
        PlayerData.SpecialSpell.SetOwner(this);


        PlayerData.Health = PlayerData.MaxHealth;
        Mana = PlayerData.MaxMana;

        UI.Instance.SetHeat(CurrentHeat);
        UI.Instance.SetMaxHeat(GetMaxHeat());
        UI.Instance.SetMaxMana(GetMaxMana());
        UI.Instance.SetMana(GetMana());
        UI.Instance.SetMaxHealth(PlayerData.MaxHealth);
        UI.Instance.SetHealth(PlayerData.Health);
        UI.Instance.UpdateDisplayValues();

        //AudioManager.Instance.SetSounds(spells);
        StartCoroutine(ManaRecharge());
        StartCoroutine(HeatLoss());
        StartCoroutine(HeatDamage());

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //ActivateSpell();
        }

        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            isFiring = true;
            if (canShoot)
            {
                
                Debug.Log("CAN fire!");
                StartCoroutine(FireBullet());
            }
            else
            {
                Debug.Log("Can't fire!");
            }
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            isFiring = false;
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            PlayerData.MeleeSpell.ActivateSpell();
        }
        if (Input.GetKeyDown(KeyCode.Mouse3))
        {
            PlayerData.SpecialSpell.ActivateSpell();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            DispelHeatMana();
        }


        if (Input.GetKeyDown(KeyCode.O))
        {
            GameManager.instance.SavePlayer(PlayerData);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            PlayerData = GameManager.instance.LoadPlayer();
            UI.Instance.UpdateDisplayValues();
            UpdateAllSliders();
        }


    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (!ChargingSpell)
        {
            horizInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");
            RB.velocity = new Vector2(horizInput * PlayerData.Speed, verticalInput * PlayerData.Speed) * GetSpeedModifier();


            MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            SetForwardDir();

            Debug.DrawRay(RB.position, ForwardDir, Color.black);
            Debug.DrawRay(RB.position, RB.velocity.normalized, Color.green);


        }
        

    }

    public void SetForwardDir()
    {
        ForwardDir = (MousePos - RB.position).normalized;

        float Dot = Vector2.Dot(ForwardDir, Vector2.up);

        bool SetAngle = false;


        if (PlayerState == State.CastingSpell)
        {
            return;
        }

        if (Dot > UpThreshold)
        {
            SetAnimState(4); //Back
        }
        else if (Dot < UpThreshold && Dot > SideThreshold)
        {
            SetAnimState(3); //DiagUp
            SetAngle = true;
        }
        else if (Dot < SideThreshold && Dot > -SideThreshold)
        {
            SetAnimState(2); //Side
            SetAngle = true;
        }
        else if (Dot < -SideThreshold && Dot > -UpThreshold)
        {
            SetAnimState(1); //DiagDown
            SetAngle = true;
        }
        else 
        {
            SetAnimState(0); //Down
        }

        if (SetAngle)
        {
            if (ForwardDir.x < 0)
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
            }
            else if (ForwardDir.x > 0)
            {
                transform.eulerAngles = new Vector3(0, 180, 0);
            }
        }

    }

    void SetAnimState(int state)
    {

        if (horizInput != 0 || verticalInput != 0)
        {
            Anim.SetBool("IsMoving", true);
        }
        else
        {
            Anim.SetBool("IsMoving", false);
        }

        if (Anim.GetInteger("CurrentMoveState") == state)
        {

            return;
        }
        Anim.SetInteger("CurrentMoveState", state); 
        Anim.SetTrigger("ChangeDirection");
    }

    public void UpdateAllSliders()
    {
        UI.Instance.SetMaxMana(GetMaxMana());
        UI.Instance.SetMana(GetMana());
        UI.Instance.SetMaxHeat(GetMaxHeat());
        UI.Instance.SetMaxHealth(GetMaxHealth());
        UI.Instance.SetHealth(GetHealth());

    }

    IEnumerator FireBullet()
    {
        while (isFiring)
        {
            canShoot = false;
            PlayerData.ProjectileSpell.ActivateSpell();
            Debug.Log("Shooting!");
            yield return new WaitForSeconds(FireRate / GetFireRateModifier());
            Debug.Log("Can Shoot again!");
            canShoot = true;
        }
    }



    public void TakeDamage(int Damage, bool ignoreInvulnerable = false)
    {
        if (Invulnerable)
        {
            if (ignoreInvulnerable)
            {
                LoseHealth(Damage);
            }
            else
            {
                Debug.Log("Ignoring Invulnerability!");
            }
            return;
        }
        Debug.Log("Not Inuvlnerable!");
        LoseHealth(Damage);


    }


    public void Die()
    {
        Debug.Log("Player dying!");
    }



    public void CastSpell(Spell spell)
    {
        if (spell.type != SpellType.Projectile)
        {
            CurrentSpell = spell;
        }

        Debug.Log("Spell Cost: " + spell.ManaCost * GetManaCostModifier());

        LoseMana(spell.ManaCost * GetManaCostModifier());
        IncreaseHeat(spell.HeatAmount);

    }

    public void CancelSpell()
    {

        CurrentSpell.CancelSpell();
 
        Debug.Log("Cancelling Spell: " + CurrentSpell.SpellName);


    }

    public void SetState(State state)
    {
        PlayerState = state;
    }

    public bool GetKey()
    {
        return PlayerData.HasKey;
    }

    public void SetHasKey(bool input)
    {
        PlayerData.HasKey = input;
    }

    public void SetColor(Color color)
    {
        GetComponent<SpriteRenderer>().color = color;  
    }


    #region ValueAdjusters

    public void RestoreMana(float amount)
    {
        Mana += amount;
        if (Mana > PlayerData.MaxMana)
        {
            Mana = PlayerData.MaxMana;
            
        }
        UI.Instance.SetMana(Mana);
    }



    public IEnumerator ManaRecharge()
    {
        while (true)
        {
            yield return new WaitForSeconds(ManaRechargeRate);
            if (Mana < PlayerData.MaxMana)
            {
                //Debug.Log("Amount recharged: " + ManaRechargeAmount * GetRechargeModifier());
                RestoreMana(ManaRechargeAmount * GetRechargeModifier());
            }
 
        }
    }


    public void LoseMana(float input)
    {
        //Debug.Log()
        Mana -= input;
        UI.Instance.SetMana(Mana);
    }

    public void LoseHealth(int amount)
    {
        PlayerData.Health -= amount;
        GameManager.instance.SpawnDamageNumber(transform, amount);
        UI.Instance.SetHealth(PlayerData.Health);
        if (GetHealth() <= 0)
        {
            Die();
        }
    }

    public void RestoreHealth(int val)
    {
        PlayerData.Health += val;
        if (GetHealth() > GetMaxHealth())
        {
            PlayerData.Health = PlayerData.MaxHealth;
        }
        UI.Instance.SetHealth(PlayerData.Health);
    }

    public void IncreaseMoney(int amount)
    {
        PlayerData.Money += amount;
        UI.Instance.SetMoney(PlayerData.Money);
    }

    public void LoseMoney(int amount)
    {
        PlayerData.Money -= amount;
        UI.Instance.SetMoney(PlayerData.Money);
    }

    public void IncreaseCrystals(int amount)
    {
        PlayerData.Crystals += amount;
        UI.Instance.SetCrystals(PlayerData.Crystals);
    }

    public void LoseCrystals(int amount)
    {
        PlayerData.Crystals -= amount;
        UI.Instance.SetCrystals(PlayerData.Crystals);
    }

    #endregion

    #region HeatStuff

    public IEnumerator HeatLoss()
    {
        while (true)
        {
            yield return new WaitForSeconds(HeatLossRate);
            DecreaseHeat(HeatLossAmount * GetHeatLossModifier());
        }
        
    }


    public void DispelHeatHealth()
    {
        RestoreHealth((int)HeatDispelAmount);
        DecreaseHeat(HeatDispelAmount);
    }

    public void DispelHeatMana()
    {
        RestoreMana(HeatDispelAmount);
        DecreaseHeat(HeatDispelAmount);
    }

    public void IncreaseHeat(float amount)
    {
        CurrentHeat += amount;
        if (CurrentHeat > PlayerData.MaxHeat)
        {
            //StartCoroutine(HeatDamage());
        }
    }

    public void DecreaseHeat(float amount)
    {
        CurrentHeat -= amount;
        //Debug.Log("Amount of Heat Lost: " + amount);
        if (CurrentHeat < 0)
        {
            CurrentHeat = 0;
        }
        UI.Instance.SetHeat(CurrentHeat);
    }

    public IEnumerator HeatDamage()
    {

        while (true)
        {
            if (CurrentHeat > PlayerData.MaxHeat)
            {
                LoseHealth(HeatHealthLossAmount);
            }

            yield return new WaitForSeconds(HeatHealthLossRate);
        }

    }

    #endregion


    #region Modifiers

    public void ModifyMaxHealth(int amount)
    {
        PlayerData.MaxHealth += amount;
    }

    public void ModifyMaxHeat(int amount)
    {
        PlayerData.MaxHeat += amount;
    }

    public void ModifyFireRate(float amount)
    {
        PlayerData.FireRateModifier += amount;
    }

    public void ModifyDamage(float amount)
    {
        PlayerData.DamageModifier += amount;
    }


    public void ModifySpeed(float amount)
    {
        PlayerData.SpeedModifier += amount;
    }


    public void ModifyHeatLoss(float amount)
    {
        PlayerData.HeatLossModifier *= (1 - amount);
    }

    public void ModifyManaRecharge(float amount)
    {
        PlayerData.ManaRechargeModifier += amount;
    }


    public void ModifyCost(float amount)
    {
        PlayerData.ManaCostModifier *= (1 - amount);
    }

    public void ModifyMaxMana(float amount)
    {
        PlayerData.MaxMana += amount;
    }

    public void ModifyMelee(float amount) { 
        PlayerData.MeleeModifier += amount;
    
    }

    public void ModifyProjectile(float amount)
    {
        PlayerData.ProjectileModifier += amount;
    }

    public void ModifySpecial(float amount)
    {
        PlayerData.SpecialModifier += amount;

    }

    public void ModifyFire(float amount)
    {
        PlayerData.FireModifier += amount;

    }

    public void ModifyIce(float amount)
    {
        PlayerData.IceModifier += amount;
    }

    public void ModifyElectric(float amount)
    {
        PlayerData.ElectricModifier += amount;

    }
    #endregion

    #region GetterFunctions

    public PlayerData GetPlayerData() { 

        return PlayerData;
    }
    public Rigidbody2D GetRB()
    {
        return RB;
    }

    public float GetCurrentHeat()
    {
        return CurrentHeat;
    }

    public float GetMaxHeat()
    {
        return PlayerData.MaxHeat;
    }

    public float GetHeatBonus()
    {
        float percent = CurrentHeat / PlayerData.MaxHeat;
        //Debug.Log("Percent: " + percent);
        return percent * HeatMultiplier;
    }

    public float GetMaxHealth()
    {
        return PlayerData.MaxHealth;
    }

    public float GetHealth()
    {
        return PlayerData.Health;
    }

    public float GetMaxMana()
    {
        return PlayerData.MaxMana;
    }

    public float GetMana()
    {
        return Mana;
    }

    public float GetSpeedModifier()
    {
        return PlayerData.SpeedModifier;
    }

    public float GetManaCostModifier()
    {
        return PlayerData.ManaCostModifier;
    }
    public float GetDamageModifier()
    {
        return PlayerData.DamageModifier;
    }

    public float GetRechargeModifier()
    {
        return PlayerData.ManaRechargeModifier;
    }

    public float GetHeatLossModifier()
    {
        return PlayerData.HeatLossModifier;
    }

    public float GetFireRateModifier()
    {
        return PlayerData.FireRateModifier;
    }

    public float GetProjectileModifier()
    {
        return PlayerData.ProjectileModifier;
    }

    public float GetMeleeModifier()
    {
        return PlayerData.MeleeModifier;
    }

    public float GetSpecialModifier()
    {
        return PlayerData.SpecialModifier;
    }

    public float GetMoney()
    {
        return PlayerData.Money;
    }

    public float GetCrystals()
    {
        return PlayerData.Crystals;
    }

    public BulletPool GetPool()
    {
        return SpellBulletPool;
    }

    public float GetFireModifier()
    {
        return PlayerData.FireModifier;
    }

    public float GetIceModifier()
    {
        return PlayerData.IceModifier;
    }

    public float GetElectricModifier()
    {
        return PlayerData.ElectricModifier;
    }

    #endregion




    public void SetInvulnerable(bool input)
    {
        Invulnerable = input;
    }

    public void AddItem(ItemData newItem)
    {
        PlayerData.Items.Add(newItem);
    }

    public void SetProjectile(Spell spell)
    {
        SpellBulletPool.ClearPool();
        PlayerData.ProjectileSpell = spell;
        PlayerData.ProjectileSpell.SetOwner(this);
        FireRate = ((Projectile)PlayerData.ProjectileSpell).FireRate;
    }

    public void SetMelee(Spell spell)
    {
        PlayerData.MeleeSpell = spell;
        PlayerData.MeleeSpell.SetOwner(this);
    }

    public void SetSpecial(Spell spell)
    {
        PlayerData.SpecialSpell = spell;
        PlayerData.SpecialSpell.SetOwner(this);
    }


    //Might be useful later if you want spells with delays on them idk
    #region DefunctSpellShit  

    public IEnumerator CastDelay(float cost, float castTime)
    {
        Debug.Log("Waiting...");
        yield return new WaitForSeconds(castTime);

        //CastSpell(cost);

    }

    public void ChargeSpell(float cost, float castTime)
    {
        if (cost > Mana)
        {
            Debug.Log("Not enough MANA!");
            return;
        }
        ChargingSpell = true;
        RB.velocity = new Vector2(0, 0);
        Debug.Log("Cast time: " + castTime);
        StartCoroutine(CastDelay(cost, castTime));
    }


    public void CastSpell(float cost, float heat, string name)
    {

        LoseMana(cost);
        IncreaseHeat(heat);

    }


    public void ActivateSpell(Spell spell)
    {
        spell.Cast();
    }
    public void ChangeCurrentSpell(bool up)
    {

    }

    #endregion 


    /*

    private void OnDrawGizmos()
    {
        Vector3 origin = new Vector3(transform.position.x, transform.position.y, 0) + new Vector3(ForwardDir.x, ForwardDir.y, 0) * 4;
        Vector3 origin2 = new Vector3(transform.position.x, transform.position.y, 0);
        Handles.DrawWireDisc(origin, Vector3.forward, thunder.AOE);
        Handles.DrawWireDisc(origin2, Vector3.forward, fire.AOE);
    }


    */

}
