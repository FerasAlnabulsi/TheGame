using System;
using UnityEngine;


public class WallWindow
{
	Line _line;
	public Line Line
	{
		get{
			return _line;
		}
		set{
			_line = value;
			Update ();
		}
	}

	Vector2 _position;
	public Vector2 Position
	{
		get{
			return _position;
		}
		set{
			_position = value;
			Update ();
		}
	}

	float _windowHeight;
	public float WindowHeight
	{
		get{
			return _windowHeight;
		}
		set{
			_windowHeight = value;
			Update ();
		}
	}

	float _windowWidth;
	public float WindowWidth
	{
		get{
			return _windowWidth;
		}
		set{
			_windowWidth = value;
			Update ();
		}
	}

	GameObject _window;
	public GameObject Window
	{
		get{
			return _window;
		}
		set{
			_window = value;
			Update ();
		}
	}
		
	public WallWindow (Line line, Vector2 position, float width, float height, GameObject windowObj)
	{
		_line = line;
		_position = position;
		_windowWidth = width;
		_windowHeight = height;
		_window = windowObj;
		Update ();
	}


	public void Update()
	{
		if (Window != null) {
			
			Vector3 start = _line.a + (_line.b - _line.a).normalized * (Position.x + WindowWidth * 0.5f);

			Vector3 lineDir = (_line.b - _line.a).normalized;
			float ang = -Mathf.Atan2 (lineDir.z, lineDir.x);

			start += Vector3.up * Position.y;

			Window.transform.localScale = new Vector3 (_windowWidth, _windowHeight, Line.Thickness);
			Window.transform.position = start;
			Window.transform.rotation = Quaternion.AngleAxis (Mathf.Rad2Deg * ang, Vector3.up);
		}
	}
}


