using System.Collections;
using UnityEngine;

public class Crystal : MonoBehaviour
{
    private float SpeedRot = 360 / 10;
    private Player Player;
    private float PickupTime = 3;
    IEnumerator Cor;
    public int CrystalValue = 100;
    public float TimeProgress = 0;
    public GameObject cristalZone;

    private AudioSource cristalaudioSource;
    
    public AudioClip cristalAudio;
    private float cristalAudioTime = 0.2f;


    void Start()
    {

        //cristalaudioSource = GetComponent<AudioSource>();

        cristalZone.SetActive(false);
        if (cristalaudioSource == null)
        {
            Debug.Log(" Création d'un AudioSource...");
            cristalaudioSource = gameObject.AddComponent<AudioSource>();
        }

        cristalaudioSource.clip = cristalAudio;

        
    }

    void Update()
    {
        this.transform.RotateAround(this.transform.position, Vector3.up, SpeedRot * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Player>(out Player))


        {

            // Activer la zone du cristal lorsque le joueur entre dans la collision
            if (cristalZone != null)
            {
                cristalZone.SetActive(true);
                Debug.Log("Cristal Zone activée");
            }

            Cor = GettingPick();
           
            TimeProgress = PickupTime;
            StartCoroutine(Cor);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Player>(out Player) && Player == other.GetComponent<Player>())
        {

            if (cristalZone != null)
            {
                cristalZone.SetActive(false);
                Debug.Log("Cristal Zone désactivée");
            }

            StopCoroutine(Cor);
            Debug.Log("Player exited the zone, picking stopped.");
        }
    }

    IEnumerator GettingPick()
    {
        while (TimeProgress > 0)
        {
            yield return new WaitForSeconds(1f);
            TimeProgress -= 1f;
            Debug.Log("Progress: " + TimeProgress);
          
        }
        
        Debug.Log("Crystal picked!");
        Player.TakeCrystal(CrystalValue);

        if (cristalaudioSource != null && cristalAudio != null)
        {
            cristalaudioSource.PlayOneShot(cristalAudio);  // Joue le son
            yield return new WaitForSeconds(cristalAudioTime);
        }


        gameObject.SetActive(false);
        cristalZone.SetActive(false);
    }
}
   