using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

public class BattleFlagTemplate : XmlLoadManager
{
	//<BattleFlag flagId="1" triggerMode="0" x="-10.15848" y="-12.03634" z="-2.095169" 
	//rx="7.058717E-08" ry="338.3673" rz="1.88816E-07" 
	//cx="1" cy="1" cz="1" 
	//triggerCount="0" triggerFunc="4" willRelive="0" triggerFlagEnter="1,2" 
	//triggerFlagAttack="0" triggerFlagKill="0" />


	public int flagId;

	public int forwardFlagId;

	public int triggerMode;

	public float x;

	public float y;

	public float z;

	public float rx;
	
	public float ry;
	
	public float rz;

	public float cx;
	
	public float cy;
	
	public float cz;

	public int triggerCount;

	public int triggerFunc;

	public int willRelive;

	public int dieable;

	public int accountable;

	public int hideInDrama;

	public int hideWithDramable;

	public int guideId;

	public int hintLabelId;

	public bool showOnUI;

	public int groupId;

	public List<int> nodeSkillAble = new List<int> ();

	public string alarmPosition;

	public string path;

	public List<int> triggerFlagEye2eye;

	public List<int> triggerFlagEnter;

	public List<int> triggerFlagAttack;

	public List<int> triggerFlagKill;

	public List<Vector2> triggerFlagBlood = new List<Vector2> ();

	public List<DistanceFlag> triggerFlagDistance = new List<DistanceFlag> ();


	public static List<BattleFlagTemplate> templates;

	public static void LoadTemplates( int chapterId, EventDelegate.Callback p_callback = null ){
		if(templates == null)
			templates = new List<BattleFlagTemplate>();
		else{
			templates.Clear();
		}
		
		UnLoadManager.DownLoad( PathManager.GetUrl( "_Data/BattleField/BattleFlags/Battle_" + chapterId + ".xml" ), CurLoad, UtilityTool.GetEventDelegateList( p_callback ), false  );
	}

	public static void CurLoad( ref WWW www, string path, Object obj ){
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
			t_has_items = t_reader.ReadToFollowing( "BattleFlag" );
			
			if( !t_has_items ){
				break;
			}
			
			BattleFlagTemplate t_template = new BattleFlagTemplate();
			
			{
				t_reader.MoveToNextAttribute();
				t_template.flagId = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.forwardFlagId = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.triggerMode = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.x = float.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.y = float.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.z = float.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.rx = float.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.ry = float.Parse( t_reader.Value );
				
				t_reader.MoveToNextAttribute();
				t_template.rz = float.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.cx = float.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.cy = float.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.cz = float.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.triggerCount = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.triggerFunc = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.willRelive = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.dieable = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.accountable = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.hideInDrama = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.hideWithDramable = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.guideId = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.hintLabelId = int.Parse( t_reader.Value );

				t_reader.MoveToNextAttribute();
				t_template.showOnUI = (int.Parse( t_reader.Value ) != 0);

				t_reader.MoveToNextAttribute();
				t_template.groupId = int.Parse( t_reader.Value );

				{
					t_reader.MoveToNextAttribute();

					string strNodeSkill = t_reader.Value;

					string[] sSkill = strNodeSkill.Split(',');

					t_template.nodeSkillAble = new List<int>();

					foreach(string st in sSkill)
					{
						if(st.Equals("0")) break;

						t_template.nodeSkillAble.Add(int.Parse(st));
					}
				}

				t_reader.MoveToNextAttribute();
				t_template.alarmPosition = t_reader.Value;

				t_reader.MoveToNextAttribute();
				t_template.path = t_reader.Value;

				{
					t_reader.MoveToNextAttribute();
					string strEnter = t_reader.Value;
					
					string[] sEye = strEnter.Split(',');
					
					t_template.triggerFlagEye2eye = new List<int>();
					
					foreach(string st in sEye)
					{
						t_template.triggerFlagEye2eye.Add(int.Parse(st));
					}
				}

				{
					t_reader.MoveToNextAttribute();
					string strEnter = t_reader.Value;

					string[] sEnters = strEnter.Split(',');

					t_template.triggerFlagEnter = new List<int>();

					foreach(string st in sEnters)
					{
						t_template.triggerFlagEnter.Add(int.Parse(st));
					}
				}

				{
					t_reader.MoveToNextAttribute();
					string strAttack = t_reader.Value;

					string[] sAttacks = strAttack.Split(',');
					
					t_template.triggerFlagAttack = new List<int>();
					
					foreach(string st in sAttacks)
					{
						t_template.triggerFlagAttack.Add(int.Parse(st));
					}
				}

				{
					t_reader.MoveToNextAttribute();
					string strKill = t_reader.Value;
					
					string[] sKills = strKill.Split(',');

					t_template.triggerFlagKill = new List<int>();

					foreach(string st in sKills)
					{
						t_template.triggerFlagKill.Add(int.Parse(st));
					}
				}

				{
					t_reader.MoveToNextAttribute();

					string strBlood = t_reader.Value;

					string[] sBloods = strBlood.Split(',');

					t_template.triggerFlagBlood = new List<Vector2>();

					foreach(string st in sBloods)
					{
						if(st.Equals("0")) break;

						string[] vec = st.Split('|');

						float x = float.Parse(vec[0]);

						float y = int.Parse(vec[1]);

						t_template.triggerFlagBlood.Add(new Vector2(x, y));
					}
				}

				{
					t_reader.MoveToNextAttribute();

					string strDistance = t_reader.Value;

					string[] sDistance = strDistance.Split(',');

					t_template.triggerFlagDistance = new List<DistanceFlag>();

					foreach(string st in sDistance)
					{
						if(st.Equals("0")) break;

						DistanceFlag df = new DistanceFlag();

						df.triggerDistance = new List<Vector2>();

						string[] values = st.Split('|');

						string distances = values[0];

						string[] distanceValues = distances.Split('_');

						foreach(string strVec in distanceValues)
						{
							string[] vec = strVec.Split('^');

							df.triggerDistance.Add(new Vector2(float.Parse(vec[0]), int.Parse(vec[1])));
						}

						df.count = int.Parse(values[1]);

						df.triggerFlag = int.Parse(values[2]);

						t_template.triggerFlagDistance.Add(df);
					}

				}
			}
			
			//			t_template.Log();
			
			templates.Add( t_template );
		}
		while( t_has_items );

		if( m_load_callback != null ){
			m_load_callback();

			m_load_callback = null;
		}
	}

	private static Global.LoadResourceCallback m_load_callback = null;

	public static void SetLoadDoneCallback( Global.LoadResourceCallback p_callback ){
		m_load_callback = p_callback;
	}

}
