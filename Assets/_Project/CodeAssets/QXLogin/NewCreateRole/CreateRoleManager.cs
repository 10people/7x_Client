using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreateRoleManager : MonoBehaviour {

	void Start ()
	{
		QXSelectRole.Instance ().SelectRolePage (QXSelectRolePage.SelectType.CREATE_ROLE);
	}
}
