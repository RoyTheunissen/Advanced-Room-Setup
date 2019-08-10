using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoyTheunissen.AdvancedRoomSetup.UI
{
    /// <summary>
    /// Manages the application window.
    /// </summary>
    public sealed class Window : MonoBehaviour
    {
        [SerializeField] private Button minimizeButton;
        [SerializeField] private Button closeButton;

        private void Awake()
        {
            if (!Application.isEditor)
                BorderlessWindow.SetFramelessWindow();
            
            minimizeButton.onClick.AddListener(OnMinimizeButtonPressed);
            closeButton.onClick.AddListener(OnCloseButtonPressed);
        }

        public void OnMinimizeButtonPressed()
        {
            EventSystem.current.SetSelectedGameObject(null);
            BorderlessWindow.MinimizeWindow();
        }

        public void OnCloseButtonPressed()
        {
            EventSystem.current.SetSelectedGameObject(null);
            Application.Quit();
        }
    }
}
