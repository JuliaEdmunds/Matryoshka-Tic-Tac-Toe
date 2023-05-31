using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour
{
    private static Plane m_Plane = new Plane(Vector3.up, 0);

    private Vector3 m_StartPos;

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
            TargetController.Instance.Rigidbody.position = ray.GetPoint(distance);
        }
    }

    private void OnMouseUp()
    {
        List<Dropzone> occupiedDropzones = TargetController.Instance.OccupiedDropzones;

        if (!enabled)
        {
            return;
        }
        
        if (occupiedDropzones.Count == 1)
        {
            Dropzone targetZone = occupiedDropzones[0];

            OnDropped(targetZone);
        }
        else
        {
            TargetController.Instance.Rigidbody.position = m_StartPos;
            OnDragEnded?.Invoke();
        }
    }
}
