using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization;
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


    private void Awake()
    {
        int polishLocale = (int)ELanguage.Polish;

        if (LocalizationSettings.SelectedLocale == LocalizationSettings.AvailableLocales.Locales[polishLocale])
        {
            m_PolishToggle.isOn = true;
            LanguageHelper.SetLocale(ELanguage.Polish);
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
