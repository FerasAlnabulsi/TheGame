using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManeger : MonoBehaviour {

    public GameObject menu;
    // Use this for initialization
    void Awake()
    {
        menu.GetComponent<Fade>().isShowing = false;
        menu.GetComponent<Fade>().FadeOut();
             // Menu.gameObject.SetActive(false);
    }

    


}
