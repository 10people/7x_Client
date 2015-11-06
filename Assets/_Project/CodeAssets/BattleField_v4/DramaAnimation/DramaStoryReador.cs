//#define DEBUG_DRAMA_STORY_READER

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SimpleJSON;

public class DramaStoryReador : MonoBehaviour
{
	public int storyBoardId;

	
	private static DramaStoryReador _instance;

	private List<int> storyBoardIds = new List<int> ();

	private Dictionary<string, Object> resList = new Dictionary<string, Object>();

	private int maxCount;

	private GameObject controllorGc;

	private bool isDebug;

	private JSONNode json;

	private List<int> modelIdList = new List<int> ();

	private int loadCount;

	private DramaStoryControllor m_storyControllor;


	public static DramaStoryReador Instance() { return _instance; }
	
	void Awake() { 
		_instance = this; 
	}

	#region Load

	public void checkinDebug(){
		#if DEBUG_DRAMA_STORY_READER
		Debug.Log( "DramaStoryReader.checkinDebug()" );
		#endif

		CityGlobalData.m_tempSection = 1;

		CityGlobalData.m_tempLevel = 1;

		ModelTemplate.LoadTemplates (null);

		EffectIdTemplate.LoadTemplates( null );

		Res2DTemplate.LoadTemplates(_checkinDebug);
	}

	private void _checkinDebug()
	{
		#if DEBUG_DRAMA_STORY_READER
		Debug.Log( "DramaStoryReader._checkinDebug()" );
		#endif

		isDebug = true;

		storyBoardIds.Clear ();
		
		bool flag = false;

		storyBoardIds.Add(storyBoardId);

		// create storyboard root and script
		{
			controllorGc = new GameObject ();
			
			controllorGc.name = "StroryBoards";
			
			controllorGc.transform.parent = transform;
			
			controllorGc.transform.position = Vector3.zero;
			
			m_storyControllor = controllorGc.AddComponent<DramaStoryControllor> ();
		}
		
		maxCount = storyBoardIds.Count;
		
		resList.Clear ();

		foreach(int storyBoardIndex in storyBoardIds)
		{
			string path = Res2DTemplate.GetResPath( Res2DTemplate.Res.BATTLE_FIELD_STORY_BOARD_PREFIX ) + storyBoardIndex;

			{
				#if DEBUG_DRAMA_STORY_READER
				Debug.Log( "DramaStoryReander.Read( " + path + " )" );
				#endif
			}

			Global.ResourcesDotLoad( path, resLoadCallback);
		}
	}

	public void checkin()
	{
		#if DEBUG_DRAMA_STORY_READER
		Debug.Log( "DramaStoryReader.checkin()" );
		#endif

		isDebug = false;

		int levelId = 100000 + CityGlobalData.m_tempSection * 100 + CityGlobalData.m_tempLevel;

		storyBoardIds.Clear ();

		bool flag = false;

		if(GuideTemplate.GetTemplates() != null)
		{
			foreach( GuideTemplate gt in GuideTemplate.GetTemplates() )
			{
				if(gt.id == levelId)
				{
					flag = true;

					if(gt.para1 == 1)
					{
						storyBoardIds.Add(gt.para2);
					}
				}
				else if(flag == true)
				{
					break;
				}
			}
		}

		//storyBoardIds.Add(0);

		// create DramaStoryController root nodes, named as StroryBoards.
		{
			controllorGc = new GameObject ();
			
			controllorGc.name = "StroryBoards";
			
			controllorGc.transform.parent = transform;
			
			controllorGc.transform.position = Vector3.zero;
			
			m_storyControllor = controllorGc.AddComponent<DramaStoryControllor> ();
		}

		maxCount = storyBoardIds.Count;

		resList.Clear ();

		foreach(int storyBoardIndex in storyBoardIds)
		{
			Global.ResourcesDotLoad( Res2DTemplate.GetResPath( Res2DTemplate.Res.BATTLE_FIELD_STORY_BOARD_PREFIX ) + storyBoardIndex, 
			                        resLoadCallback);
		}

		if(storyBoardIds.Count == 0)
		{
			if( m_load_callback != null )
			{
				m_load_callback();
				
				m_load_callback = null;
			}
		}
	}

	public void resLoadCallback(ref WWW p_www, string p_path, Object p_object)
	{
		StaticLoading.ItemLoaded( StaticLoading.m_loading_sections,
		                         StaticLoading.CONST_BATTLE_LOADING_DATA, p_path );

		if (resList.ContainsKey (p_path) == false)
		{
			resList.Add (p_path, p_object);
		}

		maxCount --;

		if (maxCount == 0) jsonResLoadOver ();
	}

	private void jsonResLoadOver()
	{
		#if DEBUG_DRAMA_STORY_READER
		Debug.Log( "DramaStoryReader.jsonResLoadOver()" );
		#endif

		foreach(int storyBoardIndex in storyBoardIds)
		{
			string path = Res2DTemplate.GetResPath( Res2DTemplate.Res.BATTLE_FIELD_STORY_BOARD_PREFIX ) + storyBoardIndex;

			Object ob;

			resList.TryGetValue(path, out ob);

			TextAsset _text = ob as TextAsset;
			
			string jsonStr = _text.text;
			
			JSONNode json = JSONNode.Parse(jsonStr);
			
			createStoryBoard(json, m_storyControllor.gameObject);
		
//			preLoadModel(json);
		}

		List<string> tempLoadModel = new List<string>();

		foreach(DramaStoryBoard board in m_storyControllor.storyBoardList.Values )
		{
			for(int i = 0; i < board.m_loadPath.Count; i ++)
			{
				if(board.m_loadIsJuqing[i])
				{
					tempLoadModel.Add(board.m_loadPath[i]);
				}

				/* By YuGu.
				 * 
				 * load boss model if previewing drama.
				 * 
				 * REMOVED, avoid multi-load.
				 */
//				if( DramaDirector.IsDramaPreviewing() ){
//					if( !board.m_loadIsJuqing[i] ){
//						tempLoadModel.Add( board.m_loadPath[i] );
//					}
//				}
			}
		}

		Global.SetResourcesDotLoadWatchers( tempLoadModel, UtilityTool.GetEventDelegateList( LoadModelOver ) );

		foreach(DramaStoryBoard board in m_storyControllor.storyBoardList.Values )
		{
			for(int i = 0; i < board.m_loadPath.Count; i ++)
			{
				if(board.m_loadIsJuqing[i])
				{
					Global.ResourcesDotLoad(board.m_loadPath[i], board.CurLoad);
				}

				/* By YuGu.
				 * 
				 * load boss model if previewing drama.
				 * 
				 * REMOVED, avoid multi-load.
				 */
//				if( DramaDirector.IsDramaPreviewing() ){
//					if( !board.m_loadIsJuqing[i] ){
//						Global.ResourcesDotLoad(board.m_loadPath[i], board.CurLoad);
//					}
//				}
			}
		}
	}

	/// <summary>
	/// Create SubStoryBoards, all actors and camera animation in this storyboard.xml will be created under this.
	/// </summary>
	private void createStoryBoard(JSONNode json, GameObject storyControllor)
	{
		#if DEBUG_DRAMA_STORY_READER
		Debug.Log( "DramaStoryReader.createStoryBoard()" );
		#endif

		GameObject TempstoryBoardGc = new GameObject();

		TempstoryBoardGc.transform.parent = storyControllor.transform;

		TempstoryBoardGc.transform.position = Vector3.zero;

		int storyboardId = json ["storyboardId"].AsInt;

		TempstoryBoardGc.name = "StoryBoard_" + storyboardId;

		DramaStoryBoard storyBoard = TempstoryBoardGc.AddComponent<DramaStoryBoard> ();

		storyBoard.storyBoardId = storyboardId;

		storyBoard.m_json = json;

		storyBoard.m_ArrayJson = json ["jsonActor"].AsArray;

//		m_loadPath = new List<string>();

		foreach(JSONNode ja in storyBoard.m_ArrayJson)
		{
			int actorModelId = ja["actorModelId"].AsInt;

			bool tempIsJuqing = ja["actorJuqing"].AsBool;

			if(BattleControlor.Instance() == null)
			{
				tempIsJuqing = true;
			}

			string actorName = ja["actorName"];

			storyBoard.m_loadIsJuqing.Add(tempIsJuqing);

			if(tempIsJuqing)
			{
				if(actorModelId <= 0)
				{
					storyBoard.m_loadPath.Add(ModelTemplate.GetResPathByModelId(1000));
				}
				else 
				{
					storyBoard.m_loadPath.Add(ModelTemplate.GetResPathByModelId(actorModelId));
				}
			}
			else
			{
				storyBoard.m_loadPath.Add(actorModelId.ToString());
			}

			storyBoard.m_actorGc.Add(null);
		}

		m_storyControllor.storyBoardList.Add (storyBoard.storyBoardId, storyBoard);
	}

	#endregion

	IEnumerator debugPlay()
	{
		yield return new WaitForSeconds(.1f);

		DramaStoryControllor dsc = controllorGc.GetComponent<DramaStoryControllor>();

		dsc.init();

		dsc.playNext(null);
	}

	private Global.LoadResourceCallback m_load_callback = null;

	public void SetLoadDoneCallback( Global.LoadResourceCallback p_callback )
	{
		m_load_callback = p_callback;
	}

	public void LoadModelOver()
	{
		#if DEBUG_DRAMA_STORY_READER
		Debug.Log( "DramaStoryReader.LoadModelOver()" );
		#endif

		int modelIndex = 0;

		foreach(DramaStoryBoard board in m_storyControllor.storyBoardList.Values )
		{
			board.createActors(false);
		}

		if( m_load_callback != null )
		{
			m_load_callback();
			
			m_load_callback = null;
		}
		
		if(isDebug == true) StartCoroutine(debugPlay());
	}

}
