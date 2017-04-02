using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class WindowMaterialPanel : MonoBehaviour {


	public BuildingArea BuildingArea;
	public WindowMaterial[] WindowMaterials;


	private Image[] images;

	void Start () {
		images = new Image[WindowMaterials.Length];
		for (int i = 0; i < WindowMaterials.Length; i++) {
			GameObject obj = new GameObject ("image" + i.ToString ());
			obj.AddComponent<CanvasRenderer> ();
			int currentMaterialID = i;
			images [i] = obj.AddComponent<Image> ();
			images [i].sprite = WindowMaterials [i].WindowMaterialImage;
			obj.AddComponent<Button> ().onClick.AddListener (new UnityEngine.Events.UnityAction (delegate() {
				materialClicked(currentMaterialID);
			}));
			obj.AddComponent<LayoutElement> ();
			obj.transform.parent = transform;
			obj.transform.localScale = Vector3.one;
			obj.transform.localRotation = Quaternion.identity;
			obj.transform.localPosition = new Vector3 (obj.transform.localPosition.x, obj.transform.localPosition.y, 0);
		}
	}

	void materialClicked(int i)
	{
		BuildingArea.SetWindowMaterials(WindowMaterials[i].Model);
		//BuildingArea.SetSelectedWallFaceMaterials (WallMaterials [i].InnerFaceMaterial, WallMaterials [i].OuterFaceMaterial, WallMaterials [i].SideFaceMaterial);
	}
}
