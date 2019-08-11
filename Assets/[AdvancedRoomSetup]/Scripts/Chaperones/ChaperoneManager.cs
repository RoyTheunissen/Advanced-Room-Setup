using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace RoyTheunissen.AdvancedRoomSetup.Chaperones
{
    /// <summary>
    /// Responsible for managing chaperone data.
    /// </summary>
    public sealed class ChaperoneManager : MonoBehaviour
    {
        private const string ChaperoneExtension = ".chaperone";
        private const string ChaperoneNameDateFormat = "yy-MM-dd H:mm:ss";
        
        [SerializeField] private Transform controllerLeft;
        [SerializeField] private Transform controllerRight;
        
        private Chaperone chaperoneWorking;
        public Chaperone ChaperoneWorking => chaperoneWorking;

        private Chaperone chaperoneNew;
        public Chaperone ChaperoneNew => chaperoneNew;

        private List<Chaperone> savedChaperones = new List<Chaperone>();
        public List<Chaperone> SavedChaperones => savedChaperones;

        private string SavedChaperonesPath => Application.persistentDataPath;

        public delegate void SavedChaperonesChangedHandler(ChaperoneManager chaperoneManager);
        public event SavedChaperonesChangedHandler SavedChaperonesChangedEvent;

        private void Awake()
        {
            chaperoneWorking = new Chaperone("Working");
            chaperoneWorking.LoadFromWorkingPlayArea();
            
            chaperoneNew = new Chaperone("New");

            LoadSavedChaperones();
        }

        private void Update()
        {
            chaperoneNew.SetViaExtremities(controllerLeft, controllerRight);
        }

        private void LoadSavedChaperones()
        {
            bool updatedChaperones = false;

            string[] filePaths = Directory.GetFiles(SavedChaperonesPath, "*" + ChaperoneExtension);
            for (int i = 0; i < filePaths.Length; i++)
            {
                try
                {
                    string json = File.ReadAllText(filePaths[i]);
                    Chaperone loadedChaperone = JsonUtility.FromJson<Chaperone>(json);
                    loadedChaperone.FilePath = filePaths[i];

                    bool isWorkingChaperone = chaperoneWorking.IsPlaySpaceTheSame(loadedChaperone);
                    if (isWorkingChaperone)
                    {
                        Debug.Log("Working chaperone is equal to loaded chaperone " +
                                  $"'{loadedChaperone.FilePath}'. Copying metadata.");
                        chaperoneWorking.CopyMetaDataFrom(loadedChaperone);
                    }
                    
                    savedChaperones.Add(loadedChaperone);
                    updatedChaperones = true;
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    throw;
                }
            }
            
            if (updatedChaperones)
                SavedChaperonesChangedEvent?.Invoke(this);
        }

        public void SaveNewChaperone()
        {
            // If there were no chaperones saved at all, save the original so we don't lose that.
            if (savedChaperones.Count == 0)
            {
                Chaperone originalChaperone = new Chaperone(
                    GetNewChaperoneName(" (Original)"), chaperoneWorking);
                savedChaperones.Add(originalChaperone);
                SaveChaperoneToFile(originalChaperone);
            }
            
            string newName = GetNewChaperoneName(" (Quick)");

            Chaperone chaperoneToSave = new Chaperone(newName, chaperoneNew);
            savedChaperones.Add(chaperoneToSave);
            
            SavedChaperonesChangedEvent?.Invoke(this);

            SaveChaperoneToFile(chaperoneToSave);

            LoadChaperone(chaperoneToSave);
        }

        private static string GetNewChaperoneName(string suffix)
        {
            DateTime dateTime = DateTime.Now;
            string newName = dateTime.ToString(ChaperoneNameDateFormat);
            return newName + suffix;
        }

        private void SaveChaperoneToFile(Chaperone chaperone)
        {
            // Replace colons because they are not allowed in filenames.
            string fileName = chaperone.Name.Replace(":", "_");
            string filePath = Path.Combine(SavedChaperonesPath, fileName + ChaperoneExtension);

            try
            {
                string json = JsonUtility.ToJson(chaperone, true);

                StreamWriter file = File.CreateText(filePath);
                file.Write(json);
                file.Close();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        private void OnDrawGizmos()
        {
            chaperoneWorking?.DrawGizmos();
        }

        public void LoadChaperone(Chaperone chaperone)
        {
            chaperoneWorking.CopySettingsFrom(chaperone);
            chaperoneWorking.CommitToLivePlayArea();
        }
        
        public void DeleteChaperone(Chaperone chaperone)
        {
            bool changed = savedChaperones.Remove(chaperone);

            // Try to remove the corresponding file too, if applicable.
            if (!string.IsNullOrEmpty(chaperone.FilePath))
            {
                try
                {
                    File.Delete(chaperone.FilePath);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    throw;
                }
            }

            if (changed)
                SavedChaperonesChangedEvent?.Invoke(this);
        }
    }
}
