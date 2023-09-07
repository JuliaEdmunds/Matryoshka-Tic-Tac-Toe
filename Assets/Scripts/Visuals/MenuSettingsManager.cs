using UnityEngine;
using UnityEngine.UI;

public class MenuSettingsManager : MonoBehaviour
{
    private const string VOLUME_PREF_KEY = "NA.Volume";

    [Header("Buttons")]
    [SerializeField] private GameObject m_BackButton;
    [SerializeField] private GameObject m_SettingsScreen;

    [Header("Audio Settings")]
    [SerializeField] private Slider m_VolumeSlider;
    [SerializeField] private Image m_VolumeImage;
    [SerializeField] private Sprite m_FullVolumeImage;
    [SerializeField] private Sprite m_LowVolumeImage;
    [SerializeField] private Sprite m_MutedImage;

    [Header("Localization")]
    [SerializeField] private Toggle m_PolishToggle;
    [SerializeField] private Toggle m_EnglishToggle;
    [SerializeField] private Toggle m_PortugueseToggle;
    [SerializeField] private Toggle m_SpanishToggle;
    [SerializeField] private Toggle m_RussianToggle;
    [SerializeField] private Toggle m_GermanToggle;

    private void Start()
    {
        if (!PlayerPrefs.HasKey(VOLUME_PREF_KEY))
        {
            PlayerPrefs.SetFloat(VOLUME_PREF_KEY, 0.5f);
        }

        LoadTheLanguage();
        LoadVolume();

        m_SettingsScreen.SetActive(false);
    }

    public void ChangeLocale()
    {
        if (m_EnglishToggle.isOn)
        {
            LanguageHelper.SetLocale(ELanguage.English);
        }
        else if (m_GermanToggle.isOn)
        {
            LanguageHelper.SetLocale(ELanguage.German);
        }
        else if (m_PolishToggle.isOn)
        {
            LanguageHelper.SetLocale(ELanguage.Polish);
        }
        else if (m_PortugueseToggle.isOn)
        {
            LanguageHelper.SetLocale(ELanguage.Portuguese);
        }
        else if (m_SpanishToggle.isOn)
        {
            LanguageHelper.SetLocale(ELanguage.Spanish);
        }
        else
        {
            LanguageHelper.SetLocale(ELanguage.Russian);
        }
    }

    private void LoadTheLanguage()
    {
        ELanguage language = LanguageHelper.GetLocale();

        switch (language)
        {
            default:
            case ELanguage.English:
                m_EnglishToggle.isOn = true;
                break;
            case ELanguage.German:
                m_GermanToggle.isOn = true;
                break;
            case ELanguage.Polish:
                m_PolishToggle.isOn = true;
                break;
            case ELanguage.Portuguese:
                m_PortugueseToggle.isOn = true;
                break;
            case ELanguage.Spanish:
                m_SpanishToggle.isOn = true;
                break;
            case ELanguage.Russian:
                m_RussianToggle.isOn = true;
                break;  
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
