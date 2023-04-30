using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dropzone : MonoBehaviour
{
    [SerializeField] private EGrid m_GridID;
    public EGrid GridID => m_GridID;

    [SerializeField] private GameObject m_HoverValidPlate;

    private void OnTriggerEnter(Collider other)
    {
        m_HoverValidPlate.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        m_HoverValidPlate.SetActive(false);
    }
}
