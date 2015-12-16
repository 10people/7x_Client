using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using ProtoBuf;
using qxmobile.protobuf;
using ProtoBuf.Meta;

public class miBaoskilltemp : MonoBehaviour {

	public GameObject mLock;

	public UISprite Icon;

	public UILabel Lv;

	public SkillInfo mSkillInfo;

	public int SKill_id;

	public bool IsActive;

	public bool beChoosed;

	void Start () {

		IsActive = false;
		Lv.text = "Lv.1";
		mLock.SetActive (true);

		MiBaoSkillTemp mMiBaoSkillTemp = MiBaoSkillTemp.getMiBaoSkillTempBy_id (SKill_id);
		
		Icon.spriteName = mMiBaoSkillTemp.icon;
	}
	void Update () {
	
	}
	public void Init()
	{
		Lv.text = "Lv."+mSkillInfo.level.ToString ();

		mLock.SetActive (false);

	}
	public void Be_CHoosed()
	{
		StartCoroutine (Changestatebtn());
	}
	IEnumerator Changestatebtn()
	{
		yield return new WaitForSeconds (0.5f);
		
		this.gameObject.GetComponent<UIToggle>().value = !this.gameObject.GetComponent<UIToggle>().value;
		

	}
	public void ShowDeilInfo()
	{
		if(NewMiBaoSkill.Instance ().SaveId == SKill_id)
		{
			return;
		}
		NewMiBaoSkill.Instance ().ShowBeChoosed_MiBao (SKill_id,IsActive);

		if(IsActive)
		{
			SendSaveMiBaoMasege(NewMiBaoSkill.Instance ().S_Type,SKill_id);
		}
	}
	public void SendSaveMiBaoMasege(int Type,int id )//保存秘宝技能
	{
		MibaoSelect Mibaoid = new MibaoSelect ();
		
		MemoryStream miBaoStream = new MemoryStream ();
		
		QiXiongSerializer MiBaoSer = new QiXiongSerializer ();
		
		Mibaoid.type = Type;
		
		Mibaoid.zuheSkill = id;
		
		MiBaoSer.Serialize (miBaoStream,Mibaoid);
		byte[] t_protof;
		t_protof = miBaoStream.ToArray();
		
		SocketTool.Instance ().SendSocketMessage (ProtoIndexes.C_MIBAO_SELECT,ref t_protof);
		
	}
}
