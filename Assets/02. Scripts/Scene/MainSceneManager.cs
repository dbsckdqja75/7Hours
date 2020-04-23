using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainSceneManager : MonoBehaviour
{

    public Image mainPanel;

    public Image loadingPanel;
    public Image loadingFill;

    private bool isLoad;
    private bool isMute;

    private AudioSource musicAudioSource;

    void Awake()
    {
        musicAudioSource = Camera.main.GetComponent<AudioSource>();
    }

    void Update()
    {
        if(isMute)
            musicAudioSource.volume -= 0.2f * Time.deltaTime;
    }

    public void QueueLoadingScene(string sceneName)
    {
        mainPanel.gameObject.SetActive(false);
        loadingPanel.gameObject.SetActive(true);

        isMute = true;

        StartCoroutine(LoadingScene(sceneName));
    }

    IEnumerator LoadingScene(string sceneName)
    {
        AsyncOperation asyncOpe = SceneManager.LoadSceneAsync(sceneName);
        asyncOpe.allowSceneActivation = false;

        while (!asyncOpe.isDone && !isLoad)
        {
            if(asyncOpe.progress >= 0.8f)
                isLoad = true;

            loadingFill.fillAmount = asyncOpe.progress;
            yield return null;
        }

        loadingFill.fillAmount = 1;

        yield return new WaitForSeconds(1.5f);
        asyncOpe.allowSceneActivation = true;
    }

    public void PanelActive(GameObject panel)
    {
        panel.gameObject.SetActive(!panel.activeSelf);
    }

    public void ApplicationQuit()
    {
        Application.Quit();
    }
}
