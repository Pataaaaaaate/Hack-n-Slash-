using UnityEditor;
using UnityEngine;


public class Lever : MonoBehaviour
{
    public bool isUsable = true;
    private bool Activated = false;
    private int ID = 0;
    public int KeyCode = 0;
    public UnlockableElement UnlockableElement = null;

    [Header("SFX")]
    private AudioSource source;
    public AudioClip leverSfx;

    [Header("Mouvement du levier")]
    public Transform manche;  
    private float closeRotX = 0f;
    private float openRotX = 40f;


    

    void Start()
    {

        source = GetComponent<AudioSource>();
        Vector3 rot = manche.transform.localEulerAngles;
        manche.transform.localEulerAngles = new Vector3(closeRotX, rot.y, rot.z);
    }

    public void SetLever(UnlockableElement locker, int id, int keycode)
    {
        UnlockableElement = locker;
        ID = id;
        KeyCode = keycode;
    }

    public void Use()
    {
        Debug.Log("Tentative d'ouverture : " + this.name);

        if (isUsable && !Activated)
        {
            isUsable = false;
            Activated = true;

            // Animation du manche 
            Vector3 rot = manche.transform.localEulerAngles;
            manche.transform.localEulerAngles = new Vector3(openRotX, rot.y, rot.z);

            // Effet sonore
            if (source != null && leverSfx != null)
            {
                source.clip = leverSfx;
                source.Play();
            }

            // Déverrouiller l'élément
            UnlockableElement.Unlock(ID, KeyCode);
        }
    }
}