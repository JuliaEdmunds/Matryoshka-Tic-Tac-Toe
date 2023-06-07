using UnityEngine;

public class DropzoneRingHelper : MonoBehaviour
{
    [SerializeField] private GameObject m_ZoneRing;
    [SerializeField] private ParticleSystem m_RingParticle;
    [SerializeField] private ParticleSystem.MinMaxGradient m_TargetRingColour;
    [SerializeField] private ParticleSystem.MinMaxGradient m_ValidRingColour;


    public void ValidRingOn()
    {
        var particleColour = m_RingParticle.colorOverLifetime;
        particleColour.color = m_ValidRingColour;

        m_ZoneRing.SetActive(true);
    }

    public void TargetRingOn()
    {
        var particleColour = m_RingParticle.colorOverLifetime;
        particleColour.color = m_TargetRingColour;

        m_ZoneRing.SetActive(true);
    }

    public void RingOff()
    {
        m_ZoneRing.SetActive(false);
    }
}
