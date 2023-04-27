using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    [SerializeField] private Rigidbody m_Rigidbody;

    private static Plane m_Plane = new Plane(Vector3.up, 0);

    private Vector3 m_StartPos;

    private List<Dropzone> m_TouchingDropzones = new();

    private void Start()
    {
        m_StartPos = transform.position;
    }

    private void OnMouseDrag()
    {
        float distance;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (m_Plane.Raycast(ray, out distance))
        {
            m_Rigidbody.position = ray.GetPoint(distance);
        }
    }

    private void OnMouseUp()
    {
        if (m_TouchingDropzones.Count == 1)
        {
            Dropzone targetZone = m_TouchingDropzones[0];

            m_Rigidbody.position = targetZone.transform.position;
        }
        else
        {
            m_Rigidbody.position = m_StartPos;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Dropzone currentDropzone = other.gameObject.GetComponent<Dropzone>();

        if (currentDropzone != null) 
        {
            m_TouchingDropzones.Add(currentDropzone);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Dropzone currentDropzone = other.gameObject.GetComponent<Dropzone>();

        if (currentDropzone != null)
        {
            m_TouchingDropzones.Remove(currentDropzone);
        }
    }
}
