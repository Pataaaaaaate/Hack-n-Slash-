using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class Spell : MonoBehaviour
{
    public MeshRenderer zoneMesh;
    public CapsuleCollider CharacterCollider;

    private bool firstCast;
    private bool secondCast;
    private bool ZoneEnable;
    private bool secondSpellTimer;
    private bool coolDown;
    private bool hasBeenCast;

    public float time = 4f;
    public float timer = 5f;
    public float coolDownSP = 4f;
    public float secondTime = 5f;
    public float healTime = 2f;
    

    [Header("Spirit")]
    public float spiritMax = 15f;
    public float currentSpirit;
    public float spellSpiritCost = 5f;

    public Ennemy ennemy; // fait appel au script ennemy
    public Player player;// fait appel au script player 


    public Transform childToDetach;
    public Transform originalParent; // Variable pour stocker le parent d'origine
    public Transform newParent;// Variable pour stocker le nouveau parent

    public Material secondSpellMaterial;  // Le matériau à utiliser pour le deuxième sort


    [Header("Audio Clips")]
    public AudioClip firstSpellSound; // Son pour le premier sort
    public AudioClip secondSpellSound; // Son pour le deuxième sort

    private AudioSource audioSource;


    public GameObject IndicationSpell;
    public GameObject firstSpellVFX;
    public GameObject secondSpellVFX;
    public GameObject Heal;
    private bool particleIsPlaying;


    
    //public ParticleSystem collisionParticles; // Référence au Particle System




    void Start()
    {
        ennemy = Object.FindFirstObjectByType<Ennemy>();
        player = Object.FindFirstObjectByType<Player>();

        zoneMesh.enabled = false; //ne pas afficher le le sprite
        firstCast = false;
        secondCast = false;

        ZoneEnable = false; //la zone du spell est desactiv�e

        //Player player = GetComponent<Player>();
        player.Mana = player.ManaMax; //le mana actuelle est �gale au mana max au start 

        currentSpirit = spiritMax; //le spirit actuelle est �gale au spirit max au start 

        player.Life = player.LifeMax;
       
        childToDetach.parent = originalParent;// mettre la zone enfant du player 

        audioSource = GetComponent<AudioSource>(); // Récupérer le composant AudioSource
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>(); // Ajouter un AudioSource s'il n'existe pas
        }

        if (Heal != null)
        {
            Heal.GetComponentInChildren<ParticleSystem>().Stop(); // Assurez-vous que le système est désactivé au départ
            particleIsPlaying = false;
        }

        firstSpellVFX.SetActive(false);
        secondSpellVFX.SetActive(false) ;

        IndicationSpell.SetActive(false);


    }
    


    void Update()
    {

        if (coolDown == false)
        {
            CastSpell();
            
        }

    }

    private void FirstCast() //coder premier cast
    {
       
        transform.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;


        ZoneEnable = true;//fait spawn le sprite de la zone. (mettre le zone de col sur le sprite, du coup de base le collider doit �tre disable
        firstCast = true;
        secondSpellVFX.SetActive(false);

        if (ZoneEnable == true) // Si la zone est activ�e
        {


            Vector3 newScale = zoneMesh.transform.localScale;
            newScale.x = 5.0f; // Changer le rayon sur l'axe X
            newScale.z = 5.0f; // Changer le rayon sur l'axe Z
            zoneMesh.transform.localScale = newScale; // change la range du deuxieme spell
            currentSpirit -= spellSpiritCost; //fait perdre du spirit
            StartCoroutine(FirstSpellTime());

            //Player player =GetComponent<Player>();
            player.Mana += player.ManaRegen; //Mana r�cup�r�
            currentSpirit -= spellSpiritCost; //fait perdre du spirit

            if (firstSpellVFX != null)
            {
                firstSpellVFX.SetActive(true); // Active les VFX
            }


            if (firstSpellSound != null)
            {
                audioSource.PlayOneShot(firstSpellSound);
            }
        }
    }


    private void SecondCast()
    {

        secondCast = true;
        // secondSpellTimer = true;

        zoneMesh.material = secondSpellMaterial;

        if (secondCast == true) // si la second zone apparait
        {

            firstSpellVFX.SetActive(false); // Désactive les VFX
            //zoneMesh.enabled = true;
            StartCoroutine(SecondSpellTime());
            Vector3 newScale = zoneMesh.transform.localScale;
            newScale.x = 10.0f; // Changer le rayon sur l'axe X
            newScale.z = 10.0f; // Changer le rayon sur l'axe Z
            zoneMesh.transform.localScale = newScale; // change la range du deuxieme spell 


            //Player player = GetComponent<Player>();
            player.Mana -= player.ManaRegen;

            if (secondSpellVFX != null)
            {
                secondSpellVFX.SetActive(true); // Active les VFX
            }

            if (secondSpellSound != null)
            {
                audioSource.PlayOneShot(secondSpellSound);
            }

            secondSpellTimer = false;
        }

        if(childToDetach.parent != null)
        {
            originalParent = newParent;
        }
        ReattachToParent();
        Debug.Log("ouaip");
        StartCoroutine(CoolDown());
        
        
    }


    private void CastSpell()
    {

        if (Input.GetKeyDown(KeyCode.E))
        {
            //zoneMesh.enabled = true;
            GetComponent<Collider>().enabled = false;
            IndicationSpell.SetActive(true);
        }


        if (Input.GetKeyUp(KeyCode.E))
        {
            IndicationSpell.SetActive(false);
            GetComponent<Collider>().enabled = true;
            DetachToParent();
            


            if (secondSpellTimer == true && firstCast == true)
            {
                SecondCast();

            }
            else
            {
                FirstCast();
                secondSpellVFX.SetActive(false);
            }
        }
    }


    private IEnumerator FirstSpellTime() // Temps du premier spell
    {
        //zoneMesh.enabled = true;


       
        
       firstSpellVFX.SetActive(true); // Assurez-vous que les VFX sont actifs au début
        


        StartCoroutine(TimerSecondSpell());
        secondSpellTimer = true;
        yield return new WaitForSeconds(time);
        zoneMesh.enabled = false;

        
        
        firstSpellVFX.SetActive(false); // Désactive les VFX
        
    }

    private IEnumerator TimerSecondSpell()
    {

        if (secondSpellVFX != null)
        {
            secondSpellVFX.SetActive(true); // Active les VFX
        }
        yield return new WaitForSeconds(timer); // timer qui se lance 
        secondSpellTimer = false;// lorsque le timer se termine il est d�sactiv� 

        if (secondSpellVFX != null)
        {
            secondSpellVFX.SetActive(false); // Désactive les VFX
        }
    }

    private IEnumerator CoolDown()
    {
        coolDown = true;
        yield return new WaitForSeconds(coolDownSP);
        coolDown = false;
        // rb.constraints = RigidbodyConstraints.None;
        //RigidbodyConstraints.None; // unfreeze la position 
    }


    private IEnumerator SecondSpellTime()
    {
        //zoneMesh.enabled = true;
        yield return new WaitForSeconds(secondTime);
        zoneMesh.enabled = false;

    }

    private IEnumerator HealTime()
    {
        yield return new WaitForSeconds(healTime);

        if (Heal != null)
        {
            Heal.GetComponentInChildren<ParticleSystem>().Stop(); // Assurez-vous que le système est désactivé au départ
            particleIsPlaying = false;
        }
    }

    


    void OnTriggerEnter(Collider other)
    {
       

        if (secondCast == true)
        {
            if(player == true)
            {
                
                player.Life += player.LifeRegen;
                // Jouer le système de particules
                if (Heal != null)
                {
                    //Heal.transform.position = other.transform.position; // Déplacer les particules à la position de collision
                    Heal.GetComponentInChildren<ParticleSystem>().Play();
                    particleIsPlaying = true;
                    StartCoroutine(HealTime());
                }
                
                    

                    
                
            }
          
            //Ennemy ennemy = other.GetComponent<Ennemy>(); // verifier si l'objet est l'ennemi 
           // if (ennemy != null)
            //{
              //  ennemy.ennemySpeed -= ennemy.removedSpeed;
               // ennemy.Life -= ennemy.removedLife;
            //}
            // faire le script lies aux ennemis

            
        }

        void OnTriggerExit(Collider other)
        {
            if (particleIsPlaying = true)
            {
                Heal.GetComponentInChildren<ParticleSystem>().Stop();
                particleIsPlaying = false;
            }
        }


        //if (firstCast == true)
        //{
            //Ennemy ennemy = other.GetComponent<Ennemy>();
            //if (ennemy != null)
            //{
               // ennemy.ennemySpeed -= ennemy.removedSpeed;
            //}
    
        //}


    }


    void DetachToParent()
    {
        if (childToDetach != null)
        {

            // Sauvegarde du parent d'origine
            //originalParent = childToDetach;

            childToDetach.parent = null;

        }
    }

    void ReattachToParent()
    {
        if (childToDetach.parent = null)
        {


            
            // Rattache l'enfant à son parent d'origine
            childToDetach.parent = originalParent;
            
        }
    }



}

   


