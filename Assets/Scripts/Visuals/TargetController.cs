using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TargetController : MonoBehaviour
{
    [Header("Game Elements")]
    [SerializeField] private List<Piece> m_AllPieces;
    [SerializeField] private LineRenderer m_LineRenderer;
    [SerializeField] private Rigidbody m_Rigidbody;
    public Rigidbody Rigidbody => m_Rigidbody;

    [Header("Positions")]
    [SerializeField] private Vector3 m_LineOffset;

    public List<Dropzone> OccupiedDropzones = new();

    public static TargetController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        Vector3 targetPos = gameObject.transform.position + m_LineOffset;
        m_LineRenderer.SetPosition(1, targetPos);
    }

    public void ActivateTarget(Piece piece)
    {
        m_LineRenderer.SetPosition(0, piece.transform.position + m_LineOffset);
        transform.position = piece.transform.position;
        gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        Dropzone dropzone = other.gameObject.GetComponent<Dropzone>();

        if (dropzone != null && dropzone.enabled)
        {
            OccupiedDropzones.Add(dropzone);

            if (OccupiedDropzones.Count == 1)
            {
                dropzone.DropzoneRingHelper.TargerRingOn();
            }
            else
            {
                for (int i = 0; i < OccupiedDropzones.Count; i++)
                {
                    Dropzone currentDropzone = OccupiedDropzones[i];
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
            OccupiedDropzones.Remove(dropzone);
            dropzone.DropzoneRingHelper.ValidRingOn();
        }

        if (OccupiedDropzones.Count == 1)
        {
            OccupiedDropzones[0].DropzoneRingHelper.TargerRingOn();
        }
    }

    public void ResetTarget()
    {
        OccupiedDropzones.Clear();
        // gameObject.transform.position = m_StartPos;
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}

