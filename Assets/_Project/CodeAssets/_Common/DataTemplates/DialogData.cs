using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class DialogData : XmlLoadManager 
{
	//<RenWu id="100001" name="710001" funDesc="710001" type="1" condition="1" jiangli="9:900001:1000#9:900006:400#10:9101:1" />


	public struct dialogData 
	{
		public int iHeadID;
		public string sDialogData;
		public string sName;
		public string sDialogSoundID;
		public bool isLeft;
	}

	public static Dictionary<int, List<dialogData> > m_DicDialog = new Dictionary<int, List<dialogData>>(); 
	
	public static void LoadTemplates( EventDelegate.Callback p_callback = null ){
		UnLoadManager.DownLoad(PathManager.GetUrl(m_LoadPath + "NpcChat.xml"), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false );
	}
	
	public static void CurLoad( ref WWW www, string path, Object obj ){
		{
			m_DicDialog.Clear();
		}

		int iCurID = -1;

		XmlReader t_reader = null;
		
		if( obj != null ){
			TextAsset t_text_asset = obj as TextAsset;
			
			t_reader = XmlReader.Create( new StringReader( t_text_asset.text ) );
			
//			Debug.Log( "Text: " + t_text_asset.text );
		}
		else{
			t_reader = XmlReader.Create( new StringReader( www.text ) );
		}

		List<dialogData> tempdata = new List<dialogData>();

		bool t_has_items = true;
		
		do{
			t_has_items = t_reader.ReadToFollowing( "NpcChat" );
			
			if( !t_has_items ){
				break;
			}

			{
//				pText="哎呦......这个样子怎么去见天子啊......哎呦......" Icon="12" />
				t_reader.MoveToNextAttribute();
				int tempID = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				//seriestId

				if( tempID != iCurID ){
//					if( m_DicDialog.ContainsKey( iCurID ) ){
//						List<dialogData> t_data = m_DicDialog[ iCurID ];
//						
//						for( int i = 0; i < t_data.Count; i++ ){
//							Debug.LogError( iCurID + " " + i + ": " + t_data[ i ].sName );
//						}
//					}
					m_DicDialog.Add( iCurID, tempdata );

					tempdata = new List<dialogData>();

					iCurID = tempID;
				}
				
				dialogData tempDialog = new dialogData();
				
				t_reader.MoveToNextAttribute();
//				Debug.Log(t_reader.Value);
//				Debug.Log(t_reader.Value == "玩家");
//				Debug.Log(t_reader.Value.Equals("玩家"));
//				Debug.Log(t_reader.Value.IndexOf("玩家"));
				tempDialog.sName = t_reader.Value;
				
				t_reader.MoveToNextAttribute();
				tempDialog.sDialogData = t_reader.Value;

				t_reader.MoveToNextAttribute();
				tempDialog.iHeadID = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				tempDialog.sDialogSoundID = t_reader.Value;

				t_reader.MoveToNextAttribute();
				if(int.Parse( t_reader.Value ) == 0)
				{
					tempDialog.isLeft = true;
				}
				else
				{
					tempDialog.isLeft = false;
				}


				tempdata.Add( tempDialog );
			}
		}
		while( t_has_items );

//		if( m_DicDialog.ContainsKey( iCurID ) ){
//			List<dialogData> t_data = m_DicDialog[ iCurID ];
//
//			for( int i = 0; i < t_data.Count; i++ ){
//				Debug.LogError( iCurID + " " + i + ": " + t_data[ i ].sName );
//			}
//		}

		m_DicDialog.Add( iCurID, tempdata );
	}

	public static List<dialogData> getDialog( int tempNum ){
		if (!m_DicDialog.ContainsKey (tempNum))
		{
			Debug.Log("npcChat找不到ID为"+tempNum+"的数据");
			tempNum = 0;

		}
		return m_DicDialog[tempNum];
	}
}
