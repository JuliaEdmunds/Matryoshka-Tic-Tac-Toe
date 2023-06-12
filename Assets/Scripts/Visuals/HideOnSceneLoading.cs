using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class HideOnSceneLoading : MonoBehaviour
{
    [SerializeField] private GameObject m_ButtonToHide;

    private void Awake()
    {
        SceneController.OnStartedLoading += OnStartedLoading;
        SceneController.OnFinishedLoading += OnFinishedLoading;

        if (SceneController.IsLoading)
        {
            OnStartedLoading();
        }
    }

    private void OnStartedLoading()
    {
        m_ButtonToHide.SetActive(false);
    }

    private void OnFinishedLoading()
    {
        m_ButtonToHide.SetActive(true);
    }

    private void OnDestroy()
    {
        SceneController.OnStartedLoading -= OnStartedLoading;
        SceneController.OnFinishedLoading -= OnFinishedLoading;
    }
}

