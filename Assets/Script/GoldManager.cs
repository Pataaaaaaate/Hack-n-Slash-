using UnityEngine;
using TMPro;
using UnityEngine.UI; 


public class GoldManager : MonoBehaviour
{

    public int goldCompteur;
    public TextMeshProUGUI goldText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        goldText.text = "Gold:" + goldCompteur.ToString();
    }

    public void AddGold(int amount)
    {
        goldCompteur += amount;
    }
}
