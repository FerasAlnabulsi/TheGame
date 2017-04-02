using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum type
{
	Door,
	Window,
	Wall,
	Furniture,
    System,
    Roof
}

[System.Serializable]
public class item {

	public string itemName;
	public Sprite image;
	public type itemType;
	public bool alignToWall;
	public bool alignToCeil;
	public bool alignToFloor;

	public float HorizontalMargin;
	public float MarginTop;
	public float MarginDown;

	public string description;
	public PrefabItem prefabItem;
}