using UnityEngine;
using System.Collections;

public class PveEnemyIfon : MonoBehaviour {

	public GameObject Bosstips;
	public UITexture enemyIc;
	public UISprite Enemy_Type;
	[HideInInspector]public string enemyIcom = "Enemy1";//资源未到位先用一张替代。。
	[HideInInspector]public string EnemyName;
	[HideInInspector]public string enemyYuanSu;
	[HideInInspector]public int EnemyLv;
	[HideInInspector]public int EnemyType;
	[HideInInspector]public int EnemyId;
	public GameObject enemyintruduce;
//	bool canpopinfo = true;
	public bool BossTips;
	GameObject showname;
	public void LoadResourceCallback(ref WWW p_www,string p_path, Object p_object)
	{
		enemyIc.mainTexture = (Texture)p_object;
	}
	public void Init()
	{
//		Debug.Log ("thisPos:" + this.transform.localPosition);
//		Debug.Log ("thisType:" + EnemyType);
		if(BossTips)
		{
			Bosstips.SetActive(true);
		}else{
			Bosstips.SetActive(false);
		}
	
		Global.ResourcesDotLoad (Res2DTemplate.GetResPath (Res2DTemplate.Res.PVE_ENEMY_1),LoadResourceCallback);

		if(EnemyType == 11)
		{
			Enemy_Type.spriteName = "dun";//盾
		}
		else if(EnemyType == 12)
		{
			Enemy_Type.spriteName = "piccar";//车
		}
		else if(EnemyType == 13)
		{
			Enemy_Type.spriteName = "gong";//弓箭
		}
		else if(EnemyType == 14)
		{
			Enemy_Type.spriteName = "hours";//Horse////........马
		}
		else{

			Enemy_Type.gameObject.SetActive(false);
		}

		Enemy_Type.spriteName = "gong";

//		Debug.Log ("初始化完成");
	}
	void Start () {
	
	}

	public void PoPEnemyInfo(){
		StartCoroutine( showinfo() );
	}

	IEnumerator showinfo(){

		if(showname == null){
			showname = Instantiate (enemyintruduce ) as GameObject;
			showname.SetActive(true);
			showname.transform.parent = this.transform;

			showname.transform.localScale = new Vector3 (1,1,1);

			showname.transform.localPosition = new Vector3 (0,105,0);

			Destroynametitle Eneminfo = showname.GetComponent<Destroynametitle>();

			Eneminfo.M_Name = EnemyName;

			Eneminfo.m_enemyid = EnemyId;

			Eneminfo.M_Level = EnemyLv;

			Eneminfo.M_YuanSu = enemyYuanSu;
			Eneminfo.init();

			yield return new WaitForSeconds(0.5f);
		
		}

		else
		{
			yield return new WaitForSeconds(0.2f);
			Destroy(showname);
		}

	}

}
