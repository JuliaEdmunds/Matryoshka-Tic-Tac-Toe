using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    [SerializeField] private EPlayer m_PlayerID;

    [SerializeField] private EPiece m_PieceType;

    [SerializeField] private DragAndDrop m_DragAndDrop;

    public event Action<Piece, Dropzone> OnGridOccupied;
    public event Action<Piece> OnPieceGrabbed;

    public EPlayer PlayerID => m_PlayerID;
    public EPiece PieceType => m_PieceType;

    private void Start()
    {
        m_DragAndDrop.OnDropped += OnDropped;
        m_DragAndDrop.OnGrabbed += OnGrabbed;
    }

    private void OnGrabbed()
    {
        OnPieceGrabbed(this);
    }

    private void OnDropped(Dropzone tile)
    {
        OnGridOccupied(this, tile);
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
