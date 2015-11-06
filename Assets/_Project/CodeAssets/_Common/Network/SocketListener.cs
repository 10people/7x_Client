using UnityEngine;
using System.Collections;

public interface SocketListener{

	/** Desc:
	 * 	Every Proto could have Many Listeners.
	 * 
	 *  Return:
	 *	true, if message listened.
	 *	false, if not listened.
	 */
	bool OnSocketEvent( QXBuffer p_message );
}
