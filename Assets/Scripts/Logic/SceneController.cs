using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    private static SceneController m_Instance;
    private static Animator m_CurtainAnimator;

    private const string MAIN_SCENE = "Main";
    private const string MENU_SCENE = "Menu";

    private const string CLOSE_TRIGGER = "isClosing";

    public static void ChangeScene()
    {
        TryInit();

        m_Instance.StartCoroutine(m_Instance.DoChangeScene());
    }

    private IEnumerator DoChangeScene()
    {
        m_CurtainAnimator.SetBool(CLOSE_TRIGGER, true);

        yield return new WaitForSeconds(2);

        SceneManager.LoadScene(MAIN_SCENE);

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