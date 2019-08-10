using System.Collections.Generic;
using RoyTheunissen.AdvancedRoomSetup.Chaperones;
using UnityEngine;

namespace RoyTheunissen.AdvancedRoomSetup.UI
{
    /// <summary>
    /// The UI for managing saved chaperone data.
    /// </summary>
    public sealed class ChaperoneManagingUi : MonoBehaviour
    {
        [SerializeField] private ChaperoneManager chaperoneManager;
        
        [Space]
        [SerializeField] private ChaperoneButton chaperoneButtonPrefab;
        [SerializeField] private Transform chaperoneButtonContainer;
        
        [Space]
        [SerializeField] private AdvancedCalibrationButton newChaperoneButton;
        public AdvancedCalibrationButton NewChaperoneButton => newChaperoneButton;

        private List<ChaperoneButton> chaperoneButtons = new List<ChaperoneButton>();

        private ChaperoneButton cachedHoveredChaperoneButton;
        private ChaperoneButton HoveredChaperoneButton
        {
            get => cachedHoveredChaperoneButton;
            set
            {
                if (cachedHoveredChaperoneButton == value)
                    return;

                ChaperoneButton previous = cachedHoveredChaperoneButton;
                
                cachedHoveredChaperoneButton = value;
                
                HoveredChaperoneChangedEvent?.Invoke(this, previous?.Chaperone, value?.Chaperone);
            }
        }

        public Chaperone HoveredChaperoneToLoad => HoveredChaperoneButton?.Chaperone;
        
        public delegate void HoveredChaperoneChangedHandler(
            ChaperoneManagingUi chaperoneManagingUi, Chaperone from, Chaperone to);
        public event HoveredChaperoneChangedHandler HoveredChaperoneChangedEvent;

        private void Awake()
        {
            UpdateButtons();
            
            newChaperoneButton.Button.onClick.AddListener(OnNewChaperoneButtonClicked);
            
            chaperoneManager.SavedChaperonesChangedEvent += HandleSavedChaperonesChangedEvent;
        }

        private void OnDestroy()
        {
            chaperoneManager.SavedChaperonesChangedEvent -= HandleSavedChaperonesChangedEvent;
            
            for (int i = 0; i < chaperoneButtons.Count; i++)
            {
                if (chaperoneButtons[i] == null)
                    continue;
                
                chaperoneButtons[i].LoadButtonPressedEvent -= HandleLoadChaperoneButtonPressedEvent;
                chaperoneButtons[i].DeleteButtonPressedEvent -= HandleDeleteChaperoneButtonPressedEvent;
                chaperoneButtons[i].HoverStateChangedEvent += HandleHoveredChaperoneButtonChangedEvent;
            }
        }

        private void UpdateButtons()
        {
            // Instantiate more buttons if needed.
            for (int i = chaperoneButtons.Count; i < chaperoneManager.SavedChaperones.Count; i++)
            {
                ChaperoneButton newButton = Instantiate(
                    chaperoneButtonPrefab, chaperoneButtonContainer);
                newButton.LoadButtonPressedEvent += HandleLoadChaperoneButtonPressedEvent;
                newButton.DeleteButtonPressedEvent += HandleDeleteChaperoneButtonPressedEvent;
                newButton.HoverStateChangedEvent += HandleHoveredChaperoneButtonChangedEvent;
                chaperoneButtons.Add(newButton);
            }

            // Update the data of all the chaperone buttons.
            for (int i = 0; i < chaperoneButtons.Count; i++)
            {
                if (i >= chaperoneManager.SavedChaperones.Count)
                {
                    chaperoneButtons[i].UpdateData(null);
                    continue;
                }
                
                chaperoneButtons[i].UpdateData(chaperoneManager.SavedChaperones[i]);
            }
        }

        private void OnNewChaperoneButtonClicked()
        {
            chaperoneManager.SaveNewChaperone();
        }

        private void HandleSavedChaperonesChangedEvent(ChaperoneManager chaperoneManager)
        {
            UpdateButtons();
        }

        private void HandleLoadChaperoneButtonPressedEvent(ChaperoneButton chaperoneButton)
        {
            chaperoneManager.LoadChaperone(chaperoneButton.Chaperone);
        }

        private void HandleDeleteChaperoneButtonPressedEvent(ChaperoneButton chaperoneButton)
        {
            if (HoveredChaperoneButton == chaperoneButton)
                HoveredChaperoneButton = null;
            
            chaperoneManager.DeleteChaperone(chaperoneButton.Chaperone);
        }

        private void HandleHoveredChaperoneButtonChangedEvent(
            ChaperoneButton chaperoneButton, bool isHovered)
        {
            if (!isHovered)
            {
                if (chaperoneButton == HoveredChaperoneButton)
                    HoveredChaperoneButton = null;
                return;
            }
            
            HoveredChaperoneButton = chaperoneButton;
        }
    }
}
