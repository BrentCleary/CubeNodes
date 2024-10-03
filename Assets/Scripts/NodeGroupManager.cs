using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Linq;
using System.Globalization;

public class NodeGroupManager : MonoBehaviour
{
    public BoardGenerator boardGeneratorScript;
    public NodeScript nodeScript;

    [System.Serializable]
    public class NodeGroup
    {
        public static int groupCounter = -1;
        
        public int GroupID {get; private set;}
        public int GroupLiberties;
        public List<GameObject> GroupNodeList = new List<GameObject>();

        // Constructor
        public NodeGroup() // Increment the counter and assign it to GroupID
        {
            groupCounter++;
            GroupID = groupCounter;
        }
    }

    public List<NodeGroup> AllGroupList = new List<NodeGroup>();
    
    // Start is called before the first frame update
    void Start()
    {
        boardGeneratorScript = gameObject.GetComponent<BoardGenerator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }





    //* ---------------------------------------- CreateNewNodeGroup  ----------------------------------------
                            // * Creates New NodeGroup - Called in TargetNode - Returns GroupID
    public int CreateNewNodeGroup(GameObject node)
    {
        NodeGroup newGroup = new NodeGroup();
        newGroup.GroupNodeList.Add(node);

        Add_To_AllGroupList(newGroup);

        int groupID = newGroup.GroupID;

        Debug.Log("NodeGM : CreateNewNodeGroup: Group ID: [ " + groupID + " ]");

        return groupID;
        
    }


    //* ---------------------------------------- JoinNodeGroups ----------------------------------------
    public List<GameObject> JoinNodeGroups(int newGroupID, int prevGroupID)     //* Adds all nodes from Previous Group to New Group by ID - Takes new/prev GroupID
    {
        NodeGroup prevNodeGroup = RetrieveNodeGroup(prevGroupID);
        NodeGroup newNodeGroup = RetrieveNodeGroup(newGroupID);

        foreach(GameObject node in prevNodeGroup.GroupNodeList) {
            newNodeGroup.GroupNodeList.Add(node);
        }
        Debug.Log("NodeGroupManager:JoinNodeGroups [ " + prevGroupID + " Nodes added to newGroupID " + newGroupID + " ]");
        return newNodeGroup.GroupNodeList;
    }


    //* ---------------------------------------- Add_To_AllGroupList ----------------------------------------
    public void Add_To_AllGroupList(NodeGroup group)                                            //* Updates All Group List
    {
        AllGroupList.Add(group);
    }

    //* ---------------------------------------- RetrieveNodeGroup ----------------------------------------
    public NodeGroup RetrieveNodeGroup(int groupID)                                    //* Returns NodeGroup by NodeScript.groupID 
    {
        NodeGroup nodeGroup = AllGroupList.Find(g => g.GroupID == groupID);
        return nodeGroup;
    }

    //* ---------------------------------------- DeleteNodeGroup -----------------------------------------
    public void DeleteNodeGroup(int prevGroupID)                            //* Clears previous group List and removes for AllGroupList 
    {
        NodeGroup groupToDelete = AllGroupList.Find(g => g.GroupID == prevGroupID);
        
        groupToDelete.GroupNodeList.Clear();
        AllGroupList.Remove(groupToDelete);
        
        Debug.Log("NodeGroup " + groupToDelete.GroupID + " cleared and deleted.");
    }

    //* ---------------------------------------- CalculateGroupLiberties ----------------------------------------
    public List<int> CalculateGroupLiberties()                                    //*  - Calculates Liberties of all NodeGroups
    {
        List<int> zeroLibertyGroupID = new List<int>();

        foreach(NodeGroup nodeGroup in AllGroupList)    // Loops over List of All Groups, Looks at adjacent nodes for each node in group
        {
            int totalGroupLiberties = 0;
            List<GameObject> currentGroupList = nodeGroup.GroupNodeList;
            List<GameObject> libertyNodes = new List<GameObject>();
            
            foreach(GameObject node in currentGroupList)    // Adds value of node to total liberties, adds node to list so it is not counted twice
            {
                NodeScript nodeScript = node.GetComponent<NodeScript>();

                if(nodeScript.leftNode != null){
                    if(!libertyNodes.Contains(nodeScript.leftNode))
                    {
                        libertyNodes.Add(nodeScript.leftNode);
                        totalGroupLiberties += nodeScript.leftNodeScript.libertyValue;
                    }
                }
                if(nodeScript.rightNode != null){
                    if(!libertyNodes.Contains(nodeScript.rightNode))
                    {
                        libertyNodes.Add(nodeScript.rightNode);
                        totalGroupLiberties += nodeScript.rightNodeScript.libertyValue;
                    }
                }
                if(nodeScript.bottomNode != null){
                    if(!libertyNodes.Contains(nodeScript.bottomNode))
                    {
                        libertyNodes.Add(nodeScript.bottomNode);
                        totalGroupLiberties += nodeScript.bottomNodeScript.libertyValue;
                    }
                }
                if(nodeScript.topNode != null){
                    if(!libertyNodes.Contains(nodeScript.topNode))
                    {
                        libertyNodes.Add(nodeScript.topNode);
                        totalGroupLiberties += nodeScript.topNodeScript.libertyValue;
                    }
                }
                // Debug.Log("node liberties are " + nodeScript.libertyValue);
            }
            
            nodeGroup.GroupLiberties = totalGroupLiberties;
            Debug.Log("currentGroup Liberties are " + totalGroupLiberties);
            Debug.Log("GroupLiberties property is " + nodeGroup.GroupLiberties);

            if(nodeGroup.GroupLiberties == 0)           // Returns a list of Groups with Liberties == 0 for deletion in other method
            {
                zeroLibertyGroupID.Add(nodeGroup.GroupID);
            }

            totalGroupLiberties = 0;
            libertyNodes.Clear();
        }
        return zeroLibertyGroupID;
    }


    public void UpdateZeroLibertyGroups(List<int> zeroLibertyGroupID)               // Receives the zeroLibertyGroupID list from CalculateGrouLiberties()
    {
        List<NodeGroup> zeroGroupList = new List<NodeGroup>();                      // Create a new list for sorting

        foreach(NodeGroup nodeGroup in AllGroupList)                                // Look through list of All Groups
        {
            if(zeroLibertyGroupID.Contains(nodeGroup.GroupID))                      // If the zeroList contains the ID of a Zero'd Node Group
            {
                zeroGroupList.Add(nodeGroup);                                       // Add it to the zeroGroupList for updating
            }

            foreach(NodeGroup zeroGroup in zeroGroupList)                           // Loop of new list of Zero liberty Groups
            {   
                Debug.Log("ZeroGroup Liberties are " + zeroGroup.GroupLiberties);
                if(zeroGroup.GroupNodeList.Count > 1)                               // If the group has more than 1 stone 
                {
                    List<GameObject> zeroList = zeroGroup.GroupNodeList;            // Get a list of the Nodes in the group

                    foreach(GameObject zeroNode in zeroList)                        // For each Node
                    {
                        Debug.Log("Setting Node " + zeroNode.name + " to empty");
                        NodeScript zeroScript = zeroNode.GetComponent<NodeScript>();    // Get the script of the node
                        
                        zeroScript.sheepValue = zeroScript.sheepValueList[0];       // Update the Sheep Value to 0 (Sheep is removed)
                        zeroScript.libertyValue = zeroScript.libertyValueList[1];   // Update the Liberty Value to 1 (It has an empty neighbor)
                        zeroScript.placeAbleBool = zeroScript.placeAbleValueList[1];   // Update the Liberty Value to 1 (It has an empty neighbor)
                        zeroScript.EmptySheepSetter();
                    }
                }
                // TODO LOGIC HERE FOR KO AND OTHER SINGLE NODE SITUATIONS          // Update this space for KO an other empty single space logic
            
            }
        }
        

    }


    //* ---------------------------------------- CalculateGroupLiberties ----------------------------------------
    public List<int> CalculateGroupLiberties____REFACTOR()                                    //*  - Calculates Liberties of all NodeGroups
    {
        List<int> zeroLibertyGroupID = new List<int>();

        foreach(NodeGroup nodeGroup in AllGroupList)    // Loops over List of All Groups, Looks at adjacent nodes for each node in group
        {
            int totalGroupLiberties = 0;
            List<GameObject> currentGroupList = nodeGroup.GroupNodeList;
            List<GameObject> libertyNodes = new List<GameObject>();
            
            foreach(GameObject node in currentGroupList)    // Adds value of node to total liberties, adds node to list so it is not counted twice
            {
                NodeScript nodeScript = node.GetComponent<NodeScript>();
                List<NodeScript> adjScriptList = nodeScript.adjNodeScriptList;
                
                if(nodeScript.leftNode != null){
                    if(!libertyNodes.Contains(nodeScript.leftNode))
                    {
                        libertyNodes.Add(nodeScript.leftNode);
                        totalGroupLiberties += nodeScript.leftNodeScript.libertyValue;
                    }
                }
                if(nodeScript.rightNode != null){
                    if(!libertyNodes.Contains(nodeScript.rightNode))
                    {
                        libertyNodes.Add(nodeScript.rightNode);
                        totalGroupLiberties += nodeScript.rightNodeScript.libertyValue;
                    }
                }
                if(nodeScript.bottomNode != null){
                    if(!libertyNodes.Contains(nodeScript.bottomNode))
                    {
                        libertyNodes.Add(nodeScript.bottomNode);
                        totalGroupLiberties += nodeScript.bottomNodeScript.libertyValue;
                    }
                }
                if(nodeScript.topNode != null){
                    if(!libertyNodes.Contains(nodeScript.topNode))
                    {
                        libertyNodes.Add(nodeScript.topNode);
                        totalGroupLiberties += nodeScript.topNodeScript.libertyValue;
                    }
                }
                // Debug.Log("node liberties are " + nodeScript.libertyValue);
            }
            
            nodeGroup.GroupLiberties = totalGroupLiberties;
            Debug.Log("currentGroup Liberties are " + totalGroupLiberties);
            Debug.Log("GroupLiberties property is " + nodeGroup.GroupLiberties);

            if(nodeGroup.GroupLiberties == 0)           // Returns a list of Groups with Liberties == 0 for deletion in other method
            {
                zeroLibertyGroupID.Add(nodeGroup.GroupID);
            }

            totalGroupLiberties = 0;
            libertyNodes.Clear();
        }
        return zeroLibertyGroupID;
    }


}
