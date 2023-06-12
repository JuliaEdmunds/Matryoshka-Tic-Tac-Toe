using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    private static SceneController m_Instance;
    public static SceneController Instance => m_Instance;
    public static bool IsCreated => Instance != null;

    private static Animator m_CurtainAnimator;

    private const string CLOSE_TRIGGER = "isClosing";

    public static bool IsLoading { get; private set; }
    public static event Action OnStartedLoading;
    public static event Action OnFinishedLoading;

    public static void ChangeScene(EScene sceneToLoad)
    {
        TryInit();

        m_Instance.StartCoroutine(m_Instance.DoChangeScene(sceneToLoad));
    }

    private IEnumerator DoChangeScene(EScene sceneToLoad)
    {
        IsLoading = true;
        OnStartedLoading?.Invoke();

        string scene = sceneToLoad.ToString();

        m_CurtainAnimator.SetBool(CLOSE_TRIGGER, true);

        yield return new WaitForSeconds(2);

        SceneManager.LoadScene(scene);

        IsLoading = false;
        OnFinishedLoading?.Invoke();

        m_CurtainAnimator.SetBool(CLOSE_TRIGGER, false);

    }

    public static Camera GetOverlayCamera()
    {
        TryInit();

        return m_Instance.GetComponentInChildren<Camera>();
    }

    private static void TryInit()
    {
        if (m_Instance == null)
        {
            GameObject curtainPrefab = Resources.Load<GameObject>("Curtain");

            Vector3 curtainPos = new(1000, 0, 0);
            GameObject curtain = Instantiate(curtainPrefab, curtainPos, Quaternion.identity);
            DontDestroyOnLoad(curtain);

            m_Instance = curtain.GetComponent<SceneController>();
            m_CurtainAnimator = curtain.GetComponent<Animator>();
        }
    }
}
