using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    private static Plane m_Plane = new Plane(Vector3.up, 0);

    private Vector3 m_StartPos;

    private void OnMouseDown()
    {
        m_StartPos = transform.position;
    }

    private void OnMouseDrag()
    {
        float distance;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (m_Plane.Raycast(ray, out distance))
        {
            transform.position = ray.GetPoint(distance);
        }
    }


    private void OnMouseUp()
    {
        transform.position = m_StartPos;
    }

    // private Vector3 m_MousePosOffset;
    // 
    // private float m_ZCoord;
    // 
    // private void OnMouseDown()
    // {
    //     m_ZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
    // 
    //     // Store offset = gameobject world pos - mouse world pos
    //     m_MousePosOffset = gameObject.transform.position - GetMouseWorldPos();
    // }
    // 
    // private void OnMouseDrag()
    // {
    //     transform.position = GetMouseWorldPos() - m_MousePosOffset;
    // }
    // 
    // private Vector3 GetMouseWorldPos()
    // {
    //     // Pixel coordinates of mouse (x,y)
    //     Vector3 mousePoint = Input.mousePosition;
    //     
    // 
    //     // z coordinate of game object on screen
    //     mousePoint.z = m_ZCoord;
    // 
    //     // TODO: doesn not work as bound system...
    //     // if (transform.position.y < 0)
    //     // {
    //     //     mousePoint.y = 0;
    //     // }
    // 
    //     // Convert it to world points
    //     return Camera.main.ScreenToWorldPoint(mousePoint);
    // }
}
