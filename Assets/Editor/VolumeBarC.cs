using UnityEditor;

using UnityEngine;

using System.Collections;

 

[CustomEditor(typeof(MicControlC))]

public class VolumeBarC : Editor {

 

    public override void OnInspectorGUI () {

        MicControlC micCon  = (MicControlC) target;

        float micInputValue = micCon.loudness;      

        VolumeReader (micInputValue/100, "Loudness " + micInputValue);      

        EditorUtility.SetDirty(target); 

        DrawDefaultInspector();

    }

 

    void VolumeReader (float value, string label) {     

        Rect vRect = GUILayoutUtility.GetRect (18, 18, "TextField");        

        EditorGUI.ProgressBar (vRect, value, label);

        EditorGUILayout.Space ();

    }

}