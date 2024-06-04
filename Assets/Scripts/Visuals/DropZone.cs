using UnityEngine;

public class DropZone : MonoBehaviour
{
    [SerializeField] private GameObject m_BlueCube;
    public GameObject BlueCube => m_BlueCube;

    [SerializeField] private GameObject m_RedCube;
    public GameObject RedCube => m_RedCube;

    [SerializeField] private GameObject m_NeutralCube;
    public GameObject NeutralCube => m_NeutralCube;

    [SerializeField] private EGrid m_GridID;
    public EGrid GridID => m_GridID;

    [SerializeField] private DropzoneRingHelper m_DropzoneRingHelper;
    public DropzoneRingHelper DropzoneRingHelper => m_DropzoneRingHelper;
}
