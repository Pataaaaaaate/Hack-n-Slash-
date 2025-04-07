using System.Collections;
using UnityEngine;

public class trap : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    Player Player;
    IEnumerator Cor;
    int Damage = 50;
    float LaunchTime = 1;
    public GameObject pics;
    private Vector3 newPosition = new Vector3(12f, 2.4f, 13f); // Nouvelle position
    private Vector3 initialPosition = new Vector3(-10, -10, -10);


    private AudioSource audioSource;
    public AudioClip soundEffect;
    public AudioClip retractSound; // Son quand les pics rentrent





    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        initialPosition = pics.transform.position; // Sauvegarde la position initiale des pics
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Player p;
        if (other.TryGetComponent<Player>(out p) )
        {
            Player = p;
            Cor = Launch(LaunchTime);
            StartCoroutine(Cor);

            if (pics != null)
            {
                //pics.transform.position = newPosition;
                pics.transform.position = new Vector3(initialPosition.x, newPosition.y, initialPosition.z);

                if (audioSource != null && soundEffect != null)
                {
                    audioSource.PlayOneShot(soundEffect);
                }
            }
        }

        
    }
    private void OnTriggerExit(Collider other)
    {
        Player p;
        if (other.TryGetComponent<Player>(out p))
        {
            if (p == Player)
            {
                StopCoroutine(Cor);
                Player = null;

                if (pics != null)
                {
                    StartCoroutine(ResetTrap()); // Démarre la coroutine pour réinitialiser les pics après 3 secondes
                }

                
            }

        }
        
    }
    IEnumerator Launch(float time)
    {
   
        Debug.Log("Prepare to launch");
        yield return new WaitForSeconds(time);
        Player.TakeHit(Damage);
        if(Player != null)
        {
            Cor = Launch(LaunchTime);
            StartCoroutine(Cor);
        }
    }

    IEnumerator ResetTrap()
    {
        yield return new WaitForSeconds(2); // Attendre 2 secondes
        if (pics != null)
        {
            if (audioSource != null && retractSound != null)
            {
                audioSource.PlayOneShot(retractSound); // Son lorsque les pics rentrent
            }
            pics.transform.position = initialPosition; // Remettre les pics à leur position d'origine
        }
    }
}
