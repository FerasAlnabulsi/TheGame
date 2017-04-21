using UnityEngine;
using System.Collections;

public delegate void MovingHandler(GameObject sender, Vector3 oldPosition, Vector3 newPosition);

public class Draggable : MonoBehaviour {

	public bool Enabled;

	public bool XEnabled;

	public bool YEnabled;

	public bool ZEnabled;

	public int XSnapDistance;

	public int YSnapDistance;

	public int ZSnapDistance;

	public bool IsDragging
	{ 
		get; 
		private set; 
	}

	public event MovingHandler StartMoving;
	public event MovingHandler Moving;
	public event MovingHandler EndMoving;




	Collider _collider = null;
	Vector3 unsnappedPosition;

	Vector3 startPosition;
	float distance;

	Vector3 offsetToGrid;
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
					unsnappedPosition = transform.position;
					offsetToGrid = snapToGrid (unsnappedPosition) - unsnappedPosition;
					//startPosition = snapToGrid (pos);
					startPosition = pos;
					distance = dst;
					if (StartMoving != null)
						StartMoving (gameObject, startPosition, startPosition);
					startedMoving = true;
				}
			} else if (Input.GetMouseButton (0) && startedMoving) {
				IsDragging = true;
				Vector3 mouseCurrentPosition = Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, distance));
				//mouseCurrentPosition = snapToGrid (mouseCurrentPosition);
			
				Vector3 dif = mouseCurrentPosition - startPosition;
		
				//dif = snapToGrid (dif);


				unsnappedPosition += dif;

				Vector3 tmp = snapToGrid (unsnappedPosition);
				if (!XEnabled)
					tmp.x = startPosition.x;
				if (!YEnabled)
					tmp.y = startPosition.y;
				if (!ZEnabled)
					tmp.z = startPosition.z;
				tmp += offsetToGrid;
				if (Moving != null && (tmp - transform.position).sqrMagnitude > 0.00001f)
					Moving (gameObject, transform.position, tmp);
				

				//transform.position = snapToGrid (transform.position);
				if (XEnabled)
					startPosition.x = mouseCurrentPosition.x;
				if (YEnabled)
					startPosition.y = mouseCurrentPosition.y;
				if (ZEnabled)
					startPosition.z = mouseCurrentPosition.z;

			} else if (Input.GetMouseButtonUp (0) && startedMoving) {

				startedMoving = false;
				IsDragging = false;
				if (EndMoving != null)
					EndMoving (gameObject, startPosition, startPosition);
			}



		}
	}

	Vector3 snapToGrid(Vector3 pos)
	{
//		if (XEnabled) {
//			int divx = (int)((pos.x + 0.5f) / XSnapDistance);
//			divx *= XSnapDistance;
//
//			if (Mathf.Abs (divx - pos.x) < Mathf.Abs (divx + XSnapDistance - pos.x))
//				pos.x = divx;
//			else
//				pos.x = divx + XSnapDistance;
//		}
//
//
//		if (YEnabled) {
//			int divy = (int)((pos.y + 0.5f) / YSnapDistance);
//			divy *= YSnapDistance;
//
//			if (Mathf.Abs (divy - pos.y) < Mathf.Abs (divy + YSnapDistance - pos.y))
//				pos.y = divy;
//			else
//				pos.y = divy + YSnapDistance;
//		}
//
//
//		if (ZEnabled) {
//			int divz = (int)((pos.z + 0.5f) / ZSnapDistance);
//			divz *= ZSnapDistance;
//
//			if (Mathf.Abs (divz - pos.z) < Mathf.Abs (divz + ZSnapDistance - pos.z))
//				pos.z = divz;
//			else
//				pos.z = divz + ZSnapDistance;
//		}
//		
//		return pos;


		int divx = (int)((pos.x > 0 ? pos.x + 0.5f * XSnapDistance : pos.x - 0.5f * XSnapDistance) / XSnapDistance);
		divx *= XSnapDistance;
		int divy = (int)((pos.y > 0 ? pos.y + 0.5f * YSnapDistance : pos.y - 0.5f * YSnapDistance) / YSnapDistance);
		divy *= YSnapDistance;
		int divz = (int)((pos.z > 0 ? pos.z + 0.5f * ZSnapDistance : pos.z - 0.5f * ZSnapDistance) / ZSnapDistance);
		divz *= ZSnapDistance;

		if (XEnabled && XSnapDistance != 0)
			pos.x = divx;
		if (YEnabled && YSnapDistance != 0)
			pos.y = divy;
		if (ZEnabled && ZSnapDistance != 0)
			pos.z = divz;

		return pos;


	}
}
