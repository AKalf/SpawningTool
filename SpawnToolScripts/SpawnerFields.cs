using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SpawnSystemTool
{
    public class SpawnerFields
    {
        static bool[] whichSpawnersGroupPerTypeToFoldout = null;

        static Dictionary<SerializedObject, SerializedProperty[]> propertiesPerSpawner = new Dictionary<SerializedObject, SerializedProperty[]>();
        // Draws spawners by grouping them depending on the unit type they spawn
        // Draws serialized properties (Spawner.transform, UntiToSpawnPrefab, SpawnFrequency, TotalUnitsToSpawn)
        public static void DrawFormatedSpawnerFields()
        {
            // if there are serialized properties to show
            if (propertiesPerSpawner.Keys.Count > 0)
            {
                bool foldout = false;
                EditorGUILayout.BeginVertical();
                int unitTypeIndex = 0;
                foreach (string unitType in SpawnTypes.GetAllTypes())
                {

                    foldout = EditorGUILayout.Foldout(whichSpawnersGroupPerTypeToFoldout[unitTypeIndex], "Show spawners with unit types: " + unitType);      
                   
                    int totalUnitsPerType = 0;
                    foreach (SerializedObject obj in propertiesPerSpawner.Keys)
                    {
                        // check if this spawner.unitType is spawning now
                        if (unitType == ((Spawner)obj.targetObject).GetUnitToSpawn().name)
                        {
                            // an other spawner with that unit type
                            totalUnitsPerType++;
                            // if foldout for this spawner's unit type is true
                            if (whichSpawnersGroupPerTypeToFoldout[unitTypeIndex])
                            {
                                EditorGUILayout.BeginHorizontal();
                                // Draw serialized properties of current serialized <Spawn> object 
                                foreach (SerializedProperty property in propertiesPerSpawner[obj])
                                {
                                    EditorGUI.BeginChangeCheck();
                                    EditorGUILayout.PropertyField(property, new GUIContent(""), GUILayout.Width(150));
                                    // Chack if any changes have been made by the user and inform the valus in the target <Spawner> script
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        ChangeSerializedPropertyValue(property);
                                    }
                                }
                                    EditorGUILayout.EndHorizontal();
                            }

                        }
                    }
                    // Check if user clicked on any foldout field and respond
                    ActivateUnitTypeFoldout(foldout, unitTypeIndex, totalUnitsPerType);
                    unitTypeIndex++;
                }
                EditorGUILayout.EndVertical();
            }
            
        }
        // changes the values of the targeted script to be equal with the values that the user set from the tool's window
        private static void ChangeSerializedPropertyValue(SerializedProperty prop) {
            string propName = prop.name;
            switch (propName) {
                //case "thisGO":
                //    ((Spawner)prop.serializedObject.targetObject).name = prop.stringValue;
                //    break;
                case "spawningFrequency" :
                    ((Spawner)prop.serializedObject.targetObject).spawningFrequency = prop.intValue;
                    break;
                case "numberOfUnitsToSpawn":
                    ((Spawner)prop.serializedObject.targetObject).numberOfUnitsToSpawn = prop.intValue;
                    break;
                case "unitToSpawn":
                    ((Spawner)prop.serializedObject.targetObject).SetUnitToSpawn((GameObject)prop.objectReferenceValue);
                    break;
            }

        }
        // Creates a bool array with length equal to the number of unit types foldout fields
        public static void  SetTotalFoldoutsPerUnitType(int length)
        {
            Debug.Log("unit types set");
            whichSpawnersGroupPerTypeToFoldout = new bool[length];
        }
        // Set active or not a unit type foldout field. Increases or decreases the Y position of the graph panel accordingly 
        private static void ActivateUnitTypeFoldout(bool should, int typeIndex, int totalUnitsPerType) {
            if (should != whichSpawnersGroupPerTypeToFoldout[typeIndex]) {
                if (should) {
                    SpawnersToolWindow.IncDecGraphPanelRect(0, 20 * totalUnitsPerType, 0 ,0);
                }
                else {
                    SpawnersToolWindow.IncDecGraphPanelRect(0, -20 * totalUnitsPerType, 0, 0);
                }
                whichSpawnersGroupPerTypeToFoldout[typeIndex] = should;
            }
        }
        // Sets the serialized objects and properties that will be displayed on window
        public static bool SetUpSerializedProperties(Spawner[] totalSpawners) {
            propertiesPerSpawner.Clear();
            foreach (Spawner spawner in totalSpawners) {
                    // Get the component <Spawner> as serialized object            
                    SerializedObject obj = new SerializedObject(spawner);
                    //Debug.Log(spawner.name + " serialized");
                    propertiesPerSpawner.Add(obj, new SerializedProperty[4]);
                    // set the variable "thisGameObject" of the script <Spawner> to be equal with the gameobject that holds the script 
                    spawner.thisGO = spawner.gameObject;
                    // Get the property that holds the "Spawner" gameobject and serialize it
                    propertiesPerSpawner[obj][0] = obj.FindProperty("thisGO");
                    //Debug.Log("thisGO property serialized");
                    // get the property of the type of unit that will be spawned
                    propertiesPerSpawner[obj][1] = obj.FindProperty("unitToSpawn");
                    //Debug.Log("unitToSpawn property serialized");
                    //get the spawning frequency
                    propertiesPerSpawner[obj][2] = obj.FindProperty("spawningFrequency");
                    //Debug.Log("spawningFrequency property serialized");
                    //get the spawning frequency
                    propertiesPerSpawner[obj][3] = obj.FindProperty("numberOfUnitsToSpawn");
                    //Debug.Log("numberOfUnitsToSpawn property serialized");
            }
            
            return true;

        }

       
        
    }
}