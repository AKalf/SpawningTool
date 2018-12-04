using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEditor;



namespace SpawnSystemTool
{
    public class SpawnersToolWindow : EditorWindow
    {


        static SpawnersToolWindow activeWindow;

        bool searchButtonPressed = false; // re- initializes everything on click (except unit type colors)
        bool hasSearchedForSpawners = false; // has the tool been init
        bool showSpawners; // spawners, properties and graph will be draw if true

        static Spawner[] totalSpawners; // total gameobjects found with component <Spawner>
        static int totalTime = 1200; // the length of time axis
        int numberOfTotalTypes = 0; // number of different gameobjects that will be spawned
        
        static SpawnGraph spawnGraph;

        #region GraphPanelVar        
        static MyEditorPanel graphPanel = new MyEditorPanel(new Rect(0, 0, 200, 200));
        Color graphPanelBackgroundColor = new Color(0.5f, 0.5f, 0.5f, 0.2f);
        static int graphPanelWidth = 600;
        static int graphPanelHeight = 600;
        static int graphPanelXOffsetFromUnitTypes = 200;
        static int graphOffsetFromProperties = 50;
        #endregion


        // Add menu named "My Window" to the Window menu
        [MenuItem("SpawnersTool/SpawnPanel")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            activeWindow = (SpawnersToolWindow)EditorWindow.GetWindow(typeof(SpawnersToolWindow));
            activeWindow.Show();
        }

        void OnGUI()
        {
            // Draw "search" button and check if clicked
            DrawSearchSpawnersButton();
            // if initialized
            if (hasSearchedForSpawners)
            {
                if (searchButtonPressed) {
                    // if button pressed - re-initialize everything
                    graphPanel = SetUpGraphPanel();
                    SpawnTypes.SetAllTypes(totalSpawners);
                    numberOfTotalTypes = SpawnTypes.GetAllTypes().Count;
                    spawnGraph = new SpawnGraph(graphPanel, totalTime + 1);
                    IncDecGraphPanelRect(0, 20 * numberOfTotalTypes, 0 ,0);  
                    SpawnerFields.SetTotalFoldoutsPerUnitType(numberOfTotalTypes);
                   

                    searchButtonPressed = false;
                }
                // if show spawners data == true
                showSpawners = EditorGUILayout.Foldout(showSpawners, "Show spawners", EditorStyles.foldout);
                if (showSpawners && numberOfTotalTypes > 0)
                {
                    // Draw spawners and properties
                    SpawnerFields.DrawFormatedSpawnerFields();
                    // Draw graph's background
                    EditorGUI.DrawRect(graphPanel.GetRect(), graphPanelBackgroundColor);
                    // Draw the graph
                    spawnGraph.DrawUnitsPerSecGraph(graphPanel.GetRect());

                }
            }
            else
            {
                EditorGUILayout.LabelField("Press: Search spawners first!", EditorStyles.miniLabel);
            }
        }
       
        private void DrawSearchSpawnersButton() {
            if (GUILayout.Button("Search spawners", GUILayout.MaxWidth(120), GUILayout.MinWidth(100)))
            {
                // if and gameobjects with component <Spawner> found
                hasSearchedForSpawners = SearchForSpawners();
                if (hasSearchedForSpawners)
                {
                    // there are spawners to initialize
                    searchButtonPressed = true;
                    
                    // if all spawners properties have not been identifined
                    if (!SpawnerFields.SetUpSerializedProperties(totalSpawners)) {
                        // dont continue to initilization, data is missing
                        hasSearchedForSpawners = false;
                        searchButtonPressed = false;
                    }
                }
            }
        }
        
       
       

        private static MyEditorPanel SetUpGraphPanel()
        {
            
            Rect rect = new Rect(graphPanelXOffsetFromUnitTypes, graphOffsetFromProperties, graphPanelWidth, graphPanelHeight);
            MyEditorPanel graphP = new MyEditorPanel(rect);
            return graphP;
        }
        public static  void IncDecGraphPanelRect(float x, float y, float w, float h) {
            graphPanel = new MyEditorPanel(new Rect(graphPanel.GetRect().x + x, graphPanel.GetRect().y + y, graphPanel.GetRect().width + w, graphPanel.GetRect().height + h));
            // inform scroll rects for new position
            spawnGraph.SetUpScrollRects(graphPanel.GetRect());
            spawnGraph.DrawUnitsPerSecGraph(graphPanel.GetRect());
        }

        private bool SearchForSpawners()
        {
            totalSpawners = null;
            totalSpawners = FindObjectsOfType<Spawner>();
            if (totalSpawners.Length > 0)
            {
                Debug.Log("Total spawners found: " + totalSpawners.Length);
                return true;
            }
            else {
                Debug.LogError("No gameobjects found with the scirpt component: <Spawner>");
                return false;
            }
            
        }

        public static Rect GetCopyGraphPanelRect() {
            return new Rect(graphPanel.GetRect());
        }

        public static Spawner[] GetSpawners() {
            return totalSpawners;
        }
    }


    public class MyEditorPanel {
        private Rect rect;
        public MyEditorPanel(Rect source) {
            rect = new Rect(source);
        }
        public MyEditorPanel() {

        }
       
        public Rect GetRect() {
            return rect;
        }

    }

    [System.Serializable]
    public class SavedUnitTypeColorsArray 
    {
        public Color[] colorArray;
        public SavedUnitTypeColorsArray(Color[] array)
        {
            colorArray = array;
        }

    }
    public class SpawnersPerTagContainer
    {
        public string spawnerTag;
        public string[] unitTypeOfEachSpawner;
        public GameObject[] spawnersWithThisTag;
        public SpawnersPerTagContainer(GameObject[] s, string[] u, string t)
        {
            spawnersWithThisTag = s;
            unitTypeOfEachSpawner = u;
            spawnerTag = t;
        }
        public SpawnersPerTagContainer(string t)
        {
            spawnerTag = t;
        }
    }
}
