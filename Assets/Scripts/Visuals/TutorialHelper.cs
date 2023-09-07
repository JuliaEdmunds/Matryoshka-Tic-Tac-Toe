using UnityEngine;

public static class TutorialHelper
{
    private const string TUTORIAL_PREF_KEY = "NA.Tutorial.HasSeen";

    public static bool HasCompletedTutorial
    {
        get => PlayerPrefs.GetInt(TUTORIAL_PREF_KEY, 0) == 1;
        set => PlayerPrefs.SetInt(TUTORIAL_PREF_KEY, value ? 1 : 0);
    }
}
