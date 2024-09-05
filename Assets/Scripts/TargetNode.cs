using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEditor.SceneManagement;
using UnityEngine;

public class TargetNode : MonoBehaviour
{
    //* ---------------------------------------- PROPERTIES ----------------------------------------
    public GameObject grassContainer;
    public GameObject parentNode;
    public GameObject nodeArray;

    public Material selectionMaterial;
    private List<Renderer> tileRendererList;
    public List<Material> tileMaterialList;

    private NodeScript parentNodeScript;
    private BoardGenerator boardGeneratorScript;
    private NodeGroupManager nodeGroupManagerScript;
    // private NodeArrayScript nodeArrayScript; // Replaced by boardGeneratorScript

    public bool nodeSelected = false;





    //* ---------------------------------------- START AND UPDATE METHODS ----------------------------------------
                                 //* Assigns Parent Object/Parent Scripts and GrassTileList 
                                          //* Calls Placement Method on Nodes 
    // Start is called before the first frame update
    void Start()
    {

        grassContainer = gameObject.transform.parent.gameObject;

        // node reference - parent reference
        parentNode = grassContainer.transform.parent.gameObject;
        parentNodeScript = parentNode.GetComponent<NodeScript>();
        
        // board array reference - highest parent
        nodeArray = parentNode.transform.parent.gameObject;
        boardGeneratorScript = nodeArray.GetComponent<BoardGenerator>();

        // get NodeGroupManagerScript
        nodeGroupManagerScript = nodeArray.GetComponent<NodeGroupManager>();

        tileRendererList = grassContainer.GetComponentsInChildren<Renderer>().ToList();
        
        foreach(Renderer renderer in tileRendererList) {
            tileMaterialList.Add(renderer.material);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        PlaceBlackSheepMethod();
        PlaceWhiteSheepMethod();
    }





    //* ---------------------------------------- ON MOUSE ENTER/EXIT METHODS ----------------------------------------
                            //* Highlights/Resets selected Nodes Color by changing GrassTiles materials 
    private void OnMouseEnter()
    {
        nodeSelected = true;

        foreach(Renderer renderer in tileRendererList) {
            renderer.material = selectionMaterial;
        }

        // DEBUGS disabled 09/01/2024 
        // Debug.Log(parentNode.name);
        // Debug.Log("OnMouseEnter Activated on " + gameObject.name);
        
        // Material Debuggeer for checking material display - Disabled 08222024.2059 
        // foreach(Material material in tileMaterialList)
        // {
        //     Debug.Log($"{material}");
        // }
    }

    private void OnMouseExit()
    {
        nodeSelected = false;
        int colorCounter = 0;

        foreach(Renderer renderer in tileRendererList) {
            renderer.material = tileMaterialList[colorCounter];
            colorCounter++;
        }
    }




    //* ---------------------------------------- PLACE SHEEP METHODS ----------------------------------------
                    //* Sets Sheep on selected Node and calls BoardGeneratorScript to reset display
                                //* Calls BoardGeneratorScript and NodeScript 
    public void PlaceBlackSheepMethod()
    {
        // Check if the left mouse button was clicked
        if (nodeSelected && parentNodeScript.placeAbleBool == true && Input.GetKeyDown(KeyCode.Mouse0))
        {
            parentNodeScript.BlackSheepSetter();
            parentNodeScript.SetGrassTileDisplayLoop();

            List<int> nodeValueMap = boardGeneratorScript.NodeValueMapper();
            boardGeneratorScript.NodeValueUpdater(nodeValueMap);

            AssignSheepToGroups();

            nodeGroupManagerScript.CalculateGroupLiberties();


            // BoardGenerator Script
            // List<int> newNodeMap = boardGeneratorScript.NodeValueMapper();
            // newNodeMap = boardGeneratorScript.NodeValueMapIndexer(newNodeMap);
            // boardGeneratorScript.NodeValueUpdater(newNodeMap);
            // Debug.Log("Black Sheep Set");
        }
    }

    public void PlaceWhiteSheepMethod()
    {
        // Check if the right mouse button was clicked
        if (nodeSelected && Input.GetKeyDown(KeyCode.Mouse1))
        {
            parentNodeScript.WhiteSheepSetter();
            parentNodeScript.SetGrassTileDisplayLoop();

            // BoardGenerator Script
            List<int> nodeValueMap = boardGeneratorScript.NodeValueMapper();
            boardGeneratorScript.NodeValueUpdater(nodeValueMap);

            // Debug.Log("White Sheep Set!");
        }
    }

    public void PlaceEmptySheepMethod()
    {
        // Check if the middle mouse button was clicked
        if (nodeSelected && Input.GetKeyDown(KeyCode.Mouse2))
        {
            parentNodeScript.EmptySheepSetter();
            parentNodeScript.SetGrassTileDisplayLoop();

            List<int> nodeValueMap = boardGeneratorScript.NodeValueMapper();
            boardGeneratorScript.NodeValueUpdater(nodeValueMap);

            Debug.Log("Empty Sheep Set!");
        }
    }


    public void RemoveDeadGroupsMethod()
    {

    }


    //* ---------------------------------------- ON MOUSE ENTER/EXIT METHODS ----------------------------------------
                            //* Highlights/Resets selected Nodes Color by changing GrassTiles materials 

    public void AssignSheepToGroups()
    {

        // Create new NodeGroup, assign groupID
        parentNodeScript.groupID = nodeGroupManagerScript.CreateNewNodeGroup(parentNode);
        int parentNodeGroupID = parentNodeScript.groupID;
        
        // Adjacent Node Scripts
        NodeScript leftNodeScript = parentNodeScript.leftNodeScript;
        NodeScript rightNodeScript = parentNodeScript.rightNodeScript;
        NodeScript bottomNodeScript = parentNodeScript.bottomNodeScript;
        NodeScript topNodeScript = parentNodeScript.topNodeScript;


        //* FIRST CHECK OCCURS ON LEFT NODE
        // Left Node Found
        if(leftNodeScript != null && leftNodeScript.sheepValue == parentNodeScript.sheepValue)
        {
            int leftNodeGroupID = leftNodeScript.groupID;

            List<GameObject> nodeGroup = nodeGroupManagerScript.JoinNodeGroups(parentNodeGroupID, leftNodeGroupID);

            foreach(GameObject node in nodeGroup)
            {
                node.GetComponent<NodeScript>().groupID = parentNodeGroupID;
            }

            nodeGroupManagerScript.DeleteNodeGroup(leftNodeGroupID);

        }

        // Right Node Found
        if(rightNodeScript != null && rightNodeScript.sheepValue == parentNodeScript.sheepValue)
        {
            int rightNodeGroupID = rightNodeScript.groupID;

            if(rightNodeGroupID != parentNodeGroupID)
            {
                List<GameObject> nodeGroup = nodeGroupManagerScript.JoinNodeGroups(parentNodeGroupID, rightNodeGroupID);

                foreach(GameObject node in nodeGroup)
                {
                    Debug.Log("parentNodeID is " + parentNodeGroupID);
                    Debug.Log("prevNodeID's are " + node.GetComponent<NodeScript>().groupID);
                    node.GetComponent<NodeScript>().groupID = parentNodeGroupID;
                    Debug.Log("newNodeID's are " + node.GetComponent<NodeScript>().groupID);
                }

                nodeGroupManagerScript.DeleteNodeGroup(rightNodeGroupID);

                Debug.Log("New Node added to GroupID: " + rightNodeScript.groupID);
            }
        }

        // Bottom Node Found
        if(bottomNodeScript != null && bottomNodeScript.sheepValue == parentNodeScript.sheepValue)
        {
            int bottomNodeGroupID = bottomNodeScript.groupID;

            if(bottomNodeGroupID != parentNodeGroupID)
            {
                List<GameObject> nodeGroup = nodeGroupManagerScript.JoinNodeGroups(parentNodeGroupID, bottomNodeGroupID);

                foreach(GameObject node in nodeGroup)
                {
                    Debug.Log("parentNodeID is " + parentNodeGroupID);
                    Debug.Log("prevNodeID's are " + node.GetComponent<NodeScript>().groupID);
                    node.GetComponent<NodeScript>().groupID = parentNodeGroupID;
                    Debug.Log("newNodeID's are " + node.GetComponent<NodeScript>().groupID);
                }

                nodeGroupManagerScript.DeleteNodeGroup(bottomNodeGroupID);

                Debug.Log("New Node added to GroupID: " + bottomNodeScript.groupID);
            }
        }

        // Top Node Found
        if(topNodeScript != null && topNodeScript.sheepValue == parentNodeScript.sheepValue)
        {
            int topNodeGroupID = topNodeScript.groupID;
            
            if(topNodeGroupID != parentNodeGroupID)
            {
                List<GameObject> nodeGroup = nodeGroupManagerScript.JoinNodeGroups(parentNodeGroupID, topNodeGroupID);

                foreach(GameObject node in nodeGroup)
                {
                    Debug.Log("parentNodeID is " + parentNodeGroupID);
                    Debug.Log("prevNodeID's are " + node.GetComponent<NodeScript>().groupID);
                    node.GetComponent<NodeScript>().groupID = parentNodeGroupID;
                    Debug.Log("newNodeID's are " + node.GetComponent<NodeScript>().groupID);
                }

                nodeGroupManagerScript.DeleteNodeGroup(topNodeGroupID);

                Debug.Log("New Node added to GroupID: " + topNodeScript.groupID);
            }
        }
    }

    // Possible Simplification of AssignSheep Method to be refactored later
    // public void RefactoredAssignSheepMethod()
    // {
    //     // Adjacent Node Scripts
    //     List<NodeScript> NodeScriptList = new List<NodeScript>();

    //     NodeScript leftNodeScript = parentNodeScript.leftNodeScript;
    //     NodeScript rightNodeScript = parentNodeScript.rightNodeScript;
    //     NodeScript bottomNodeScript = parentNodeScript.bottomNodeScript;
    //     NodeScript topNodeScript = parentNodeScript.topNodeScript;

    //     NodeScriptList.Add(leftNodeScript);
    //     NodeScriptList.Add(rightNodeScript);
    //     NodeScriptList.Add(bottomNodeScript);
    //     NodeScriptList.Add(topNodeScript);

    //     foreach(NodeScript nodeScript in NodeScriptList)
    //     {
    //         // Top Node Found
    //         if(nodeScript != null && nodeScript.sheepValue == parentNodeScript.sheepValue)
    //         {
    //             int nodeGroupID = nodeScript.groupID;
    //             int parentNodeGroupID = parentNodeScript.groupID;

    //             List<GameObject> nodeGroup = nodeGroupManagerScript.JoinNodeGroups(parentNodeGroupID, nodeGroupID);

    //             foreach(GameObject node in nodeGroup)
    //             {
    //                 Debug.Log("parentNodeID is " + parentNodeGroupID);
    //                 Debug.Log("prevNodeID's are " + node.GetComponent<NodeScript>().groupID);
    //                 node.GetComponent<NodeScript>().groupID = parentNodeGroupID;
    //                 Debug.Log("newNodeID's are " + node.GetComponent<NodeScript>().groupID);
    //             }

    //             nodeGroupManagerScript.DeleteNodeGroup(nodeGroupID);

    //             Debug.Log("New Node added to GroupID: " + nodeScript.groupID);
    //         }
    //     }
    // }


    public void RemoveGroupsWithZeroLiberties()
    {

    }
}
