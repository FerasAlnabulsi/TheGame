using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

public class UiFloorsController : MonoBehaviour
{
    // Use this for initialization
    public GameObject prefab;
    public Transform contentPanel;
    private int Floornumber = 0;

	public EditableBuilding editableBuilding;

    void Start()
    {

    }




    public void SetButtonLast(GameObject btn)
    {
        btn.transform.SetAsLastSibling();
    }

    public void SetButtonFirst(GameObject btn)
    {
        btn.transform.SetAsFirstSibling();
    }

    public void AddNewButton()
    {
        GameObject newButton = Instantiate(prefab) as GameObject;
        Floornumber++;
		int floorID = Floornumber;

		newButton.GetComponentInChildren<Text>().text = "Floor " + Floornumber.ToString();

		newButton.GetComponent<Button>().onClick.AddListener (new UnityEngine.Events.UnityAction (delegate() {
			editableBuilding.SelectLayer(floorID);
		}));
        newButton.transform.SetParent(contentPanel.transform, false);
        newButton.transform.SetAsFirstSibling();
    }


}
