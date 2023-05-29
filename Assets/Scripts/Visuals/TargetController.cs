using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    [SerializeField] private VisualGameManager m_VisualGameManager;
    [SerializeField] private List<Piece> m_AllPieces;
    [SerializeField] private Vector3 m_Offset;

    private bool m_WasDropped;

    private void Start()
    {
        gameObject.SetActive(false);
        m_AllPieces.ForEach(piece => { piece.OnGridOccupied += OnGridOccupied; piece.OnPieceGrabbed += OnPieceGrabbed; piece.OnPieceReleased += OnPieceReleased; });
    }

    private void OnPieceGrabbed(Piece piece)
    {
        gameObject.SetActive(true);
        m_WasDropped = false;

        StartCoroutine(KeepMvoingTarget(piece));
    }

    private IEnumerator KeepMvoingTarget(Piece piece)
    {
        while (!m_WasDropped)
        {
            gameObject.transform.position = piece.transform.position + m_Offset;
            yield return null;
        }

        gameObject.SetActive(false);
    }

    private void OnGridOccupied(Piece piece, Dropzone dropzone)
    {
        m_WasDropped = true;
        Debug.Log("On grid");
    }

    private void OnPieceReleased()
    {
        m_WasDropped = true;
        Debug.Log("Dropped");
    }

    private void OnDestroy()
    {
        m_AllPieces.ForEach(piece => { piece.OnGridOccupied -= OnGridOccupied; piece.OnPieceGrabbed -= OnPieceGrabbed; piece.OnPieceReleased -= OnPieceReleased; });
    }
}

