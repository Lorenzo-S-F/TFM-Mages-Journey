using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingHandler : MonoBehaviour
{
    public Camera m_InitCamera;

    public string m_MenusScene;
    public string m_GameplayScene;

    public Image m_FadeImage;

    private bool m_LoadingScene;
    private string m_CurrentSceneLoaded = string.Empty;

    private void Awake()
    {
        m_LoadingScene = true;
        SceneManager.sceneLoaded += OnSceneLoaded;
        StartCoroutine(LoadSceneCoroutine(m_MenusScene, SCENE.MENUS));
    }

    public void LoadScene(SCENE scene)
    {
        if (m_LoadingScene)
            return;
        m_LoadingScene = true;

        switch (scene)
        {
            case SCENE.UNDEFINED:
                break;
            case SCENE.GAMEPLAY:
                StartCoroutine(LoadSceneCoroutine(m_GameplayScene, SCENE.GAMEPLAY));
                break;
            case SCENE.MENUS:
                StartCoroutine(LoadSceneCoroutine(m_MenusScene, SCENE.MENUS));
                break;
        }
    }

    private IEnumerator LoadSceneCoroutine(string loadScene, SCENE scene)
    {
        m_FadeImage.raycastTarget = true;
        float t = 0;
        float from = 0;
        float to = 1;
        if (m_CurrentSceneLoaded != string.Empty)
        {
            while (t <= 1)
            {
                m_FadeImage.color = new Color(m_FadeImage.color.r, m_FadeImage.color.g, m_FadeImage.color.b, Mathf.Lerp(from, to, t));
                t += Time.deltaTime;
                yield return null;
            }

            // Camera change Scene -> Generic
            m_InitCamera.gameObject.SetActive(true);
            switch (scene)
            {
                case SCENE.GAMEPLAY:
                    MenusManagers.Instance.m_MenusCamera.gameObject.SetActive(false);
                    break;
                case SCENE.MENUS:
                    GameplayManagers.Instance.m_GameplayCamera.gameObject.SetActive(false);
                    break;
            }

            yield return SceneManager.UnloadSceneAsync(m_CurrentSceneLoaded);
        }

        PopupsManager.Instance.ClosePopup();

        yield return SceneManager.LoadSceneAsync(loadScene, LoadSceneMode.Additive);
        
        // Camera change Generic -> Scene
        m_InitCamera.gameObject.SetActive(false);
        switch (scene)
        {
            case SCENE.GAMEPLAY:
                GameplayManagers.Instance.m_GameplayCamera.gameObject.SetActive(true);
                yield return GameplayManagers.Instance.Initialize();
                break;
            case SCENE.MENUS:
                MenusManagers.Instance.m_MenusCamera.gameObject.SetActive(true);
                yield return MenusManagers.Instance.Initialize();
                break;
        }

        t = 0;
        from = 1;
        to = 0;
        while (t <= 1)
        {
            m_FadeImage.color = new Color(m_FadeImage.color.r, m_FadeImage.color.g, m_FadeImage.color.b, Mathf.Lerp(from, to, t));
            t += Time.deltaTime;
            yield return null;
        }

        m_CurrentSceneLoaded = loadScene;

        m_FadeImage.raycastTarget = false;

        m_LoadingScene = false;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        m_LoadingScene = false;
    }

    public enum SCENE
    {
        UNDEFINED,
        GAMEPLAY,
        MENUS
    }
}
