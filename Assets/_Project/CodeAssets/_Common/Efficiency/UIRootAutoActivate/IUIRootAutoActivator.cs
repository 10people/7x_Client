using UnityEngine;
using System.Collections;

/// Interface for auto activate&deactivate ngui root gameobject.
/// 
/// Notice:
/// 1.all subclass MUST be MonoBehaviour.
/// 2.All subclass should have only 1 camera(best case).
public interface IUIRootAutoActivator {

	/// true, if NGUI page should be visible;
	/// false, invisible.
	bool IsNGUIVisible();
}