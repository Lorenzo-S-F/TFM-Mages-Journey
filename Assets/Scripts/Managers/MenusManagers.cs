using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenusManagers : Singleton<MenusManagers>
{
    public Camera m_MenusCamera;

    public IEnumerator Initialize()
    {
        yield return null;
    }
}
