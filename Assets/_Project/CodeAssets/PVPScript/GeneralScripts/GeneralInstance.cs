using UnityEngine;

public class GeneralInstance<T> : MYNGUIPanel where T : MYNGUIPanel 
{
	public static T m_instance;
	
	public void Awake ()
	{
		m_instance = (T)FindObjectOfType(typeof(T));
	}
	
	public void OnDestroy ()
	{
		m_instance = null;
	}

	public override void MYClick (GameObject ui) {}
	public override void MYMouseOver(GameObject ui) {}
	public override void MYMouseOut(GameObject ui) {}
	public override void MYPress(bool isPress, GameObject ui) {}
	public override void MYelease(GameObject ui) {}
	public override void MYondrag(Vector2 delta) {}
	public override void MYoubleClick(GameObject ui) {}
	public override void MYonInput(GameObject ui, string c) {}
}