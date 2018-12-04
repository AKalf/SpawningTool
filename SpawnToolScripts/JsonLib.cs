using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace SpawnSystemTool
{
    public static class JsonLib
    {

        private static string gameDataFileName = @"\StreamingAssets\data.json";

        public static void SaveGameData(SavedUnitTypeColorsArray objToSerialize)
        {

            string dataAsJson = JsonUtility.ToJson(objToSerialize);

            string filePath = Application.dataPath + gameDataFileName;
            File.WriteAllText(filePath, dataAsJson);

        }

        public static SavedUnitTypeColorsArray LoadGameData()
        {
            string filePath = Application.dataPath + gameDataFileName;

            if (File.Exists(filePath))
            {
                SavedUnitTypeColorsArray objToReturn = null;
                string dataAsJson = File.ReadAllText(filePath);
                try
                {
                    objToReturn = JsonUtility.FromJson<SavedUnitTypeColorsArray>(dataAsJson);
                    return objToReturn;
                }
                catch
                {
                    return objToReturn;
                }

            }
            else
            {
                return null;
            }
        }
    }
}