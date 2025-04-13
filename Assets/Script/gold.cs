using UnityEngine;

public class gold : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    int goldvalue = 100;

    [Header("Audio")]
    public AudioClip goldSound;
    private AudioSource audioSource;

    public GoldManager goldManager;  // Référence au GoldManager

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Si l'objet n'a pas d'AudioSource, on peut en ajouter un automatiquement
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetGoldValue(int value)
    {
        goldvalue = value;
    }
    public int GetGoldValue()
    {
        return goldvalue;
    }
    public int Pickup()
    {



        if (goldSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(goldSound);
        }

        goldManager.AddGold(1);  // Ajouter 1 

        this.transform.position = new Vector3(1000, 1000, 1000);

        //gameObject.SetActive(false);  // Désactive l'objet


        return goldvalue;
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Onenter");
        if(other.tag == "Player")
        {
            Pickup();
        }
    }
}
