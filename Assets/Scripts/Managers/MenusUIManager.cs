using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenusUIManager : MonoBehaviour
{
    public void OnPlayButtonClicked()
    {
        MainManagers.Instance.m_LoadingHandler.LoadScene(LoadingHandler.SCENE.GAMEPLAY);
    }
}
