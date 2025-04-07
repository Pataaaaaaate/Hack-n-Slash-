using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;


public class Player : MonoBehaviour
{
    int PlayerCrystal = 0;
    int PlayerGold = 0;
    List<Item> PlayerItem = new List<Item>();
    Vector3 lastcursorpointgroundtarget = Vector3.zero;
    GameObject UnderCursorEnnemy = null;

    //Base stats
    public int Strength = 15;
    public int Intelligence = 15;
    public int Dexterity = 15;

    //mana
    public float ManaMax = 200;
    public float Mana = 100;
    [Tooltip("In mana per second (%Manamax/s)")]// en %
    public float ManaRegen = 2;
    //Life
    public float LifeMax = 200;
    public float Life = 100;
    [Tooltip("In Life per second (Life/s)")]// en valeur brut
    public float LifeRegen = 2; 
    // Energyshield
    public float EnergyShieldMax = 200;
    public float EnergyShield = 100;
    public float EnergyShieldRegen = 20; // en %
    public float EnergyShieldDelay = 4; // en seconde
    public float EnergyShieldHitRecently = 0;
    // Res en %
    public int FireResist = 0;
    public int IceResist = 0;
    public int LightningResist = 0;
    public int ChaosResist = 0;

    // Spells 

    // SpellsStat
    List<SpellData> PlayerSpells = new List<SpellData>();
    //public List<SpellData> Spells = new List<SpellData>();
    float CastingSpeed = 0.25f;
    private bool bCasting = false;
    
    
    //Spells prefab
    public GameObject ProjectileSpell;
    public GameObject MeteorSpell;
    public GameObject RebondSpell;
    public GameObject OrbSpell;

    // LightningJudgement spell prefab
    public LightningJudgement Judgement;

    // deplacement et navigation 
    NavMeshAgent agent;
    float MvtSpeed = 5;
    Vector3 LookAtPos = new Vector3();
    Vector3 LookAtDest = new Vector3();
    bool bLookAtSpell = false;
    float DelayBeforeStopLookingAtSpell = 1.5f;
    float LastCastSpell = 0;
    float SpeedRotation = 360; // � per sec

    // Input
    bool Spell1_Hold = false;

    private PlayeraActions playerControls;
    private InputAction interactionPorte;


    public Vector3 TargetGround()
    {
        Camera cam = Camera.main;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        int layerMask = LayerMask.GetMask("ground");


        if (Physics.Raycast(ray.origin, ray.direction, out hit, 50, layerMask))
        {
            //Debug.Log("Ground hit");
            lastcursorpointgroundtarget = hit.point; 
            return hit.point;

        }
        else
        {
            return Vector3.zero;
        }

    }
    public GameObject TargetEnnemy()
    {
        Camera cam = Camera.main;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        int layerMask = LayerMask.GetMask("ennemy");


        if (Physics.Raycast(ray.origin, ray.direction, out hit, 50, layerMask))
        {
            //Debug.Log("ennemy hit");
            UnderCursorEnnemy = hit.rigidbody.gameObject;
            return hit.rigidbody.gameObject;

        }
        else
        {
            return null;
        }

    }


    void Start()
    {
        playerControls = new PlayeraActions();

        agent = GetComponent<NavMeshAgent>();
        agent.speed = MvtSpeed;
        // PlayerSpells = Datas.LoadSpellData(); // ne pas s'en occuper pour l'instant
    }
    public void OnMoveTo(InputAction.CallbackContext ctx)
    {
        
        if (ctx.performed)
        {
            agent.SetDestination(TargetGround());
            LookAtDest = TargetGround();
            if (bLookAtSpell)
            {
                bLookAtSpell = false;
            }
        }
    }

    private void OnEnable()
    {

        // Assurez-vous que playerControls est initialis� ici
        if (playerControls == null)
        {
            playerControls = new PlayeraActions();
        }

        //interactionPorte = playerControls.interactionPorte;
        interactionPorte = playerControls.PlayersBaseActions.interactionPorte;
        interactionPorte.Enable();
        interactionPorte.performed += OnInteract;
    }

    private void OnDisable()
    {

        if (interactionPorte != null)
        {
            interactionPorte.performed -= OnInteract;
            interactionPorte.Disable();
        }
    }

    public void OnSpell_1(InputAction.CallbackContext ctx)
    {
        //
        
        
        if (ctx.performed)
        {
            Spell1_Hold = true;
            if ( bCasting == false && Judgement.Manacost <= Mana)
            {
                //Debug.Log("" + CastSpell(new SpellData()));
                Vector3 targetPos = TargetGround();
                Judgement.ActivateSpell(targetPos, CastingSpeed, this);
                bCasting = true;
                LookAtPos = TargetGround();
                bLookAtSpell = true;
                LastCastSpell = Time.time;
            }
        }
        else if(ctx.canceled)
        {
            Spell1_Hold = false;
            Debug.Log("SpellCancelled");
            if (bCasting )
            {
                Judgement.DesactiveSpell();
                bCasting = false;

            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        ManaRegenerated();
        LifeRegenerated();
        EnergyShieldManager();
        InputManager();
        //
        RotateToLookAt();
        PickUpGold();
       // InputManagerBalance();// ne pas s'en occuper pour l'instant

    }
    // test gold
    private void PickUpGold()
    {
        Collider[] NearObj = Physics.OverlapBox(this.transform.position,Vector3.one*2);
        foreach (Collider col in NearObj) {
            if(col.tag=="Gold")
            {
                PlayerGold =  col.GetComponent<gold>().Pickup();
                Debug.Log("New Gold : " + PlayerGold);
            }
        }
    }
    public void TakeCrystal(int val)
    {
        Debug.Log("More crystal");
        PlayerCrystal += val;
    }
    private void RotateToLookAt()
    {
        if(LastCastSpell + DelayBeforeStopLookingAtSpell <= Time.time)
        {
            bLookAtSpell = false;
        }
        if(bLookAtSpell)
        {
            
            this.transform.LookAt(new Vector3(LookAtPos.x, this.transform.position.y, LookAtPos.z));
        }
        else // rotate to look at the target
        {
            //this.transform.LookAt(new Vector3(LookAtDest.x, this.transform.position.y, LookAtDest.z));
            Vector3 TargetPos = new Vector3(LookAtDest.x, this.transform.position.y, LookAtDest.z);

            Quaternion rotarget = Quaternion.LookRotation(TargetPos - this.transform.position);

            this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, rotarget, SpeedRotation * Time.deltaTime);
        }

    }
    private void ManaRegenerated()
    {
        Mana += Time.deltaTime * (ManaMax*ManaRegen);
        if(Mana> ManaMax)
        {
            Mana = ManaMax;
        }
    }
    private void LifeRegenerated()
    {
        Life += Time.deltaTime * LifeRegen;
        if (Life > LifeMax)
        {
            Life = LifeMax;
        }
    }
    public void TakeHit(float dmg)
    {
        EnergyShieldHitRecently = Time.time;
        Debug.Log("HitTaken");
    }

    private void EnergyShieldManager()
    {
        if(EnergyShield < EnergyShieldMax && EnergyShieldHitRecently + EnergyShieldDelay <= Time.time)
        {
            EnergyShield += EnergyShieldRegen * EnergyShieldMax * Time.deltaTime;
            if(EnergyShield > EnergyShieldMax)
            {
                EnergyShield = EnergyShieldMax;
            }
        }
    }
    private void InputManager()
    {


        if (Spell1_Hold && bCasting) // relocate pos of spell
        {
            Judgement.ChangeDestinationPos(TargetGround());
            LastCastSpell = Time.time;
        }
    }
    public void ManaComsuption( int manavalue)
    {

    }
    public void CurrentSpellEnded()
    {

        bCasting = false;
    }
    private bool CastSpell(SpellData spell)
    {
        bool IsCast = false;

        return IsCast;
    }
    private void InputManagerBalance()// ne pas s'en occuper pour l'instant
    {
        if (Input.GetMouseButtonDown(1))
        {
            agent.SetDestination(TargetGround());
        }
        if (bCasting == false)
        {
            if (Input.GetKeyDown(KeyCode.Q) && PlayerSpells[0].manacost <= Mana) // cast frost spike
            {
                bCasting = true;
                Mana -= PlayerSpells[0].manacost;
                StartCoroutine(CastSpellBalance(PlayerSpells[0]));

            }
            if (Input.GetKeyDown(KeyCode.W) && PlayerSpells[1].manacost <= Mana)
            {
                TargetGround();
                bCasting = true;
                Mana -= PlayerSpells[1].manacost;
                StartCoroutine(CastSpellBalance(PlayerSpells[1]));
            }
            if (Input.GetKeyDown(KeyCode.E) && PlayerSpells[2].manacost <= Mana)
            {
                if (TargetEnnemy())//si il y a une cible �ligible pour le sort
                {
                    bCasting = true;
                    Mana -= PlayerSpells[2].manacost;
                    StartCoroutine(CastSpellBalance(PlayerSpells[2]));
                }

            }
            if (Input.GetKeyDown(KeyCode.R) && PlayerSpells[2].manacost <= Mana)
            {
                TargetGround();
                bCasting = true;
                Mana -= PlayerSpells[2].manacost;
                StartCoroutine(CastSpellBalance(PlayerSpells[2]));
                

            }
        }
    }
    
    IEnumerator CastSpellBalance(SpellData spelldata)// ne pas s'en occuper pour l'instant
    {
       
        yield return new WaitForSeconds(spelldata.casttime);
        bCasting = false;
        switch (spelldata.type)
        {
            case "projectile":
                if (ProjectileSpell){
                    GameObject proj = Instantiate(ProjectileSpell);
                    proj.GetComponent<projectile>().Initialize(this.transform.forward, this.transform.forward + this.transform.position, spelldata);
                }
                else
                {
                    Debug.Log("No proj prefab available");
                }
                break;
            case "aoe":
                if (MeteorSpell){
                    GameObject meteor = Instantiate(MeteorSpell);
                    meteor.GetComponent<meteor>().Initialize(lastcursorpointgroundtarget, this.transform.up *20, spelldata);
                }
                else{
                    Debug.Log("No AOE prefab available");
                }

                break;
            case "rebond":
                if (RebondSpell)
                {
                    GameObject rebond = Instantiate(RebondSpell);
                    rebond.GetComponent<rebond>().Initialize(UnderCursorEnnemy, this.transform.forward + this.transform.position, spelldata);
                    UnderCursorEnnemy = null;
                }
                else
                {
                    Debug.Log("No Rebond prefab available");
                }
                break;
            case "orb": // miss data in excell
                if (OrbSpell)
                {
                    GameObject orb = Instantiate(RebondSpell);
                    orb.GetComponent<Orb>().Initialize(lastcursorpointgroundtarget, spelldata);
                    UnderCursorEnnemy = null;
                }
                else
                {
                    Debug.Log("No Orb prefab available");
                }
                break;

        }
 
    }

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {

            TryInteract();
        }
    }



    private void TryInteract()
    {
        Collider[] cols = Physics.OverlapSphere(this.transform.position, 2);
        float dist = 1000;
        Lever lever = null;
        foreach (Collider col in cols)
        {
            if (col.GetComponent<Lever>() != null)
            {
                if (col.GetComponent<Lever>().isUsable)
                {
                    Vector3 playertoobj = col.gameObject.transform.position - this.transform.position;
                    float currentdist = Vector3.Magnitude(playertoobj);
                    if (currentdist < dist)
                    {
                        dist = currentdist;
                        lever = col.GetComponent<Lever>();
                    }
                }
            }
        }
        if (lever != null)
        {
            lever.GetComponent<Lever>().Use();
        }
    }


}
