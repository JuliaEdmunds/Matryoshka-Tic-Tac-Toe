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
        else
        {
            LanguageHelper.SetLocale(ELanguage.Polish);
        }
    }

    public void MoveToMenu()
    {
        SceneManager.LoadScene(EScene.Menu.ToString());
    }
}
