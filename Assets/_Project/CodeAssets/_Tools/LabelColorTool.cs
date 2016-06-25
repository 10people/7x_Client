using UnityEngine;
using System.Collections;

public class LabelColorTool : MonoBehaviour {

	public int m_ColorID;

	public int m_OutLineColorID = -1;

	// Use this for initialization
	void Start () {
		InItLabelColor ();
	}

	public void InItLabelColor ()
	{
		UILabel label = gameObject.GetComponent<UILabel>();
		
		if( label != null ){
			label.color = Global.getStringColor( MyColorData.getColor( m_ColorID ) );
			
			if( m_OutLineColorID != -1 ){
				label.effectColor = Global.getStringColor( MyColorData.getColor( m_OutLineColorID ) );
			}
		}
	}
}
