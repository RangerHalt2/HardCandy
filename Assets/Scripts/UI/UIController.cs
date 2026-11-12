// Manages the UI during gameplay and allows for easy navigation between pages
using System.Collections.Generic;
using UnityEngine;


public class UIController : MonoBehaviour
{
    private PlayerController player;
    public static UIController instance;

    [Header("Page Management")]
    [Tooltip("The Pages (or Panels) managed by the UI Controller.")]
    public List<UIPage> pages;

    [Tooltip("The index of the currently active page.")]
    public int currentPage = 0;

    [Tooltip("The index of the page to show on startup.")]
    public int defaultPage = 0;

    private void Awake()
    {
        Time.timeScale = 1f;

        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        player = FindFirstObjectByType<PlayerController>();
        GoToPage(defaultPage);
    }

    public void GoToPage(int pageIndex)
    {
        // Debug.Log("UI CONTROLLER - GO TO PAGE CALLED WITH " + pageIndex);

        if (pages == null || pageIndex < 0 || pageIndex >= pages.Count || pages[pageIndex] == null)
        {
            Debug.LogWarning($"UI CONTROLLER - INVALID PAGE INDEX {pageIndex}");
            return;
        }

        SetActiveAllPages(false);
        pages[pageIndex].gameObject.SetActive(true);
        currentPage = pageIndex;
        ApplyCursorState(pages[pageIndex].needsCursor);
    }

    public void SetActiveAllPages(bool activated)
    {
        if (pages == null) return;

        foreach (UIPage page in pages)
        {
            if (page != null) page.gameObject.SetActive(activated);
        }
    }

    private void ApplyCursorState(bool showCursor)
    {
        if (showCursor)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (player != null)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (player != null) player.blockInputs = showCursor;
    }
}