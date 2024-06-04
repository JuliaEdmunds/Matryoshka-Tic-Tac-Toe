using System;
using UnityEngine;

public class Piece : MonoBehaviour
{
    [SerializeField] private GameObject m_ValidPieceRing;
    public GameObject ValidPieceRing => m_ValidPieceRing;

    [SerializeField] private EPlayerColour m_PlayerID;
    public EPlayerColour PlayerID => m_PlayerID;

    [SerializeField] private EPiece m_PieceType;
    public EPiece PieceType => m_PieceType;

    [SerializeField] private DragAndDrop m_DragAndDrop;

    [SerializeField] private AnimationController m_AnimationController;

    [SerializeField] private ParticleSystem m_Smoke;

    [SerializeField] private Crasher m_Crasher;

    public event Action<Piece, DropZone> OnGridOccupied;
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
        TargetController.Instance.ActivateTarget(transform);
        OnPieceGrabbed(this);
    }

    private void OnDropped(DropZone tile)
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

    public void GetReadyToCrash()
    {
        m_Crasher.gameObject.SetActive(true);
    }

    public void CrashOpponent()
    {
        m_AnimationController.CrashOpponent();

        m_Crasher.gameObject.SetActive(false);
    }

    public void BeCrashed()
    {
        m_AnimationController.BeCrashed();

        m_Smoke.Play();
    }

    public void ResetAnimation()
    {
        m_AnimationController.ResetAnimation();
    }
}
