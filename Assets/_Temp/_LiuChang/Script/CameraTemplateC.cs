using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraTemplateC : MonoBehaviour
{

	private List<cameraValue> tempList = new List<cameraValue> ();


	void Start () 
	{
		tempList.Clear ();

		PlotChatTemplate.LoadTemplates( TemplateLoadedCallback );
	}
	
	private void TemplateLoadedCallback()
	{
		foreach(PlotChatTemplate template in PlotChatTemplate.GetTemplates() )
		{
			Vector3 t_po = new Vector3(template.cameraPx, template.cameraPy, template.cameraPz);

			Vector3 t_ro = new Vector3(template.cameraRx, template.cameraRy, 0).normalized;

			bool check = checkList(t_po, t_ro);

			if(check == true)
			{

			}
			else
			{
				tempList.Add(new cameraValue(t_po, t_ro.normalized));
			}
		}

//		Debug.Log ("PlotChatTemplate Length: " + PlotChatTemplate.templates.Count);

//		Debug.Log ("tempList Length: " + tempList.Count);
	}

	private bool checkList(Vector3 po, Vector3 ro)
	{
//		Debug.Log ("=======================================");

		foreach(cameraValue ca in tempList)
		{
			float lengthP = Vector3.Distance(po, ca.positon);

			float lengthR = Vector3.Distance(ro, ca.rotation);

			Debug.Log("Values: " + po + ", " + ca.positon + " ;;;;;;;;;;;   " + ro + ", " + ca.rotation);

			if(lengthP < .2f && lengthR < .05f)
			{
				return true;
			}
		}

		return false;
	}

}

class cameraValue
{
	public Vector3 positon;

	public Vector3 rotation;

	public cameraValue(Vector3 _po, Vector3 _ro)
	{
		positon = _po;

		rotation = _ro;
	}
}
