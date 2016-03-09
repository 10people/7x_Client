using UnityEngine;

public class DestroyAfterRendered : MonoBehaviour{

	public void OnRenderObject(){
		gameObject.SetActive( false );
		
		Destroy( gameObject );
	}
}