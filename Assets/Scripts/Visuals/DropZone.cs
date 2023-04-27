using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dropzone : MonoBehaviour
{
    [SerializeField] private GameObject m_HoverValidPlate;

    // TODO: Turn on and off the plate on collision

    private void OnTriggerEnter(Collider other)
    {
        m_HoverValidPlate.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        m_HoverValidPlate.SetActive(false);
    }
}
