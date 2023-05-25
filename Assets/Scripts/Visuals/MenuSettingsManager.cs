using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class MenuSettingsManager : MonoBehaviour
{
    private const string VOLUME_PREF_KEY = "NA.Volume";
    //private const string LANGUAGE_PREF_KEY = "NA.Language";

    [Header("Buttons")]
    [SerializeField] private GameObject m_BackButton;
    [SerializeField] private GameObject m_SettingsScreen;

    [Header("Audio")]
    [SerializeField] private Slider m_VolumeSlider;
    [SerializeField] private AudioSource m_AudioSource;
    [SerializeField] private Image m_VolumeImage;
    [SerializeField] private Sprite m_FullVolumeImage;
    [SerializeField] private Sprite m_LowVolumeImage;
    [SerializeField] private Sprite m_MutedImage;

    [Header("Localization")]
    [SerializeField] private Toggle m_PolishToggle;
    [SerializeField] private Toggle m_EnglishToggle;

    private void Start()
    {
        if (!PlayerPrefs.HasKey(VOLUME_PREF_KEY))
        {
            PlayerPrefs.SetFloat(VOLUME_PREF_KEY, 0.5f);
        }

        LoadTheLanguage();
        LoadVolume();

        m_AudioSource.Play();
    }

    //public void SetEnglish() => LanguageHelper.SetLocale(ELanguage.English);
    //
    //public void SetPolish() => LanguageHelper.SetLocale(ELanguage.English);

    public void ChangeLocale()
    {
        if (m_EnglishToggle.isOn)
        {
            LanguageHelper.SetLocale(ELanguage.English);
        }
        else
        {
            LanguageHelper.SetLocale(ELanguage.Polish);
        }
    }

    private void LoadTheLanguage()
    {
        ELanguage language = LanguageHelper.GetLocale();

        if (language == ELanguage.English)
        {
            m_EnglishToggle.isOn = true;
        }
        else
        {
            m_PolishToggle.isOn = true;
        }
    }

    public void VolumeValueChanged()
    {
        AudioListener.volume = m_VolumeSlider.value;

        CheckImage();

        SaveVolume();
    }

    private void LoadVolume()
    {
        m_VolumeSlider.value = PlayerPrefs.GetFloat(VOLUME_PREF_KEY);
        CheckImage();
    }

    private void SaveVolume()
    {
        PlayerPrefs.SetFloat(VOLUME_PREF_KEY, m_VolumeSlider.value);
    }

    private void CheckImage()
    {
        Image currentImage = m_VolumeImage.GetComponent<Image>();

        if (m_VolumeSlider.value == 0)
        {
            currentImage.sprite = m_MutedImage;
        }
        else if (m_VolumeSlider.value <= 0.5f)
        {
            currentImage.sprite = m_LowVolumeImage;
        }
        else
        {
            currentImage.sprite = m_FullVolumeImage;
        }
    }

    public void CloseSettings()
    {
        m_SettingsScreen.SetActive(false);
    }
}

