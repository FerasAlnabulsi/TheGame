using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMaterialChanger : MonoBehaviour {
    
    //public  Material[] mats;
   // public Material gridMaterial;

    
    // Use this for initialization
    void Start () {

    }
    

    public void GridChanger()
    {
        
            GetComponent<Renderer>().enabled = !GetComponent<Renderer>().enabled;


    }


}
