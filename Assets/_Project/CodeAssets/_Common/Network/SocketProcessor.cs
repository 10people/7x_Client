using UnityEngine;
using System.Collections;

public interface SocketProcessor{

	/** Desc:
	 * 	Every Proto ONLY could have 1 processor, if not will raise an Error.
	 * 
	 *  Return:
	 *	true, if message processed, socket will delete the message.
	 *	false, if not requested message.
	 */	
	bool OnProcessSocketMessage( QXBuffer p_message );
}
