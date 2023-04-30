using System;
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

    private List<Dropzone> m_OccupiedDropzones = new();

    // Game Logic needs to track which piece occupies which dropzone
    public event Action<Dropzone> OnDropped;

    public event Action OnGrabbed;

    private void Start()
    {
        m_StartPos = transform.position;
    }

    private void OnMouseDown()
    {
        if (!enabled)
        {
            return;
        }

        OnGrabbed?.Invoke();
    }

    private void OnMouseDrag()
    {
        if (!enabled)
        {
            return;
        }

        float distance;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (m_Plane.Raycast(ray, out distance))
        {
            m_Rigidbody.position = ray.GetPoint(distance);
        }
    }

    private void OnMouseUp()
    {
        if (!enabled)
        {
            return;
        }

        if (m_OccupiedDropzones.Count == 1)
        {
            Dropzone targetZone = m_OccupiedDropzones[0];

            m_Rigidbody.position = targetZone.transform.position;

            OnDropped(targetZone);
        }
        else
        {
            m_Rigidbody.position = m_StartPos;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Dropzone currentDropzone = other.gameObject.GetComponent<Dropzone>();

        if (currentDropzone != null && currentDropzone.enabled) 
        {
            m_OccupiedDropzones.Add(currentDropzone);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Dropzone currentDropzone = other.gameObject.GetComponent<Dropzone>();

        if (currentDropzone != null && currentDropzone.enabled)
        {
            m_OccupiedDropzones.Remove(currentDropzone);
        }
    }
}
