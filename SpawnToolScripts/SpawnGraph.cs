using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace SpawnSystemTool
{
    public class SpawnGraph 
    {

         Rect totalScrollRect;
         Rect currentlyViewingRect;
         MyEditorPanel graphPanel;

         Color[] unitTypesColors = null;

         Dictionary<string, float[]> totalUnitsAndUnitsPerSecForAllTypes = new Dictionary<string, float[]>();

         Vector2 waveScrollPosition;

         int maxUnits;

         int totalTimeToCount = 60;

         int collumAndLineSize = 20;

         int graphCollumWidth = 46;
         int graphCollusOffset = 4;

         int graphLineHeight = 20;

         int unitsAxisOffsetFromWavePaenl = 40;

        int wavePanelXOffsetFromUnitTypes = 200;
        int unitTypesListWidth = 100;
        int unitTypesListHeight = 20;

        public SpawnGraph(MyEditorPanel graphP, int totalTime) {
           
            totalTimeToCount = totalTime;
           
            graphPanel = graphP;
           
            maxUnits = (int)SpawnTypes.GetMaximunNumberOfUnits(totalTimeToCount);
            totalUnitsAndUnitsPerSecForAllTypes = SpawnTypes.GetUnitsPerSec(totalTimeToCount);
            // Set up color array with random colors
            if (unitTypesColors == null)
            {
                SavedUnitTypeColorsArray savedCollorArrayObject = JsonLib.LoadGameData();
                if (savedCollorArrayObject != null && savedCollorArrayObject.colorArray.Length > 0)
                {
                    //Debug.Log("Unit types graph colors loaded from previous session");
                    unitTypesColors = savedCollorArrayObject.colorArray;
                }
                else {
                    //Debug.Log("No unit types graph colors loaded from previous session");
                    unitTypesColors = new Color[150];
                    for (int i = 0; i != unitTypesColors.Length; i++)
                    {
                        unitTypesColors[i] = new Color(Random.Range(0, 0.9f), Random.Range(0, 0.9f), Random.Range(0, 0.9f), Random.Range(0, 0.9f));
                        unitTypesColors[i].a = 1;
                    }
                }
                JsonLib.SaveGameData(new SavedUnitTypeColorsArray(unitTypesColors));
                
                
            }

        
        }

        public void DrawUnitsPerSecGraph(Rect graphPanel)
        {

            int unitInex = 0;
            // create scroll bars
            waveScrollPosition = GUI.BeginScrollView(currentlyViewingRect, waveScrollPosition, totalScrollRect, true, true);
            // for each unit type that will be spawned

            foreach (string unitType in totalUnitsAndUnitsPerSecForAllTypes.Keys)
            {
                // for each sec of the total time to be dispalayed
                for (int sec = 0; sec < totalTimeToCount; sec++)
                {
                    // create a new rect equal to graphPanel
                    Rect rect = new Rect(graphPanel);
                    // set X position to the current second multiplied by collum width + an offset from each graph collum
                    rect.x += sec * (graphCollumWidth + graphCollusOffset);
                    // set the center of the rect above the number of time axis
                    rect.x -= graphCollumWidth / 2;
                    // set Y postion to the bottom of the total rect
                    rect.y += totalScrollRect.height;
                    // set width to be equal to a division of graph collum width that depends on the number of total units
                    rect.width = graphCollumWidth - (unitInex * (graphCollumWidth / totalUnitsAndUnitsPerSecForAllTypes.Keys.Count));
                    // set height to be equal to the total units of current type at current second multiplied by graph line height
                    rect.height = -(((int)((totalUnitsAndUnitsPerSecForAllTypes[unitType])[sec])) * graphLineHeight );
                    // draw the rect
                    EditorGUI.DrawRect(rect, unitTypesColors[unitInex]);
                }
                unitInex++;
            }
            GUI.EndScrollView();
            DrawTimeAxis(graphPanel);
            DrawUnitsAxis(graphPanel);
            DrawUnitTypeList(graphPanel);
        }
        // draws the unit and time axis around graph panel
        private  void DrawTimeAxis(Rect graphPanel)
        {
            // time axis

            // for each graph collum that fits to the currentlyViewingRect  
            for (int sec = 0; sec < (int)(graphPanel.width / (graphCollumWidth + graphCollusOffset)); sec++)
            {
                // create new rect
                Rect collum = new Rect(graphPanel);
                string label = ((int)(sec + (waveScrollPosition.x / (graphCollumWidth + graphCollusOffset)))).ToString();
                // set potion X to current second multiplied by the total graph collum width. Set Y 20 units above graphPanelHeight
                collum.position += new Vector2(sec * (graphCollumWidth + graphCollusOffset) - (label.ToCharArray().Length -1)  * 12.5f, graphPanel.height + 20);
                // set the width to be equal to graph collum width
                collum.width = (graphCollumWidth + graphCollusOffset);
                // typical height, does not really matter
                collum.height = 20;
                // create a label at the rect's position and write the sec + the amount that horizontal scrollbar has moved. + 0.5f as an offset to be set on center
                EditorGUI.LabelField(collum, label);
            }

            // units axis
           
        }
        private  void DrawUnitsAxis(Rect graphPanel) {
            // for each graph line that fits to the currentlyViewingRect
            if (maxUnits > (int)(graphPanel.height / (graphLineHeight)))
            {
                for (int number = 0; number < (int)((graphPanel.height - graphLineHeight) / (graphLineHeight)); number++)
                {

                    // create a new rect for a new line
                    Rect line = new Rect(graphPanel);
                    // set Y position to the currently number displayed multiplied by graph line height. Set X at offset from graphPanel X position
                    line.position += new Vector2(-unitsAxisOffsetFromWavePaenl, number * graphLineHeight + graphLineHeight /2);
                    // set the height to be equal to graph collum height
                    line.height = graphLineHeight;
                    // typical width, does not really matter
                    line.width = 50;
                    string lineToString = (maxUnits - number - (int)((waveScrollPosition.y / graphLineHeight))).ToString();
                    EditorGUI.LabelField(line, lineToString);

                }
            }
            else
            {
                for (int number = 0; number < maxUnits; number++)
                {

                    // create a new rect for a new line
                    Rect line = new Rect(graphPanel);
                    // set Y position to the currently number displayed multiplied by graph line height. Set X at offset from graphPanel X position
                    line.position += new Vector2(-unitsAxisOffsetFromWavePaenl, number * graphLineHeight);
                    // set the height to be equal to graph collum height
                    line.height = graphLineHeight;
                    // typical width, does not really matter
                    line.width = 20;
                    string lineToString = (maxUnits - number - (int)((waveScrollPosition.y / graphLineHeight))).ToString();
                    EditorGUI.LabelField(line, lineToString);

                }

            }
        }
        private void DrawUnitTypeList(Rect graphPanel)
        {
            for (int line = 0; line != SpawnTypes.GetAllTypes().Count; line++)
            {
                Rect rect = new Rect(graphPanel);
                rect.position = rect.position + new Vector2(-wavePanelXOffsetFromUnitTypes, line * collumAndLineSize);
                rect.width = unitTypesListWidth;
                rect.height = unitTypesListHeight;
                EditorGUI.LabelField(rect, SpawnTypes.GetAllTypes()[line]);
                rect.x += unitTypesListWidth;
                rect.width = 50;
                rect.height = 10;
                EditorGUI.BeginChangeCheck();
                unitTypesColors[line] = EditorGUI.ColorField(rect, unitTypesColors[line]);
                if (EditorGUI.EndChangeCheck()) {
                    Debug.Log("Color of unit type: " + SpawnTypes.GetAllTypes()[line] + " changed to " + unitTypesColors[line].ToString());
                    JsonLib.SaveGameData(new SavedUnitTypeColorsArray (unitTypesColors));
                } 
            }
        }
        public  void SetUpScrollRects(Rect targetRect) {
            totalScrollRect = new Rect(targetRect);
            currentlyViewingRect = new Rect(targetRect);
            // the total amount of time multiplied by collum width
            totalScrollRect.width = (graphCollumWidth + graphCollusOffset) * totalTimeToCount;
            // the max number of units that will be reached in time displayed multiplied by the height of line
            if (maxUnits > (int)(graphPanel.GetRect().height / (graphLineHeight)))
            {
                totalScrollRect.height = maxUnits * graphLineHeight + graphLineHeight;
            }
            else
            {
                totalScrollRect.height = graphPanel.GetRect().height;
                graphLineHeight = (int)(graphPanel.GetRect().height / maxUnits);
            }
            // the rect that will be actually displayed depending on the scroll bars value
            currentlyViewingRect.width = graphPanel.GetRect().width;
            currentlyViewingRect.height = graphPanel.GetRect().height;
        }
    }
}
