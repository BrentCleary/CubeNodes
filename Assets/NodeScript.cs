using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class NodeScript : MonoBehaviour
{

    private int nodeValue;
    private List<int> nodeValueList = new List<int> { 0, 1, 2, 3, 4 };   // Node Values when not occupied
    
    private int libertyValue;
    private List<int> libertyValueList = new List<int> { 0, 1 };          //  LibertyValue{ 1 , 0 }
    
    private bool placeAbleBool;
    private List<bool> placeAbleValueList = new List<bool> { false, true };   // is node placeable for current player
    
    public List<GameObject> GrassTileList = new List<GameObject> {};
    
    private List<string> nodeContainsList = new List<string> { "empty", "sheepBlack", "sheepWhite" }; // List displaying current Node GameObject
    private List<string> transitionStatesList = new List<string> { "beingCaptured" };             // States for transition

    


    // TEMP VARS FOR TEST
    public bool settingState;



    // Start is called before the first frame update
    void Start()
    {
        nodeValue = 4;
        libertyValue = 1;
        placeAbleBool = true;

        foreach(GameObject tile in GrassTileList)
        {
            Debug.Log("GameObject is " + tile);
        }
    }

    // Update is called once per frame
    void Update()
    {
        NodeValueSetter();
        SetGrassTileDisplayLoop();
    }

    // Test Method for Node Value Setting
    public void NodeValueSetter()
    {
        if(Input.GetKeyDown(KeyCode.Keypad0))  // Press 0 on Keypad to set nodeValue to 0
        {
            settingState = true;
            Debug.Log("Button Pressed = 0 and settingState = " + settingState);
            nodeValue = nodeValueList[0];
        }

        if(Input.GetKeyDown(KeyCode.Keypad1))  // Press 1 on Keypad to set nodeValue to 1
        {
            settingState = true;
            nodeValue = nodeValueList[1];
            Debug.Log("Button Pressed = 1");
        }

        if(Input.GetKeyDown(KeyCode.Keypad2))  // Press 2 on Keypad to set nodeValue to 2
        {
            settingState = true;
            nodeValue = nodeValueList[2];
            Debug.Log("Button Pressed = 2");
        }

        if(Input.GetKeyDown(KeyCode.Keypad3))  // Press 3 on Keypad to set nodeValue to 3
        {
            settingState = true;
            nodeValue = nodeValueList[3];
            Debug.Log("Button Pressed = 3");
        }

        if(Input.GetKeyDown(KeyCode.Keypad4))  // Press 4 on Keypad to set nodeValue to 4
        {
            settingState = true;
            nodeValue = nodeValueList[4];
            Debug.Log("Button Pressed = 4");
        }

    }

    public void SetGrassTileDisplayLoop()
    {
        bool isActive = true;                   // Sets initialize bool

        if(settingState)
        {
            for (int i = nodeValue; i >= 0; i--)    // Sets all tiles from nodeValue and lower true
            {
                GrassTileList[i].GetComponent<MeshRenderer>().enabled = isActive;
            }
            for (int i = nodeValueList.Count- 1; i > nodeValue; i--)   // Sets all tiles from nodeValue and higher false
            {
                GrassTileList[i].GetComponent<MeshRenderer>().enabled = !isActive;
            }
        }

        settingState = false;

    }

    public void SetGrassTileDisplay()
    {
        bool isActive = true;

        if(nodeValue == nodeValueList[0] && settingState)               // Set Value for 0
        {
            GrassTileList[0].GetComponent<MeshRenderer>().enabled = isActive;
            GrassTileList[1].GetComponent<MeshRenderer>().enabled = !isActive;
            GrassTileList[2].GetComponent<MeshRenderer>().enabled = !isActive;
            GrassTileList[3].GetComponent<MeshRenderer>().enabled = !isActive;
            GrassTileList[4].GetComponent<MeshRenderer>().enabled = !isActive;

            settingState = false;
            Debug.Log("SetGrassTile 0 Triggered and settingState = " + settingState);
        }

        
        if(nodeValue == nodeValueList[1] && settingState)               // Set Value for 1
        {
            GrassTileList[0].GetComponent<MeshRenderer>().enabled = isActive;
            GrassTileList[1].GetComponent<MeshRenderer>().enabled = isActive;
            GrassTileList[2].GetComponent<MeshRenderer>().enabled = !isActive;
            GrassTileList[3].GetComponent<MeshRenderer>().enabled = !isActive;
            GrassTileList[4].GetComponent<MeshRenderer>().enabled = !isActive;
            
            settingState = false;
            Debug.Log("SetGrassTile 1 Triggered and settingState = " + settingState);
        }

        if(nodeValue == nodeValueList[2] && settingState)               // Set Value for 2
        {
            GrassTileList[0].GetComponent<MeshRenderer>().enabled = isActive;
            GrassTileList[1].GetComponent<MeshRenderer>().enabled = isActive;
            GrassTileList[2].GetComponent<MeshRenderer>().enabled = isActive;
            GrassTileList[3].GetComponent<MeshRenderer>().enabled = !isActive;
            GrassTileList[4].GetComponent<MeshRenderer>().enabled = !isActive;
            
            settingState = false;
            Debug.Log("SetGrassTile 2 Triggered and settingState = " + settingState);
        }

        if(nodeValue == nodeValueList[3] && settingState)               // Set Value for 3
        {
            GrassTileList[0].GetComponent<MeshRenderer>().enabled = isActive;
            GrassTileList[1].GetComponent<MeshRenderer>().enabled = isActive;
            GrassTileList[2].GetComponent<MeshRenderer>().enabled = isActive;
            GrassTileList[3].GetComponent<MeshRenderer>().enabled = isActive;
            GrassTileList[4].GetComponent<MeshRenderer>().enabled = !isActive;
            
            settingState = false;
            Debug.Log("SetGrassTile 3 Triggered and settingState = " + settingState);
        }

        if(nodeValue == nodeValueList[4] && settingState)               // Set Value for 3
        {
            GrassTileList[0].GetComponent<MeshRenderer>().enabled = isActive;
            GrassTileList[1].GetComponent<MeshRenderer>().enabled = isActive;
            GrassTileList[2].GetComponent<MeshRenderer>().enabled = isActive;
            GrassTileList[3].GetComponent<MeshRenderer>().enabled = isActive;
            GrassTileList[4].GetComponent<MeshRenderer>().enabled = isActive;
            
            settingState = false;
            Debug.Log("SetGrassTile 3 Triggered and settingState = " + settingState);
        }

        settingState = true;
    }

}
