#define DEBUG_DRAMA_DIRECTOR

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DramaDirector : SingletonMono<DramaDirector> {

	#region Mono

	void Awake(){
		if ( HaveInstance () ) {
			#if DEBUG_DRAMA_DIRECTOR
			Debug.Log( "DramaDirector.Destroy.Instance()" );
			#endif

			GameObjectHelper.HideAndDestroy( Instance().gameObject );
		}

		{
			SetInstance( this );

			DontDestroyOnLoad( gameObject );

			gameObject.name = gameObject.name + " " + TimeHelper.GetCurMonthDayHourMinSec();
		}

		{
			OnPreviewDrama();
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnLevelWasLoaded(){
//		#if DEBUG_DRAMA_DIRECTOR
//		Debug.Log( "DramaDirector.OnLevelWasLoaded( " + Application.loadedLevel + " )" );
//		#endif

		{
			LoadStoryBoard();
		}
	}

	void OnDestroy() {
		{
			SetInstance( null );
		}
	}

	#endregion



	#region Interaction

	[HideInInspector]
	public string m_story_board_id				= "0";

	[HideInInspector]
	public string m_battle_field_id 			= "0";

	[HideInInspector]
	private string m_boss_stand_pos_id			= "0";

	public void OnPreviewDrama(){
		#if DEBUG_DRAMA_DIRECTOR
		Debug.Log( "DramaDirector.OnPreviewDrama( " + m_battle_field_id + " )" );
		#endif

		{
			Application.LoadLevel( LevelHelper.GetBattleSceneNameById( m_battle_field_id ) );
		
//			Application.LoadLevel ( ConstInGame.CONST_SCENE_NAME_LOGIN );
		}
	}

	#endregion 



	#region Load StoryBoard

	private GameObject m_story_board_root = null;

	private void LoadStoryBoard(){
		#if DEBUG_DRAMA_DIRECTOR
		Debug.Log( "DramaDirector.LoadStoryBoard()" );
		#endif

		m_story_board_root = new GameObject();
		
		m_story_board_root.name = "Loaded Story Board Root";

		DramaStoryReador t_reader = m_story_board_root.AddComponent<DramaStoryReador>();

		int t_board_id = int.Parse( m_story_board_id );

		if ( t_board_id < 0 ) {
			#if DEBUG_DRAMA_DIRECTOR
			Debug.Log( "DramaDirector.CreateEmptyBoard()" );
			#endif

			return;
		}

		t_reader.storyBoardId = t_board_id;

		t_reader.checkinDebug ();
	}

	#endregion



	#region Save StoryBoard

	public void SaveStoryBoard(){
		#if DEBUG_DRAMA_DIRECTOR
		Debug.Log( "DramaDirector.SaveStoryBoard()" );
		#endif

		{
			CleanForDramaWriter();
		}

		DramaStroyWritor t_writer = m_story_board_root.GetComponent<DramaStroyWritor> ();

		if ( t_writer == null ) {
			t_writer = m_story_board_root.AddComponent<DramaStroyWritor>();
		}

		t_writer.checkout ();
	}

	protected void CleanForDramaWriter(){
		#if DEBUG_DRAMA_DIRECTOR
		Debug.Log( "DramaDirector.CleanForDramaWriter()" );
		#endif

		{
			DramaStoryControllor t_controllor = m_story_board_root.GetComponentInChildren<DramaStoryControllor>();

			if( t_controllor == null ){
				Debug.LogError( "DramaStoryControllor not found." );

				return;
			}

			t_controllor.storyBoardList.Clear();
		}
	}

	#endregion



	#region Director Helper

	public static bool IsDramaPreviewing(){
		return DramaDirector.HaveInstance();
	}

	#endregion
}