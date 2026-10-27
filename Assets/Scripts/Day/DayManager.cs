using System;
using TMPro;
using UnityEngine;

public class DayManager : MonoBehaviour
{
    public static DayManager Instance { get; private set; }

    [SerializeField] private float dayLengthSeconds = 300f;   // real seconds per in-game day (5 min)
    [SerializeField] private float startHour = 0f;            // hour each new day begins at

    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private TextMeshProUGUI timeText;

    public int Day { get; private set; } = 1;
    public float Hour { get; private set; }                   // 0 - 24

    public event Action<int> OnNewDay;

    private void Awake()
    {
        Instance = this;
        Hour = startHour;
    }

    private void Start()
    {
        UpdateUI();
    }

    private void Update()
    {
        Hour += 24f / dayLengthSeconds * Time.deltaTime;
        if (Hour >= 24f)
        {
            NextDay();
            return;
        }
        UpdateUI();
    }

    public void NextDay()
    {
        Day++;
        Hour = startHour;
        UpdateUI();
        OnNewDay?.Invoke(Day);
    }

    public string GetTimeOfDay()
    {
        if (Hour < 6f) return "dawn";
        if (Hour < 12f) return "morning";
        if (Hour < 17f) return "afternoon";
        return "night";
    }

    private void UpdateUI()
    {
        int hours = Mathf.FloorToInt(Hour);
        int minutes = Mathf.FloorToInt((Hour - hours) * 60f);

        if (dayText != null) dayText.text = "Day " + Day;
        if (timeText != null) timeText.text = $"{hours:00}:{minutes:00} - {GetTimeOfDay()}";
    }
}
