using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class UIYindao : MonoBehaviour, IUIRootAutoActivator {

	public static UIYindao m_UIYindao;
	public int m_iCurId;
	public bool m_isOpenYindao = false;
	public UIAtlas m_UIAtlas;
	public UISprite m_UISpriteCenter;
	public UISprite m_UISpriteTop;
	public UISprite m_UISpriteBom;
	public UISprite m_UISpriteLeft;
	public UISprite m_UISpriteRight;

	public UISprite m_UISpriteButton;
	public bool m_isQiangzhi;
	// Use this for initialization
	private iTween[] m_iTween;
	private bool m_isMoveOne = true;
	private Vector3 m_v3MoveB;
	private Vector3 m_v3MoveE;
	private YindaoTemp.YindaoElenemt m_YindaoElenemt;

	private int m_iImageIndex = 0;

    public int m_iIsColl = 0;

	private bool m_isAddSound = false;

	private int m_iNum = 0;

	public int m_iCurdialogId = -1;
	public int m_iPardialogId = -1;
	
	private struct moveData
	{
		public UISprite m_SprteImage;
		public UILabel m_Label;
		public string name;
		public int type;
		public bool ismove;
		public int speedx;
		public int speedy;
		public int moveend;
		public int curmove;
		public int x;
		public int y;
	}
	
	private List<moveData> m_listMoveData = new List<moveData>();
	private List<UISprite> m_listImage = new List<UISprite>();


	void Awake(){
		if( m_UIYindao != null ){
			gameObject.SetActive( false );

			Destroy( gameObject );

			return;
		}

		{
			UIRootAutoActivator.RegisterAutoActivator( this );

			ComponentHelper.AddIfNotExist( m_UISpriteCenter, typeof(FreshGuideMaskEffect) );

			ComponentHelper.AddIfNotExist( m_UISpriteLeft, typeof(FreshGuideMaskEffect) );

			ComponentHelper.AddIfNotExist( m_UISpriteRight, typeof(FreshGuideMaskEffect) );

			ComponentHelper.AddIfNotExist( m_UISpriteBom, typeof(FreshGuideMaskEffect) );

			ComponentHelper.AddIfNotExist( m_UISpriteTop, typeof(FreshGuideMaskEffect) );
		}
	}

	void Start (){

		m_UIYindao = this;

		DontDestroyOnLoad( gameObject );
	}
	
	// Update is called once per frame
	void Update () 
	{
		m_iNum ++;
		if(m_iNum == 150)
		{
			if(m_iCurId >= 20501 && m_iCurId <= 20900)
			{
				if(m_YindaoElenemt.m_ImageMove != null)
				{
					m_listMoveData[0].m_SprteImage.gameObject.SetActive(false);
				}
			}
		}
	}

	void OnDestroy(){
		{
			UIRootAutoActivator.UnregisterAutoActivator( this );
		}
	}

	private string m_sPos = "c";
	private int cur_YinDaoid = 0;
	/// <summary>
	/// 判断引导是否开启  如果开启就关闭并获得关闭前的引导id
	/// </summary>
	/// <returns><c>true</c> if this instance is O pen Y in DAO; otherwise, <c>false</c>.</returns>
	public void  IsOPenYInDao()
	{
	   if(m_UIYindao.m_isOpenYindao)
		{
			cur_YinDaoid = m_iCurId;
			m_UIYindao.CloseUI();

		}
	}
	/// <summary>
	/// 如果之前引导是打开的 就打开之前的引导
	/// </summary>
	/// <returns><c>true</c> if this instance is O pen Y in DAO; otherwise, <c>false</c>.</returns>
	public void  NeedOPenYInDao( )
	{
		if(cur_YinDaoid > 0)
		{
			setOpenYindao(cur_YinDaoid);
			cur_YinDaoid = -1;
		}
	}
	public void setOpenYindao(int id)
    {
//		Debug.Log(id);
		m_iNum = 0;
	//cancel yindao if in house.
//        if(id < 200000)
//        {
//            return;
//        }
		if(!Global.m_isOpenJiaoxue)
		{
			return;
		}
		m_iCurId = id;

//		Debug.Log(m_iCurId);

		if(id == 0)
		{
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
			return;
		}
	    if (MainCityUI.m_PlayerPlace != MainCityUI.PlayerPlace.MainCity)
	    {
	        Debug.LogWarning("Cancel set open yindao cause not in main city.");

	        return;
	    }

//		Debug.Log( "--------- SetOpenYindao: " + id + " : " + TaskData.Instance.m_iCurMissionIndex + " ---------   " );

//		Debug.Log( TaskData.Instance.m_iCurMissionIndex );

		if( FreshGuide.Instance().IsActive( TaskData.Instance.m_iCurMissionIndex ) ){
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];

//			Debug.Log(tempTaskData.m_iCurIndex);
		}

//		FreshGuide.Instance().LogActiveTask();
		if( MainCityUIRB.isOpen&&(id == 603 || id == 6030)){
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];

			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);

			return;
		}

		if (MainCityUIRB.isOpen&&(id == 604 || id == 6040))
        {
			ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[TaskData.Instance.m_iCurMissionIndex];
			
			UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[tempTaskData.m_iCurIndex++]);
			
			return;
		}

		if(YindaoEditor.m_open_guide_editor)
		{
			YindaoEditor.YINDAOEDITOR.setCurYindao(id);
		}

//		int index = 1;
		if(m_isOpenYindao)
		{

		}
		for (int i = 0; i < YindaoTemp.m_listYindaoElenemt.Count; i++)
		{
			if (id == YindaoTemp.m_listYindaoElenemt[i].m_ID)
			{
				setOpenYindao(YindaoTemp.m_listYindaoElenemt[i]);
				return;
			}
		}
	}

	public void setOpenYindao(YindaoTemp.YindaoElenemt yindaoElenemt)
	{
//		if(YindaoEditor.m_open_guide_editor)
//		{
//			YindaoEditor.YINDAOEDITOR.setCurYindao(yindaoElenemt.m_ID);
//		}
		if(!Global.m_isOpenJiaoxue)
		{
			return;
		}
		if(m_isOpenYindao)
		{
			CloseUI();
		}
		iTween[] tempiTewwn = gameObject.GetComponents<iTween>();
		for(int i = 0; i < tempiTewwn.Length; i ++)
		{
			Destroy(tempiTewwn[i]);
		}
		
		for(int i = 0; i < m_listImage.Count; i ++)
		{
			GameObject.Destroy(m_listImage[i].gameObject);
		}
		
		for(int i = 0; i < m_listMoveData.Count; i ++)
		{
			GameObject.Destroy(m_listMoveData[i].m_SprteImage.gameObject);
		}
		
		if(m_YindaoElenemt.m_Eff != null)
		{
			if(m_YindaoElenemt.m_Eff.eff != null)
			{
				GameObject.Destroy(m_YindaoElenemt.m_Eff.eff);
				m_YindaoElenemt.m_Eff.eff = null;
			}
		}

		m_listImage = new List<UISprite>();
		
		m_listMoveData = new List<moveData>();

		m_YindaoElenemt = yindaoElenemt;

		m_isOpenYindao = true;

		int x = 0;
		int y = 0;
		int w = 0;

		if(yindaoElenemt.m_Label != null)
		{
			m_sPos = yindaoElenemt.m_Label.pos;
			m_iCurdialogId = yindaoElenemt.m_Label.dialogid;
			x = yindaoElenemt.m_Label.x;
			y = yindaoElenemt.m_Label.y;
			w = yindaoElenemt.m_Label.w;
			//			string str = yindaoElenemt.m_Label.str;
			if(m_iPardialogId != m_iCurdialogId)
			{
				ClientMain.m_ClientMain.m_UIDialogSystem.setOpenDialogID(m_iCurdialogId, x, y , w, 99999, ClickDialogOver);
			}
		}
		else
		{
			m_iPardialogId = -1;
		}
		if(yindaoElenemt.m_Click != null)
		{
            m_iIsColl = yindaoElenemt.m_Click.isColl;
			if(yindaoElenemt.m_Click.isColl == 0)
			{
				m_isQiangzhi = true;
				m_UISpriteCenter.gameObject.SetActive(true);
				m_UISpriteTop.gameObject.SetActive(true);
				m_UISpriteBom.gameObject.SetActive(true);
				m_UISpriteLeft.gameObject.SetActive(true);
				m_UISpriteRight.gameObject.SetActive(true);
			}
			else
			{
				m_isQiangzhi = false;
				m_UISpriteCenter.gameObject.SetActive(false);
				m_UISpriteTop.gameObject.SetActive(false);
				m_UISpriteBom.gameObject.SetActive(false);
				m_UISpriteLeft.gameObject.SetActive(false);
				m_UISpriteRight.gameObject.SetActive(false);
			}
			int type = yindaoElenemt.m_Click.type;
			m_sPos = yindaoElenemt.m_Click.pos;
			if(YindaoEditor.m_open_guide_editor)
			{
				m_UISpriteCenter.spriteName = "edittype" + type;
				m_UISpriteLeft.gameObject.GetComponent<UISprite>().spriteName = "editrect";
				m_UISpriteRight.gameObject.GetComponent<UISprite>().spriteName = "editrect";
				m_UISpriteTop.gameObject.GetComponent<UISprite>().spriteName = "editrect";
				m_UISpriteBom.gameObject.GetComponent<UISprite>().spriteName = "editrect";
			}
			else
			{
				m_UISpriteCenter.spriteName = "edittype" + type;
				m_UISpriteLeft.gameObject.GetComponent<UISprite>().spriteName = "editrect";
				m_UISpriteRight.gameObject.GetComponent<UISprite>().spriteName = "editrect";
				m_UISpriteTop.gameObject.GetComponent<UISprite>().spriteName = "editrect";
				m_UISpriteBom.gameObject.GetComponent<UISprite>().spriteName = "editrect";
			}
			x = yindaoElenemt.m_Click.x;
			y = yindaoElenemt.m_Click.y;
//			m_sX = x +"";
//			m_sY = y +"";
			x -= 480;
			y = 320 - y;
			getPos(ref x, ref y, m_sPos);
			w = yindaoElenemt.m_Click.w;
			int h = yindaoElenemt.m_Click.h;
//			m_sW = w +"";
//			m_sH = h +"";

			m_UISpriteCenter.gameObject.transform.localPosition = new Vector3(x, y, 0);
			m_UISpriteCenter.gameObject.GetComponent<UISprite>().SetDimensions(w + 2, h + 2 );
			
			m_UISpriteTop.gameObject.transform.localPosition = new Vector3(x, y + h / 2, 0);
			m_UISpriteBom.gameObject.transform.localPosition = new Vector3(x, y - h / 2, 0);
			m_UISpriteLeft.gameObject.transform.localPosition = new Vector3(x - w / 2, y, 0);
			m_UISpriteRight.gameObject.transform.localPosition = new Vector3(x + w / 2, y, 0);
			
			int tempCollW = 4096;
			int tempCollH = 4096;
			m_UISpriteTop.gameObject.GetComponent<UISprite>().SetDimensions(tempCollW,tempCollH);
			m_UISpriteBom.gameObject.GetComponent<UISprite>().SetDimensions(tempCollW,tempCollH);
			m_UISpriteLeft.gameObject.GetComponent<UISprite>().SetDimensions(tempCollW,h+2);
			m_UISpriteRight.gameObject.GetComponent<UISprite>().SetDimensions(tempCollW,h+2);
			
			m_UISpriteTop.gameObject.GetComponent<BoxCollider>().size = new Vector3(tempCollW,tempCollH,1);
			m_UISpriteTop.gameObject.GetComponent<BoxCollider>().center = new Vector3(0,(tempCollH / 2),0);
			
			m_UISpriteBom.gameObject.GetComponent<BoxCollider>().size = new Vector3(tempCollW,tempCollH,1);
			m_UISpriteBom.gameObject.GetComponent<BoxCollider>().center = new Vector3(0,-(tempCollH / 2),0);
			
			m_UISpriteLeft.gameObject.GetComponent<BoxCollider>().size = new Vector3(tempCollW,h,1);
			m_UISpriteLeft.gameObject.GetComponent<BoxCollider>().center = new Vector3(-(tempCollW / 2),0,0);
			
			m_UISpriteRight.gameObject.GetComponent<BoxCollider>().size = new Vector3(tempCollW,h,1);
			m_UISpriteRight.gameObject.GetComponent<BoxCollider>().center = new Vector3((tempCollW / 2),0,0);
		}
		else
		{
			m_UISpriteCenter.gameObject.SetActive(false);
			m_UISpriteTop.gameObject.SetActive(false);
			m_UISpriteBom.gameObject.SetActive(false);
			m_UISpriteLeft.gameObject.SetActive(false);
			m_UISpriteRight.gameObject.SetActive(false);
		}
		if(yindaoElenemt.m_ImageMove != null)
		{
			moveData tempMove = new moveData();
			string imagename = yindaoElenemt.m_ImageMove.name;
			m_sPos = yindaoElenemt.m_ImageMove.pos;
			int angle = yindaoElenemt.m_ImageMove.Angle;
			int desID = yindaoElenemt.m_ImageMove.desID;
			x = yindaoElenemt.m_ImageMove.x;
			y = yindaoElenemt.m_ImageMove.y;
			x -= 480;
			y = 320 - y;
			getPos(ref x, ref y, m_sPos);
			tempMove.x = x;
			tempMove.y = y;
			tempMove.name = imagename;
			tempMove.ismove = true;
			tempMove.type = yindaoElenemt.m_ImageMove.type;
			tempMove.speedx = yindaoElenemt.m_ImageMove.speedx;
			tempMove.speedy = yindaoElenemt.m_ImageMove.speedy;
			tempMove.moveend = yindaoElenemt.m_ImageMove.moveend;
			GameObject tempObj = (GameObject.Instantiate(m_UISpriteButton.gameObject) as GameObject);
//			Debug.Log(YindaoEditor.m_open_guide_editor);

			tempObj.transform.parent = m_UISpriteButton.transform.parent;
			tempObj.transform.localScale = Vector3.one;
			tempObj.transform.localPosition = new Vector3(x, y, 0);
//			m_curYindaoElenemt.m_ImageMove.Angle = 2;
			Vector3 tempaaaa= new Vector3((angle & 1) != 0 ? 180 : 0,(angle & 2) != 0 ? 180 : 0,0);
			tempObj.transform.localRotation = Quaternion.Euler(tempaaaa);
//			m_SoundPlayEff = tempObj.GetComponentInChildren<SoundPlayEff>();
			tempMove.m_Label = tempObj.GetComponentInChildren<UILabel>();
			tempMove.m_Label.gameObject.transform.localRotation = Quaternion.Euler(new Vector3(-tempaaaa.x, -tempaaaa.y, 0));
			if(yindaoElenemt.m_ImageMove.desID > 0)
			{
				tempMove.m_Label.text = DialogData.getDialog(yindaoElenemt.m_ImageMove.desID)[0].sDialogData;
				if(!m_isAddSound)
				{
					m_isAddSound = true;
					ClientMain.m_ClientMain.m_SoundPlayEff.PlaySound(DialogData.getDialog(yindaoElenemt.m_ImageMove.desID)[0].sDialogSoundID);
				}
			}
			tempMove.m_SprteImage = tempObj.GetComponent<UISprite>();
			tempMove.m_SprteImage.spriteName = tempMove.name;
//			tempMove.m_SprteImage.gameObject.transform.localRotation = Quaternion.Euler(0,0,angle);
			tempMove.m_SprteImage.SetDimensions(300,100);
			m_iImageIndex++;
			m_listMoveData.Add(tempMove);

			if(yindaoElenemt.m_Label == null || m_iPardialogId == m_iCurdialogId)
			{
				tempObj.SetActive(true);
			}
			else if(!YindaoEditor.m_open_guide_editor)
			{
				tempObj.SetActive(false);
			}
			else
			{
				tempObj.SetActive(true);
			}
		}
		if(yindaoElenemt.m_Eff != null)
		{
			if(yindaoElenemt.m_Label == null || m_iPardialogId == m_iCurdialogId)
			{
				createEff();
			}
		}

		Refresh();
	}

	public void createEff()
	{
		m_YindaoElenemt.m_Eff.eff = new GameObject();
		m_YindaoElenemt.m_Eff.eff.name = "eff";
		
		int x = m_YindaoElenemt.m_Eff.x;
		int y = m_YindaoElenemt.m_Eff.y;
		x -= 480;
		y = 320 - y;
		getPos(ref x, ref y, m_YindaoElenemt.m_Eff.pos);
		
		m_YindaoElenemt.m_Eff.eff.transform.parent = m_UISpriteButton.transform.parent;
		
		m_YindaoElenemt.m_Eff.eff.layer = LayerMask.NameToLayer("NGUI");
		
		m_YindaoElenemt.m_Eff.eff.transform.localPosition = new Vector3(x, y, 0);

		if( UI3DEffectTool.Instance() == null ){
			Debug.LogError( UI3DEffectTool.Instance() );

			Debug.LogError( m_YindaoElenemt.m_Eff );
		
			Debug.LogError( EffectTemplate.getEffectTemplateByEffectId( m_YindaoElenemt.m_Eff.id ) );
		}

		UI3DEffectTool.ShowTopLayerEffect(UI3DEffectTool.UIType.PopUI_2, m_YindaoElenemt.m_Eff.eff, EffectTemplate.getEffectTemplateByEffectId( m_YindaoElenemt.m_Eff.id ).path);
	}

	public void MoveImage(Vector3 tempData)
	{
		for(int i = 0; i < m_listMoveData.Count; i ++)
		{
			moveData temp = m_listMoveData[i];
			temp.m_SprteImage.gameObject.transform.localPosition = tempData;
		}
	}

	public void getPos(ref int x, ref int y, string pos)
	{
		if(pos.IndexOf("l") != -1 || pos.IndexOf("L") != -1 )
		{
			x -= ClientMain.m_iMoveX;
		}
		else if(pos.IndexOf("r") != -1 || pos.IndexOf("R") != -1 )
		{
			x += ClientMain.m_iMoveX;
		}
		if(pos.IndexOf("t") != -1 || pos.IndexOf("T") != -1 )
		{
			y += ClientMain.m_iMoveY;
		}
		else if(pos.IndexOf("b") != -1 || pos.IndexOf("B") != -1 )
		{
			y -= ClientMain.m_iMoveY;
		}
	}

	public void ClickDialogOver()
	{
		if(m_listMoveData.Count > 0)
		{
			m_listMoveData[0].m_SprteImage.gameObject.SetActive(true);
		}
		if(m_YindaoElenemt.m_Eff != null)
		{
			createEff();
		}
		m_iPardialogId = m_iCurdialogId;
	}

	public void CloseUI()
	{
//        Debug.Log("CloseYinDao");
        if (!Global.m_isOpenJiaoxue)
		{
			return;
		}
		m_isAddSound = false;
		m_UISpriteCenter.gameObject.SetActive(false);
		m_UISpriteTop.gameObject.SetActive(false);
		m_UISpriteBom.gameObject.SetActive(false);
		m_UISpriteLeft.gameObject.SetActive(false);
		m_UISpriteRight.gameObject.SetActive(false);
		for(int i = 0; i < m_listMoveData.Count; i ++)
		{
			m_listMoveData[i].m_SprteImage.gameObject.SetActive(false);
		}
		for(int i = 0; i < m_listImage.Count; i ++)
		{
			m_listImage[i].gameObject.SetActive(false);
		}
		if(ClientMain.m_ClientMain.m_UIDialogSystem != null)
		{
			ClientMain.m_ClientMain.m_UIDialogSystem.CloseDialog();
		}
		if(m_YindaoElenemt.m_Eff != null)
		{
			if(m_YindaoElenemt.m_Eff.eff != null)
			{
				GameObject.Destroy(m_YindaoElenemt.m_Eff.eff);
				m_YindaoElenemt.m_Eff.eff = null;
			}
		}
		m_isOpenYindao = false;
	}

	public void CloseJiantou()
	{
		if(m_isOpenYindao)
		{
			for(int i = 0; i < m_listMoveData.Count; i ++)
			{
				m_listMoveData[i].m_SprteImage.gameObject.SetActive(false);
			}
		}
	}

	public void EndDialog()
	{
		if(m_isOpenYindao)
		{
			for(int i = 0; i < m_listMoveData.Count; i ++)
			{
				m_listMoveData[i].m_SprteImage.gameObject.SetActive(true);
			}
		}
	}

	public void setOpenUIEff()
	{
		if(m_isOpenYindao)
		{
			if(m_YindaoElenemt.m_Eff != null)
			{
				if(m_YindaoElenemt.m_Eff.eff != null)
				{
					m_YindaoElenemt.m_Eff.eff.SetActive(true);
					UI3DEffectTool.ShowTopLayerEffect(UI3DEffectTool.UIType.PopUI_2, m_YindaoElenemt.m_Eff.eff, EffectTemplate.getEffectTemplateByEffectId( m_YindaoElenemt.m_Eff.id ).path);
				}
			}
		}
	}

	public void setCloseUIEff()
	{
		if(m_isOpenYindao)
		{
			if(m_YindaoElenemt.m_Eff != null)
			{
				if(m_YindaoElenemt.m_Eff.eff != null)
				{
					m_YindaoElenemt.m_Eff.eff.SetActive(false);
					UI3DEffectTool.ClearUIFx(m_YindaoElenemt.m_Eff.eff);
				}
			}
		}
	}

	#region IUIRootAutoActivator
	
	public bool IsNGUIVisible(){
		return m_isOpenYindao;
	}
	
	#endregion



	#region Utilities

	private void Refresh(){
		m_UISpriteCenter.gameObject.SetActive( !m_UISpriteCenter.gameObject.activeSelf );
		m_UISpriteCenter.gameObject.SetActive( !m_UISpriteCenter.gameObject.activeSelf );

		m_UISpriteLeft.gameObject.SetActive( !m_UISpriteLeft.gameObject.activeSelf );
		m_UISpriteLeft.gameObject.SetActive( !m_UISpriteLeft.gameObject.activeSelf );

		m_UISpriteRight.gameObject.SetActive( !m_UISpriteRight.gameObject.activeSelf );
		m_UISpriteRight.gameObject.SetActive( !m_UISpriteRight.gameObject.activeSelf );

		m_UISpriteBom.gameObject.SetActive( !m_UISpriteBom.gameObject.activeSelf );
		m_UISpriteBom.gameObject.SetActive( !m_UISpriteBom.gameObject.activeSelf );

		m_UISpriteTop.gameObject.SetActive( !m_UISpriteTop.gameObject.activeSelf );
		m_UISpriteTop.gameObject.SetActive( !m_UISpriteTop.gameObject.activeSelf );
	}

	#endregion
}