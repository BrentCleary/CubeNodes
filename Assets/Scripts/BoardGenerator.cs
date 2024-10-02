using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UIElements;

public class BoardGenerator : MonoBehaviour
{

    //* ---------------------------------------- PROPERTIES ----------------------------------------
    // MasterNode for default Value for Reference
    private GameObject masterNode;

    [SerializeField] public GameObject[,] gNodeArray;
    [SerializeField] public List<GameObject> gNodeList;
    
    // ! Array Size Controls
    private int arrayColumnLength = 2;                  // Array Dimensions - Column
    private int arrayRowLength = 2;                     // Array Dimensions - Row
    private int nodeSpacingValue = 2;                   // Space Between Nodes
    private int arrayTotalNodes;                        // arrayColumnLength * arrayRowLength

    public GameObject nodePrefab;
    public List<int> startNodeValueMap;
    public List<int> currentNodeValueMap;

    public Transform gNodeArrayTransform;

    public List<Node> sheepGroupList;



    //* ---------------------------------------- START AND UPDATE METHODS ----------------------------------------
                                     //* Generates Node GameObjects and gNodeArray 

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Board Generator Started");

        masterNode = GameObject.Find("MasterNode");

        gNodeArray = new GameObject[arrayColumnLength, arrayRowLength];
        arrayTotalNodes = arrayColumnLength * arrayRowLength;
        
        gNodeList = new List<GameObject>();
        gNodeArrayTransform = gameObject.transform;

        InstantiateNodes();
        SetNodeTransformPosition();
        
        BuildNodeArray();
        AdjacentSheepNodeMapper(gNodeList);

        // Generate Initial Value Map
        startNodeValueMap = NodeValueMapper();
        NodeValueUpdater(startNodeValueMap);
    }

    // Update is called once per frame
    void Update()
    {
        DebugControls();
    }




    // *---------------------------------------- BOARD NODE CREATION METHODS ----------------------------------------
                                               //* Called in Start Method
    public void InstantiateNodes()
    {
        for(int i = 1; i <= arrayTotalNodes; i++) {
            GameObject gNode = Instantiate(nodePrefab, gNodeArrayTransform);
            gNode.name = "Node (" + i + ")";
            gNodeList.Add(gNode);
        }
    }


    public void SetNodeTransformPosition()
    {
        int nodeCounter = 0;                                // Increments Node reference in gNodeList 
        for(int i = 0; i < arrayColumnLength; i ++){         // Assigns positions to each gNode in gNodeArray
            for(int j = 0; j < arrayRowLength; j ++){
                gNodeList[nodeCounter].transform.position = new Vector3(i * nodeSpacingValue, 0, j * nodeSpacingValue); // Example positioning
                nodeCounter++;
            }
        }
    }



    // *---------------------------------------- ARRAY GENERATION AND VALUE SET METHODS ----------------------------------------
                                                    //* Called in Start Method

    // ------------------------------ //* gNodeArray Assigner 0 ------------------------------
    public void BuildNodeArray()                         // Generate a Length x Row array containing Nodes contained gNodeList
    {               
        int nodeCounter = 0;                                // Increments Node reference in gNodeList 
        for(int i = 0; i < arrayColumnLength; i ++){         // Assigns positions to each gNode in gNodeArray
            for(int j = 0; j < arrayRowLength; j ++){
                gNodeArray[i,j] = gNodeList[nodeCounter];   // Set curent gNodeList object to current array position
                
                // Maps Board Array position for reference
                NodeScript nodeScript = gNodeList[nodeCounter].GetComponent<NodeScript>();
                nodeScript.arrayPosition[0] = i;
                nodeScript.arrayPosition[1] = j;
                
                // Add Array Position to Node Name
                gNodeArray[i,j].name = gNodeArray[i,j].name + " [" + i + "," + j+ "]";

                nodeCounter++;                              // Increment gNodeList index
            }
        }
    }



    // *---------------------------------------- SHEEP GROUP METHODS ----------------------------------------
                                //* Loops over gNodeList and assigns adjacent Nodes 
                                            //* CALLED IN START() METHOD
    
    public void AdjacentSheepNodeMapper(List<GameObject> gNodeList)
    {
        foreach(GameObject currentNode in gNodeList)
        {
            NodeScript nodeScript = currentNode.GetComponent<NodeScript>();
            int[] arrayPos = nodeScript.arrayPosition;   
        
            // Left index
            if(arrayPos[0] == 0){      // left index is not out of range 
                nodeScript.leftNode = null;
            }
            if(arrayPos[0] != 0){      // left index is a node
                nodeScript.leftNode = gNodeArray[arrayPos[0] - 1, arrayPos[1]].gameObject;
                nodeScript.leftNodeScript = gNodeArray[arrayPos[0] - 1, arrayPos[1]].GetComponent<NodeScript>();
            }

            // Right index
            if(arrayPos[0] == arrayColumnLength - 1){      // right index is not out of range 
                nodeScript.rightNode = null;
            }
            else if(arrayPos[0] != arrayColumnLength - 1){      // right index is a node
                nodeScript.rightNode = gNodeArray[arrayPos[0] + 1, arrayPos[1]].gameObject;
                nodeScript.rightNodeScript = gNodeArray[arrayPos[0] + 1, arrayPos[1]].GetComponent<NodeScript>();
            }

            // Bottom index
            if(arrayPos[1] == 0){      // bottom index is not out of range 
                nodeScript.bottomNode = null;
            }
            if(arrayPos[1] != 0){      // bottom index is a node
                nodeScript.bottomNode = gNodeArray[arrayPos[0], arrayPos[1] - 1].gameObject;
                nodeScript.bottomNodeScript = gNodeArray[arrayPos[0], arrayPos[1] - 1].GetComponent<NodeScript>();
            }

            // Top index
            if(arrayPos[1] == arrayRowLength - 1){      // top index is not out of range 
                nodeScript.topNode = null;
            }
            else if(arrayPos[1] != arrayRowLength - 1){      // top index is a node
                nodeScript.topNode = gNodeArray[arrayPos[0], arrayPos[1] + 1].gameObject;
                nodeScript.topNodeScript = gNodeArray[arrayPos[0], arrayPos[1] + 1].GetComponent<NodeScript>();
            }

        }
    }







    // *---------------------------------------- NODE VALUE DISPLAY AND UPDATE METHODS ----------------------------------------
                                                    //* Called in Start Method
                                                    //* Called in TargetNode.cs
    
     // --------------------------------------------- // Calls Functions 1, 2, 3  ---------------------------------------------
    public void BoardUpdaterFunction()
    {
        List<int> nodeValueMap = NodeValueMapper();
        NodeValueUpdater(nodeValueMap);
    }

    // --------------------------------------------- // gNodeValue Updater Part 1 ---------------------------------------------
    public List<int> NodeValueMapper()          // Displays Array based on nodeValues
    {
        List<int> nodeValueMap = new List<int>();  // List to hold update values for arrayNodes
        
        // Reset all node liberty values to 1
        foreach(GameObject node in gNodeList){         // Loops over all nodes in gNodeList    
            NodeScript nodeScript = node.GetComponent<NodeScript>();  // Set liberty value based on masterNode
            // No Sheep Placed
            if(nodeScript.sheepValue == nodeScript.sheepValueList[0]){
                nodeValueMap.Add(nodeScript.nodeValueList[4]);     // Assigns max NodeValue to each position
                nodeScript.libertyValue = nodeScript.libertyValueList[1];
            }
            // Sheep placed
            else {
                nodeValueMap.Add(nodeScript.nodeValueList[0]);     // Assigns min NodeValue to each position
                nodeScript.libertyValue = nodeScript.libertyValueList[0];
            }
        }
        
        // Update nodeValueMap based on position and board state
        List<int> nodeValueMapUpdated = NodeValueMapIndexer(nodeValueMap);

        // NodeValueMapDebugDisplayValue(nodeValueMap);  // DEBUG METHOD 

        // Debug.Log("gNodeArray nodeValues Mapped to List");

        return nodeValueMapUpdated;
    }

    // ---------------------------------------- -----// gNodeValue Updater Part 2 ---------------------------------------------
    public List<int> NodeValueMapIndexer(List<int> nodeValueMap)
    {
        // Counter to increment through index in nodeValueMap
        int mapIndex = 0;     

        // Map node values to nodeValueMap based on current board state
        for(int i = 0; i < arrayColumnLength; i++){         // Assigns positions to each gNode in gNodeArray
            for(int j = 0; j < arrayRowLength; j++){                
                // Left Index Check
                if(j == 0){      // Check left index is not out of range 
                    nodeValueMap[mapIndex] -= 1;
                }
                if(j != 0){      // Check left position
                    if(gNodeArray[i, j-1].GetComponent<NodeScript>().libertyValue == 0){     // Check left mapIndex is not null
                        nodeValueMap[mapIndex] -= 1;
                    }
                }

                // Right Index Check
                if(j == arrayRowLength-1){      // Check right mapIndex is not out of range 
                    nodeValueMap[mapIndex] -= 1;
                }
                if(j != arrayRowLength-1){       // Check right position
                    if(gNodeArray[i, j+1].GetComponent<NodeScript>().libertyValue == 0){     // Check right mapIndex is not null
                        nodeValueMap[mapIndex] -= 1;
                    }
                }

                // Top Index Check
                if(i == 0){      // Check top mapIndex is not out of range 
                    nodeValueMap[mapIndex] -= 1;
                }
                if(i != 0){      //  Check top position
                    if(gNodeArray[i-1, j].GetComponent<NodeScript>().libertyValue == 0){
                        nodeValueMap[mapIndex] -= 1;
                    }
                }

                // Bottom Index Check
                if(i == arrayColumnLength-1){      // Check bottom mapIndex is not out of range 
                    nodeValueMap[mapIndex] -= 1;
                }
                if(i != arrayColumnLength-1){      // Check bottom position 
                    if(gNodeArray[i+1, j].GetComponent<NodeScript>().libertyValue == 0){
                        nodeValueMap[mapIndex] -= 1;
                    }
                }

                mapIndex += 1;
            }
        }
        // Debug.Log("gNodeArray MapIndex Updated");

        return nodeValueMap;
    }

    // --------------------------------------------- // gNodeValue Updater Part 3 ---------------------------------------------
                                                  //* Calls SetGrassTileDisplayLoop
    public void NodeValueUpdater(List<int> nodeValueMap)
    {
        int arrayIndex = 0;

        // Map nodeValueMap values to nodeArray
        for(int i = 0; i < arrayColumnLength; i++){         // Assigns positions to each gNode in gNodeArray
            for(int j = 0; j < arrayRowLength; j++){
                GameObject currentNode = gNodeArray[i,j];
                NodeScript currentNodeScript = currentNode.GetComponent<NodeScript>();
                
                // TODO - Update here to check state of node to change liberty Value
                if(nodeValueMap[arrayIndex] < 0) {
                    currentNodeScript.nodeValue = 0;
                }
                else {
                    currentNodeScript.nodeValue = nodeValueMap[arrayIndex];
                }
                
                currentNode.GetComponent<NodeScript>().SetGrassTileDisplayLoop();
                // Debug.Log(currentNode.GetComponent<NodeScript>().name + "'s nodeValue is " + currentNode.GetComponent<NodeScript>().nodeValue);

                arrayIndex += 1;
            }
        }

        // Debug.Log("gNodeArray Update Complete");
    }


    public void NodeDisplayUpdate()
    {
        foreach(GameObject node in gNodeList)
        {
            node.GetComponent<NodeScript>().SetGrassTileDisplayLoop();
        }
    }



    // GNODE LIST MAPPER
    // public List<int> AdjacentNodeMapIndexer(List<int> nodeValueMap)
    // {
    //     // Counter to increment through index in nodeValueMap
    //     int index = 0;     

    //     // Map node values to nodeValueMap based on current board state
    //     foreach(GameObject node in gNodeList)
    //     {
    //         NodeScript nScript = node.GetComponent<NodeScript>();
    //         List<NodeScript> adjList = nScript.adjNodeScriptList;

    //         foreach(NodeScript adjScript in adjList)
    //         {
    //             if(adjScript == null || adjScript.libertyValue == 0)
    //             {
    //                 nodeValueMap[index] -= 1;
    //                 Debug.Log("Adj Script " + adjScript.name + " is null at " + node.name);
    //             }
    //         }
            
    //         index += 1;
    //     }   
        
    //     return nodeValueMap;
    // }




    // --------------------------------------------- // gNodeValue Updater Part 3 ---------------------------------------------
                                                  //* Calls SetGrassTileDisplayLoop
    // public void NodeValueListUpdater(List<int> nodeValueMap)
    // {
    //     int arrayIndex = 0;

    //     // Map nodeValueMap values to nodeArray
    //     foreach(GameObject node in gNodeList)
    //     {         

    //         NodeScript currentNodeScript = node.GetComponent<NodeScript>();
            
    //         if(nodeValueMap[arrayIndex] < 0) {
    //             currentNodeScript.nodeValue = 0;
    //         }
    //         else {
    //             currentNodeScript.nodeValue = nodeValueMap[arrayIndex];
    //         }
            
    //         node.GetComponent<NodeScript>().SetGrassTileDisplayLoop();
    //         Debug.Log(node.GetComponent<NodeScript>().name + "'s nodeValue is " + node.GetComponent<NodeScript>().nodeValue);

    //         arrayIndex += 1;
            
    //     }

    //     // Debug.Log("gNodeArray Update Complete");
    // }







    // *---------------------------------------- DEBUG METHODS ----------------------------------------
                                          //* Called in Update Method 
    
    // Debug Value Setters
    public void DebugControls()
    {
        if(Input.GetKeyDown(KeyCode.I)) { ArrayPositionBlackSheepSetter(); }
        if(Input.GetKeyDown(KeyCode.U)) { ArrayPositionWhiteSheepSetter(); }
        if(Input.GetKeyDown(KeyCode.Y)) { ArrayPositionEmptySheepSetter(); }

        if(Input.GetKeyDown(KeyCode.P)) { DisplayArray(); }
        
        if(Input.GetKeyDown(KeyCode.G)) { SheepValueDisplayDebug(); }
        if(Input.GetKeyDown(KeyCode.H)) { NodeValueDisplayDebug(); }

    }


    public void NodeValueDisplayDebug()
    {
        string rowValues = "\n NodeValues \n";    
        
        for(int i = 0; i < arrayColumnLength; i ++){
            for(int j = 0; j < arrayRowLength; j ++){
                rowValues += gNodeArray[i,j].GetComponent<NodeScript>().nodeValue + "  ";   
            }
            rowValues += "\n";
        }
        Debug.Log(rowValues);
    }

    public void SheepValueDisplayDebug()
    {
        string rowValues = "\n SheepValues \n";    
        
        for(int i = 0; i < arrayColumnLength; i ++){
            for(int j = 0; j < arrayRowLength; j ++){
                rowValues += gNodeArray[i,j].GetComponent<NodeScript>().sheepValue + "  ";   
            }
            rowValues += "\n";
        }
        Debug.Log(rowValues);
    }


    public void ArrayPositionBlackSheepSetter()      // Displays Array based on nodeValues
    {
        for(int i = 0; i < arrayColumnLength; i++)         // Assigns positions to each gNode in gNodeArray
        {
            for(int j = 0; j < arrayRowLength; j++)
            {
                GameObject currentNode = gNodeArray[i,j];
                NodeScript currentNodeScript = currentNode.GetComponent<NodeScript>();
                
                currentNodeScript.BlackSheepSetter();           // Resets nodeValue of all Nodes to 4 before Setting

                currentNode.GetComponent<NodeScript>().SetGrassTileDisplayLoop();
            }
        }
    }

    public void ArrayPositionWhiteSheepSetter()      // Displays Array based on nodeValues
    {
        for(int i = 0; i < arrayColumnLength; i++)         // Assigns positions to each gNode in gNodeArray
        {
            for(int j = 0; j < arrayRowLength; j++)
            {
                GameObject currentNode = gNodeArray[i,j];
                NodeScript currentNodeScript = currentNode.GetComponent<NodeScript>();
                
                currentNodeScript.WhiteSheepSetter();           // Resets nodeValue of all Nodes to 4 before Setting

                currentNode.GetComponent<NodeScript>().SetGrassTileDisplayLoop();
            }
        }
    }

    public void ArrayPositionEmptySheepSetter()      // Displays Array based on nodeValues
    {
        for(int i = 0; i < arrayColumnLength; i++)         // Assigns positions to each gNode in gNodeArray
        {
            for(int j = 0; j < arrayRowLength; j++)
            {
                GameObject currentNode = gNodeArray[i,j];
                NodeScript currentNodeScript = currentNode.GetComponent<NodeScript>();
                
                currentNodeScript.libertyValue = currentNodeScript.libertyValueList[1];     // Set all libertyValue to 1 (Empty sheep object)

                currentNodeScript.EmptySheepSetter();           // Resets nodeValue of all Nodes to 4 before Setting

                List<int> nodeValueMap = NodeValueMapper();
                NodeValueUpdater(nodeValueMap);
                currentNode.GetComponent<NodeScript>().SetGrassTileDisplayLoop();
            }
        }
    }

    public void DisplayArray(){                      // Debugs Array Nodes in Console
        for(int i = 0; i < arrayColumnLength; i ++){         // Assigns positions to each gNode in gNodeArray
            for(int j = 0; j < arrayRowLength; j ++){
                Debug.Log(gNodeArray[i,j].name);
            }
        }
    }

    // Debug for displaying NodeValueMap - Disabled
    public void NodeValueMapDebugDisplayValue(List<int> nodeValueMap)
    {
        int debugCounter = 0;
        foreach(int mapValue in nodeValueMap)
        {
            Debug.Log("nodeValueMapValue[" + debugCounter + "] is " + mapValue);
            debugCounter += 1;
        }
    }
}
    
