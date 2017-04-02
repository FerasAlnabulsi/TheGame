using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

public class ItemsManager : MonoBehaviour
{
	public BuildingArea BuildingArea;
    public List<item> allObjectList;
    public GameObject MenuItem;
    public GameObject windwoPanel;
    public GameObject doorPanel;
    public GameObject wallPanel;
    public GameObject furniturePanel;
    public GameObject systemPanel;
    public GameObject roofPanel;
    // Use this for initialization

    void Start()
    {
        ToggleGroup tg = GetComponent<ToggleGroup>();
        for (int i = 0; i < allObjectList.Count; i++)
        {
            GameObject button = (GameObject)Instantiate(MenuItem);
			Toggle toggle = button.GetComponent<Toggle> ();

			Image buttonImage = System.Array.Find (button.GetComponentsInChildren<Image> (), delegate (Image img) {
				return img.name == "Image";
			});

			buttonImage.sprite = allObjectList [i].image;

			button.GetComponentInChildren<Text> (true).text = allObjectList [i].itemName;

            toggle.group = tg;
			int currentI = i;
			toggle.onValueChanged.AddListener (new UnityEngine.Events.UnityAction<bool> (delegate(bool arg0) {
				if (arg0)
					itemSelected(allObjectList[currentI]);
				else
					itemSelected(null);

			}));
            if (allObjectList[i].itemType == type.Window)
            {
                button.transform.SetParent(windwoPanel.transform);//Setting button parent
            }

            else if (allObjectList[i].itemType == type.Door)
            {
                button.transform.SetParent(doorPanel.transform);//Setting button parent
            }

            else if (allObjectList[i].itemType == type.Wall)
            {
                button.transform.SetParent(wallPanel.transform);//Setting button parent
            }

            else if (allObjectList[i].itemType == type.Furniture)
            {
                button.transform.SetParent(furniturePanel.transform);//Setting button parent
            }

            else if (allObjectList[i].itemType == type.Roof)
            {
                button.transform.SetParent(roofPanel.transform);//Setting button parent
            }

            else if (allObjectList[i].itemType == type.System)
            {
                button.transform.SetParent(systemPanel.transform);//Setting button parent
            }

            button.transform.localRotation = Quaternion.identity;
            button.transform.localPosition = new Vector3(100, -25, 0);
            button.transform.localScale = Vector3.one;
        }  
    }

	void itemSelected(item item)
	{
		BuildingArea.SelectedItem = item;
	}
}