using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class GameplayAudioManager : MonoBehaviour
{
    public static GameplayAudioManager Instance { get; private set; }

    [SerializeField] private AudioSource m_AudioSource;
    [SerializeField] private AudioClip m_CharacterCrashedClip;
    [SerializeField] private AudioClip m_CharacterMovingClip;

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

    public void PlayCharacterMoving()
    {
        m_AudioSource.PlayOneShot(m_CharacterMovingClip);
    }

    public void PlayCharacterCrashed()
    {
        m_AudioSource.PlayOneShot(m_CharacterCrashedClip);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}


