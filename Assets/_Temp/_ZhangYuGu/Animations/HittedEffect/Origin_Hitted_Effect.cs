using UnityEngine;
using System.Collections;

public class Origin_Hitted_Effect : MonoBehaviour {

//	public Color m_fx_color = Color.black;

	private HittedEffect m_hitted = null;

	private Renderer m_renderer = null;

	// Use this for initialization
	void Start () {
		m_renderer = gameObject.GetComponentInChildren<Renderer>();

		m_hitted = (HittedEffect)gameObject.GetComponent<HittedEffect>();

		InvokeRepeating( "Play", 1.0f, 3.0f );
	}

	public void Play(){
		Animation t_anim = GetComponent<Animation>();

		t_anim.Play();
	}
	
	// Update is called once per frame
	void Update () {
		m_renderer.material.SetColor( "_FxColor", m_hitted.m_fx_color );
	}
}
