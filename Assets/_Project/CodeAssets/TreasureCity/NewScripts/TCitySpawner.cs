using UnityEngine;
using System.Collections;

public class TCitySpawner : MonoBehaviour {

	public float m_spawnRadius = 10;
	public int m_numberOfAgents = 50;
	public GameObject m_enemyPrefab;
	public Transform m_player;
	
	void Start ()
	{
		for (int i=0; i < m_numberOfAgents; i++) 
		{
			// Choose a random location within the spawnRadius
			Vector2 randomLoc2d = Random.insideUnitCircle * m_spawnRadius;
			Vector3 randomLoc3d = new Vector3 (transform.position.x + randomLoc2d.x, transform.position.y, transform.position.z + randomLoc2d.y);
			
			// Make sure the location is on the NavMesh
			NavMeshHit hit;
			if (NavMesh.SamplePosition(randomLoc3d, out hit, 100, 1))
			{
				randomLoc3d = hit.position;
			}
			
			// Instantiate and make the enemy a child of this object
			GameObject o = (GameObject)Instantiate(m_enemyPrefab, randomLoc3d, transform.rotation);
//			o.GetComponent< TCityPersonMove >().m_player = m_player;
		}
	}
	
	void OnDrawGizmosSelected ()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere (transform.position, m_spawnRadius);
	}

}
