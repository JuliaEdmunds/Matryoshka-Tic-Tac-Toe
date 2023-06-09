﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private EPlayerType m_CharacterType;
    public EPlayerType CharacterType => m_CharacterType;

    [SerializeField] private GameObject m_ValidPieceRing;
    public GameObject ValidPieceRing => m_ValidPieceRing;

    [SerializeField] private DragAndDrop m_DragAndDrop;

    public event Action<Character> OnCharacterGrabbed;
    public event Action OnCharacterReleased;

    private void Start()
    {
        m_DragAndDrop.OnGrabbed += OnGrabbed;
        m_DragAndDrop.OnDragEnded += OnDragEnded;
    }

    private void OnGrabbed()
    {
        TargetController.Instance.ActivateTarget(transform);
        OnCharacterGrabbed?.Invoke(this);
    }

    private void OnDragEnded()
    {
        TargetController.Instance.ResetTarget();
        OnCharacterReleased?.Invoke();
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