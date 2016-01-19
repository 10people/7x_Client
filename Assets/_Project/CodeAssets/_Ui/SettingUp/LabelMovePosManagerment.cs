using UnityEngine;
using System.Collections;
public class LabelMovePosManagerment : MonoBehaviour {
     
    private static GameObject _Clone = null;
    public static void CreateMove(GameObject move, string content,int move_index)
    {
        if (_Clone == null)
        {
            _Clone = NGUITools.AddChild(move.transform.parent.gameObject, move);
            _Clone.SetActive(true);
            _Clone.transform.localPosition = move.transform.localPosition;
            _Clone.transform.localRotation = move.transform.localRotation;
            _Clone.transform.localScale = move.transform.localScale;
            _Clone.GetComponent<UILabel>().text = MyColorData.getColorString(5, content);
            _Clone.AddComponent(typeof(TweenPosition));
            //clone.AddComponent(typeof(TweenAlpha));
            _Clone.GetComponent<TweenPosition>().from = move.transform.localPosition;
            _Clone.GetComponent<TweenPosition>().to = move.transform.localPosition + Vector3.up * move_index;
            _Clone.GetComponent<TweenPosition>().duration = 0.5f;
            //clone.GetComponent<TweenAlpha>().from = 1.0f;
            //clone.GetComponent<TweenAlpha>().to = 0;
            _Clone.GetComponent<TweenPosition>().duration = 1.2f;
            EventDelegate.Add(_Clone.GetComponent<TweenPosition>().onFinished, MoveFinish);
        }
    }

   private static  void MoveFinish()
   {
       Destroy(_Clone);
   }
}
