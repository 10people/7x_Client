//#define DEBUG_UTILITIES



using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using UnityEditor.Callbacks;
using System.Data;
using Mono.Data.Sqlite;
using System.Linq;  
using System.Text;  
//using System.Threading.Tasks;  



[InitializeOnLoad]
public class EditorUtilityUtilities : MonoBehaviour
{

    public enum MenuItemPriority
    {
        BUILD_DEBUG = 1,

		UTILITIES_DEBUG = 10,
		UTILITIES___VIEW_JSON,
		UTILITIES___FIND_LARGE_TEX,
		UTILITIES___FIND_TEX_IN_RES,
		UTILITIES___FIND_WARNNING_TEX_FORMAT,

		UTILITIES___ASSETS_FILTER,
		UTILITIES___COMBINE_MESH,
		UTILITIES___FIND_MISSING_MONO,
		UTILITIES___FIND_MISSING_MONO_IN_SEL,
		UTILITIES___FIND_GB_AND_MONOS,
		UTILITIES___CUR_UI_IMAGES,
		UTILITIES___SVN_PATH,



		UPGRADE___NAV_UPGRADE = 200,
		UPGRADE___LIGHTMAP_UPGRADE,

		NEW_CHAR_DEVELOP = 300,
    }

    static EditorUtilityUtilities()
    {
        //		Debug.Log( "EditorUtilities()" );

        //		AssetDatabase.ImportAsset( "Assets/Resources/_Data/Design/BaiZhan.xml" );

        AssetDatabase.Refresh();
    }


	#region Debug

	[MenuItem("Utility/Utilities/Debug", false, (int)MenuItemPriority.UTILITIES_DEBUG)]
	static void UtilitiesDebug(){
		Resources.UnloadUnusedAssets();
	}

	#endregion



    #region Open Menu

	[MenuItem("Utility/Utilities/Json Viewer", false, (int)MenuItemPriority.UTILITIES___VIEW_JSON)]
	static void OpenJson(){
		EditorWindow.GetWindow<JsonViewer>( false, "View Json", true );
	}

	#endregion



	#region Utilities Functions

	[MenuItem("Utility/Utilities/Combine Mesh", false, (int)MenuItemPriority.UTILITIES___COMBINE_MESH)]
    static void CombineMesh()
    {
        Object[] selection = Selection.GetFiltered(typeof(GameObject), SelectionMode.TopLevel);

        GameObject t_gb = (GameObject)selection[ 0 ];

        Debug.Log("Select GameObject: " + t_gb);

        StaticBatchingUtility.Combine( t_gb );
    }

	[MenuItem("Utility/Utilities/Assets Filter", false, (int)MenuItemPriority.UTILITIES___ASSETS_FILTER)]
    static void FilterAssets(){
        AssetsFilter.Filter();
    }

	#endregion



	#region Upgrade

	[MenuItem("Utility/Upgrade/Nav Upgrade", false, (int)MenuItemPriority.UPGRADE___NAV_UPGRADE)]
	static void UpgradeNav(){
		string[] t_scene_path = {
//			"Assets/_Project/ArtAssets/Scenes/Login",
//			"Assets/_Project/ArtAssets/Scenes/Loading",
//			"Assets/_Project/ArtAssets/Scenes/CreateRole",

//			"Assets/_Project/ArtAssets/Scenes/House",
			
			"Assets/_Project/ArtAssets/Scenes/MainCity",
			"Assets/_Project/ArtAssets/Scenes/AllianceCity",
			
			"Assets/_Project/ArtAssets/Scenes/PVE",
			"Assets/_Project/ArtAssets/Scenes/PVP",
			
			"Assets/_Project/ArtAssets/Scenes/Carriage",
		};
		
		for (int i = 0; i < t_scene_path.Length; i++) {
			UpgradeAllSceneNav( t_scene_path[ i ] );
		}
	}
	
	private static void UpgradeAllSceneNav( string p_path ){
		Debug.Log( "UpgradeAllSceneNav( " + p_path + " )" );

		List<string> t_file_paths = GetFilePaths( p_path, "*.unity" );
		
		for( int i = 0; i < t_file_paths.Count; i++ ){
			Debug.Log( "UpdateScene( " + t_file_paths[ i ] + " )" );

			EditorApplication.OpenScene( t_file_paths[ i ] );
			
			NavMeshBuilder.BuildNavMesh();
			
			EditorApplication.SaveScene();
		}
	}

	[MenuItem("Utility/Upgrade/LightMap Upgrade", false, (int)MenuItemPriority.UPGRADE___LIGHTMAP_UPGRADE)]
	static void UpgradeLightMap(){
		string[] t_scene_path = {
//			"Assets/_Project/ArtAssets/Scenes/Login",
//			"Assets/_Project/ArtAssets/Scenes/Loading",
//			"Assets/_Project/ArtAssets/Scenes/CreateRole",

//			"Assets/_Project/ArtAssets/Scenes/House",
	
			"Assets/_Project/ArtAssets/Scenes/MainCity",
			"Assets/_Project/ArtAssets/Scenes/AllianceCity",
			
			"Assets/_Project/ArtAssets/Scenes/PVE",
			"Assets/_Project/ArtAssets/Scenes/PVP",
			
			"Assets/_Project/ArtAssets/Scenes/Carriage",
		};
		
		for (int i = 0; i < t_scene_path.Length; i++) {
			UpgradeAllSceneLightMap( t_scene_path[ i ] );
		}
	}
	
	private static void UpgradeAllSceneLightMap( string p_path ){
		List<string> t_file_paths = GetFilePaths( p_path, "*.unity" );
		
		for( int i = 0; i < t_file_paths.Count; i++ ){
			EditorApplication.OpenScene( t_file_paths[ i ] );
			
			Lightmapping.Bake();
			
			EditorApplication.SaveScene();
		}
	}

	[MenuItem("Utility/LightMap Utility")]
	private static void LightMapUtility(){
		GameObject t_cur = Selection.activeGameObject;

		if( t_cur == null ){
			return;
		}

		Debug.Log( "Selected: " + t_cur );

		LightMapUtility t_char_develop = t_cur.GetComponent<LightMapUtility>();

		if( t_char_develop == null ){
			t_char_develop = t_cur.AddComponent<LightMapUtility>();
		}
	}

//	[MenuItem("Utility/Utilities/Check FS Names", false, (int)MenuItemPriority.UTILITIES___CHECK_FS_NAMES)]
	static void CheckFSNames(){
		Debug.Log( "CheckFSNames()" );

		ResetFSNameCount();

		CheckFileSystemNames( Application.dataPath );

		{
			string t_text = "";

			string t_path = Application.dataPath + "/Report/PathNameWithSpace.txt";

			for( int i = 0; i < m_change_path_list.Count; i++ ){
				t_text = t_text + i + " " +
					m_change_path_list[ i ] + "\n";
			}

			FileHelper.OutputFile( t_path, t_text );
		}
	}

	#endregion



	#region Char Dev

	[MenuItem("Utility/New Char Develop", false, (int)MenuItemPriority.NEW_CHAR_DEVELOP)]
	static void NewCharDevelop(){
		GameObject t_gb = new GameObject();

		t_gb.name = "New Char";

		DevelopCharacters t_char_develop = t_gb.AddComponent<DevelopCharacters>();

		t_char_develop.m_name = t_gb.name;

		{
			Object[] selection = Selection.GetFiltered( typeof(GameObject), SelectionMode.TopLevel );

			if( selection.Length != 0 ){
				GameObject t_selected = (GameObject)selection[ 0 ];
				
				t_char_develop.gameObject.transform.position = t_selected.transform.position + new Vector3( 0, 1.0f, 0 );
			}
		}
	}

	#endregion



	#region Bottom Part

	[MenuItem("Utility/Find Font Usage")]
    private static void FindFontUsage()
    {
        EditorWindow.GetWindow<FindFontUsageEditorWindow>(false, "Find Font Usage", true);
    }

    [MenuItem("Utility/Check Missing Script Entries")]
    public static void DoRemove()
    {
        foreach (Transform root in Selection.transforms)
        {
			TransformHelper.ErgodicChilds(root).ForEach(child => CheckComponentNull(child, root));
        }
    }

    [MenuItem("Utility/Set Label Font Window")]
    public static void SetLabelFont()
    {
        EditorWindow.GetWindow<SetLabelFontWindow>(false, "Set Label Font", true);
    }

    [MenuItem("Utility/Check XML")]
    public static void CheckXML()
    {
        Thread temp = new Thread(DoCheckXML);
        temp.Start();
    }

    private static readonly string XmlFolder = Application.dataPath + "/Resources/_Data/Design/";

    private static void DoCheckXML()
    {
        List<string> filePaths = new DirectoryInfo(XmlFolder).GetFiles("*.xml").Select(item => item.FullName).ToList();

        foreach (var filePath in filePaths)
        {
            DoCheckXML(filePath);
        }

        Debug.LogWarning("CheckXML ends.");

        //Check xml period 2.
        CheckXml.StartCheckXML();
    }

    private static void DoCheckXML(string path)
    {
        if (new DirectoryInfo(XmlFolder).GetFiles("*.xml").Where(item => item.Name == "syspara.xml" || item.Name == "Activity.xml").Select(item => item.FullName).Contains(path))
        {
            return;
        }

        if (new DirectoryInfo(XmlFolder).GetFiles("*.xml").Where(item => item.Name == "HeroGrow.xml").Select(item => item.FullName).Contains(path))
        {
//            Debug.Log("");
        }

        var xmlDicList = XMLFileIO.ReadXML(path);
        if (xmlDicList == null || xmlDicList.Count == 0)
        {
            Debug.LogError("Fail to read xml file: " + path);
            return;
        }

        float temp;
        Dictionary<string, bool> typeDic = xmlDicList.First().ToDictionary(pair => pair.Key, pair =>
        {
            int count = 0;

            foreach (var item in xmlDicList)
            {
                if (string.IsNullOrEmpty(item[pair.Key]))
                {
                    count++;
                }

                //if cannot parse and not null, or contains comma.
                if ((!string.IsNullOrEmpty(item[pair.Key]) && !float.TryParse(item[pair.Key], out temp)) || item[pair.Key].Contains(","))
                {
                    return false;
                }
            }

            if (count >= 10)
            {
                return false;
            }

            return true;
        });

        foreach (var item in xmlDicList)
        {
            foreach (var pair in item)
            {
                if (!typeDic.ContainsKey(pair.Key))
                {
                    Debug.LogError("Inconsistent attribute: " + pair.Key + " in element index: " + xmlDicList.IndexOf(item) + " in file: " + path);
                    continue;
                }

                if (typeDic[pair.Key] && string.IsNullOrEmpty(pair.Value))
                {
                    Debug.LogError("Null or empty value in attribute: " + pair.Key + " in element index: " + xmlDicList.IndexOf(item) + " in file: " + path);
                    continue;
                }

                //if (float.TryParse(pair.Value, out temp) != typeDic[pair.Key])
                //{
                //    Debug.LogError("Wrong value type: " + (float.TryParse(pair.Value, out temp) ? "NUMBER" : "NOT_NUMBER") + " in attribute: " + pair.Key + " in element index: " + xmlDicList.IndexOf(item) + " in file: " + path);
                //    continue;
                //}
            }
        }
    }

    public static void CheckComponentNull(Transform targetTransform, Transform rootTransform)
    {
        foreach (Component c in targetTransform.GetComponents(typeof(Component)))
        {
            if (c == null)
            {
                Transform nowPathTransform = targetTransform;
                string path = targetTransform.name;
                while (nowPathTransform != rootTransform)
                {
                    if (nowPathTransform.parent == null)
                    {
                        break;
                    }
                    path = nowPathTransform.parent.name + "/" + path;
                    nowPathTransform = nowPathTransform.parent;
                }
                Debug.Log("NULL reference in: " + path);
            }
        }
    }

    #endregion



	#region Cur UI

//	[MenuItem("Utility/Utilities/Get Cur UI Image", false, (int)EditorUtilityUtilities.MenuItemPriority.UTILITIES___CUR_UI_IMAGES)]
	static void CurUI(){
		//		Debug.Log( "CurUI()" );

		TextAsset t_text = (TextAsset)AssetDatabase.LoadAssetAtPath( "Assets/Resources/_Temp.txt", typeof(TextAsset) );

		if( t_text == null ){
			Debug.LogError( "Text is null." );

			return;
		}

		Debug.Log( "t_text: " + t_text );

		string[] t_lines = t_text.text.Split( '\n' );

		for( int i = 0; i < t_lines.Length; i++ ){
			string t_line = t_lines[ i ];

			Debug.Log( "line " + i + " : " + t_line );

			string[] t_contents = t_line.Split( ' ' );

			string t_final = t_contents[ t_contents.Length - 1 ];

			int t_index = t_final.LastIndexOf( "/" );

			if( t_index < 0 ){
				Debug.LogError( "path error: " + t_final );

				continue;
			}

			t_final = t_final.Trim();

			string t_name = t_final.Substring( t_final.LastIndexOf( "/" ) + 1 );

			t_name = t_name.Trim();

			Debug.Log( "name: " + t_name );

			Debug.Log( "desktop: " + PathHelper.GetDeskTopPath() );

			string t_path = Path.Combine( PathHelper.GetDeskTopPath(), "Lab" );

			t_path = Path.Combine( t_path, "Images" );

			t_path = Path.Combine( t_path, t_name );

			Debug.Log( "full path: " + t_path );

			FileHelper.FileCopy( PathHelper.GetFullPath_WithRelativePath( t_final ), t_path );
		}
	}

	#endregion



	#region Check FS Names

	private static int m_need_to_change_path_count = 0;

	private static List<string> m_change_path_list = new List<string>();

	private static void ResetFSNameCount(){
		m_need_to_change_path_count = 0;

		m_change_path_list.Clear();
	}

	private static void CheckFileSystemNames( string p_path ){
		{
			CheckName( p_path );
		}

		DirectoryInfo t_dir = new DirectoryInfo( p_path );
		
		{
			DirectoryInfo[] t_dirs = t_dir.GetDirectories();
			
			for( int i = 0; i < t_dirs.Length; i++ ){
				DirectoryInfo t_sub_dir = t_dirs[ i ];
				
				CheckFileSystemNames( t_sub_dir.FullName.Replace( "\\", "/" ) );
			}
		}

		{
			FileInfo[] t_files = t_dir.GetFiles ();
			
			for( int i = 0; i < t_files.Length; i++ ){
				CheckName( t_files[ i ].FullName.Replace( "\\", "/" ) );
			}
		}
	}

	private static void CheckName( string p_path ){
//		Debug.Log( "CheckName( " + p_path + " )" );

		if( p_path.Contains( " " ) ){
			string t_final_path = p_path.Substring( Application.dataPath.Length + 1 );

			Debug.LogError( "Contain SPACE: " + t_final_path );

			m_change_path_list.Add( t_final_path );
		}
	}


	#endregion



	#region SVN Path

	/// Source: _Temp/_ZhangYuGu/Editor/Utility/Utilities/EditorUtilityUtilities.cs
	/// Target: /release/v1.1/Client/Unity_Project/Assets/_Project/ArtAssets/UIs/Loading/Prefabs/UI_Loading_Bg.prefab
//	[MenuItem("Utility/Utilities/Get SVN Path", false, (int)EditorUtilityUtilities.MenuItemPriority.UTILITIES___SVN_PATH)]
	static void ConvertSVNPath(){
		Debug.Log( "ConvertSVNPath()" );

		TextAsset t_text = (TextAsset)AssetDatabase.LoadAssetAtPath( "Assets/Resources/_Temp.txt", typeof(TextAsset) );

		string t_path = Application.dataPath + "/Resources/_SVN.txt";

		StringBuilder t_sb = new StringBuilder();

		string[] t_lines = t_text.text.Split( '\n' );

		for( int i = 0; i < t_lines.Length; i++ ){
			string t_line = t_lines[ i ];

			t_line = "/release/v1.2/Client/Unity_Project/Assets/" + t_line;

			t_sb.Append( t_line + "\n" );
		}

		FileHelper.WriteFile( t_path, t_sb.ToString() );
	}

	#endregion



	#region Utilities

	public static List<string> GetFilePaths( string p_dir, string p_pattern ){
		List<string> t_scene_names = new List<string>();

		string[] t_paths = Directory.GetFiles ( PathHelper.GetFullPath_WithRelativePath( p_dir ), p_pattern, SearchOption.AllDirectories );

		for (int i = 0; i < t_paths.Length; i++) {
			t_scene_names.Add( t_paths[ i ] );
		}

		return t_scene_names;
	}


	#endregion
}