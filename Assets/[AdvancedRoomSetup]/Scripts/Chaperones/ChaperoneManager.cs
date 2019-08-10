using System.Collections.Generic;
using UnityEngine;

namespace RoyTheunissen.AdvancedRoomSetup.Chaperones
{
    /// <summary>
    /// Responsible for managing chaperone data.
    /// </summary>
    public sealed class ChaperoneManager : MonoBehaviour
    {
        [SerializeField] private ChaperoneRenderer chaperoneRendererPrefab;
        [SerializeField] private Transform controllerLeft;
        [SerializeField] private Transform controllerRight;
        
        [Space]
        [SerializeField] private OverheadCameraFraming overheadCameraFraming;

        // TODO: Move the rendering to a separate class.
        private ChaperoneRenderer chaperoneRendererWorking;
        private ChaperoneRenderer chaperoneRendererNew;
        
        private Chaperone chaperoneWorking;
        private Chaperone chaperoneNew;

        private List<Chaperone> savedChaperones = new List<Chaperone>();
        public List<Chaperone> SavedChaperones => savedChaperones;

        private bool showNewChaperonePreview;
        public bool ShowNewChaperonePreview
        {
            get { return showNewChaperonePreview; }
            set { showNewChaperonePreview = value; }
        }

        public delegate void SavedChaperonesChangedHandler(ChaperoneManager chaperoneManager);
        public event SavedChaperonesChangedHandler SavedChaperonesChangedEvent;

        private void Awake()
        {
            chaperoneWorking = new Chaperone("Working");
            chaperoneWorking.LoadFromWorkingFile();

            chaperoneRendererWorking = Instantiate(chaperoneRendererPrefab);
            chaperoneRendererWorking.Initialize(chaperoneWorking);
            
            chaperoneNew = new Chaperone("New");
            chaperoneRendererNew = Instantiate(chaperoneRendererPrefab);
            chaperoneRendererNew.Initialize(chaperoneNew);
        }

        private void Update()
        {
            overheadCameraFraming.transform.rotation = Quaternion.Euler(
                0.0f, chaperoneWorking.Origin.rotation.eulerAngles.y, 0.0f);
            
            chaperoneRendererWorking.Opacity = showNewChaperonePreview ? 0.05f : 1.0f;
            
            chaperoneNew.SetViaExtremities(controllerLeft, controllerRight);
            chaperoneRendererNew.Active = showNewChaperonePreview;
        }

        public void SaveNewChaperone()
        {
            Chaperone chaperoneToSave = new Chaperone(chaperoneNew);
            savedChaperones.Add(chaperoneToSave);
            
            SavedChaperonesChangedEvent?.Invoke(this);
        }

        private void OnDrawGizmos()
        {
            chaperoneWorking?.DrawGizmos();
        }

        public void LoadChaperone(Chaperone chaperone)
        {
            
        }
        
        public void DeleteChaperone(Chaperone chaperone)
        {
            bool changed = savedChaperones.Remove(chaperone);
            
            if (changed)
                SavedChaperonesChangedEvent?.Invoke(this);
        }
    }
}
