//#define DEBUG_SOCKET_HELPER

using UnityEngine;
using System.Collections;

public class SocketHelper {

	/**Any global listener and processor could be registered here.
	 * 
	 * Notes:
	 * 1.Execute when SocketTool.Awake().
	 * 2.Should not manual enovke this.
	 * 3.Every processor or listener should be only exist once.
 	 */
	public static void RegisterGlobalProcessorAndListeners(){
		#if DEBUG_SOCKET_HELPER
		Debug.Log( "RegisterGlobalProcessorAndListeners()" );
		#endif

		GameObject t_gb = UtilityTool.GetDontDestroyOnLoadGameObject ();

		ComponentHelper.AddIfNotExist ( t_gb, typeof(PushAndNotificationHelper) );
	}

}
