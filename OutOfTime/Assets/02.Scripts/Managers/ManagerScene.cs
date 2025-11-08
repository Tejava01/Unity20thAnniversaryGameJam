using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneName
{
    SceneTitle,
    SceneBoss,
    SceneTestAge,
}

public class ManagerScene : Singleton<ManagerScene>
{

    [SerializeField] private float _minLoadingTime = 1f;

    public void LoadScene(SceneName sceneName)
    {
        StartCoroutine(CoroutineLoadScene(sceneName.ToString()));
    }

    //---------------------------------------------

    private IEnumerator CoroutineLoadScene(string sceneName)
    {
        float startTime = Time.time;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        string previousActiveScene = SceneManager.GetActiveScene().name;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));

        float loadingTime = Time.time - startTime;
        if (loadingTime < _minLoadingTime)
        {
            yield return new WaitForSeconds(_minLoadingTime - loadingTime);
        }

        SceneManager.UnloadSceneAsync(previousActiveScene);
    }
}
