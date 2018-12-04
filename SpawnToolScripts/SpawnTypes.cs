using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SpawnSystemTool
{
    public class SpawnTypes : Editor
    {

        // all unit types that will be spawned
        static List<string> allTypes = new List<string>();
       
        
       
        public static void SetAllTypes(Spawner[] spawners)
        {
            allTypes.Clear();
            foreach (Spawner spawner in spawners)
            {
                if (!allTypes.Contains(spawner.GetUnitToSpawn().name))
                {
                    allTypes.Add(spawner.GetUnitToSpawn().name);
                }
            }   
        }
        private static void AddType(string type)
        {
            if (!allTypes.Contains(type))
            {
                allTypes.Add(type);
            }
        }
        public static void RemoveType(string type) {
            if (allTypes.Contains(type)) {
                allTypes.Remove(type);
            }
        }
        public static List<string> GetAllTypes()
        {
            return allTypes;
        }

        public static Dictionary<string, float[]> GetUnitsPerSec(int totalTime) {
            Dictionary<string, float[]> unitsPerType = new Dictionary<string, float[]>();
            #region FindTotalUnitsPerType

            Spawner[] totalSpawners = SpawnersToolWindow.GetSpawners();
            // for each group of spawners with the same tag
            foreach (Spawner spawner in totalSpawners) {
                    // if dictionary has already defined this unit type
                    if (unitsPerType.ContainsKey(spawner.GetUnitToSpawn().name))
                    {
                        // get this spawner's unit / sec activity
                        float[] unitsPerSec = FindUnitsPerSec(spawner.numberOfUnitsToSpawn, spawner.spawningFrequency, totalTime);
                        for (int time = 0; time < totalTime; time++) {
                            unitsPerType[spawner.GetUnitToSpawn().name][time] += unitsPerSec[time];
                        } 
                    }
                    else {
                        unitsPerType.Add(spawner.GetUnitToSpawn().name, FindUnitsPerSec(spawner.numberOfUnitsToSpawn, spawner.spawningFrequency, totalTime));
                    }                         
            }
            #endregion
            return unitsPerType;

        }
        private static float[] FindUnitsPerSec(int numberOfUnits, int spawnFequency, int totalTime) {
            float[] unitsPerSec = new float[totalTime];
            int lastIndexSet  = 0;
            for (int unit = 0; unit <= numberOfUnits; unit++) {
                for (int sec = 0; sec < unit * spawnFequency; sec++) {
                    if (unit * spawnFequency + sec < unitsPerSec.Length)
                    {
                        unitsPerSec[unit * spawnFequency + sec] = unit;
                        lastIndexSet = unit * spawnFequency + sec;
                    }
                    
                }
            }
            if (lastIndexSet < unitsPerSec.Length)
            {
                for (int index = lastIndexSet; index != unitsPerSec.Length; index++)
                {
                    unitsPerSec[index] = numberOfUnits;
                }
            }
            return unitsPerSec;
        }

        public static float GetMaximunNumberOfUnits(int totalTime) {
            Dictionary<string, float[]> unitsPerType = GetUnitsPerSec(totalTime);
            float maxNumber = 0;
            foreach (string unitType in unitsPerType.Keys) {  
                for (int sec = 0; sec != totalTime; sec++)
                {
                    if (unitsPerType[unitType][sec] > maxNumber)
                    {
                        maxNumber = unitsPerType[unitType][sec];
                    }
                }                
            }
            return maxNumber;
        }
        
    }
    
}
