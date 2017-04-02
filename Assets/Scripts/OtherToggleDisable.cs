using System.Collections;

using System.Collections.Generic;

using UnityEngine.UI;

using UnityEngine;


public class OtherToggleDisable : MonoBehaviour
{
    public void DisableAllByTogglesInside()
    {
        Toggle[] toggles = this.GetComponentsInChildren<Toggle>(true);
        for (int i = 0; i < toggles.Length; i++)
        {
            toggles[i].isOn = false;
        }
    }
}