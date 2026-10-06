using UnityEngine;
using TMPro;

public class PlayerMoney : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText;

    private int currentMoney = 0;

    // Public getter/setter, hidden in the inspector
    public int CurrentMoney
    {
        get { return currentMoney; }
        set
        {
            currentMoney = value;
            UpdateUI();
        }
    }

    private void Start()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (moneyText != null)
        {
            moneyText.text = "$" + currentMoney;
        }
    }
}