using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    [SerializeField] private GameObject m_ValidPieceRing;
    public GameObject ValidPieceRing => m_ValidPieceRing;

    [SerializeField] private EPlayerColour m_PlayerID;
    public EPlayerColour PlayerID => m_PlayerID;
    public EPiece PieceType => m_PieceType;

    [SerializeField] private EPiece m_PieceType;

    [SerializeField] private DragAndDrop m_DragAndDrop;

    public event Action<Piece, Dropzone> OnGridOccupied;
    public event Action<Piece> OnPieceGrabbed;
    public event Action OnPieceReleased;


    private void Start()
    {
        m_DragAndDrop.OnDropped += OnDropped;
        m_DragAndDrop.OnGrabbed += OnGrabbed;
        m_DragAndDrop.OnDragEnded += OnDragEnded;
    }

    private void OnGrabbed()
    {
        TargetController.Instance.ActivateTarget(this);
        OnPieceGrabbed(this);
    }

    private void OnDropped(Dropzone tile)
    {
        TargetController.Instance.ResetTarget();
        OnGridOccupied(this, tile);
    }

    private void OnDragEnded()
    {
        TargetController.Instance.ResetTarget();
        OnPieceReleased();
    }

    public void EnableDrag()
    {
        m_DragAndDrop.enabled = true;
    }

    public void DisableDrag()
    {
        m_DragAndDrop.enabled = false;
    }
}
