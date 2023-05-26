using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour
{
    [SerializeField] private Rigidbody m_Rigidbody;

    private Vector3 m_Offset;

    private static Plane m_Plane = new Plane(Vector3.up, 0);

    private Vector3 m_StartPos;

    private List<Dropzone> m_OccupiedDropzones = new();

    // Game Logic needs to track which piece occupies which dropzone
    public event Action<Dropzone> OnDropped;

    public event Action OnGrabbed;

    public event Action OnDragEnded;

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

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (m_Plane.Raycast(ray, out float distance)) // && !EventSystem.current.IsPointerOverGameObject()
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

            OnDropped(targetZone);
        }
        else
        {
            m_Rigidbody.position = m_StartPos;
            OnDragEnded?.Invoke();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Dropzone dropzone = other.gameObject.GetComponent<Dropzone>();

        if (dropzone != null && dropzone.enabled) 
        {
            m_OccupiedDropzones.Add(dropzone);
            
            if (m_OccupiedDropzones.Count == 1)
            {
                dropzone.DropzoneRingHelper.TargerRingOn();
            }
            else
            {
                for (int i = 0; i < m_OccupiedDropzones.Count; i++)
                {
                    Dropzone currentDropzone = m_OccupiedDropzones[i];
                    currentDropzone.DropzoneRingHelper.ValidRingOn();
                }
            }
        }    
    }

    private void OnTriggerExit(Collider other)
    {
        Dropzone dropzone = other.gameObject.GetComponent<Dropzone>();

        if (dropzone != null && dropzone.enabled)
        {
            m_OccupiedDropzones.Remove(dropzone);
           dropzone.DropzoneRingHelper.ValidRingOn();
        }

        if (m_OccupiedDropzones.Count == 1)
        {
            m_OccupiedDropzones[0].DropzoneRingHelper.TargerRingOn();
        }
    }
}
