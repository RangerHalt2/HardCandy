using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DragGhost : MonoBehaviour
{
    public static DragGhost Instance { get; private set; }

    private Image image;

    void Awake()
    {
        Instance = this;
        image = GetComponent<Image>();
        image.raycastTarget = false;
        Hide();
    }

    public void Show(Sprite s) { image.sprite = s; image.enabled = true; }
    public void Follow(Vector2 screenPos) => transform.position = screenPos;
    public void Hide() => image.enabled = false;
}