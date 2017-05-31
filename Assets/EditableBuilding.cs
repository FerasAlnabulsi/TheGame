using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditableBuilding : MonoBehaviour {

	public GameObject Grid;
	public ItemsManager itemsManager;
	public float CeilThickness = 0.2f;
	bool ceilFlag = false;
	void Start () {

//		slider.minValue = 0;
//		slider.maxValue = 0;

	}



	List<BuildingArea> GetLayers()
	{
		List<BuildingArea> layers = new List<BuildingArea> ();

		for (int i = 0; i < transform.childCount; i++)
		{
			BuildingArea ba = transform.GetChild (i).gameObject.GetComponent<BuildingArea> ();
			if (ba != null)
				layers.Add (ba);
		}

		//sort from down to up
		layers.Sort (delegate(BuildingArea x, BuildingArea y) {
			if (x.lines.Count == 0)
				return (1).CompareTo(0);
			else if (y.lines.Count == 0)
				return (0).CompareTo(1);
			
			return x.lines[0].a.y.CompareTo(y.lines[0].a.y);
		});

		return layers;
	}

	public void NewLayer()
	{
		// get upper ceil
		// make new object (building area)
		// copy lines
		// offset to up
		// disable old layer

		List<BuildingArea> layers = GetLayers ();


		if (layers [layers.Count - 1].lines.Count == 0)
			return;
		
		Mesh lastCeil = layers [layers.Count - 1].IsBasement ? layers [layers.Count - 1].GetOuterCeil () : layers [layers.Count - 1].GetCeil ();
		GameObject go;
		if (ceilFlag)
			go = new GameObject ("FloorCeil" + (layers.Count - 1).ToString ());
		else
			go = new GameObject ("Floor" + layers.Count.ToString ());
		


		go.AddComponent<MeshFilter> ().mesh = lastCeil;
		go.AddComponent<MeshCollider> ();
		go.AddComponent<MeshRenderer> ();
		BuildingArea newLayer = go.AddComponent<BuildingArea> ();


		if (layers.Count > 0)
			layers [layers.Count - 1].CopyToNewLayer (newLayer);


		go.transform.parent = this.transform;

		newLayer.lines.Clear ();
//		if (newLayer.lines.Count != 0) {
//			//go.transform.position += Vector3.up * layers [layers.Count - 1].lines [0].Height;
//			for (int i = 0; i < newLayer.lines [0].Vertices.Count; i++) {
//				newLayer.lines [0].Vertices [i] += Vector3.up * layers [layers.Count - 1].lines [0].Height;
//			}
//
//			for (int i = 0; i < newLayer.lines.Count; ++i) {
//				for (int j = 0; j < newLayer.lines [i].Doors.Count; ++j)
//					newLayer.lines [i].Doors [j].Update ();
//
//				for (int j = 0; j < newLayer.lines [i].Windows.Count; ++j)
//					newLayer.lines [i].Windows [j].Update ();
//			}
//		}

		if (layers [layers.Count - 1].IsBasement) {
			GameObject.Destroy (newLayer.Basement);

			newLayer.Basement = null;
			for (int i = 0; i < newLayer.lines.Count; ++i) {
				newLayer.lines [i].Destroy ();
			}
			GameObject.Destroy (newLayer.upperWallFace);
			newLayer.upperWallFace = null;
			newLayer.lines.Clear ();
			newLayer.IsBasement = false;
		}

		newLayer.isCeil = ceilFlag;

		if (ceilFlag) {
			Mesh mesh = GameObject.Instantiate (layers [0].gameObject.GetComponent<MeshCollider> ().sharedMesh);
			Vector3[] newVerts = new Vector3[mesh.vertices.Length];
			for (int i = 0; i < mesh.vertices.Length; i++) {
				newVerts[i] = mesh.vertices [i] + Vector3.up * (layers [layers.Count - 1].lines [0].a.y + layers [layers.Count - 1].Height);
			}
			mesh.vertices = newVerts;
			newLayer.GetComponent<MeshCollider> ().sharedMesh = mesh;
			newLayer.Height = CeilThickness;

		} else {
			for (int i = 0; i < newLayer.lines.Count; i++) {
				newLayer.lines [i].Height = layers[layers.Count - 2].lines[0].Height;
			}

		}

		newLayer.SetWorkingHeight (layers [layers.Count - 1].lines [0].a.y + layers [layers.Count - 1].Height);


		layers.Add (newLayer);
		newLayer.regeneratePath (true);






		//enable roof on last layer only
		for (int i = 1; i < layers.Count; i++) {
			layers [i].RoofEnabled = false;
		}
//		slider.minValue = 0;
//		slider.maxValue = layers.Count - 1;
//		slider.value = slider.maxValue;

		ceilFlag = !ceilFlag;

		SelectLayer (layers.Count - 1);
	}

	public void SelectLayer(int _id)
	{
		int id = (int)(_id + 0.5f);
		
		List<BuildingArea> layers = GetLayers ();

		itemsManager.BuildingArea = layers [id];

		for (int i = 0; i < layers.Count; i++) {
			layers [i].gameObject.SetActive (id >= i);
			layers [i].enabled = id == i;
			if (i == id) {

				Vector3 pos = Grid.transform.position;

				if (layers [i].lines.Count != 0)
					pos.y = layers [i].lines [0].a.y;
				else
					pos.y = layers [i].GetComponent<Collider> ().bounds.min.y;
				
				pos.y += 0.01f;
				Grid.transform.position = pos;
			}
		}
	}

	public void DeleteSelectedWall()
	{
		List<BuildingArea> layers = GetLayers ();

		for (int i = 0; i < layers.Count; i++) {
			if (layers [i].isActiveAndEnabled) {
				layers [i].DeleteSelectedWall ();
			}
		}
	}

	void Update () {
		
	}
}
















//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//
//public class EditableBuilding : MonoBehaviour {
//
//	public GameObject Grid;
//	public ItemsManager itemsManager;
//	public float CeilThickness = 0.2f;
//	void Start () {
//
//		//		slider.minValue = 0;
//		//		slider.maxValue = 0;
//
//	}
//
//	public int FloorsCount
//	{
//		get {
//			int tmp = GetLayers ().Count;
//			if (tmp <= 2)
//				return tmp;
//			else 
//				return tmp / 2 + 1 ;
//		}
//	}
//
//	List<BuildingArea> GetLayers()
//	{
//		List<BuildingArea> layers = new List<BuildingArea> ();
//
//		for (int i = 0; i < transform.childCount; i++)
//		{
//			BuildingArea ba = transform.GetChild (i).gameObject.GetComponent<BuildingArea> ();
//			if (ba != null)
//				layers.Add (ba);
//		}
//
//		//sort from down to up
//		layers.Sort (delegate(BuildingArea x, BuildingArea y) {
//			if (x.lines.Count == 0)
//				return (1).CompareTo(0);
//			else if (y.lines.Count == 0)
//				return (0).CompareTo(1);
//
//			return x.lines[0].a.y.CompareTo(y.lines[0].a.y);
//		});
//
//		return layers;
//	}
//
//	public void NewLayer()
//	{
//		// get upper ceil
//		// make new object (building area)
//		// copy lines
//		// offset to up
//		// disable old layer
//
//		List<BuildingArea> layers = GetLayers ();
//
//		for (int x = 0; x < 2; x++) {
//
//			if (layers [layers.Count - 1].lines.Count == 0)
//				break;
//
//			Mesh lastCeil = layers [layers.Count - 1].IsBasement ? layers [layers.Count - 1].GetOuterCeil () : layers [layers.Count - 1].GetCeil ();
//			GameObject go;
//			bool CeilFlag = false;
//			if (x == 1 || layers.Count == 1) {
//				if (layers.Count == 1) {
//					go = new GameObject ("Floor1");
//				} else {
//					go = new GameObject ("Floor" + ((layers.Count + 1) / 2).ToString ());
//				}
//			} else {
//				go = new GameObject (layers [layers.Count - 1].name + "CeilThickness");
//				CeilFlag = true;
//			}
//			go.AddComponent<MeshFilter> ().mesh = lastCeil;
//			go.AddComponent<MeshCollider> ();
//			go.AddComponent<MeshRenderer> ();
//			BuildingArea newLayer = go.AddComponent<BuildingArea> ();
//
//
//			if (layers.Count > 0)
//				layers [layers.Count - 1].CopyToNewLayer (newLayer);
//
//
//			go.transform.parent = this.transform;
//
//			if (newLayer.lines.Count != 0) {
//				//go.transform.position += Vector3.up * layers [layers.Count - 1].lines [0].Height;
//				for (int i = 0; i < newLayer.lines [0].Vertices.Count; i++) {
//					newLayer.lines [0].Vertices [i] += Vector3.up * layers [layers.Count - 1].lines [0].Height;
//				}
//
//				for (int i = 0; i < newLayer.lines.Count; ++i) {
//					for (int j = 0; j < newLayer.lines [i].Doors.Count; ++j)
//						newLayer.lines [i].Doors [j].Update ();
//
//					for (int j = 0; j < newLayer.lines [i].Windows.Count; ++j)
//						newLayer.lines [i].Windows [j].Update ();
//				}
//			}
//
//			if (layers [layers.Count - 1].IsBasement) {
//				GameObject.Destroy (newLayer.Basement);
//
//				newLayer.Basement = null;
//				for (int i = 0; i < newLayer.lines.Count; ++i) {
//					newLayer.lines [i].Destroy ();
//				}
//				GameObject.Destroy (newLayer.upperWallFace);
//				newLayer.upperWallFace = null;
//				newLayer.lines.Clear ();
//				newLayer.IsBasement = false;
//			}
//
//			if (CeilFlag) {
//				for (int i = 0; i < newLayer.lines.Count; i++) {
//					newLayer.lines [i].Height = CeilThickness;
//				}
//			} else {
//				for (int i = 0; i < newLayer.lines.Count; i++) {
//					newLayer.lines [i].Height = layers[layers.Count - 2].lines[0].Height;
//				}
//			}
//
//
//			layers.Add (newLayer);
//			newLayer.regeneratePath (true);
//
//
//		}
//
//
//
//		//enable roof on last layer only
//		for (int i = 1; i < layers.Count; i++) {
//			layers [i].RoofEnabled = i == layers.Count - 1;
//		}
//		//		slider.minValue = 0;
//		//		slider.maxValue = layers.Count - 1;
//		//		slider.value = slider.maxValue;
//		SelectLayer (FloorsCount - 1);
//	}
//
//	public void SelectLayer(int _id)
//	{
//		int id = _id;//(int)(_id + 0.5f);
//		if (id != 0)
//			id = id * 2 - 1;
//
//		List<BuildingArea> layers = GetLayers ();
//
//		itemsManager.BuildingArea = layers [id];
//
//		for (int i = 0; i < layers.Count; i++) {
//			layers [i].gameObject.SetActive (id >= i);
//			layers [i].enabled = id == i;
//			if (i == id) {
//
//				Vector3 pos = Grid.transform.position;
//
//				if (layers [i].lines.Count != 0)
//					pos.y = layers [i].lines [0].a.y;
//				else
//					pos.y = layers [i].GetComponent<Collider> ().bounds.min.y;
//
//				pos.y += 0.01f;
//				Grid.transform.position = pos;
//			}
//		}
//	}
//
//
//
//	void Update () {
//
//	}
//}
