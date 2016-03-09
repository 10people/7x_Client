//#define DEBUG_VIDEO

#define PC_VIDEO



using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class VideoHelper : MonoBehaviour {

	public enum VideoControlMode{
		None,				// Hidden
		NoneButCancelable,	// CancelOnInput
		Minimal,			// Minimal
		Full,				// Full
	}

	#region Mono

	// Use this for initialization
	void Start () {
	
	}

	#if PC_VIDEO && UNITY_EDITOR
	public void OnGUI(){
		if ( m_playing_pc_video && m_movie_texture != null ){
			GUI.DrawTexture (new Rect( 0, 0, Screen.width, Screen.height ), m_movie_texture );
		}
	}
	#endif
	
	// Update is called once per frame
	void Update () {
//		#if DEBUG_VIDEO
//		ComponentHelper.LogMovieTexture( m_movie_texture );
//
//		ComponentHelper.LogAudioSource( m_movie_audio );
//		#endif

		#if PC_VIDEO && UNITY_EDITOR
		if( m_movie_texture != null ){
			if( Input.anyKeyDown ){
				#if DEBUG_VIDEO
				Debug.Log( "Input Cancelled." );
				#endif

				m_playing_pc_video = false;
				
				m_movie_texture.Stop();
				
				m_movie_texture = null;
			}
			else if( !m_movie_texture.isPlaying ){
				if( m_movie_audio.clip != null ){
					if( m_movie_audio.time < m_movie_audio.clip.length ){
						#if DEBUG_VIDEO
						Debug.Log( "Error Found, Replay Movie." );
						#endif

						m_movie_texture.Play();

						m_movie_audio.time = 0.0f;

						return;
					}
				}
				else if( m_pc_video_status == PCVideoStatus.Loaded ){
					#if DEBUG_VIDEO
					Debug.Log( "Error Found, Replay Movie." );
					#endif

					m_pc_video_status = PCVideoStatus.WaitForDone;

					m_movie_texture.Play();

					m_movie_audio.time = 0.0f;

					return;
				}

				#if DEBUG_VIDEO
				Debug.Log( "Movie Stop Cancelled." );
				#endif
				
				m_playing_pc_video = false;
				
				m_movie_texture = null;
			}
		}
		#endif
	}

	#endregion



	#region Interface

	/// All mp4 file must be put under "StreamingAssets\Video" and "ResourcesCache\Video".
	/// path: Video/x7.mp4
	public static void PlayDramaVideo( string p_path, EventDelegate.Callback p_callback = null ){
		#if DEBUG_VIDEO
		Debug.Log( "PlayDramaVideo( " + p_path + " )" );
		#endif

		PlayVideo( p_path, VideoControlMode.None, p_callback );
	}

	#if PC_VIDEO && UNITY_EDITOR
	public MovieTexture m_movie_texture;

	public AudioSource m_movie_audio;

	private bool m_playing_pc_video = false;

	private enum PCVideoStatus{
		None,
		Loaded,
		WaitForDone
	}

	private PCVideoStatus m_pc_video_status = PCVideoStatus.None;
	#endif

	private static string m_video_path = "";

	private static VideoControlMode m_play_mode = VideoControlMode.Minimal;

	private static EventDelegate.Callback m_video_done_callback = null;

	/// Debug and Internal use, please use PlayDramaVideo() instead.
	public static void PlayVideo( string p_path, VideoControlMode p_mode, EventDelegate.Callback p_callback = null ){
		#if DEBUG_VIDEO
		Debug.Log( "PlayVideo( " + p_path + " - " + p_mode + " )" );
		#endif

		{
			m_video_path = p_path;

			m_play_mode = p_mode;

			m_video_done_callback = p_callback;
		}
		
		VideoHelper t_video_helper = GetVideoHelper();

		t_video_helper.StartCoroutine( t_video_helper.PlayVideo() );
	}

	public IEnumerator PlayVideo(){
		#if PC_VIDEO && UNITY_EDITOR
		{
			string t_win_path = "Assets/ResourcesCache/" + m_video_path;

			m_movie_texture = (MovieTexture)AssetDatabase.LoadAssetAtPath( t_win_path, typeof(MovieTexture) );

			if( m_movie_texture == null ){
				Debug.Log( "Move Texture not loaded: " + t_win_path );
			}

			if( m_movie_texture == null ){
				Debug.LogError( "Error, No Available Movie Assigned, please Install \"QuickTimeInstaller.exe\", then restart your computer, and finally reimport all videos." );
				
				VideoPlayedDone();
				
				yield break;
			}

			m_playing_pc_video = true;
			
			m_movie_texture.loop = false;

			m_movie_audio = (AudioSource)ComponentHelper.AddIfNotExist( 
					GameObjectHelper.GetDontDestroyOnLoadGameObject(),
					typeof(AudioSource) );

			if( m_movie_texture.audioClip == null ){
				Debug.LogError( "Error, Movie Has no Audio." );
			}

			m_movie_audio.clip = m_movie_texture.audioClip;

			m_pc_video_status = PCVideoStatus.Loaded;

			#if DEBUG_VIDEO
			ComponentHelper.LogMovieTexture( m_movie_texture, "After Loaded" );
			#endif
			
			m_movie_audio.Play();

			m_movie_texture.Play();

			#if DEBUG_VIDEO
			ComponentHelper.LogMovieTexture( m_movie_texture, "After Played" );
			#endif
			
			while( m_playing_pc_video ){
//				#if DEBUG_VIDEO
//				Debug.Log( "Waiting For Play End or Cancelled." );
//				#endif
				
				yield return new WaitForEndOfFrame();
			}
			
			yield return new WaitForEndOfFrame();
		}
		#elif UNITY_IOS || UNITY_ANDROID
		{
			Debug.Log( "Play Handheld: " + m_video_path );

			Debug.Log( "Mode: " + GetPlayMode() );

			Handheld.PlayFullScreenMovie( m_video_path, Color.black, GetPlayMode() );

			yield return new WaitForEndOfFrame();
		}
		#else
		yield return new WaitForEndOfFrame();
		#endif

		VideoPlayedDone();

		yield return null;
	}

	private void VideoPlayedDone(){
		#if DEBUG_VIDEO
		Debug.Log( "VideoPlayedDone()" );
		#endif

		{
			AudioSource t_audio = gameObject.GetComponent<AudioSource>();
			
			if( t_audio != null ){
				t_audio.enabled = false;
				
				Destroy( t_audio );
			}
		}

		if( m_video_done_callback != null ){
			m_video_done_callback();
			
			m_video_done_callback = null;
		}
	}

	#endregion



	#region Utility

	private static VideoHelper GetVideoHelper(){
		return (VideoHelper)ComponentHelper.AddIfNotExist( 
				GameObjectHelper.GetDontDestroyOnLoadGameObject(),
				typeof(VideoHelper) );
	}

	#if !UNITY_STANDALONE_WIN
	private static FullScreenMovieControlMode GetPlayMode(){
		switch( m_play_mode ){
		case VideoControlMode.Minimal:
			return FullScreenMovieControlMode.Minimal;

		case VideoControlMode.None:
			return FullScreenMovieControlMode.Hidden;

		case VideoControlMode.NoneButCancelable:
			return FullScreenMovieControlMode.CancelOnInput;

		case VideoControlMode.Full:
			return FullScreenMovieControlMode.Full;

		default:
			return FullScreenMovieControlMode.Full;
		}
	}
	#endif


	#endregion



}
