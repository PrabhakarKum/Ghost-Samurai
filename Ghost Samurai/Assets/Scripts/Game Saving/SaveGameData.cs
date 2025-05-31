using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;

public class SaveGameData : MonoBehaviour
{
    public string saveDataDirectoryPath = "";
    [FormerlySerializedAs("saveDataFileName")] public string saveFileName = "";


    // BEFORE WE CREATE A SAVE FILE, WE MUST CHECK TO SEE IF ONE OF THIS CHARACTER SLOT ALREADY EXISTS(MAX 10 CHARACTER SLOTS)
    public bool CheckToSeeIfFileExists()
    {
        if (File.Exists(Path.Combine(saveDataDirectoryPath, saveFileName)))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // USED TO DELETE CHARACTER SAVE FILES
    public void DeleteSaveData()
    {
        File.Delete(Path.Combine(saveDataDirectoryPath, saveFileName));
    }

    // USED TO CREATE A SAVE FILE UPON STARTING A NEW GAME
    public void CreateNewCharacterSaveFile(CharacterSaveData characterData)
    {
        // MAKE A PATH TO SAVE THE FILE( A LOCATION TO MACHINE)
        string savePath = Path.Combine(saveDataDirectoryPath, saveFileName);

        try
        {
            // CREATE THE DIRECTORY THE FILE WILL BE WRITTEN TO, IF IT DOES NOT ALREADY EXIST
            Directory.CreateDirectory(Path.GetDirectoryName(savePath));
            Debug.Log("CREATING SAVE FILE, AT SAVE PATH: "+ savePath);
            
            //SERIALIZE THE C3 GAME DATA OBJECT TO JSON FILE
            string dataToStore = JsonUtility.ToJson(characterData, true);
            
            // WRITE THE FILE TO OUR SYSTEM
            using (FileStream fileStream = new FileStream(savePath, FileMode.Create))
            {
                using (StreamWriter fileWriter = new StreamWriter(fileStream))
                {
                    fileWriter.Write(dataToStore);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log("ERROR WHILE TRYING TO SAVE CHARACTER DATA, GAME NOT SAVED "+ savePath + "\n" + ex);
        }
    }
    
    // USED TO LOAD A SAVE FILE UPON LOADING A PREVIOUS GAM
    public CharacterSaveData LoadSaveData()
    {
        
        CharacterSaveData characterSaveData = null;
        // MAKE A PATH TO SAVE THE FILE( A LOCATION TO MACHINE)
        string loadPath = Path.Combine(saveDataDirectoryPath, saveFileName);

        if (File.Exists(loadPath))
        {
            try
            {
                string dataToLoad = "";
                using (FileStream fileStream = new FileStream(loadPath, FileMode.Open))
                {
                    using (StreamReader fileReader = new StreamReader(fileStream))
                    {
                        dataToLoad = fileReader.ReadToEnd();
                    }
                }

                // DESERIALIZE THE DATA FROM JSON BACK TO UNITY
                characterSaveData = JsonUtility.FromJson<CharacterSaveData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.Log("ERROR!");
            }
        }
        return characterSaveData;
        
    }
}
