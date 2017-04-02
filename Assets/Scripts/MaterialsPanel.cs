using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaterialsPanel : MonoBehaviour {

	public BuildingArea BuildingArea;
	public WallMaterial[] WallMaterials;


	private Image[] images;
	void Start()
	{
		images = new Image[WallMaterials.Length];
		for (int i = 0; i < WallMaterials.Length; i++) {
			GameObject obj = new GameObject ("image" + i.ToString ());
			obj.AddComponent<CanvasRenderer> ();
			int currentMaterialID = i;
			images [i] = obj.AddComponent<Image> ();
			images [i].sprite = WallMaterials [i].MaterialImage;
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
		BuildingArea.SetSelectedWallFaceMaterials (WallMaterials [i].InnerFaceMaterial, WallMaterials [i].OuterFaceMaterial, WallMaterials [i].SideFaceMaterial);
	}



}
