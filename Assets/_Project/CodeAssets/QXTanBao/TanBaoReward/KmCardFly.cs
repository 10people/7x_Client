using UnityEngine;
using System.Collections;

public class KmCardFly : MonoBehaviour {

//	public int tanBaoType;
//
//	private float time = 1;
//	
//	private float speed = 2.0f;
//	
//	private float ranPosX;
//	private float ranPosY;
//	private float minX = -5;
//	private float maxX = -2;
//	private float minY = -5;
//	private float maxY = -2;
//	
//	private Vector3 lastPos;//记录当前pos
//	
//	private bool canRandom = false;
//	
//	//cardItem移动
//	public void CardTrans (Vector3 tempPos) {
//		
//		//		args.Add ("easeType",iTween.EaseType.easeInBack);//先后退再移动，从慢到快
//		//		args.Add ("easeType",iTween.EaseType.easeOutBack);//先移动再后退，从快到慢
//		//		args.Add ("easeType",iTween.EaseType.easeInOutCubic);//慢->快->慢
//		//		args.Add ("easeType",iTween.EaseType.easeInQuart);//从慢到快
//		//		args.Add ("easeType",iTween.EaseType.easeOutQuart);//从快倒慢
//		
//		lastPos = tempPos;
//		
//		Hashtable trans = new Hashtable ();
//		trans.Add ("time",time);                  //移动时间,如果有speed,后于speed
//		trans.Add ("onstart","CardTranStart");    //开始移动时调用HashStart方法
//		trans.Add ("oncomplete", "CardTranEnd");  //结束移动时调用HashEnd方法
//		trans.Add ("onupdate","CardTranUpdate");  //移动中调用HashUpdate方法
//		trans.Add ("position",tempPos);
//
//		if (tanBaoType == 10 || tanBaoType == 12)
//		{
//			trans.Add ("easeType",iTween.EaseType.easeOutBack);
//		}
//		else
//		{
//			trans.Add ("easeType",iTween.EaseType.easeOutQuart);
//		}
//
//		trans.Add ("islocal",true);
//		iTween.MoveTo (this.gameObject,trans);
//		
//		Hashtable scales = new Hashtable ();
//		scales.Add ("time",time);
//		scales.Add ("scale",Vector3.one);
//		iTween.ScaleTo (this.gameObject,scales);
//	}
//	
//	//cardItem开始移动调用
//	void CardTranStart ()
//	{
//		//		Debug.Log ("flyStart!");
//	}
//	
//	//cardItem结束移动调用
//	void CardTranEnd ()
//	{
//		//		Debug.Log ("flyEnd!");
//		canRandom = true;
//		RewardCardInfo reward = this.gameObject.GetComponent<RewardCardInfo> ();
//		if (tanBaoType == 0 || tanBaoType == 1 || tanBaoType == 11)
//		{
//			reward.ClickCardBg (gameObject);
//		}
//		else
//		{
//			reward.cardHandler.GetComponent<BoxCollider> ().enabled = true;
//		}
////		if (tanBaoType == 0)
////		{
//////			if(UIYindao.m_UIYindao.m_isOpenYindao)
//////			{
//////				if(FreshGuide.Instance().IsActive(200000)&& TaskData.Instance.m_TaskInfoDic[200000].progress >= 0 && FirstTanBao.Instance.GetTanBaoState1 ())
//////				{
//////					ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[200000];
//////					UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[3]);
//////				}
//////				
//////				else
//////				{
//////					CityGlobalData.m_isRightGuide = true;
//////				}
//////			}
////		}
////		
////		else if (tanBaoType == 1)
////		{
////			if(UIYindao.m_UIYindao.m_isOpenYindao)
////			{
////				if(FreshGuide.Instance().IsActive(100009)&& TaskData.Instance.m_TaskInfoDic[100009].progress >= 0 && FirstTanBao.Instance.GetTanBaoState2())
////				{
////					ZhuXianTemp tempTaskData = TaskData.Instance.m_TaskInfoDic[100009];
////					UIYindao.m_UIYindao.setOpenYindao(tempTaskData.m_listYindaoShuju[10]);
////				}
////				
////				else
////				{
////					CityGlobalData.m_isRightGuide = true;
////				}
////			}
////		}
//	}
//	
//	//cardItem移动中调用
//	void CardTranUpdate ()
//	{
//		
//	}
//	
//	//卡片漂浮效果,向随机一个位置漂浮
//	void LittleFly ()
//	{
//		ranPosX = Random.Range (minX,maxX);
//		ranPosY = Random.Range (minY,maxY);
//		
//		//		Debug.Log ("ranPosX:" + ranPosX);
//		//		Debug.Log ("ranPosY" + ranPosY);
//		
//		Vector3 posTo = lastPos + new Vector3 (ranPosX,ranPosY,0);
//		
//		Hashtable fly = new Hashtable ();
//		fly.Add ("speed",speed);
//		fly.Add ("easetype",iTween.EaseType.easeInOutSine);
//		fly.Add ("position",posTo);
//		fly.Add ("islocal", true);
//		fly.Add ("oncomplete", "FlyEnd");
//		iTween.MoveTo (this.gameObject,fly);
//	}
//	
//	//卡片从原位置向某个位置漂浮结束时,向原来的位置漂浮回去
//	void FlyEnd ()
//	{
//		Hashtable fly = new Hashtable ();
//		fly.Add ("speed",speed);
//		fly.Add ("easetype",iTween.EaseType.easeInOutSine);
//		fly.Add ("position",lastPos);
//		fly.Add ("islocal", true);
//		fly.Add ("oncomplete", "FlyBackEnd");
//		iTween.MoveTo (this.gameObject,fly);
//	}
//	
//	//当卡片漂浮回原位置时
//	void FlyBackEnd ()
//	{
//		canRandom = true;
//	}
//	
//	void Update ()
//	{
//		if (canRandom == true) 
//		{
//			canRandom = false;
//			LittleFly ();
//		}
//	}
}
