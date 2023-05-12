using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    [SerializeField] private GameObject m_ValidPieceRing;
    public GameObject ValidPieceRing => m_ValidPieceRing;

    [SerializeField] private EPlayer m_PlayerID;
    public EPlayer PlayerID => m_PlayerID;
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
        OnPieceGrabbed(this);
    }

    private void OnDropped(Dropzone tile)
    {
        OnGridOccupied(this, tile);
    }

    private void OnDragEnded()
    {
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
