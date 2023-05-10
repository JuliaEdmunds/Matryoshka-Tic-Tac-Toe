using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dropzone : MonoBehaviour
{
    [SerializeField] private GameObject m_BlueCube;
    public GameObject BlueCube => m_BlueCube;

    [SerializeField] private GameObject m_RedCube;
    public GameObject RedCube => m_RedCube;

    [SerializeField] private GameObject m_NeutralCube;
    public GameObject NeutralCube => m_NeutralCube;

    [SerializeField] private GameObject m_ValidZoneRing;
    public GameObject ValidZoneRing => m_ValidZoneRing;

    [SerializeField] private EGrid m_GridID;
    public EGrid GridID => m_GridID;
}
