using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class EmailTemp : XmlLoadManager
{

	public int emailType;//邮件类型
	
	public int icon;//邮件icon类型
	
	public string title;//邮件标题
	
	public string taitou;//邮件抬头
	
	public string content;//邮件内容
	
	public string sender;//邮件发送者
	
	public int award;//奖励
	
	public int remainTime;//自动删除时间
	
	public int operateType;//邮件操作类型 0-玩家邮件或系统邮件 1-阅后即删 2-领取即删 3-操作即删

	public int whichType;//邮件类型 1-玩家邮件 2-房屋 3-联盟 4-荒野 5-百战 6-国战 7-运镖 8-运营
	
	public static List<EmailTemp> m_templates = new List<EmailTemp>();

	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "Mail.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
	}
	
	public static void CurLoad( ref WWW p_www, string p_path, Object p_object ){
		{
			m_templates.Clear();
		}
		
		XmlReader t_reader = null;
		
		if(p_object != null){
			TextAsset t_text_asset = p_object as TextAsset;
			
			t_reader = XmlReader.Create( new StringReader( t_text_asset.text ) );
			
//						Debug.Log( "Text: " + t_text_asset.text );
//			Debug.Log( "EmailTemp.Load! ");
		}
		else{
			t_reader = XmlReader.Create( new StringReader( p_www.text ) );
		}
		
		bool t_has_items = true;
		
		do{
			t_has_items = t_reader.ReadToFollowing( "Mail" );
			
			if( !t_has_items ){
				break;
			}
			
			EmailTemp t_template = new EmailTemp();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.emailType = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.icon = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.title = t_reader.Value;
				
				t_reader.MoveToNextAttribute();
				t_template.taitou = t_reader.Value;
				
				t_reader.MoveToNextAttribute();
				t_template.content = t_reader.Value;
				
				t_reader.MoveToNextAttribute();
				t_template.sender = t_reader.Value;
				
				t_reader.MoveToNextAttribute();
				t_template.award = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.remainTime = int.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.operateType = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.whichType = int.Parse( t_reader.Value );
			}
			m_templates.Add(t_template);
		}
		while( t_has_items );
	}
	
	public static EmailTemp getEmailTempByType(int type)
	{
		foreach(EmailTemp template in m_templates)
		{
			if(template.emailType == type)
			{
				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get EmailTemp with emailType " + type);
		
		return null;
	}
}
