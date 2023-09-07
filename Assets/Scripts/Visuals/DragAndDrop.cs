using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour
{
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
        // block if not enabled or pointer is over some UI
        if (!enabled || EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        OnGrabbed?.Invoke();
    }

    private void OnMouseDrag()
    {
        // block if not enabled or pointer is over some UI
        if (!enabled || EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        int layerMask = 1 << LayerMask.NameToLayer("DropSurface");

        if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f, layerMask, QueryTriggerInteraction.Collide))
        {
            TargetController.Instance.Rigidbody.position = hitInfo.point;
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
