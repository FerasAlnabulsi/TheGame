using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public enum WallFaceType
{
	Outer,
	Inner,
	DoorSide
};

public class WallFace
{
	public WallFace(Vector3 a, Vector3 b, float upoffset, float height, Material wallMaterial, Material wireframeMaterial, Material selectedWallMaterial, Line relatedSegment){
		_a = a;
		_b = b;
		_upOffset = upoffset;
		_height = height;

		wireframeLines = new Line[6];
		for (int i = 0; i < wireframeLines.Length; i++) {
			wireframeLines [i] = new Line (new List<Vector3> () { a, b }, 0, 1, 0, wireframeMaterial, null, null, null);
		}
		WireframeMaterial = wireframeMaterial;
		update ();
		WallMaterial = wallMaterial;
		gameObject.AddComponent<MeshCollider> ();
		Wireframe = false;
		SelectedMaterial = selectedWallMaterial;
		RelatedLine = relatedSegment;
	}


	public WallFaceType WallFaceType;

	public void Destroy()
	{
		GameObject.Destroy (gameObject.GetComponent<MeshFilter> ());
		GameObject.Destroy (gameObject.GetComponent<MeshRenderer> ());
		GameObject.Destroy (gameObject.GetComponent<MeshCollider> ());
		GameObject.DestroyImmediate (gameObject);
		for (int i = 0; i < wireframeLines.Length; i++) {
			wireframeLines [i].Destroy ();
		}
	}

	public bool IsMouseOver()
	{
		RaycastHit rh;
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		Collider coll = gameObject.GetComponent<MeshCollider> ();
		return coll.Raycast (ray, out rh, float.MaxValue);
	}

	public bool IsMouseOver(out float dst)
	{
		RaycastHit rh;
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		Collider coll = gameObject.GetComponent<MeshCollider> ();
		bool b = coll.Raycast (ray, out rh, float.MaxValue);
		dst = rh.distance;
		return b;
	}

	public Line RelatedLine;

	Material _selectedMaterial;
	public Material SelectedMaterial
	{
		get{
			return _selectedMaterial;
		}
		set{
			if (_selectedMaterial == wireframeLines [0].LineMaterial) {
				for (int i = 0; i < wireframeLines.Length; i++) {
					wireframeLines [i].LineMaterial = value;
				}
			}
			_selectedMaterial = value;
		}
	}

	Material _wireframeMaterial;
	public Material WireframeMaterial
	{
		get{
			return _wireframeMaterial;
		}
		set{
			if (_wireframeMaterial == wireframeLines [0].LineMaterial) {
				for (int i = 0; i < wireframeLines.Length; i++) {
					wireframeLines [i].LineMaterial = value;
				}
			}
			_wireframeMaterial = value;
		}
	}

	public Material WallMaterial
	{
		get{
			if (gameObject == null)
				return null;
			return mr.material;
		}

		set{
			mr.material = value;
		}
	}

	Mesh WallMesh;
	MeshRenderer mr;
	Line[] wireframeLines;

	Vector3 _a;
	Vector3 _b;
	public Vector3 a {
		get { 
			return _a; 
		}
		set {
			_a = value;
			update ();
		}
	}
	public Vector3 b {
		get { 
			return _b; 
		}
		set {
			_b = value;
			update ();
		}
	}

	float _height = 5;
	public float Height
	{
		get {
			return _height;
		}
		set {
			_height = value;
			update ();
		}
	}

	float _upOffset = 0;
	public float UpOffset
	{
		get {
			return _upOffset;
		}
		set {
			_upOffset = value;
			update ();
		}
	}

	public GameObject gameObject = null;

	bool selected;
	public bool Selected
	{
		get{
			return selected;
		}
		set{
			selected = value;
			if (selected) {
				for (int i = 0; i < wireframeLines.Length; i++) {
					wireframeLines [i].LineMaterial = SelectedMaterial;
					wireframeLines [i].Enabled = true;
				}
			} else {
				for (int i = 0; i < wireframeLines.Length; i++) {
					wireframeLines [i].LineMaterial = WireframeMaterial;
					wireframeLines [i].Enabled = wireframe;
				}
			}
		}
	}

	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="WallFace"/> is solid.
	/// </summary>
	/// <value><c>true</c> if solid; otherwise, <c>false</c>.</value>
	public bool Solid
	{
		get{
			return mr.enabled;
		}
		set{
			mr.enabled = value;
		}
	}

	bool wireframe;
	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="WallFace"/> is wireframe.
	/// </summary>
	/// <value><c>true</c> if wireframe; otherwise, <c>false</c>.</value>
	public bool Wireframe
	{
		get{
			return wireframe;
		}
		set{
			wireframe = value;
			for (int i = 0; i < wireframeLines.Length; i++) {
				wireframeLines [i].Enabled = value;
			}
		}
	}

	/// <summary>
	/// Gets or sets the parent.
	/// </summary>
	/// <value>The parent.</value>
	public Transform Parent
	{
		get {
			return gameObject.transform.parent;
		}
		set {
			for (int i = 0; i < wireframeLines.Length; i++) {
				wireframeLines [i].Parent = value;
			}
			gameObject.transform.parent = value;
		}
	}


	public bool IsFacingCamera
	{
		get {
			Plane p = new Plane (a, b, a + Vector3.up);
			return p.GetDistanceToPoint (Camera.main.transform.position) > 0;
		}
	}

	private void update()
	{
		WallMesh = new Mesh ();
		WallMesh.vertices = new Vector3[] { _a + Vector3.up * UpOffset, _b + Vector3.up * UpOffset, _b + Vector3.up * _height + Vector3.up * UpOffset, _a + Vector3.up * _height + Vector3.up * UpOffset };
		WallMesh.uv = new Vector2[] { Vector2.zero, new Vector2(Vector3.Distance(_a, _b), 0), new Vector2(Vector3.Distance(_a, _b), _height), new Vector2(0, _height) };
		WallMesh.triangles = new int[] { 0, 1, 2, 2, 3, 0 };
		WallMesh.RecalculateNormals ();

		if (gameObject != null) {
			GameObject.Destroy (mr);
			GameObject.DestroyImmediate (gameObject);
		}
		gameObject = new GameObject ("wall");
		gameObject.AddComponent<MeshFilter> ().mesh = WallMesh;
		mr = gameObject.AddComponent<MeshRenderer> ();
		mr.material = WallMaterial;

		wireframeLines [0].a = _a;
		wireframeLines [0].b = _b;
		wireframeLines [1].a = _a + Vector3.up * _height;
		wireframeLines [1].b = _b + Vector3.up * _height;
		wireframeLines [2].a = _a;
		wireframeLines [2].b = _a + Vector3.up * _height;
		wireframeLines [3].a = _b;
		wireframeLines [3].b = _b + Vector3.up * _height;

	}
}