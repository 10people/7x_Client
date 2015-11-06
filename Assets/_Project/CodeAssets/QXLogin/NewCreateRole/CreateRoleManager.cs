using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreateRoleManager : MonoBehaviour {

	public static CreateRoleManager roleManager;

	public GameObject selectRoleUIObj;
	private NewSelectRole selectRole;

	public GameObject createRole3DObj;//selectRoleUI's parent

	private List<GameObject> roleObjList = new List<GameObject>();
	private List<GameObject> shadowList = new List<GameObject> ();
	private int loadIndex;//导入的模型个数

	private Vector3 loadSize;

	private int curRole = -1;

	public enum TurnType
	{
		LEFT,
		RIGHT,
	}
	private TurnType turnType = TurnType.LEFT;//旋转方向

	void Awake ()
	{
		roleManager = this;
	}

	void Start ()
	{
		loadSize = Vector3.one;
		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.SELECT_ROLE ),
		                        SelectRoleUILoadBack );
	}

	void SelectRoleUILoadBack ( ref WWW p_www, string p_path, Object p_object )
	{
		selectRoleUIObj = Instantiate( p_object ) as GameObject;
		
		selectRoleUIObj.transform.parent = createRole3DObj.transform;
		selectRoleUIObj.transform.localPosition = Vector2.zero;
		selectRoleUIObj.transform.localScale = Vector3.one;

		RoleSelectAwake ();
	}

	void RoleSelectAwake()
	{
		loadIndex = 0;
		
		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.HAOJIE_CREATE_ROLE ),
		                        HaoJieLoadCallback );
		
		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.RUYA_CREATE_ROLE ),
		                        RuYaLoadCallback );
		
		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.YUJIE_CREATE_ROLE ),
		                        YuJieLoadCallback );
		
		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.LUOLI_CREATE_ROLE ),
		                        LuoLiLoadCallback );
	}

	void HaoJieLoadCallback( ref WWW p_www, string p_path,  Object p_object )
	{
		GameObject haojie = Instantiate( p_object ) as GameObject;

		haojie.name = "0";
		haojie.SetActive (false);
		haojie.transform.parent = transform;
		haojie.transform.localPosition = new Vector3(0,0.04f,0);
		haojie.transform.localScale = loadSize * 0.8f;

		roleObjList.Add ( haojie );

		AddLoadIndex ();
	}
	
	void RuYaLoadCallback( ref WWW p_www, string p_path,  Object p_object )
	{
		GameObject ruya = Instantiate( p_object ) as GameObject;

		ruya.name = "1";
		ruya.SetActive (false);
		ruya.transform.parent = transform;
		ruya.transform.localPosition = new Vector3(0,0.035f,0);
		ruya.transform.localScale = loadSize * 0.7f;

		roleObjList.Add ( ruya );
	
		AddLoadIndex ();
	}
	
	void YuJieLoadCallback( ref WWW p_www, string p_path,  Object p_object )
	{
		GameObject yujie = Instantiate( p_object ) as GameObject;

		yujie.name = "2";
		yujie.SetActive (false);
		yujie.transform.parent = transform;
		yujie.transform.localPosition = new Vector3(0,0.03f,0);
		yujie.transform.localScale = loadSize * 0.7f;

		roleObjList.Add ( yujie );

		AddLoadIndex ();
	}
	
	void LuoLiLoadCallback( ref WWW p_www, string p_path,  Object p_object )
	{
		GameObject luoli = Instantiate( p_object ) as GameObject;

		luoli.name = "3";
		luoli.SetActive (false);
		luoli.transform.parent = transform;
		luoli.transform.localPosition = new Vector3(0,0.03f,0);
		luoli.transform.localScale = loadSize * 0.7f;

		roleObjList.Add ( luoli );

		AddLoadIndex ();
	}

	void AddLoadIndex()
	{
		loadIndex ++;

		if(loadIndex == 4)
		{
			Init();
		}
	}

	void Init()
	{
		for (int i = 0;i < shadowList.Count;i ++)
		{

		}

		selectRole = selectRoleUIObj.GetComponent<NewSelectRole> ();

		selectRole.roleHanderList.ForEach(item => item.m_handler += Select);

//		Debug.Log ("可选人物角色数：" + selectRole.roleHanderList.Count);

		int ranNum = Random.Range (0,selectRole.roleHanderList.Count);

//		Debug.Log ("随机人物:" + ranNum);

		int roleHanderId = int.Parse (selectRole.roleHanderList [ranNum].name);

//		Debug.Log ("roleHanderId：" + roleHanderId);

		SelectResult(roleHanderId);     //刚进入创建人物场景随机一个人物

		//没有平台接入，随机一个名字
		selectRole.NameInIt ();
	}

	void Select (GameObject tempOjbect)
	{
//		Debug.Log("选择角色为：" + tempOjbect.name);
		if (int.Parse (tempOjbect.name) != curRole)
		{
			int tempSenderName = int.Parse (tempOjbect.name);
			
			SelectResult (tempSenderName);
		}
	}

	//选择人物
	void SelectResult (int whichRole)
	{	
		curRole = whichRole;
//		Debug.Log ("whichRole:" + whichRole);

		selectRole.RoleId = whichRole;
		selectRole.ShowNameLabel (whichRole);
		selectRole.RoleDes (whichRole);
		//显示选择框
		selectRole.roleSelectBox.transform.parent = selectRole.roleHanderList[whichRole].gameObject.transform;
		selectRole.roleSelectBox.transform.localPosition = Vector3.zero;

		CityGlobalData.m_king_model_Id = whichRole + 1;

//		Debug.Log ("RoleId:" + CityGlobalData.m_king_model_Id);

		for (int i = 0;i < roleObjList.Count;i ++)
		{
			if (roleObjList[i].name == whichRole.ToString ())
			{
				roleObjList[i].SetActive (true);

				RoleRotate roleRotate = roleObjList[i].GetComponent<RoleRotate> ();
				roleRotate.rotateObj.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

				Animator roleAnimat = roleObjList[i].GetComponent<Animator> ();
				roleAnimat.SetTrigger ("Show");

//				StopCoroutine ("ShowRoleAnim");
//				StartCoroutine ("ShowRoleAnim");
			}
			else
			{
				roleObjList[i].SetActive (false);
			}
		}
	}

	IEnumerator ShowRoleAnim ()
	{
		yield return new WaitForSeconds (10);
		Debug.Log ("hehehe");
		Animator roleAnimat = roleObjList[curRole].GetComponent<Animator> ();
		roleAnimat.SetTrigger ("Show");
	}

	//拖动旋转
	public void DragRotateRole (Vector2 delta)
	{
		RoleRotate roleRotate = roleObjList[curRole].GetComponent<RoleRotate> ();
		roleRotate.DragRotate (delta);
	}
	//点击旋转
	public void ClickRotate (RoleRotate.Direction direct)
	{
		RoleRotate roleRotate = roleObjList[curRole].GetComponent<RoleRotate> ();
		roleRotate.ClickRotate (direct);
	}
}
