using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LanguageSettingsController : MonoBehaviour
{
    [SerializeField] private Button m_NextButton;
    [SerializeField] private Toggle m_EnglishToggle;
    [SerializeField] private Toggle m_PolishToggle;
    [SerializeField] private Toggle m_PortugueseToggle;
    [SerializeField] private Toggle m_SpanishToggle;
    [SerializeField] private Toggle m_RussianToggle;
    [SerializeField] private Toggle m_GermanToggle;


    private void Awake()
    {
        if (LocalizationSettings.SelectedLocale == LocalizationSettings.AvailableLocales.Locales[(int)ELanguage.Polish])
        {
            m_PolishToggle.isOn = true;
            LanguageHelper.SetLocale(ELanguage.Polish);
        }
        else if (LocalizationSettings.SelectedLocale == LocalizationSettings.AvailableLocales.Locales[(int)ELanguage.Portuguese])
        {
            m_PortugueseToggle.isOn = true;
            LanguageHelper.SetLocale(ELanguage.Portuguese);
        }
        else if (LocalizationSettings.SelectedLocale == LocalizationSettings.AvailableLocales.Locales[(int)ELanguage.Spanish])
        {
            m_SpanishToggle.isOn = true;
            LanguageHelper.SetLocale(ELanguage.Spanish);
        }
        else if (LocalizationSettings.SelectedLocale == LocalizationSettings.AvailableLocales.Locales[(int)ELanguage.Russian])
        {
            m_RussianToggle.isOn = true;
            LanguageHelper.SetLocale(ELanguage.Russian);
        }
        else if (LocalizationSettings.SelectedLocale == LocalizationSettings.AvailableLocales.Locales[(int)ELanguage.German])
        {
            m_GermanToggle.isOn = true;
            LanguageHelper.SetLocale(ELanguage.German);
        }
        else
        {
            m_EnglishToggle.isOn = true;
            LanguageHelper.SetLocale(ELanguage.English);
        }
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

    public void MoveToMenu()
    {
        SceneManager.LoadScene(EScene.Menu.ToString());
    }
}
