using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneHandler : MonoBehaviour
{
    public static SceneHandler Instance { get; private set; }
    public readonly static string TitleScene = "TitleScene";
    public readonly static string RoomScene = "RoomScene";
    public readonly static string InGameScene = "InGameScene";
    public readonly static string ServerManagerScene = "ServerManagerScene";
    public Dictionary<string, LoadSceneMode> loadScenes = new Dictionary<string, LoadSceneMode>();
    public Image fadeImage;
    public float fadeDuration = 1.0f;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        InitSceneInfo();
        if (SceneManager.sceneCount != 1)
        {
            return;
        }
        LoadScene(ServerManagerScene);
        LoadSceneWithFade(TitleScene);
    }
    
    private void InitSceneInfo()
    {
        loadScenes.Add(TitleScene, LoadSceneMode.Additive);
        loadScenes.Add(RoomScene, LoadSceneMode.Additive);
        loadScenes.Add(InGameScene, LoadSceneMode.Additive);
        loadScenes.Add(ServerManagerScene, LoadSceneMode.Additive);
    }

    public void LoadSceneWithFade(string sceneName)
    {
        if (loadScenes.ContainsKey(sceneName))
        {
            StartCoroutine(LoadSceneFadeRoutine(sceneName, loadScenes[sceneName]));
        }
        else
        {
            Debug.LogError($"Scene {sceneName} not found in loadScenes dictionary");
        }
    }

    public void LoadScene(string sceneName)
    {
        if (loadScenes.ContainsKey(sceneName))
        {
            StartCoroutine(LoadSceneRoutine(sceneName, loadScenes[sceneName]));
        }
        else
        {
            Debug.LogError($"Scene {sceneName} not found in loadScenes dictionary");
        }
    }
    private IEnumerator LoadSceneFadeRoutine(string sceneName, LoadSceneMode mode)
    {
        yield return StartCoroutine(FadeOut());

        yield return StartCoroutine(LoadSceneRoutine(sceneName, mode));

        yield return StartCoroutine(FadeIn());
    }

    private IEnumerator LoadSceneRoutine(string sceneName, LoadSceneMode mode)
    {
        if (sceneName == RoomScene)
        {
            yield return SceneManager.UnloadSceneAsync(TitleScene);
        }
        else if (sceneName == InGameScene)
        {
            yield return SceneManager.UnloadSceneAsync(RoomScene);
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, mode);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
    
    private IEnumerator FadeOut()
    {
        float elapsedTime = 0.0f;
        Color tempColor = fadeImage.color;
        fadeImage.raycastTarget = true;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            tempColor.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeImage.color = tempColor;
            yield return null;
        }
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0.0f;
        Color tempColor = fadeImage.color;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            tempColor.a = Mathf.Clamp01(1.0f - (elapsedTime / fadeDuration));
            fadeImage.color = tempColor;
            yield return null;
        }

        fadeImage.raycastTarget = false;
    }
}
