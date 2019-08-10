using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoyTheunissen.AdvancedRoomSetup.Chaperones
{
    /// <summary>
    /// Responsible for managing chaperone data.
    /// </summary>
    public sealed class ChaperoneManager : MonoBehaviour
    {
        [SerializeField] private Transform controllerLeft;
        [SerializeField] private Transform controllerRight;
        
        
        private Chaperone chaperoneWorking;
        public Chaperone ChaperoneWorking => chaperoneWorking;

        private Chaperone chaperoneNew;
        public Chaperone ChaperoneNew => chaperoneNew;

        private List<Chaperone> savedChaperones = new List<Chaperone>();
        public List<Chaperone> SavedChaperones => savedChaperones;

        public delegate void SavedChaperonesChangedHandler(ChaperoneManager chaperoneManager);
        public event SavedChaperonesChangedHandler SavedChaperonesChangedEvent;

        private void Awake()
        {
            chaperoneWorking = new Chaperone("Working");
            chaperoneWorking.LoadFromWorkingFile();
            
            chaperoneNew = new Chaperone("New");
        }

        private void Update()
        {
            chaperoneNew.SetViaExtremities(controllerLeft, controllerRight);
        }

        public void SaveNewChaperone()
        {
            // If there were no chaperones saved at all, save the original so we don't lose that.
            if (savedChaperones.Count == 0)
            {
                Chaperone originalChaperone = new Chaperone("Original", chaperoneWorking);
                savedChaperones.Add(originalChaperone);
            }
            
            DateTime dateTime = DateTime.Now;
            string newName = dateTime.ToShortDateString() + " " + dateTime.ToShortTimeString();
            
            Chaperone chaperoneToSave = new Chaperone(newName, chaperoneNew);
            savedChaperones.Add(chaperoneToSave);
            
            SavedChaperonesChangedEvent?.Invoke(this);
            
            LoadChaperone(chaperoneToSave);
        }

        private void OnDrawGizmos()
        {
            chaperoneWorking?.DrawGizmos();
        }

        public void LoadChaperone(Chaperone chaperone)
        {
            chaperoneWorking.CopySettingsFrom(chaperone);
            
            // TODO: Save to file.
        }
        
        public void DeleteChaperone(Chaperone chaperone)
        {
            bool changed = savedChaperones.Remove(chaperone);
            
            if (changed)
                SavedChaperonesChangedEvent?.Invoke(this);
        }
    }
}
