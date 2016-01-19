using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class PlotChatTemplate : XmlLoadManager
{
	//<PlotChat plotChatId="10101" seriestId="1" pName="xxxxx" pText="这就到顶层了？[D2691E]千重楼[-]也不过如此!" icon="0" position="1" 
	//forwardFlagId="0" cameraTarget="0" cameraPx="-0.826" cameraPy="1.473" cameraPz="2.223" cameraRx="7.460129" cameraRy="165.74" />

	public int plotChatId;

	public int seriestId;

	public string pName;

	public string pText;

	public int icon;

	public int position;

	public int forwardFlagId;

	public string cameraTarget;

	public bool isLocal;

	public int cameraId;

	public float cameraPx;

	public float cameraPy;

	public float cameraPz;

	public float cameraRx;
	
	public float cameraRy;


	public static List<PlotChatTemplate> templates = new List<PlotChatTemplate>();
	
	
	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "PlotChat.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
	}
	
	public static void CurLoad(ref WWW www, string path, Object obj){
		{
			templates.Clear();
		}
		
		XmlReader t_reader = null;
		
		if( obj != null ){
			TextAsset t_text_asset = obj as TextAsset;
			
			t_reader = XmlReader.Create( new StringReader( t_text_asset.text ) );
			
			//			Debug.Log( "Text: " + t_text_asset.text );
		}
		else{
			t_reader = XmlReader.Create( new StringReader( www.text ) );
		}
		
		bool t_has_items = true;
		
		do{
			t_has_items = t_reader.ReadToFollowing( "PlotChat" );
			
			if( !t_has_items ){
				break;
			}
			
			PlotChatTemplate t_template = new PlotChatTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.plotChatId = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.seriestId = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.pName = t_reader.Value;

				t_reader.MoveToNextAttribute();
				t_template.pText = t_reader.Value;

				t_reader.MoveToNextAttribute();
				t_template.icon = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.position = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.forwardFlagId = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.cameraTarget = t_reader.Value;

				t_reader.MoveToNextAttribute();
				t_template.cameraId = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.isLocal = int.Parse( t_reader.Value ) != 0;

				t_reader.MoveToNextAttribute();
				t_template.cameraPx = float.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.cameraPy = float.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.cameraPz = float.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.cameraRx = float.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.cameraRy = float.Parse( t_reader.Value );
			}
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );
	}
	
	public static PlotChatTemplate getPlotChatTemplateById(int plotChatId)
	{
		foreach(PlotChatTemplate template in templates)
		{
			if(template.plotChatId == plotChatId)
			{
				if(template.cameraId != 0)
				{
					CameraTemplate ct = CameraTemplate.getCameraTemplateById(template.cameraId);

					if(ct != null)
					{
						template.cameraPx = ct.cameraPx;

						template.cameraPy = ct.cameraPy;

						template.cameraPz = ct.cameraPz;

						template.cameraRx = ct.cameraRx;

						template.cameraRy = ct.cameraRy;
					}
				}

				return template;
			}
		}
		
		Debug.LogError("XML ERROR: Can't get PlotChatTemplate with id " + plotChatId);
		
		return null;
	}

}
