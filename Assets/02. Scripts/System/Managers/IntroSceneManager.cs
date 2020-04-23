using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroSceneManager : MonoBehaviour
{

    private bool active;

    public void allowSceneActivation()
    {
        active = true;
    }

    IEnumerator LoadingScene(string sceneName)
    {
        AsyncOperation asyncOpe = SceneManager.LoadSceneAsync(sceneName);
        asyncOpe.allowSceneActivation = false;

        while (!asyncOpe.isDone)
        {
            if(active)
                asyncOpe.allowSceneActivation = true;

            yield return null;
        }
    }
}
