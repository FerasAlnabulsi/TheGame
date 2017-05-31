using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

// @testcase: prevent split wall in window/door

public enum BuildingEditMode
{
    None,
    Drawing,
    WallFaceSelected,
    WallFaceMoving,
    WallVertexMoving
};

public enum ViewingMode
{
    Interior,
    Exterior
};

public class BuildingArea : MonoBehaviour
{

    BuildingEditMode _mode;
    public BuildingEditMode Mode
    {
        get
        {
            return _mode;
        }
        set
        {
            _mode = value;
            if (_mode == BuildingEditMode.WallFaceSelected)
            {
                //MaterialsPanel.SetActive (true);
                //WindowMaterialPanel.SetActive (true);
            }
            else
            {
                //MaterialsPanel.SetActive (false);
                //WindowMaterialPanel.SetActive (false);
            }
        }
    }


    ViewingMode _viewingMode;
    public ViewingMode viewingMode
    {
        get {
            return _viewingMode;
        }
        set {
			_viewingMode = value;
			if (_viewingMode == ViewingMode.Interior) {
				if (Roof != null)
					Roof.SetActive (false);
			} else {
				if (Roof != null)
					Roof.SetActive (true);
			}
		}
    }


    // PARAMETERS
    public int snapGridDistance = 1;
    public GameObject VertexHandle;
    public GameObject snapObject;
    public Material DraggedLineMaterial;
    public Material LineMaterial;
    public Material WallSelectedMaterial;
    public Material WallWireframeMaterial;

    //public GameObject MaterialsPanel;
    public GameObject WindowMaterialPanel;

    public Material DefaultOuterWallMaterial;
    public Material DefaultInnerWallMaterial;
    public Material DefaultSideMaterial;
    public Material DefaultRoofMaterial;
    public Material DefaultFloorMaterial;

	public float Height = 2.0f;
	public bool isCeil = false;
	public Material CeilMaterial;
    public float DoubleClickCatchTime = 0.25f;

    private WallFace _selectedWallFace = null;
    private WallFace selectedWallFace
    {
        get
        {
            return _selectedWallFace;
        }
        set
        {

            if (_selectedWallFace != null)
            {
                _selectedWallFace.Selected = false;
            }

            _selectedWallFace = value;

            if (_selectedWallFace != null)
            {
                // activate vertex handle


                _selectedWallFace.Selected = true;
                wallFaceHandleObject.transform.position = (_selectedWallFace.RelatedLine.a + _selectedWallFace.RelatedLine.b) * 0.5f;
                wallFaceHandleObject.transform.position += Vector3.up * _selectedWallFace.RelatedLine.Height;
                wallFaceHandleObject.SetActive(true);
                wallFaceHandleDraggable.Enabled = true;
//				wallFaceHandleDraggable.FreezeY = false;

                vertexAHandleObject.transform.position = _selectedWallFace.RelatedLine.a;
                vertexAHandleObject.transform.position += Vector3.up * _selectedWallFace.RelatedLine.Height;
                vertexAHandleObject.SetActive(true);
                vertexAHandleDraggable.Enabled = true;
//				vertexAHandleDraggable.FreezeY = false;

                vertexBHandleObject.transform.position = _selectedWallFace.RelatedLine.b;
                vertexBHandleObject.transform.position += Vector3.up * _selectedWallFace.RelatedLine.Height;
                vertexBHandleObject.SetActive(true);
                vertexBHandleDraggable.Enabled = true;
//				vertexBHandleDraggable.FreezeY = false;

                DetachButton.interactable = true;
                DeleteButton.interactable = true;
            }
            else
            {
                // dectivate vertex handle

                wallFaceHandleDraggable.Enabled = false;
                wallFaceHandleObject.SetActive(false);

                vertexAHandleObject.SetActive(false);
                vertexAHandleDraggable.Enabled = false;

                vertexBHandleObject.SetActive(false);
                vertexBHandleDraggable.Enabled = false;
                DetachButton.interactable = false;
                DeleteButton.interactable = false;
				Mode = BuildingEditMode.None;

            }
        }
    }

	bool roofEnabled;
	public bool RoofEnabled
	{
		get{
			return roofEnabled;
		}
		set {
			roofEnabled = value;
			regeneratePath (false);
		}
	}

	public void CopyToNewLayer(BuildingArea ba)
	{
		ba.BuildingAreaCollider = ba.GetComponent<Collider>();
		ba.snapObject = snapObject;
		ba.VertexHandle = VertexHandle;
		ba.DetachButton = GameObject.Find("Detach button").GetComponent<Button>();
		ba.DeleteButton = GameObject.Find("Delete button").GetComponent<Button>();
		ba.WallWireframeMaterial = WallWireframeMaterial;
		ba.WallSelectedMaterial = WallSelectedMaterial;

		ba.wallFaceHandleObject = GameObject.Instantiate(ba.VertexHandle);
		ba.wallFaceHandleDraggable = ba.wallFaceHandleObject.AddComponent<Draggable>();
		ba.wallFaceHandleDraggable.Enabled = false;
//		ba.wallFaceHandleDraggable.XEnabled = true;
//		ba.wallFaceHandleDraggable.YEnabled = false;
//		ba.wallFaceHandleDraggable.ZEnabled = true;
//		ba.wallFaceHandleDraggable.FreezeY = false;
//		ba.wallFaceHandleDraggable.XSnapDistance = ba.snapGridDistance;
//		ba.wallFaceHandleDraggable.YSnapDistance = ba.snapGridDistance;
//		ba.wallFaceHandleDraggable.ZSnapDistance = ba.snapGridDistance;
		ba.wallFaceHandleDraggable.StartMoving += ba.WallFaceHandleDraggable_StartMoving;
		ba.wallFaceHandleDraggable.Moving += ba.WallFaceHandleDraggable_Moving;
		ba.wallFaceHandleDraggable.EndMoving += ba.WallFaceHandleDraggable_EndMoving;


		ba.vertexAHandleObject = GameObject.Instantiate(ba.VertexHandle);
		ba.vertexAHandleDraggable = ba.vertexAHandleObject.AddComponent<Draggable>();
		ba.vertexAHandleDraggable.Enabled = false;
//		ba.vertexAHandleDraggable.XEnabled = true;
//		ba.vertexAHandleDraggable.YEnabled = false;
//		ba.vertexAHandleDraggable.ZEnabled = true;
//		ba.vertexAHandleDraggable.FreezeY = false;
//		ba.vertexAHandleDraggable.XSnapDistance = ba.snapGridDistance;
//		ba.vertexAHandleDraggable.YSnapDistance = ba.snapGridDistance;
//		ba.vertexAHandleDraggable.ZSnapDistance = ba.snapGridDistance;
		ba.vertexAHandleDraggable.StartMoving += ba.vertexAHandleDraggable_StartMoving;
		ba.vertexAHandleDraggable.Moving += ba.vertexAHandleDraggable_Moving;
		ba.vertexAHandleDraggable.EndMoving += ba.vertexAHandleDraggable_EndMoving;

		ba.vertexBHandleObject = GameObject.Instantiate(ba.VertexHandle);
		ba.vertexBHandleDraggable = ba.vertexBHandleObject.AddComponent<Draggable>();
		ba.vertexBHandleDraggable.Enabled = false;
//		ba.vertexBHandleDraggable.XEnabled = true;
//		ba.vertexBHandleDraggable.YEnabled = false;
//		ba.vertexBHandleDraggable.ZEnabled = true;
//		ba.vertexBHandleDraggable.FreezeY = false;
//		ba.vertexBHandleDraggable.XSnapDistance = ba.snapGridDistance;
//		ba.vertexBHandleDraggable.YSnapDistance = ba.snapGridDistance;
//		ba.vertexBHandleDraggable.ZSnapDistance = ba.snapGridDistance;
		ba.vertexBHandleDraggable.StartMoving += ba.vertexBHandleDraggable_StartMoving;
		ba.vertexBHandleDraggable.Moving += ba.vertexBHandleDraggable_Moving;
		ba.vertexBHandleDraggable.EndMoving += ba.vertexBHandleDraggable_EndMoving;

		ba.DefaultFloorMaterial = DefaultFloorMaterial;
		ba.DefaultInnerWallMaterial = DefaultInnerWallMaterial;
		ba.DefaultOuterWallMaterial = DefaultOuterWallMaterial;
		ba.DefaultRoofMaterial = DefaultRoofMaterial;
		ba.DefaultSideMaterial = DefaultSideMaterial;
		ba.DoubleClickCatchTime = DoubleClickCatchTime;
		ba.lastClickTime = lastClickTime;
		//ba.floors auto generated
		ba.floors = new List<GameObject>();
		ba.items = new List<GameObject> (items);
		ba.LineMaterial = LineMaterial;
		ba.isCeil = isCeil;
		ba.CeilMaterial = CeilMaterial;
		ba.lines = new List<Line> ();
		{
			ba.lineVertices = new List<Vector3> (lines [0].Vertices);

			for (int i = 0; i < lines.Count; i++) {
				Line l = new Line (ba.lineVertices, lines [i].aID, lines [i].bID, lines [i].Thickness, GameObject.Instantiate (lines [i].LineMaterial), GameObject.Instantiate (lines [i].InnerMaterial), GameObject.Instantiate (lines [i].OuterMaterial), GameObject.Instantiate (lines [i].SideMaterial));

				for (int j = 0; j < lines [i].Doors.Count; j++) {
					WallDoor wd = new WallDoor (l, lines [i].Doors [j].Position.x, lines [i].Doors [j].DoorWidth, lines [i].Doors [j].DoorHeight, GameObject.Instantiate (lines [i].Doors [j].Door));
					l.Doors.Add (wd);
				}
				for (int j = 0; j < lines [i].Windows.Count; j++) {
					WallWindow ww = new WallWindow (l, lines [i].Windows [j].Position, lines [i].Windows [j].WindowWidth, lines [i].Windows [j].WindowHeight, GameObject.Instantiate (lines [i].Windows [j].Window));
					l.Windows.Add (ww);
				}
				l.Height = lines [i].Height;
				l.LedgeHeight = lines [i].LedgeHeight;
				l.LineType = lines [i].LineType;
				l.Parent = ba.transform;
				l.ParentLine = null;
				l.WindowHeight = lines [i].WindowHeight;
				l.Enabled = lines [i].Enabled;
				ba.lines.Add (l);
			}
		}
		ba.Mode = Mode;
		ba.MouseStartDistance = MouseStartDistance;
		ba.MouseStartPosition = MouseStartPosition;
		ba.SelectedItem = SelectedItem;
		ba.selectedWallFace = null;
		ba.snapEnabled = snapEnabled;
		ba.pointASelected = false;
		ba.verticesSelected = new List<int> ();


		ba.viewingMode = viewingMode;
		ba.cameraTarget = cameraTarget;
		ba.SelectedItem = null;
		ba.DraggedLineMaterial = DraggedLineMaterial;
		ba.gameCamera = Camera.main.GetComponent<ObjectFollowCamera>();
		ba.DraggedLine = new Line(new List<Vector3>() { Vector3.zero, Vector3.zero }, 0, 1, 0.4f, ba.DraggedLineMaterial, null, null, null);
		ba.DraggedLine.Enabled = false;
		ba.IsBasement = IsBasement;
	}

	public void SetWorkingHeight(float y)
	{
		List<Vector3> grid = new List<Vector3> (); // 11 * 11
		for (int i = -5; i <= 5; i++) {
			for (int j = -5; j <= 5; j++) {
				if (((MeshCollider)BuildingAreaCollider).sharedMesh.triangles.Length == 0 || IsWallOverBuildingArea (new Vector3 (i, y + 0.001f, j), new Vector3 (i, y + 0.001f, j))) {
					grid.Add(new Vector3 (i, y, j));
				}
			}
		}
		Vector3[] aGrid = grid.ToArray ();
		vertexAHandleDraggable.SetAllowedPoints (aGrid);
		vertexBHandleDraggable.SetAllowedPoints (aGrid);
		wallFaceHandleDraggable.SetAllowedPoints (aGrid);
	}

    List<GameObject> items = new List<GameObject>();
    private List<WallFace> wallFaces = new List<WallFace>();

    private List<Vector3> lineVertices = new List<Vector3>();
    public List<Line> lines = new List<Line>();

	private List<GameObject> floors = new List<GameObject>();
	private List<Collider> floorColliders = new List<Collider> ();
	private GameObject Roof;

    List<int> verticesSelected = new List<int>();

    bool pointASelected = false;
    bool snapEnabled = true;
    Vector3 pointA;

    Vector3 MouseStartPosition;
    float MouseStartDistance;

	//basement is rectangle only
	public bool IsBasement = false;
	public GameObject Basement;
	public float BasementHeight = 0.8f;

    Draggable wallFaceHandleDraggable;
    GameObject wallFaceHandleObject;

    void WallFaceHandleDraggable_StartMoving(GameObject sender, Vector3 oldPosition, Vector3 newPosition)
    {
        Mode = BuildingEditMode.WallFaceMoving;
        vertexAHandleDraggable.Enabled = false;
        vertexBHandleDraggable.Enabled = false;
    }

    void WallFaceHandleDraggable_Moving(GameObject sender, Vector3 oldPosition, Vector3 newPosition)
    {
        Vector3 dif = newPosition - oldPosition;
//        if (IsWallOverBuildingArea(vertexAHandleObject.transform.position + dif, vertexBHandleObject.transform.position + dif))
        {
            wallFaceHandleObject.transform.position += dif;
            selectedWallFace.RelatedLine.a += dif;
            selectedWallFace.RelatedLine.b += dif;
            vertexAHandleObject.transform.position += dif;
            vertexBHandleObject.transform.position += dif;
            regeneratePath(false);
        }
    }

    void WallFaceHandleDraggable_EndMoving(GameObject sender, Vector3 oldPosition, Vector3 newPosition)
    {
//        if (IsWallOverBuildingArea(vertexAHandleObject.transform.position, vertexBHandleObject.transform.position))
        {
            Mode = BuildingEditMode.WallFaceSelected;
            vertexAHandleDraggable.Enabled = true;
            vertexBHandleDraggable.Enabled = true;
            regeneratePath(true);
        }
    }

    Draggable vertexAHandleDraggable;
    GameObject vertexAHandleObject;

    Draggable vertexBHandleDraggable;
    GameObject vertexBHandleObject;

    void vertexAHandleDraggable_StartMoving(GameObject sender, Vector3 oldPosition, Vector3 newPosition)
    {
        Mode = BuildingEditMode.WallVertexMoving;
        vertexBHandleDraggable.Enabled = false;
        vertexBHandleObject.SetActive(false);
        wallFaceHandleObject.SetActive(false);
        wallFaceHandleDraggable.Enabled = false;
    }

    void vertexAHandleDraggable_Moving(GameObject sender, Vector3 oldPosition, Vector3 newPosition)
    {
        if (newPosition != vertexBHandleObject.transform.position)
        {
            Vector3 dif = newPosition - oldPosition;
//            if (IsWallOverBuildingArea(vertexAHandleObject.transform.position + dif, vertexBHandleObject.transform.position))
            {

                vertexAHandleObject.transform.position += dif;
                wallFaceHandleObject.transform.position += dif * 0.5f;
                Vector3 pos = selectedWallFace.RelatedLine.a;

                for (int i = 0; i < lines.Count; i++)
                {
                    if ((lines[i].a - pos).sqrMagnitude <= 0.00001f)
                        lines[i].a += dif;

                    if ((lines[i].b - pos).sqrMagnitude <= 0.00001f)
                        lines[i].b += dif;
                }

                regeneratePath(false);
            }
        }
    }

    void vertexAHandleDraggable_EndMoving(GameObject sender, Vector3 oldPosition, Vector3 newPosition)
    {
//        if (IsWallOverBuildingArea(vertexAHandleObject.transform.position, vertexBHandleObject.transform.position))
        {
            Mode = BuildingEditMode.WallFaceSelected;
            vertexBHandleDraggable.Enabled = true;
            vertexBHandleObject.SetActive(true);
            wallFaceHandleDraggable.Enabled = true;
            wallFaceHandleObject.SetActive(true);
            regeneratePath(true);
        }
    }


    void vertexBHandleDraggable_StartMoving(GameObject sender, Vector3 oldPosition, Vector3 newPosition)
    {
        Mode = BuildingEditMode.WallVertexMoving;
        vertexAHandleDraggable.Enabled = false;
        vertexAHandleObject.SetActive(false);
        wallFaceHandleDraggable.Enabled = false;
        wallFaceHandleObject.SetActive(false);
    }

    void vertexBHandleDraggable_Moving(GameObject sender, Vector3 oldPosition, Vector3 newPosition)
    {
        if (newPosition != vertexAHandleObject.transform.position)
        {
            Vector3 dif = newPosition - oldPosition;
//            if (IsWallOverBuildingArea(vertexAHandleObject.transform.position, vertexBHandleObject.transform.position + dif))
            {

                vertexBHandleObject.transform.position += dif;
                wallFaceHandleObject.transform.position += dif * 0.5f;
                Vector3 pos = selectedWallFace.RelatedLine.b;

                for (int i = 0; i < lines.Count; i++)
                {
                    if ((lines[i].a - pos).sqrMagnitude <= 0.00001f)
                        lines[i].a += dif;

                    if ((lines[i].b - pos).sqrMagnitude <= 0.00001f)
                        lines[i].b += dif;
                }

                regeneratePath(false);
            }
        }
    }

    void vertexBHandleDraggable_EndMoving(GameObject sender, Vector3 oldPosition, Vector3 newPosition)
    {
//        if (IsWallOverBuildingArea(vertexAHandleObject.transform.position, vertexBHandleObject.transform.position))
        {
            Mode = BuildingEditMode.WallFaceSelected;
            vertexAHandleDraggable.Enabled = true;
            vertexAHandleObject.SetActive(true);
            wallFaceHandleDraggable.Enabled = true;
            wallFaceHandleObject.SetActive(true);
            regeneratePath(true);
        }
    }

    Button DetachButton;
    Button DeleteButton;

    public GameObject upperWallFace;

    public void DetachSelectedWall()
    {
        selectedWallFace.RelatedLine.DetachA();
        selectedWallFace.RelatedLine.DetachB();

        regeneratePath(true);
    }


    public void DeleteSelectedWall()
    {
		WallFace wf = selectedWallFace;
		selectedWallFace = null;
        lines.Remove(wf.RelatedLine);
		wf.RelatedLine.Destroy ();
//		wf.Destroy ();

        regeneratePath(true);
    }

    public void SetSelectedWallFaceMaterials(Material innerMaterial, Material outerMaterial, Material sideMaterial)
    {
        // this function will be called from material panel
        List<Vector3> endpoints = new List<Vector3>();
        endpoints.Add(selectedWallFace.RelatedLine.a);
        selectedWallFace.RelatedLine.InnerMaterial = innerMaterial;
        selectedWallFace.RelatedLine.OuterMaterial = outerMaterial;
        selectedWallFace.RelatedLine.SideMaterial = sideMaterial;
        for (int i = 0; i < endpoints.Count; i++)
        {

            for (int j = 0; j < lines.Count; j++)
            {
                if (lines[j] != selectedWallFace.RelatedLine)
                {

                    if ((endpoints[i] - lines[j].a).sqrMagnitude <= 0.00001f && endpoints.FindIndex(delegate (Vector3 v) { return (lines[j].b - v).sqrMagnitude <= 0.00001f; }) == -1)
                    {
                        lines[j].InnerMaterial = innerMaterial;
                        lines[j].OuterMaterial = outerMaterial;
                        lines[j].SideMaterial = sideMaterial;
                        endpoints.Add(lines[j].b);
                    }
                    else if ((endpoints[i] - lines[j].b).sqrMagnitude <= 0.00001f && endpoints.FindIndex(delegate (Vector3 v) { return (lines[j].a - v).sqrMagnitude <= 0.00001f; }) == -1)
                    {
                        lines[j].InnerMaterial = innerMaterial;
                        lines[j].OuterMaterial = outerMaterial;
						lines[j].SideMaterial = sideMaterial;
                        endpoints.Add(lines[j].a);
                    }
                }
            }
        }
    }

    Collider BuildingAreaCollider;

    bool IsWallOverBuildingArea(Vector3 a, Vector3 b)
    {
        RaycastHit h1, h2 = new RaycastHit();
        return BuildingAreaCollider.Raycast(new Ray(a, Vector3.down), out h1, float.MaxValue) && BuildingAreaCollider.Raycast(new Ray(b, Vector3.down), out h2, float.MaxValue);
    }

    public void SetWindowMaterials(GameObject obj)
    {
        for (int i = 0; i < selectedWallFace.RelatedLine.Windows.Count; i++)
        {
            selectedWallFace.RelatedLine.Windows[i].Window = Instantiate(obj);
            //Vector3 start = selectedWallFace.RelatedSegment.a + (selectedWallFace.RelatedSegment.b - selectedWallFace.RelatedSegment.a).normalized * (selectedWallFace.RelatedSegment.Windows [i].Position.x + selectedWallFace.RelatedSegment.Windows [i].WindowWidth * 0.5f);


            //start += Vector3.up * selectedWallFace.RelatedSegment.Windows [i].Position.y;

            selectedWallFace.RelatedLine.Windows[i].Update();
        }
    }

    public void SetSelectedWallFaceWindowCount(string count)
    {
        SetSelectedWallFaceWindowCount(int.Parse(count));
    }

    public void SetSelectedWallFaceWindowCount(int count)
    {
        if (count == 0)
        {
            _selectedWallFace.RelatedLine.LineType = 0;
            _selectedWallFace.RelatedLine.Windows.Clear();
        }
        else
        {
            float frac = 1.0f / (2.0f * count + 1);
            _selectedWallFace.RelatedLine.LineType = LineType.Window;

            _selectedWallFace.RelatedLine.Windows.Clear();

            for (int i = 0; i < count; i++)
            {
                //              WallWindow window = new WallWindow (_selectedWallFace.RelatedLine, new Vector2((i * 2 + 1) * frac * (_selectedWallFace.RelatedLine.b - _selectedWallFace.RelatedLine.a).magnitude, _selectedWallFace.RelatedLine.Height * 0.2f),(_selectedWallFace.RelatedLine.b - _selectedWallFace.RelatedLine.a).magnitude * 1.0f / (count * 2 + 1), _selectedWallFace.RelatedLine.Height * 0.5f, null);
                //              _selectedWallFace.RelatedLine.Windows.Add (window);

                WallDoor w = new WallDoor(_selectedWallFace.RelatedLine, (i * 2 + 1) * frac * (_selectedWallFace.RelatedLine.b - _selectedWallFace.RelatedLine.a).magnitude, (_selectedWallFace.RelatedLine.b - _selectedWallFace.RelatedLine.a).magnitude * 1.0f / (count * 2 + 1), _selectedWallFace.RelatedLine.Height * 0.5f, null);
                _selectedWallFace.RelatedLine.Doors.Add(w);
            }
        }
        regeneratePath(false);
    }

    public item SelectedItem { get; set; }


    ObjectFollowCamera gameCamera;
    // set on the first click or second click and used to set camera target on double click
    Vector3 cameraTarget;





    void Awake()
    {
		Basement = GameObject.CreatePrimitive (PrimitiveType.Cube);
		Basement.SetActive (false);

        BuildingAreaCollider = GetComponent<Collider>();
        viewingMode = ViewingMode.Exterior;
        SelectedItem = null;
        gameCamera = Camera.main.GetComponent<ObjectFollowCamera>();
        DetachButton = GameObject.Find("Detach button").GetComponent<Button>();
        DeleteButton = GameObject.Find("Delete button").GetComponent<Button>();

		if (snapObject != null)
			snapObject = GameObject.Instantiate (snapObject);
		
        DraggedLine = new Line(new List<Vector3>() { Vector3.zero, Vector3.zero }, 0, 1, 0.4f, DraggedLineMaterial, null, null, null);
		DraggedLine.Height = Height;
		DraggedLine.Enabled = false;

		if (VertexHandle != null) {
			wallFaceHandleObject = GameObject.Instantiate (VertexHandle);
			wallFaceHandleDraggable = wallFaceHandleObject.AddComponent<Draggable> ();
			wallFaceHandleDraggable.Enabled = false;
		
//        wallFaceHandleDraggable.XEnabled = true;
//        wallFaceHandleDraggable.YEnabled = false;
//        wallFaceHandleDraggable.ZEnabled = true;
//		wallFaceHandleDraggable.FreezeY = false;
//        wallFaceHandleDraggable.XSnapDistance = snapGridDistance;
//        wallFaceHandleDraggable.YSnapDistance = snapGridDistance;
//        wallFaceHandleDraggable.ZSnapDistance = snapGridDistance;
			wallFaceHandleDraggable.StartMoving += WallFaceHandleDraggable_StartMoving;
			wallFaceHandleDraggable.Moving += WallFaceHandleDraggable_Moving;
			wallFaceHandleDraggable.EndMoving += WallFaceHandleDraggable_EndMoving;


			vertexAHandleObject = GameObject.Instantiate (VertexHandle);
			vertexAHandleDraggable = vertexAHandleObject.AddComponent<Draggable> ();
			vertexAHandleDraggable.Enabled = false;
//        vertexAHandleDraggable.XEnabled = true;
//        vertexAHandleDraggable.YEnabled = false;
//        vertexAHandleDraggable.ZEnabled = true;
//		vertexAHandleDraggable.FreezeY = false;
//        vertexAHandleDraggable.XSnapDistance = snapGridDistance;
//        vertexAHandleDraggable.YSnapDistance = snapGridDistance;
//        vertexAHandleDraggable.ZSnapDistance = snapGridDistance;
			vertexAHandleDraggable.StartMoving += vertexAHandleDraggable_StartMoving;
			vertexAHandleDraggable.Moving += vertexAHandleDraggable_Moving;
			vertexAHandleDraggable.EndMoving += vertexAHandleDraggable_EndMoving;

			vertexBHandleObject = GameObject.Instantiate (VertexHandle);
			vertexBHandleDraggable = vertexBHandleObject.AddComponent<Draggable> ();
			vertexBHandleDraggable.Enabled = false;
//        vertexBHandleDraggable.XEnabled = true;
//        vertexBHandleDraggable.YEnabled = false;
//        vertexBHandleDraggable.ZEnabled = true;
//		vertexBHandleDraggable.FreezeY = false;
//
//        vertexBHandleDraggable.XSnapDistance = snapGridDistance;
//        vertexBHandleDraggable.YSnapDistance = snapGridDistance;
//        vertexBHandleDraggable.ZSnapDistance = snapGridDistance;
			vertexBHandleDraggable.StartMoving += vertexBHandleDraggable_StartMoving;
			vertexBHandleDraggable.Moving += vertexBHandleDraggable_Moving;
			vertexBHandleDraggable.EndMoving += vertexBHandleDraggable_EndMoving;
		}
    }




    Line DraggedLine = null;
	//used for basement
	Line[] DraggedAreaLines = new Line[4];

    float lastClickTime;

    Bounds? alignToFloor(Bounds aabb, int maxTries)
    {
        while (maxTries >= 0)
        {
			bool flag = false;
			int floorID = -1;
			RaycastHit hp = new RaycastHit();

			for (int i = 0; i < floorColliders.Count; i++)
			{
				bool wasEnabled = floorColliders [i].enabled;
				floorColliders [i].enabled = true;
				int tmp = 0;
//				RaycastHit hp;
				if (!floorColliders[i].Raycast (new Ray (new Vector3(aabb.min.x, aabb.max.y, aabb.min.z), Vector3.down), out hp, float.MaxValue))
					tmp++;
				if (!floorColliders[i].Raycast (new Ray (aabb.max, Vector3.down), out hp, float.MaxValue))
					tmp++;
				if (!floorColliders[i].Raycast (new Ray (new Vector3 (aabb.min.x, aabb.max.y, aabb.max.z), Vector3.down), out hp, float.MaxValue))
					tmp++;
				if (!floorColliders[i].Raycast (new Ray (new Vector3 (aabb.max.x, aabb.max.y, aabb.min.z), Vector3.down), out hp, float.MaxValue))
					tmp++;

				if (tmp == 0) {
					floorID = i;
					flag = true;
					floorColliders [i].enabled = wasEnabled;
					break;
				}
				floorColliders [i].enabled = wasEnabled;
			}

			if (!flag)
				return null;



//            if (!BuildingAreaCollider.Raycast(new Ray(aabb.min, Vector3.down), out hp, float.MaxValue))//mofeed
//                return null;
//            if (!BuildingAreaCollider.Raycast(new Ray(aabb.max, Vector3.down), out hp, float.MaxValue))
//                return null;
//            if (!BuildingAreaCollider.Raycast(new Ray(new Vector3(aabb.min.x, aabb.min.y, aabb.max.z), Vector3.down), out hp, float.MaxValue))
//                return null;
//            if (!BuildingAreaCollider.Raycast(new Ray(new Vector3(aabb.max.x, aabb.min.y, aabb.min.z), Vector3.down), out hp, float.MaxValue))
//                return null;

			{
				bool wasEnabled = floorColliders [floorID].enabled;
				floorColliders [floorID].enabled = true;
				if (floorColliders [floorID].Raycast (new Ray (aabb.min, Vector3.down), out hp, float.MaxValue)) {
					aabb.center += Vector3.down * hp.distance;
				}
				floorColliders [floorID].enabled = wasEnabled;
			}

            Bounds oldAABB = aabb;
            aabb = alignToFloor(aabb);
            if (oldAABB == aabb)
                return new Bounds?(aabb);

            maxTries--;
        }
        return null;
    }

    Bounds alignToFloor(Bounds aabb)
    {


        for (int i = 0; i < items.Count; i++)
        {
            Collider[] colliders = items [i].GetComponentsInChildren<Collider> ();
            if (colliders.Length == 0)
                continue;
            
            Bounds aabb2 = colliders[0].bounds;
            for (int j = 0; j < colliders.Length; j++) {
                aabb2.Encapsulate(colliders[j].bounds);
            }

            if (aabb.Intersects(aabb2))
            {
                Vector3 dif = aabb.center - aabb2.center;
                if (Mathf.Abs(dif.x) > Mathf.Abs(dif.z))
                {
                    // x
                    if (dif.x > 0)
                        aabb.center += Vector3.right * (aabb2.max.x - aabb.min.x);
                    else
                        aabb.center += Vector3.right * (aabb.max.x - aabb2.min.x);


                }
                else
                {
                    // z
                    if (dif.z > 0)
                        aabb.center += Vector3.forward * (aabb2.max.z - aabb.min.z);
                    else
                        aabb.center += Vector3.forward * (aabb.max.z - aabb2.min.z);

                }

            }
        }
        return aabb;
    }

	public Button BasementButton;
	/// <summary>
	/// this will enable basement button (done) whenever basement area > 0
	/// </summary>
	void UpdateEnableBasementButton()
	{
		if (lines.Count > 0) {
			Vector3 tmp = lines [0].Vertices [0] - lines [0].Vertices [2];
			BasementButton.interactable = Mathf.Abs (tmp.x * tmp.z) > 0.001f;
		} else {
			BasementButton.interactable = false;
		}
	}

    void Update()
    {
		if (!enabled)
			return;
		
		if (IsBasement) {

			if ((Mode == BuildingEditMode.None || Mode == BuildingEditMode.Drawing) && SelectedItem == null) {
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				Collider coll = GetComponent<MeshCollider> ();
				RaycastHit hit;
				snapObject.SetActive (false);
				if (Physics.Raycast (ray, out hit, float.MaxValue) && hit.collider == coll && !EventSystem.current.IsPointerOverGameObject ()) {

					if (snapEnabled) {
						hit.point = snapToGrid (hit.point);
					}


					snapObject.SetActive (true);
					snapObject.transform.position = hit.point;
					if (Input.GetMouseButton (0) && !EventSystem.current.IsPointerOverGameObject ()) {

						if (!pointASelected) {
							pointA = hit.point;
							if (DraggedAreaLines [0] == null) {
								DraggedAreaLines [0] = new Line (new List<Vector3> () { pointA, pointA, pointA, pointA }, 0, 1, 0.4f, DraggedLineMaterial, DefaultOuterWallMaterial, DefaultOuterWallMaterial, DefaultOuterWallMaterial);
								for (int i = 0; i < 3; i++)
									DraggedAreaLines [i + 1] = new Line (DraggedAreaLines [0].Vertices, i + 1, (i + 2) % 4, 0.4f, DraggedLineMaterial, DefaultOuterWallMaterial, DefaultOuterWallMaterial, DefaultOuterWallMaterial);
							}

							for (int i = 0; i < DraggedAreaLines [0].Vertices.Count; i++) {
								DraggedAreaLines [0].Vertices [i] = pointA;
								DraggedAreaLines [i].Enabled = true;
								DraggedAreaLines [i].aID = DraggedAreaLines [i].aID;
								DraggedAreaLines [i].bID = DraggedAreaLines [i].bID;
							}
							
							pointASelected = true;
						}
						else {
							DraggedAreaLines [0].Vertices [2] = hit.point;

							DraggedAreaLines [0].Vertices [1] = new Vector3 (pointA.x, DraggedAreaLines [0].Vertices [1].y, hit.point.z);
							DraggedAreaLines [0].Vertices [3] = new Vector3 (hit.point.x, DraggedAreaLines [0].Vertices [1].y, pointA.z);
							for (int i = 0; i < 4; i++) {
								DraggedAreaLines [i].aID = DraggedAreaLines [i].aID;
								DraggedAreaLines [i].bID = DraggedAreaLines [i].bID;
							}


							//DraggedAreaLines [3].bID = 0;
						}
					}
					else if (Input.GetMouseButtonUp(0) && DraggedAreaLines[0] != null  && !EventSystem.current.IsPointerOverGameObject () && (pointA - hit.point).sqrMagnitude > 0.0001f) {
						for (int i = 0; i < DraggedAreaLines.Length; i++) {
							DraggedAreaLines [i].Enabled = false;
							DraggedAreaLines [i].Height = BasementHeight;
						}
						lines.Clear ();
						lines.AddRange (DraggedAreaLines);
						DraggedAreaLines = new Line[4];

						regeneratePath (true);
						UpdateEnableBasementButton ();
						for (int i = wallFaces.Count - 1; i >= 0; i--) {
							if (wallFaces [i].WallFaceType == WallFaceType.Inner) {
								wallFaces [i].Destroy ();
								wallFaces.RemoveAt (i);
							}
						}
						GameObject.Destroy (upperWallFace);
						upperWallFace = null;

						upperWallFace = new GameObject ("upper wall face");
						upperWallFace.AddComponent<MeshFilter> ().mesh = GetOuterCeil ();
						upperWallFace.AddComponent<MeshRenderer> ();
						upperWallFace.AddComponent<MeshCollider> ();
						upperWallFace.AddComponent<MeshCollider> ();

						for (int i = 0; i < floors.Count; i++) {
							GameObject.Destroy (floors [i]);
							GameObject.Destroy (floorColliders [i]);
						}
						floors.Clear ();
						floorColliders.Clear ();


						GameObject.Destroy (Roof);
						Roof = null;

//						Basement.transform.localScale = new Vector3(Mathf.Abs(DraggedAreaLines[0].Vertices[0].x - DraggedAreaLines[0].Vertices[2].x), BasementHeight, Mathf.Abs(DraggedAreaLines[0].Vertices[0].z - DraggedAreaLines[0].Vertices[2].z));
//						Basement.transform.position = (DraggedAreaLines [0].Vertices [0] + DraggedAreaLines [0].Vertices [2]) * 0.5f + Vector3.up * BasementHeight * 0.5f;
//						Basement.SetActive (true);
						pointASelected = false;
					}
				}
			}

		} else {
			if (viewingMode == ViewingMode.Interior) {
				for (int i = 0; i < wallFaces.Count; i++) {
					if (!wallFaces [i].IsFacingCamera || wallFaces [i].WallFaceType == WallFaceType.Outer) {
						wallFaces [i].Wireframe = true;
						wallFaces [i].Solid = false;
						for (int j = 0; j < wallFaces [i].RelatedLine.Doors.Count; j++) {
							wallFaces [i].RelatedLine.Doors [j].Door.SetActive (false);
						}
						for (int j = 0; j < wallFaces [i].RelatedLine.Windows.Count; j++) {
							wallFaces [i].RelatedLine.Windows [j].Window.SetActive (false);
						}
						wallFaces [i].gameObject.GetComponent<Collider> ().enabled = false;
					} else {
						wallFaces [i].Wireframe = false;
						wallFaces [i].Solid = true;
						for (int j = 0; j < wallFaces [i].RelatedLine.Doors.Count; j++) {
							wallFaces [i].RelatedLine.Doors [j].Door.SetActive (true);
						}
						for (int j = 0; j < wallFaces [i].RelatedLine.Windows.Count; j++) {
							wallFaces [i].RelatedLine.Windows [j].Window.SetActive (true);
						}
						wallFaces [i].gameObject.GetComponent<Collider> ().enabled = true;

					}
				}
			} else {

				for (int i = 0; i < wallFaces.Count; i++) {
					wallFaces [i].Wireframe = false;
					wallFaces [i].Solid = true;
					for (int j = 0; j < wallFaces [i].RelatedLine.Doors.Count; j++) {
						wallFaces [i].RelatedLine.Doors [j].Door.SetActive (true);
					}
					for (int j = 0; j < wallFaces [i].RelatedLine.Windows.Count; j++) {
						wallFaces [i].RelatedLine.Windows [j].Window.SetActive (true);
					}
					wallFaces [i].gameObject.GetComponent<Collider> ().enabled = true;

				}
			}

			//if (!planningMode) 
			{

				switch (Mode) {
				case BuildingEditMode.None:
					{
						if (Input.GetMouseButtonUp (0)) {

							if (SelectedItem == null) {

								WallFace wallface = getSelectedWallFace ();
								if (wallface != null) {
									selectedWallFace = wallface;
									Mode = BuildingEditMode.WallFaceSelected;
								}
							} else {

								for (int i = 0; i < floorColliders.Count; i++) {
									floorColliders [i].enabled = true;
								}

								if (SelectedItem.itemType == type.Window || SelectedItem.itemType == type.Door) {
									WallFace wallface = getSelectedWallFace ();
									if (wallface != null) {
										Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
										RaycastHit hit;
										if (Physics.Raycast (ray, out hit, float.MaxValue) && !EventSystem.current.IsPointerOverGameObject ()) {
											Vector2 location;
											Vector2? correctedLocation;
											if (wallface.RelatedLine.LocateItemInWall (hit.point, SelectedItem, out location, 100, out correctedLocation)) {
												if (SelectedItem.itemType == type.Window) {
													wallface.RelatedLine.Windows.Add (new WallWindow (wallface.RelatedLine, location, SelectedItem.prefabItem.Size.z, SelectedItem.prefabItem.Size.y, Instantiate (SelectedItem.prefabItem.gameObject)));
													regeneratePath (false);
												} else if (SelectedItem.itemType == type.Door) {
													wallface.RelatedLine.Doors.Add (new WallDoor (wallface.RelatedLine, location.x, SelectedItem.prefabItem.Size.z, SelectedItem.prefabItem.Size.y, Instantiate (SelectedItem.prefabItem.gameObject)));
													regeneratePath (false);
												}
											} else if (correctedLocation.HasValue) {
												if (SelectedItem.itemType == type.Window) {
													wallface.RelatedLine.Windows.Add (new WallWindow (wallface.RelatedLine, correctedLocation.Value, SelectedItem.prefabItem.Size.z, SelectedItem.prefabItem.Size.y, Instantiate (SelectedItem.prefabItem.gameObject)));
													regeneratePath (false);
												} else if (SelectedItem.itemType == type.Door) {
													wallface.RelatedLine.Doors.Add (new WallDoor (wallface.RelatedLine, correctedLocation.Value.x, SelectedItem.prefabItem.Size.z, SelectedItem.prefabItem.Size.y, Instantiate (SelectedItem.prefabItem.gameObject)));
													regeneratePath (false);
												}
											}
										}
									}
								} else { // not window and not door

									Vector3 location;
									Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
									RaycastHit hit;
									if (Physics.Raycast (ray, out hit, float.MaxValue) && !EventSystem.current.IsPointerOverGameObject ()) {
										location = hit.point - ray.direction * SelectedItem.prefabItem.Size.z * 0.5f;
										if (SelectedItem.alignToFloor) {
											RaycastHit floorHit;
											if (hit.collider.Raycast (new Ray (location, Vector3.down), out floorHit, float.MaxValue)) {
												Bounds aabb = new Bounds (floorHit.point + Vector3.up * SelectedItem.prefabItem.Size.y, SelectedItem.prefabItem.Size);
											 
												Bounds? nAABB = alignToFloor (aabb, 10);
												if (nAABB.HasValue) {
													GameObject go = Instantiate (SelectedItem.prefabItem.gameObject);
													PrefabItem pItem = go.GetComponent<PrefabItem> ();
													Draggable draggable = go.AddComponent<Draggable> ();
//													draggable.XEnabled = true;
//													draggable.YEnabled = false;
//													draggable.ZEnabled = true;
//													draggable.XSnapDistance = 0;
//													draggable.ZSnapDistance = 0;
													draggable.Enabled = true;
													draggable.StartMoving += delegate(GameObject sender, Vector3 oldPosition, Vector3 newPosition) {
														Bounds _aabb = new Bounds (newPosition + Vector3.up * pItem.Size.y, pItem.Size);
														Debug.Log ("start " + newPosition);
														Bounds? _nAABB = alignToFloor (_aabb, 10);
														if (_nAABB != null)
															sender.transform.position = newPosition;
													};
													draggable.Moving += delegate(GameObject sender, Vector3 oldPosition, Vector3 newPosition) {
														Bounds _aabb = new Bounds (newPosition + Vector3.up * pItem.Size.y, pItem.Size);
												
														Bounds? _nAABB = alignToFloor (_aabb, 10);
														if (_nAABB != null)
															sender.transform.position = newPosition;
													};
													draggable.EndMoving += delegate(GameObject sender, Vector3 oldPosition, Vector3 newPosition) {
														Bounds _aabb = new Bounds (newPosition + Vector3.up * pItem.Size.y, pItem.Size);
														Debug.Log ("end " + newPosition);

														Bounds? _nAABB = alignToFloor (_aabb, 10);
														if (_nAABB != null)
															sender.transform.position = newPosition;
													};


													go.transform.position = nAABB.Value.center;
													items.Add (go);
												}
											}
										}
									}
								}

								for (int i = 0; i < floorColliders.Count; i++) {
									floorColliders [i].enabled = false;
								}
							}
						}
					}
					break;
				case BuildingEditMode.WallFaceSelected:
					{
						if (Input.GetMouseButtonUp (0) && getSelectedWallFace () != null) {
							selectedWallFace = null;
						}
					}
					break;
				case BuildingEditMode.WallFaceMoving:
					{
					}
					break;
				}

				if (Input.GetMouseButtonDown (0)) {
					if (selectedWallFace != null) {
						cameraTarget = (selectedWallFace.a + selectedWallFace.b) * 0.5f + Vector3.up * selectedWallFace.Height * 0.5f;
					}
					if (Time.time - lastClickTime < DoubleClickCatchTime) {
						gameCamera.TargetObject = cameraTarget;
					}
					lastClickTime = Time.time;
				}
			}
			//else 
			{

				if ((Mode == BuildingEditMode.None || Mode == BuildingEditMode.Drawing) && SelectedItem == null) {
					Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
					Collider coll = GetComponent<MeshCollider> ();
					RaycastHit hit;
					snapObject.SetActive (false);
					if (Physics.Raycast (ray, out hit, float.MaxValue) && hit.collider == coll && !EventSystem.current.IsPointerOverGameObject ()) {

						//if (coll.Raycast (ray, out hit, float.MaxValue)) {
						if (snapEnabled) {
							hit.point = snapToGrid (hit.point);
							//                      Debug.Log (hit.point);
						}

						if (verticesSelected.Count != 0) {
							for (int i = 0; i < verticesSelected.Count; i++) {
								if (verticesSelected [i] % 2 == 0)
									lines [verticesSelected [i] / 2].a = hit.point;
								else
									lines [verticesSelected [i] / 2].b = hit.point;
							}
						}



						if (DraggedLine != null)
							DraggedLine.b = hit.point;

						snapObject.SetActive (true);
						snapObject.transform.position = hit.point;
						if (Input.GetMouseButtonDown (0) && verticesSelected.Count == 0) {

							if (!pointASelected) {
								DraggedLine.Enabled = true;
								//DraggedLine = new Line (hit.point, hit.point, DraggedLineMaterial);
								pointA = hit.point;
								DraggedLine.a = hit.point;
								DraggedLine.b = hit.point;
								pointASelected = true;

							} else {

								Vector3 pointB = hit.point;
								lines = Line.Split (lines, pointA);
								lines = Line.Split (lines, pointB);
								int id1 = lineVertices.FindIndex (delegate(Vector3 obj) {
									return (obj - pointA).sqrMagnitude <= 0.0001f;
								});
								int id2 = lineVertices.FindIndex (delegate(Vector3 obj) {
									return (obj - pointB).sqrMagnitude <= 0.0001f;
								});
								if (id1 == -1) {
									id1 = lineVertices.Count;
									lineVertices.Add (pointA);
								}
								if (id2 == -1) {
									id2 = lineVertices.Count;
									lineVertices.Add (pointB);
								}

								lines.Add (new Line (lineVertices, id1, id2, 0.2f, LineMaterial, DefaultInnerWallMaterial, DefaultOuterWallMaterial, DefaultSideMaterial));
								lines [lines.Count - 1].Height = Height;
								lines [lines.Count - 1].Parent = this.transform;
								pointASelected = false;
								DraggedLine.Enabled = false;
								//                          DraggedLine.Destroy ();
								//                          DraggedLine = null;

								for (int i = 0; i < lines.Count; i++) {
									lines [i].Enabled = false;
								}
								regeneratePath (true);

							}
						} else if (Input.GetMouseButtonDown (1) || (Input.GetMouseButton (0) && verticesSelected.Count != 0)) {

							if (verticesSelected.Count == 0) {
								for (int i = 0; i < lines.Count; i++) {
									if (hit.point == lines [i].a) {
										verticesSelected.Add (i * 2);
									}
									if (hit.point == lines [i].b) {
										verticesSelected.Add (i * 2 + 1);
									}
								}
							} else {
								verticesSelected.Clear ();
							}

						}
					}
				}

				if (Input.GetKeyDown (KeyCode.K)) {

					//              List<Vector3> verts = new List<Vector3> ();
					//              for (int i = 0; i < lines.Count; i++)
					//              {
					//                  if (!verts.Contains (lines [i].a))
					//                      verts.Add (lines [i].a);
					//
					//                  if (!verts.Contains (lines [i].b))
					//                      verts.Add (lines [i].b);
					//              }

					//              List<Line> nlines = new List<Line> ();
					//              {
					//                  List<Vector3> vvv = new List<Vector3> ();
					//                  vvv.Add (new Vector3 (0, 0, 0));
					//                  vvv.Add (new Vector3 (0, 0, -1));
					//                  vvv.Add (new Vector3 (-1, 0, -1));
					//                  vvv.Add (new Vector3 (-1, 0, 0));
					//                  nlines.Add (new Line (vvv, 0, 1, 0.1f, LineMaterial, null, null, null));
					//                  nlines.Add (new Line (vvv, 1, 2, 0.1f, LineMaterial, null, null, null));
					//                  nlines.Add (new Line (vvv, 2, 3, 0.1f, LineMaterial, null, null, null));
					//                  nlines.Add (new Line (vvv, 3, 0, 0.1f, LineMaterial, null, null, null));
					//              }
					//




					//
					//              List<int> triangles;
					//              List<Vector3> vs;
					//              List<Vector2> uvs;
					//              List<Vector3> normals;
					//
					//              Line.FillCap (nlines, out triangles, out vs, out uvs, out normals);
					////
					//              Mesh m = new Mesh ();
					//              m.vertices = vs.ToArray ();
					//              m.uv = uvs.ToArray ();
					//              m.triangles = triangles.ToArray ();
					//              m.normals = normals.ToArray ();

					//              GameObject go = new GameObject ("wal");
					//              go.AddComponent<MeshFilter> ().mesh = m;
					//              go.AddComponent<UpperWallFace> ().CreateFromLines (nlines);
					//              MeshRenderer mr = go.AddComponent<MeshRenderer> ();
					//              mr.material = LineMaterial;



				}

			}
		}

    }

	void OnDisable() {
		if (!gameObject.activeSelf) {
			for (int i = 0; i < lines.Count; i++) {
				for (int j = 0; j < lines [i].Windows.Count; j++) {
					lines [i].Windows [j].Window.SetActive (false);
				}

				for (int j = 0; j < lines [i].Doors.Count; j++) {
					lines [i].Doors [j].Door.SetActive (false);
				}
			}
		}
	}

	void OnEnable() {
		if (gameObject.activeSelf) {
			for (int i = 0; i < lines.Count; i++) {
				for (int j = 0; j < lines [i].Windows.Count; j++) {
					lines [i].Windows [j].Window.SetActive (true);
				}

				for (int j = 0; j < lines [i].Doors.Count; j++) {
					lines [i].Doors [j].Door.SetActive (true);
				}
			}
		}
	}

    WallFace getSelectedWallFace()
    {
        float dst = float.MaxValue;
        WallFace selectedFace = null;
        for (int i = 0; i < wallFaces.Count; i++)
        {
            float det;
            if (wallFaces[i].IsMouseOver(out det))
            {
                if (det < dst)
                {
                    dst = det;
                    selectedFace = wallFaces[i];
                }
            }
        }

        return selectedFace;
    }

	public Mesh GetCeil()
	{
//		if (IsBasement) {
//			GameObject quad = GameObject.CreatePrimitive (PrimitiveType.Quad);
//			quad.transform.
//		} else {
		Mesh m = new Mesh ();
		List<CombineInstance> meshes = new List<CombineInstance> ();
		for (int i = 0; i < floors.Count; i++) {
			CombineInstance ci = new CombineInstance ();
			ci.mesh = floors [i].GetComponent<MeshFilter> ().mesh;
			ci.transform = Matrix4x4.identity;
			meshes.Add (ci);
		}
		m.CombineMeshes (meshes.ToArray ());

		Vector3[] verts = new Vector3[m.vertices.Length];
		m.vertices.CopyTo (verts, 0);

		for (int i = 0; i < verts.Length; i++) {
			verts [i] += Vector3.up * lines [0].Height;
		}
		m.vertices = verts;

		return m;
//		}
	}

	public Mesh GetOuterCeil()
	{
		List<Line> outer = new List<Line> ();
		List<Vector3> verts = new List<Vector3> ();
		for (int i = 0; i < wallFaces.Count; i++) {
			if (wallFaces [i].WallFaceType == WallFaceType.Outer) {

				verts.Add (wallFaces [i].a);
				verts.Add (wallFaces [i].b);
				outer.Add (new Line (verts, verts.Count - 2, verts.Count - 1, 0.1f, null, null, null, null));
			}
		}
		Line.WeldVertices (outer);
		Line.OptimizePath (ref outer);

		List<int> triangles;
		List<Vector2> uvs;
		List<Vector3> normals;
		Line.FillCap (outer, out triangles, out verts, out uvs, out normals);
		for (int i = 0; i < verts.Count; i++) {
			verts [i] += wallFaces [0].Height * Vector3.up;
		}
		Mesh MeshCap = new Mesh ();
		MeshCap.vertices = verts.ToArray ();
		MeshCap.uv = uvs.ToArray ();
		MeshCap.normals = normals.ToArray ();
		MeshCap.triangles = triangles.ToArray ();
		return MeshCap;
	}


    //List<Vector3> vlines = new List<Vector3>();
    bool snap(Vector3 pos, float maxlength, out Vector3 nearest)
    {
        //snap to line vertices
        nearest = pos;
        float len = float.MaxValue;
        for (int i = 0; i < lines.Count; ++i)
        {
            {
                float det = Vector3.Distance(pos, lines[i].a);
                if (det < len)
                {
                    len = det;
                    nearest = lines[i].a;
                }
            }
            {
                float det = Vector3.Distance(pos, lines[i].b);
                if (det < len)
                {
                    len = det;
                    nearest = lines[i].b;
                }
            }
        }

        return len < maxlength;
    }

    Vector3 snapToGrid(Vector3 pos)
    {
        int divx = (int)((pos.x > 0 ? pos.x + 0.5f * snapGridDistance : pos.x - 0.5f * snapGridDistance) / snapGridDistance);
        divx *= snapGridDistance;
        int divy = (int)((pos.y > 0 ? pos.y + 0.5f * snapGridDistance : pos.y - 0.5f * snapGridDistance) / snapGridDistance);
        divy *= snapGridDistance;
        int divz = (int)((pos.z > 0 ? pos.z + 0.5f * snapGridDistance : pos.z - 0.5f * snapGridDistance) / snapGridDistance);
        divz *= snapGridDistance;

        pos.x = divx;
        //pos.y = divy;
        pos.z = divz;

        return pos;
    }

    public void regeneratePath(bool optimize)
    {
        Vector3 _selectedWallFaceA = Vector3.zero, _selectedWallFaceB = Vector3.zero;
        if (_selectedWallFace != null)
        {
            _selectedWallFaceA = _selectedWallFace.a;
            _selectedWallFaceB = _selectedWallFace.b;
        }

        for (int i = 0; i < wallFaces.Count; i++)
        {
            wallFaces[i].Destroy();
        }
        wallFaces.Clear();
        GameObject.Destroy(upperWallFace);
        upperWallFace = null;

        List<WallFace> outerWall;
        List<WallFace> doorSides;
        List<WallFace> innerWall;


        List<Mesh> floors;


		if (optimize) {
			Line.OptimizePath (ref lines);
		}

		Line.Generate3DWallFacesFromLines(lines, WallWireframeMaterial, WallSelectedMaterial, out outerWall, out doorSides, out innerWall, out upperWallFace, out floors);

		try{

	        //		gggg (lines, WallWireframeMaterial, WallSelectedMaterial, out outerWall, out doorSides, out innerWall, out upperWallFace, out floors);


			for (int i = 0; i < this.floors.Count; i++)
			{
				GameObject.Destroy(this.floors[i]);
			}

			this.floors.Clear();
			this.floorColliders.Clear();

	        for (int i = 0; i < floors.Count; i++)
	        {
	            GameObject floor = new GameObject("Room" + i.ToString() + "Floor");
				floor.transform.position += Vector3.up * 0.001f;
	            floor.AddComponent<MeshFilter>().mesh = floors[i];
				floor.AddComponent<MeshRenderer>().material = DefaultFloorMaterial;
				floorColliders.Add(floor.AddComponent<MeshCollider>());
				floorColliders[i].enabled = false;
	            floor.transform.parent = this.transform;
				this.floors.Add(floor);
			}


			if (Roof != null){
				GameObject.Destroy(Roof);
				Roof = null;
			}

			if (roofEnabled)
			{
				Roof = new GameObject("roof");
				Roof.AddComponent<Roof>().CreateFromLines(lines, 0.4f, 0.4f);
				Roof.transform.parent = transform;
				Roof.GetComponent<MeshRenderer>().material = DefaultRoofMaterial;
				if (_viewingMode == ViewingMode.Interior)
					Roof.SetActive (false);
			}
		}
		catch {
		}

        for (int i = 0; i < outerWall.Count; i++)
        {
			if (isCeil)
				outerWall [i].WallMaterial = CeilMaterial;
            wallFaces.Add(outerWall[i]);
            wallFaces[wallFaces.Count - 1].Parent = transform;
            wallFaces[wallFaces.Count - 1].Wireframe = !wallFaces[wallFaces.Count - 1].IsFacingCamera;
        }

        for (int i = 0; i < innerWall.Count; i++)
        {
            wallFaces.Add(innerWall[i]);
            wallFaces[wallFaces.Count - 1].Parent = transform;
            wallFaces[wallFaces.Count - 1].Wireframe = !wallFaces[wallFaces.Count - 1].IsFacingCamera;
        }

        for (int i = 0; i < doorSides.Count; i++)
        {
            wallFaces.Add(doorSides[i]);
            wallFaces[wallFaces.Count - 1].Parent = transform;
            wallFaces[wallFaces.Count - 1].Wireframe = !wallFaces[wallFaces.Count - 1].IsFacingCamera;
        }

        if (_selectedWallFace != null)
        {
            float dst = float.MaxValue;
            for (int i = 0; i < wallFaces.Count; i++)
            {
                float det1 = (wallFaces[i].a - _selectedWallFaceA).sqrMagnitude + (wallFaces[i].b - _selectedWallFaceB).sqrMagnitude;
                float det2 = (wallFaces[i].b - _selectedWallFaceA).sqrMagnitude + (wallFaces[i].a - _selectedWallFaceB).sqrMagnitude;
                if (det1 < dst)
                {
                    _selectedWallFace = wallFaces[i];
                    dst = det1;
                }
                if (det2 < dst)
                {
                    _selectedWallFace = wallFaces[i];
                    dst = det2;
                }
            }
            _selectedWallFace.Selected = true;
        }
    }

    public void WireFrameWallViewInterior()
    {
        //haytham
        viewingMode = ViewingMode.Interior;

    }

    public void WireFrameWallViewExterior()
    {
        viewingMode = ViewingMode.Exterior;
    }

    public void SetRoofMaterial(Material Mat)
    {
        if (Mat != null)
            Roof.GetComponent<MeshRenderer>().material = Mat;
        DefaultRoofMaterial = Mat;
    }


    public void SetOuterWallMaterial(Material Mat)
    {
        if (Mat != null)
        {
            SetSelectedWallFaceMaterials(DefaultInnerWallMaterial,Mat,DefaultSideMaterial);
        }
    }


    public void GetWallArea()
    {
        float wallSum = 0;
        wallSum+=(selectedWallFace.RelatedLine.a - selectedWallFace.RelatedLine.b).magnitude;
        wallSum *= selectedWallFace.Height;
        Debug.Log ("the area of the wall = " + wallSum +" M");
    }

    public void GetAllWindowArea()
    {
        float windowSum = 0;
        float h , w;
        for (int i = 0; i <selectedWallFace.RelatedLine.Windows.Count ; i++) 
        {
            h = selectedWallFace.RelatedLine.Windows [i].WindowHeight;
            w = selectedWallFace.RelatedLine.Windows [i].WindowWidth;
            windowSum += (h * w);
        }
        Debug.Log ("The area of all windows at this wall ="+ windowSum);
    }


    public void GetAllDoorArea()
    {
        float doorSum = 0;
        float h , w;
        for (int i = 0; i <selectedWallFace.RelatedLine.Doors.Count ; i++) 
        {
            h = selectedWallFace.RelatedLine.Doors [i].DoorHeight;
            w = selectedWallFace.RelatedLine.Doors [i].DoorWidth;
            doorSum += (h * w);
        }
        Debug.Log ("The area of all doors at this wall ="+ doorSum);
    }


    public void GetWallThickness()
    {
        float th = selectedWallFace.RelatedLine.Thickness;
        Debug.Log ("the thickness of this wall =" + th);
    }

}