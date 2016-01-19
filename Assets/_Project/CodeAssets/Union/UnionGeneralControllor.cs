using UnityEngine;
using System.Collections;

public class UnionGeneralControllor : MonoBehaviour
{
	public GameObject gc_root;

	public UnionUIControllor union_1;

	public UnionDetailUIControllor union_2;


	private static UnionGeneralControllor _instance;


	void Awake() { _instance = this; }
	
	public static UnionGeneralControllor Instance() { return _instance; }

	void Start()
	{
		if(JunZhuData.Instance().m_junzhuInfo.lianMengId <= 0)
		{
			OnShowUnion_1();
		}
		else
		{
			OnShowUnion_2();
		}
	}

	void OnDestroy(){
		_instance = null;
	}

	private void OnCloseLayer()
	{
		union_1.gameObject.SetActive(false);

		union_2.gameObject.SetActive(false);
	}

	public void OnShowUnion_1()
	{
		OnCloseLayer();

		union_1.gameObject.SetActive(true);
	}

	public void OnShowUnion_2()
	{
		OnCloseLayer();

		union_2.gameObject.SetActive(true);
	}

	public void close()
	{
		Destroy(gc_root);
	}

}
