using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

public class UnlockableElement : MonoBehaviour
{
    public int KeyCode = 0;
    private int  LockNumber = 0;
    private int UnlockNumber = 0;
    [SerializeField]
    public List<Lever> Levers ;

    public Transform porte;
    private float closeRotX = 0f;
    private float openRotY = 70f;

    private AudioSource audioSource;
    public AudioClip openDoorSound;
    public AudioClip closeDoorSound;

    private bool isDoorOpen = false;

    void Start()
    {

        audioSource = GetComponent<AudioSource>();
        if (Levers.Count <=0)
        {
            Vector3 rot = porte.transform.localEulerAngles;
            porte.transform.localEulerAngles = new Vector3(closeRotX, rot.y, rot.z);
            Debug.LogWarning(this.name+" has no lever");
        }
    }
    public void AddLock(Lever Iojb)
    {

        if(Levers==null)
        {
            Levers = new List<Lever> ();
        }
        LockNumber++;
        Levers.Add(Iojb);
    }
    public List<Lever> ResetLevers()
    {
        List<Lever> tLevers = Levers;
        Levers = null;
        LockNumber = 0;
        return tLevers;
    }
    public virtual void OpenLock()
    {


        if (!isDoorOpen) // Si la porte n'est pas déjà ouverte
        {
            // Ouvre la porte
            Vector3 rot = porte.transform.localEulerAngles;
            porte.transform.localEulerAngles = new Vector3(rot.x, openRotY, rot.z);
            Debug.Log("La porte est ouverte");

            if (audioSource != null && openDoorSound != null)
            {
                audioSource.PlayOneShot(openDoorSound); // Joue le son d'ouverture
            }

            isDoorOpen = true; // Met à jour l'état de la porte
        }
    }
    public virtual void CloseLock(int id)
    {
        if (isDoorOpen) // Si la porte est ouverte, on la ferme
        {
            // Ferme la porte
            Vector3 rot = porte.transform.localEulerAngles;
            porte.transform.localEulerAngles = new Vector3(closeRotX, rot.y, rot.z);
            Debug.Log("La porte est fermée");

            if (audioSource != null && closeDoorSound != null)
            {
                audioSource.PlayOneShot(closeDoorSound); // Joue le son de fermeture
            }

            isDoorOpen = false; // Met à jour l'état de la porte
        }
    }
    public virtual void Unlock(int id,int keycode)
    {
        Debug.Log("Index : " + id);
        if(keycode == KeyCode)
        {
            UnlockNumber ++;
        }
        CheckOpening();
    }
    private void CheckOpening ()
    {
        Debug.Log("bop");
        if(UnlockNumber >= LockNumber)
        {
            OpenLock();
        }
    }
   
}
