using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public delegate void MovingHandler(GameObject sender, Vector3 oldPosition, Vector3 newPosition);

public enum DraggablePlane
{
	XY,
	YZ,
	XZ
};

public class Draggable : MonoBehaviour {

	DraggablePlane draggingPlane = DraggablePlane.XZ;
	public DraggablePlane DraggingPlane
	{
		get {
			return draggingPlane;
		}
		set {
			if (draggingPlane != value) {
				Vector3[] verts = new Vector3[((MeshCollider)_collider).sharedMesh.vertices.Length];
				((MeshCollider)_collider).sharedMesh.vertices.CopyTo (verts, 0);

				switch (draggingPlane) {
				case DraggablePlane.XY:
					for (int i = 0; i < verts.Length; i++) {
						float tmp = verts [i].z;
						verts [i].z = verts [i].y;
						verts [i].y = tmp;
					}
					break;
				case DraggablePlane.XZ:
					break;
				case DraggablePlane.YZ:
					for (int i = 0; i < verts.Length; i++) {
						float tmp = verts [i].x;
						verts [i].x = verts [i].y;
						verts [i].y = tmp;
					}
					break;
				}

				draggingPlane = value;


				switch (draggingPlane) {
				case DraggablePlane.XY:
					for (int i = 0; i < verts.Length; i++) {
						float tmp = verts [i].z;
						verts [i].z = verts [i].y;
						verts [i].y = tmp;
					}
					break;
				case DraggablePlane.XZ:
					break;
				case DraggablePlane.YZ:
					for (int i = 0; i < verts.Length; i++) {
						float tmp = verts [i].x;
						verts [i].x = verts [i].y;
						verts [i].y = tmp;
					}
					break;
				}
			}
		}
	}

	List<Vector3> allowedPoints;
	public void SetAllowedPoints (Vector3[] points)
	{
		allowedPoints = new List<Vector3> (points);
	}


	Vector3 saved;

	public bool IsDragging
	{ 
		get; 
		private set; 
	}

	public event MovingHandler StartMoving;
	public event MovingHandler Moving;
	public event MovingHandler EndMoving;

	public bool Enabled = true;


	Collider _collider = null;
	//Vector3 unsnappedPosition;

	Vector3 savedPosition;
	Vector3 startPosition;
	float distance;

	//Vector3 offsetToGrid;
	bool startedMoving = false;
	// Use this for initialization
	void Start () {
		_collider = GetComponent<Collider> ();
		if (_collider == null) {
			_collider = GetComponentInChildren<Collider> ();
		}
		if (_collider == null) {
			Debug.Log ("No collider found on this object");
		}
	}


	// Update is called once per frame
	void Update () {
		if (Enabled) {
			bool isMouseOver = false;
			Vector3 pos = Vector3.zero;
			float dst = 0;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit rh;
			if (Physics.Raycast (ray, out rh)) {
				if (rh.collider == _collider) {
					isMouseOver = true;
					pos = rh.point;
					dst = rh.distance;
				}
			}



			if (Input.GetMouseButtonDown (0)) {
				if (isMouseOver) {
					//unsnappedPosition = transform.position;
					//offsetToGrid = snapToGrid (unsnappedPosition) - unsnappedPosition;
					saved = transform.position;
					startPosition = snapToGrid (pos);
//					startPosition = pos;
					distance = dst;
					if (StartMoving != null)
						StartMoving (gameObject, startPosition, startPosition);
//					saved = startPosition;
					startedMoving = true;
				}
			} else if (Input.GetMouseButton (0) && startedMoving) {
				IsDragging = true;
				Vector3 mouseCurrentPosition = Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, distance));
				mouseCurrentPosition = snapToGrid (mouseCurrentPosition);
			
				Vector3 dif = mouseCurrentPosition - startPosition;
				saved += dif;
				transform.position = saved;
				startPosition = mouseCurrentPosition;
				
				//dif = snapToGrid (dif);


				//unsnappedPosition += dif;

//				Vector3 tmp = snapToGrid (unsnappedPosition);
//				if (!XEnabled)
//					tmp.x = startPosition.x;
//				if (!YEnabled)
//					tmp.y = startPosition.y;
//				if (!ZEnabled)
//					tmp.z = startPosition.z;
//				tmp += offsetToGrid;



				if (Moving != null && dif.sqrMagnitude > 0.00001f)
					Moving (gameObject, startPosition - dif, startPosition);
				

				//transform.position = snapToGrid (transform.position);
//				if (XEnabled)
//					startPosition.x = mouseCurrentPosition.x;
//				if (YEnabled)
//					startPosition.y = mouseCurrentPosition.y;
//				if (ZEnabled)
//					startPosition.z = mouseCurrentPosition.z;

			} else if (Input.GetMouseButtonUp (0) && startedMoving) {

//				if (FreezeX)
//					startPosition.x = saved.x;
//				if (FreezeY)
//					startPosition.y = saved.y;
//				if (FreezeZ)
//					startPosition.x = saved.z;


				startedMoving = false;
				IsDragging = false;
				if (EndMoving != null)
					EndMoving (gameObject, startPosition, startPosition);
			}



		}
	}

	Vector3 snapToGrid(Vector3 pos)
	{
		float dst = float.MaxValue;
		Vector3 output = pos;
		for (int i = 0; i < allowedPoints.Count; i++) {
			float det = (allowedPoints [i] - pos).sqrMagnitude;
			if (dst > det) {
				dst = det;
				output = allowedPoints [i];
			}
		}
		return output;
	}
}
