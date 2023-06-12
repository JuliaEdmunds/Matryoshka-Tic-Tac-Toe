using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TargetController : MonoBehaviour
{
    [Header("Game Elements")]
    [SerializeField] private LineRenderer m_LineRenderer;
    [SerializeField] private Rigidbody m_Rigidbody;
    public Rigidbody Rigidbody => m_Rigidbody;

    private List<Dropzone> m_OccupiedDropzones = new();
    public List<Dropzone> OccupiedDropzones => m_OccupiedDropzones;

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
        Vector3 targetPos = gameObject.transform.position;
        m_LineRenderer.SetPosition(1, targetPos);
    }

    public void ActivateTarget(Transform objTransform)
    {
        m_LineRenderer.SetPosition(0, objTransform.position);
        transform.position = objTransform.position;
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
                dropzone.DropzoneRingHelper.TargetRingOn();
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
            OccupiedDropzones[0].DropzoneRingHelper.TargetRingOn();
        }
    }

    public void ResetTarget()
    {
        OccupiedDropzones.Clear();
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

