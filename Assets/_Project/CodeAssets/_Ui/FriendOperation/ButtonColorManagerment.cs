using UnityEngine;
using System.Collections;

public class ButtonColorManagerment : MonoBehaviour 
{
    public GameObject m_BackObj;
    private GameObject m_ObjShake;
    private enum ShakeType
    {
        SHAKE_NONE,
        SHAKE_RUNNING,
        SHAKE_STOP,
    };
    private ShakeType _ShakeTypeInfo;
    private float _timeInterval = 0;
    private float _ValueTime = 2.0f;
    void Update()
    {
        if (_ShakeTypeInfo == ShakeType.SHAKE_RUNNING && _timeInterval < _ValueTime)
        {
           _timeInterval += Time.deltaTime;
        }

        if ( _timeInterval >= _ValueTime)
        {
            switch (_ShakeTypeInfo)
            {
                case ShakeType.SHAKE_RUNNING:
                    {
                        _timeInterval = 0;
                        iTween.ShakeRotation(m_ObjShake, new Vector3(0, 0, 20f), 0.8f);
                    }
                    break;
               case ShakeType.SHAKE_STOP:
                    {

                    }
                    break;
            }
        }
    }
 
    public void ButtonsControl(bool colliderEnable)
    {
        if (m_BackObj.GetComponent<TweenColor>() == null)
        {
            m_BackObj.gameObject.AddComponent<TweenColor>();
            m_BackObj.gameObject.AddComponent<TweenColor>().enabled = false;
        }
        if (colliderEnable)
        {
            m_BackObj.GetComponent<TweenColor>().from = new Color(100 / 255.0f, 100 / 255.0f, 100 / 255.0f);
            m_BackObj.GetComponent<TweenColor>().to = new Color(1.0f, 1.0f, 1.0f);
        }
        else 
        {
            m_BackObj.GetComponent<TweenColor>().from = new Color(1.0f, 1.0f, 1.0f);
            m_BackObj.GetComponent<TweenColor>().to = new Color(100 / 255.0f, 100 / 255.0f, 100 / 255.0f);
        }
        m_BackObj.GetComponent<TweenColor>().duration = 0.08f;
        m_BackObj.GetComponent<TweenColor>().enabled = true;
        transform.GetComponent<Collider>().enabled = colliderEnable;
       EventDelegate.Add(m_BackObj.GetComponent<TweenColor>().onFinished, TweenColorDestroy);

    }

    void TweenColorDestroy()
    {
        if (m_BackObj.GetComponent<TweenColor>() != null)
        {
            EventDelegate.Remove(m_BackObj.GetComponent<TweenColor>().onFinished, TweenColorDestroy);
            Destroy(m_BackObj.GetComponent<TweenColor>());
        }
    }
    public void ShakeEffectShow(bool isTurn = false,GameObject obj = null)
    {
        if (isTurn)
        {
            if (obj)
            {
                m_ObjShake = obj;
            }
            else
            {
                m_ObjShake = m_BackObj;
            }
            _ShakeTypeInfo = ShakeType.SHAKE_RUNNING;
        }
        else
        {
            _ShakeTypeInfo = ShakeType.SHAKE_STOP;
        }
    }
 
}
