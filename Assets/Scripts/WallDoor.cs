using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WallDoor : WallWindow
{
	public float DoorHeight
	{
		get{
			return base.WindowHeight;
		}
		set{
			base.WindowHeight = value;
		}
	}

	public float DoorWidth
	{
		get{
			return base.WindowWidth;
		}
		set{
			base.WindowWidth = value;
		}
	}

	public GameObject Door
	{
		get{
			return base.Window;
		}
		set{
			base.Window = value;
		}
	}

	public WallDoor (Line line, float position, float width, float height, GameObject doorObj) : base(line, new Vector2(position, 0), width, height, doorObj)
	{
	}
}

