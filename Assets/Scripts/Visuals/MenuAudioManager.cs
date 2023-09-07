using UnityEngine;

public class MenuAudioManager : MonoBehaviour
{
    public static MenuAudioManager Instance { get; private set; }

    [SerializeField] private AudioSource m_AudioSource;
    [SerializeField] private AudioClip m_CharacterAppearanceClip;

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
        m_AudioSource.Play();
    }

    public void PlayCharacterAppeared()
    {
        m_AudioSource.PlayOneShot(m_CharacterAppearanceClip);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
