using UnityEngine;
using System.Collections;

public class Destroynametitle : MonoBehaviour {

	public float mytime;
	public GameObject eName;
	public GameObject eLevel;
	public GameObject[] Enemyuansu;
	public GameObject yuansuParet;

	[HideInInspector]public string M_Name;
	[HideInInspector]public string M_YuanSu;
	[HideInInspector]public int M_Level;
	[HideInInspector]public int m_enemyid;
	public UILabel desc ;
	UISprite[] yuansu;
	public void init()
	{
		UILabel Enemyname = eName.GetComponent<UILabel> ();
		Enemyname.text = M_Name;
		UILabel enemyLevel = eLevel.GetComponent<UILabel> ();
		enemyLevel.text = "LV "+M_Level.ToString();

		DescIdTemplate mDescIdTmplate = DescIdTemplate.getDescIdTemplateByNameId (m_enemyid);
		//Debug.Log ("mDescIdTmplate.description   " + mDescIdTmplate.description);
		
		desc.text = mDescIdTmplate.description;


	}


}
