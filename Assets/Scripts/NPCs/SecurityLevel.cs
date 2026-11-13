using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSecurityLevel", menuName = "Prison/Security Level")]
public class SecurityLevel : ScriptableObject
{
    [SerializeField] private string levelName = "Restricted";
    [SerializeField] private int severityTier = 1;
    [SerializeField] private Color debugZoneColor = Color.yellow;

    [Header("Enforcement Rules")]
    [Tooltip("Used as safe teleport locations")]
    [SerializeField] private bool isSafeZone = false;

    [Tooltip("Immediate fine, shakedown, and teleport")]
    [SerializeField] private bool immediatePenalty = false;

    [Tooltip("Give a warning timer before escalating")]
    [SerializeField] private bool allowWarning= false;
    [SerializeField] private float warningDuration = 30f;

    public string LevelName => levelName;
    public int SeverityTier => severityTier;
    public Color DebugZoneColor => debugZoneColor;

    public bool IsSafeZone => isSafeZone;
    public bool ImmediatePenalty => immediatePenalty;
    public bool AllowWarning => allowWarning;
    public float WarningDuration => warningDuration;
}
