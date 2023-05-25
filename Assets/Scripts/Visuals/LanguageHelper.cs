using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

public static class LanguageHelper
{
    private const string LANGUAGE_PREF_KEY = "NA.Language";

    public static bool HasChosenLanguage => PlayerPrefs.HasKey(LANGUAGE_PREF_KEY);

    public static ELanguage GetLocale()
    {
        return (ELanguage)PlayerPrefs.GetInt(LANGUAGE_PREF_KEY);
    }

    public static void SetLocale(ELanguage locale)
    {
        CoroutineManager.Instance.StartCoroutine(SetLocale((int)locale));
    }

    private static IEnumerator SetLocale(int localeID)
    {
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeID];
        PlayerPrefs.SetInt(LANGUAGE_PREF_KEY, localeID);
    }
}

