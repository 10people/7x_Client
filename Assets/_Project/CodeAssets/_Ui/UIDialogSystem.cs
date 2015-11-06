
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class UIDialogSystem : MonoBehaviour, IUIRootAutoActivator
{
	private int m_iCurentDialogIndex;
	private int m_iCurentNum;
	private int m_iMissionID = -1;
	private List<DialogData.dialogData> m_dialogData;
	public UISprite sprite;
	public UISprite spriteUp;
	public UILabel label;
	public UILabel m_sName;
	public GameObject m_labelAG;
	public UITexture avatar;
	private string m_sCutDialogData;
	private int i_Num = 0;
	private bool m_isNextDialog = false;
	private float m_fTime;
	private float m_fCreateTime;
	private const int m_iMoveX = 100;

	public delegate void onclick();
	
	public onclick m_onclick;

	private DateTime m_FTimeNew;

	void Awake(){
		{
			UIRootAutoActivator.RegisterAutoActivator( this );
		}
	}

	void Start () 
	{

	}

	void Update () 
	{
//		headStream.Write(head, 0, head.Length);
//		headStream.Position = 0;
//		int bodyLen = headStream.Read(head, 0, 4);
//		int cmd = headStream.Read(head, 4, 4);

//		aa.re
		if(!m_isNextDialog)
		{
			i_Num ++;
			if(i_Num > m_sCutDialogData.Length)
			{
				m_isNextDialog = true;
				m_labelAG.gameObject.SetActive(true);

				if(gameObject.GetComponent<iTween>() == null)
				{
					iTween.ValueTo(gameObject, iTween.Hash(
						"name", "movejiantou",
						"from", m_labelAG.transform.localPosition,
						"to", new Vector3(m_labelAG.transform.localPosition.x, m_labelAG.transform.localPosition.y - 10, m_labelAG.transform.localPosition.z),
						"delay", 0,
						"time", 0.3,
						"easeType", iTween.EaseType.easeOutCubic,
						"onupdate", "MoveImage",
						"LoopType", "pingPong"
						));
				}
			}
			else
			{
				while(m_sCutDialogData.ToCharArray()[i_Num -1] == '[')
				{
					i_Num = m_sCutDialogData.IndexOf("]", i_Num) + 2;
					if(i_Num >= m_sCutDialogData.Length)
					{
						i_Num = m_sCutDialogData.Length;
						break;
					}
				}
				label.text = m_sCutDialogData.Substring(0, i_Num);
			}
		}
		if(gameObject.activeSelf)
		{
//			Debug.Log(Time.realtimeSinceStartup);
//			Debug.Log(m_fTime);
//			Debug.Log(m_fCreateTime);
			if(Time.realtimeSinceStartup - m_fCreateTime > m_fTime)
			{
				OnPress(true);
			}
		}
	}

	void OnDestroy(){
		avatar = null;

		{
			UIRootAutoActivator.UnregisterAutoActivator( this );
		}
	}

	public void EndDialog()
	{
		label.text = m_sCutDialogData.Substring(0, m_sCutDialogData.Length);
		i_Num = m_sCutDialogData.Length + 1;
		if(i_Num > m_sCutDialogData.Length)
		{
			m_isNextDialog = true;
			m_labelAG.gameObject.SetActive(true);
			
			if(gameObject.GetComponent<iTween>() == null)
			{
				iTween.ValueTo(gameObject, iTween.Hash(
					"name", "movejiantou",
					"from", m_labelAG.transform.localPosition,
					"to", new Vector3(m_labelAG.transform.localPosition.x, m_labelAG.transform.localPosition.y - 10, m_labelAG.transform.localPosition.z),
					"delay", 0,
					"time", 0.3,
					"easeType", iTween.EaseType.easeOutCubic,
					"onupdate", "MoveImage",
					"LoopType", "pingPong"
					));
			}
		}
	}

	public void MoveImage(Vector3 tempData)
	{
		m_labelAG.transform.localPosition = new Vector3(m_labelAG.transform.localPosition.x, tempData.y, m_labelAG.transform.localPosition.z);
	}

	public void setOpenDialogID(int index, float time = 999999)
	{
		setOpenDialog(DialogData.getDialog(index), time);
	}

	public void setOpenDialogID(int index, int x, int y, int w, float time = 999999, onclick onclick = null)
	{
		setOpenDialog(DialogData.getDialog(index), x, y, w, time, onclick);
	}

	public void setOpenDialog(int npcID, float time = 999999)
	{
//		Debug.Log("TaskData.Instance.m_TaskInfoDic.Count="+TaskData.Instance.m_TaskInfoDic.Count);
//		setOpenDialog(DialogData.getDialog(100101));
		foreach (ZhuXianTemp value in TaskData.Instance.m_TaskInfoDic.Values)
		{
			if(npcID == value.NpcId && value.progress >= 0)
			{
				m_iMissionID = value.id;
				setOpenDialog(DialogData.getDialog(int.Parse(value.doneCond)), time);
			}
		}
	}

	public void setOpenDialog(List<DialogData.dialogData> dialogData, float time = 999999, onclick onclick = null)
	{
		m_fTime = time;
		m_fCreateTime = Time.realtimeSinceStartup;
		m_dialogData = dialogData;
		gameObject.SetActive(true);
		m_onclick = onclick;
		setDialogPosB(m_dialogData[m_iCurentDialogIndex].isLeft);
		m_sCutDialogData = MyColorData.getColorString(2, m_dialogData[m_iCurentDialogIndex].sDialogData);

		label.text = "";
		if(m_dialogData[m_iCurentDialogIndex].sName.Equals("xxxxx"))
		{
			m_sName.text = MyColorData.getColorString(1, JunZhuData.Instance().m_junzhuInfo.name);
		}
		else
		{
			m_sName.text = MyColorData.getColorString(1, m_dialogData[m_iCurentDialogIndex].sName);
		}
		int tempHeadID = m_dialogData[ m_iCurentDialogIndex ].iHeadID;
		if(tempHeadID == 0)
		{
			tempHeadID = CityGlobalData.m_king_model_Id + 100;
		}
		Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.YINDAO_DRAMA_AVATAR_PREFIX ) + tempHeadID,
		                        ResourceLoadCallback );
		avatar.gameObject.SetActive(true);
		sprite.width = 960 + (ClientMain.m_iMoveX * 2);

		m_FTimeNew = System.DateTime.Now;
		if(m_dialogData[m_iCurentDialogIndex].sDialogSoundID != "" && m_dialogData[m_iCurentDialogIndex].sDialogSoundID != null)
		{
			ClientMain.m_ClientMain.m_SoundPlayEff.PlaySound(m_dialogData[m_iCurentDialogIndex].sDialogSoundID);
		}
	}

	public void ResourceLoadCallback( ref WWW p_www, string p_path, UnityEngine.Object p_object ){
		Texture temptext = ( Texture )p_object;

		avatar.mainTexture = temptext;
		avatar.gameObject.SetActive(true);
	}

	public void setOpenDialog(List<DialogData.dialogData> dialogData, int x, int y, int w, float time = 999999, onclick onclick = null)
	{
		setOpenDialog(dialogData, time, onclick);
//		if(x == 0 && y == 0 && w == 0)
//		{
//
//		}
//		else
//		{
//			sprite.gameObject.transform.localPosition = new Vector3(ClientMain.m_iMoveX + x, 0 + ClientMain.m_iMoveY + (640 - y), 0);
//			avatar.gameObject.transform.localPosition = new Vector3(ClientMain.m_iMoveX + x, 0 + ClientMain.m_iMoveY + (640 - y), 0);
//			if(w != 0)
//			{
//				sprite.width = w;
//				label.width = w - 250;
//				m_labelAG.transform.localPosition = new Vector3(ClientMain.m_iMoveX + x + 60 + w - 150, 0 + ClientMain.m_iMoveY + (640 - y + 25), 0);
//			}
//			else
//			{
//				m_labelAG.transform.localPosition = new Vector3(ClientMain.m_iMoveX + x + 960 - 150, 0 + ClientMain.m_iMoveY + (640 - y + 25), 0);
//			}
//		}
	}

	public void setDialogPosB(bool isLeft = true)
	{
		m_labelAG.gameObject.SetActive(false);
		label.width = (960 + (ClientMain.m_iMoveX * 2)) - 255 - (255 - m_iMoveX) - 40;
		sprite.gameObject.transform.localPosition = new Vector3(0, 0, 0);
		//Debug.Log(ClientMain.m_iMoveY * 2);
		spriteUp.gameObject.transform.localPosition = new Vector3(0, 640 + (ClientMain.m_iMoveY * 2) + 5, 0);
		spriteUp.width = (960 + (ClientMain.m_iMoveX * 2));
		m_labelAG.transform.localPosition = new Vector3(960 + (ClientMain.m_iMoveX * 2) - 210, 25, m_labelAG.transform.localPosition.z);
		if(isLeft)
		{
			avatar.gameObject.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
			avatar.gameObject.transform.localPosition = new Vector3(0,0,0);

			m_sName.gameObject.transform.localPosition = new Vector3(255,104,0);
			label.gameObject.transform.localPosition = new Vector3(255,77,0);

			m_labelAG.transform.localPosition = new Vector3((960 + (ClientMain.m_iMoveX * 2)) - (255 - m_iMoveX) - 25,20,0);
		}
		else
		{
//			m_iMoveX

//			avatar.gameObject.transform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
			avatar.gameObject.transform.localPosition = new Vector3((960 + (ClientMain.m_iMoveX * 2)) - 250, 0,0);

			m_sName.gameObject.transform.localPosition = new Vector3(255 - m_iMoveX,104,0);
			label.gameObject.transform.localPosition = new Vector3(255 - m_iMoveX,77,0);
			
			m_labelAG.transform.localPosition = new Vector3((960 + (ClientMain.m_iMoveX * 2)) - (255 - m_iMoveX) - 25 - m_iMoveX,20,0);
		}
		m_isNextDialog = false;
		i_Num = 0;
		UIYindao.m_UIYindao.CloseJiantou();
	}

	public void OnPress(bool pressed)
	{
		if(pressed && !YindaoEditor.YINDAOEDITOR.m_ObjBox.activeSelf && m_isNextDialog)
		{
			if((System.DateTime.Now - m_FTimeNew).TotalSeconds > 0.3f)
			{
				if(m_iCurentDialogIndex + 1 == m_dialogData.Count)
				{
					avatar.gameObject.SetActive(false);
					gameObject.SetActive(false);
					m_iCurentDialogIndex = 0;
					if(m_iMissionID != -1)
					{
						TaskData.Instance.SendData(m_iMissionID, 1);
						m_iMissionID = -1;
					}
					if(m_onclick != null)
					{
						m_onclick();
					}
					UIYindao.m_UIYindao.EndDialog();
				}
				else
				{
					nextDialog();
				}
			}
		}
		else if(pressed && !YindaoEditor.YINDAOEDITOR.m_ObjBox.activeSelf && !m_isNextDialog)
		{
			label.text = m_sCutDialogData;
			m_isNextDialog = true;
			m_labelAG.gameObject.SetActive(true);
		}
	}

	public void nextDialog()
	{
		m_iCurentDialogIndex ++;
		setOpenDialog(m_dialogData, m_fTime, m_onclick);
	}

	public void CloseDialog()
	{
		m_labelAG.gameObject.SetActive(false);
		gameObject.SetActive(false);
		m_iCurentDialogIndex = 0;
		if(m_iMissionID != -1)
		{
			TaskData.Instance.SendData(m_iMissionID, 1);
			m_iMissionID = -1;
		}
		m_isNextDialog = false;
		i_Num = 0;
		m_onclick = null;
	}

	#region IUIRootAutoActivator

	public bool IsNGUIVisible(){
		return gameObject.activeSelf;
	}

	#endregion
}
