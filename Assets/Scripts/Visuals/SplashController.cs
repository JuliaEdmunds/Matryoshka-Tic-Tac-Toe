using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashController : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(LoadScene());
    }

    private IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(3);

        EScene sceneToLoad = LanguageHelper.HasChosenLanguage ? EScene.Menu : EScene.LanguageSettings;
        SceneManager.LoadScene(sceneToLoad.ToString());
    }
}
