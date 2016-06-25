using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraDivisionControllor : MonoBehaviour 
{
	public enum LayerIndex
	{
		Ground = 1024,		//grounded
		Model = 8192,		//3D Layer WithoutLight
		UI3D = 2097152,		//real carrige
		Fx = 256,			//3D Layer
		Player = 512,		//Drag Collide Layer
	}

	public bool inWorking = false;


	private Dictionary<LayerIndex, Camera> cameraDict = new Dictionary<LayerIndex, Camera>();

	private Dictionary<LayerIndex, bool> cameraOnshow = new Dictionary<LayerIndex, bool> ();


	private static CameraDivisionControllor _instance;
	
	
	void Awake()
	{
		_instance = this; 

		inWorking = ConfigTool.GetBool( ConfigTool.CONST_SHOW_CAMERA_DIVITION_OPS);
	}
	
	public static CameraDivisionControllor Instance() 
	{ 
		return _instance; 
	}

	void OnDestroy()
	{
		_instance = null;
	}

	public void init () 
	{
		if (cameraDict.Count > 0) return;

		Camera[] cl = gameObject.GetComponentsInChildren<Camera>();

		foreach(Camera camera in cl)
		{
			if(camera.cullingMask == (int)LayerIndex.Ground + (int)LayerIndex.Model)
			{
				if(!cameraDict.ContainsKey(LayerIndex.Ground)) cameraDict.Add(LayerIndex.Ground, camera);

				if(!cameraDict.ContainsKey(LayerIndex.Model)) cameraDict.Add(LayerIndex.Model, camera);
			}
			else if(camera.cullingMask == (int)LayerIndex.Fx + (int)LayerIndex.Player)
			{
				if(!cameraDict.ContainsKey(LayerIndex.Fx)) cameraDict.Add(LayerIndex.Fx, camera);

				if(!cameraDict.ContainsKey(LayerIndex.Player)) cameraDict.Add(LayerIndex.Player, camera);
			}
			else if(camera.cullingMask == (int)LayerIndex.UI3D)
			{
				if(!cameraDict.ContainsKey(LayerIndex.UI3D)) cameraDict.Add(LayerIndex.UI3D, camera);
			}
		}

		if(!cameraOnshow.ContainsKey(LayerIndex.Fx)) cameraOnshow.Add (LayerIndex.Fx, true);

		if(!cameraOnshow.ContainsKey(LayerIndex.Ground)) cameraOnshow.Add (LayerIndex.Ground, true);

		if(!cameraOnshow.ContainsKey(LayerIndex.Model)) cameraOnshow.Add (LayerIndex.Model, true);

		if(!cameraOnshow.ContainsKey(LayerIndex.Player)) cameraOnshow.Add (LayerIndex.Player, true);

		if(!cameraOnshow.ContainsKey(LayerIndex.UI3D)) cameraOnshow.Add (LayerIndex.UI3D, true);
	}

	public void changeLayer(LayerIndex layer)
	{
		bool onshow = cameraOnshow [layer];

		if(onshow)
		{
			minusLayer(layer);
		}
		else
		{
			addLayer(layer);
		}

		cameraOnshow [layer] = !onshow;
	}

	private void addLayer(LayerIndex layer)
	{
		Camera tempCamera = null;

		cameraDict.TryGetValue (layer, out tempCamera);

		if (tempCamera == null)
		{
			Debug.LogError ("addLayer Can't get camera with Layer " + layer);

			return;
		}

		tempCamera.cullingMask += (int)layer;
	}

	private void minusLayer(LayerIndex layer)
	{
		Camera tempCamera = null;
		
		cameraDict.TryGetValue (layer, out tempCamera);
		
		if (tempCamera == null)
		{
			Debug.LogError ("minusLayer Can't get camera with Layer " + layer);
			
			return;
		}
		
		tempCamera.cullingMask -= (int)layer;
	}

	public void end()
	{
//		foreach(LayerIndex culling in cameraOnshow.Keys)
//		{
//			bool onshow = cameraOnshow[culling];
//
//			if(!onshow)
//			{
//				changeLayer(culling);
//			}
//		}
	}

}
