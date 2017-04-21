using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditableBuilding : MonoBehaviour {

	void Start () {
		
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

		Mesh lastCeil = layers [layers.Count - 1].GetCeil ();
		GameObject go = new GameObject ("Floor" + layers.Count.ToString ());
		go.AddComponent<MeshFilter> ().mesh = lastCeil;
		go.AddComponent<MeshCollider> ();
		go.AddComponent<MeshRenderer> ();


		System.Type type = layers[layers.Count - 1].GetType();
		Component copy = go.AddComponent(type);
		// Copied fields can be restricted with BindingFlags
		System.Reflection.FieldInfo[] fields = type.GetFields(); 
		foreach (System.Reflection.FieldInfo field in fields)
		{
			field.SetValue(copy, field.GetValue(layers[layers.Count - 1]));
		}

		go.transform.parent = this.transform;
	}

	public void SelectLayer(int id)
	{
	}



	void Update () {
		
	}
}
