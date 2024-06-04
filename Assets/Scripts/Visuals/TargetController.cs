using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    [Header("Game Elements")]
    [SerializeField] private LineRenderer m_LineRenderer;
    [SerializeField] private Rigidbody m_Rigidbody;
    public Rigidbody Rigidbody => m_Rigidbody;

    private readonly List<DropZone> m_OccupiedDropzones = new();
    public List<DropZone> OccupiedDropzones => m_OccupiedDropzones;

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
        DropZone dropZone = other.gameObject.GetComponent<DropZone>();

        if (dropZone != null && dropZone.enabled)
        {
            OccupiedDropzones.Add(dropZone);

            if (OccupiedDropzones.Count == 1)
            {
                dropZone.DropzoneRingHelper.TargetRingOn();
            }
            else
            {
                for (int i = 0; i < OccupiedDropzones.Count; i++)
                {
                    DropZone currentDropZone = OccupiedDropzones[i];
                    currentDropZone.DropzoneRingHelper.ValidRingOn();
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        DropZone dropZone = other.gameObject.GetComponent<DropZone>();

        if (dropZone != null && dropZone.enabled)
        {
            OccupiedDropzones.Remove(dropZone);
            dropZone.DropzoneRingHelper.ValidRingOn();
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
