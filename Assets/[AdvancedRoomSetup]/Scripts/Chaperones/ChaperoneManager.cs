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
            chaperoneWorking.LoadFromWorkingFile();
            
            chaperoneNew = new Chaperone("New");

            LoadSavedChaperones();
        }

        private void Update()
        {
            chaperoneNew.SetViaExtremities(controllerLeft, controllerRight);
        }

        private void LoadSavedChaperones()
        {
            Debug.LogFormat($"Searching for saved chaperone files in directory '{SavedChaperonesPath}'");

            bool updatedChaperones = false;

            string[] filePaths = Directory.GetFiles(SavedChaperonesPath, "*" + ChaperoneExtension);
            for (int i = 0; i < filePaths.Length; i++)
            {
                Debug.LogFormat($"\tFound chaperone file: '{filePaths[i]}'");
                try
                {
                    string json = File.ReadAllText(filePaths[i]);
                    Debug.Log($"\t\t{json}'");
                    Chaperone loadedChaperone = JsonUtility.FromJson<Chaperone>(json);
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
            Debug.LogFormat($"Saving chaperone '{chaperone.Name}' to file '{filePath}'");

            try
            {
                string json = JsonUtility.ToJson(chaperone, true);
                Debug.Log(json);

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
