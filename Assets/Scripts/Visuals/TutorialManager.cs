using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class TutorialManager : MonoBehaviour
{
    [Header("Tutorial")]
    [SerializeField] private GameObject m_TutorialScreen;
    public GameObject TutorialScreen => m_TutorialScreen;
    [SerializeField] private TextAsset m_TutorialTextFile;
    [SerializeField] private TextMeshProUGUI m_TutorialText;

    [Header("Slots")]
    [SerializeField] private CharacterSlot m_BlueCharacterSlot;
    [SerializeField] private CharacterSlot m_RedCharacterSlot;
    [SerializeField] private ParticleSystem m_BlueRingParticleSystem;
    [SerializeField] private ParticleSystem m_RedRingParticleSystem;

    [Header("Scripts")]
    [SerializeField] private MenuManager m_MenuManager;

    private int m_CurrentRoundOfMoves;
    private List<Character> m_CharacterTypes;
    private Character m_CurrentCharacter;

    public Character CurrentTutorialCharacter => m_CurrentCharacter;

    public void StartTutorial()
    {
        m_CharacterTypes = m_MenuManager.CharacterTypes;

        CharacterSlot.OnCharacterTypeChanged += OnCharacterTypeChanged;

        StartTutorialTurn();
    }

    private void StartTutorialTurn()
    {
        StartCoroutine(ExecuteTurn());
    }

    private IEnumerator ExecuteTurn()
    {
        if (m_CurrentRoundOfMoves == 0)
        {
            DisableAllCharacters();
        }

        yield return ShowNextTutorialText();

        if (m_CurrentRoundOfMoves == 0)
        {
            FirstTutorialStep();
        }
        else if (m_CurrentRoundOfMoves == 1)
        {
            SecondTutorialStep();
        }
        else if (m_CurrentRoundOfMoves == 2)
        {
            ThirdTutorialStep();
        }
        else
        {
            yield break;
        }
    }

    public IEnumerator ShowNextTutorialText()
    {
        m_TutorialScreen.SetActive(true);

        if (m_CurrentRoundOfMoves >= 3)
        {
            yield break;
        }        

        var op = LocalizationSettings.StringDatabase.GetLocalizedStringAsync("TextTable", $"MenuInstruction{m_CurrentRoundOfMoves}");
        
        while (!op.IsDone)
        {
            yield return null;   
        }

        string currentLine = op.Result;
        yield return PrintText(currentLine);
    }

    private IEnumerator PrintText(string line)
    {
        for (int i = 0; i <= line.Length; i++)
        {
            string currentText = line.Substring(0, i);
            m_TutorialText.text = currentText;
            yield return new WaitForSeconds(0.01f);
        }
    }

#pragma warning disable CS0618 // Type or member is obsolete
    private void FirstTutorialStep()
    {
        // Enable only human character and the blue slot
        EnableCharacter(EPlayerType.Human);

        m_RedCharacterSlot.GetComponentInParent<BoxCollider>().enabled = false;
        m_RedRingParticleSystem.enableEmission = false;
    }

    private void SecondTutorialStep()
    {
        // Enable only basicAI character and the red slot
        m_RedRingParticleSystem.enableEmission = true;
        EnableCharacter(EPlayerType.BasicAI);

        m_BlueCharacterSlot.GetComponentInParent<BoxCollider>().enabled = false;
        m_BlueRingParticleSystem.enableEmission = false;
    }

    private void ThirdTutorialStep()
    {
        // Don't enable any characters just wait fot the player to press play
        m_BlueRingParticleSystem.enableEmission = false;
        m_RedRingParticleSystem.enableEmission = false;

        DisableAllCharacters();
    }
#pragma warning restore CS0618 // Type or member is obsolete

    private void EnableCharacter(EPlayerType characterType)
    {
        for (int i = 0; i < m_CharacterTypes.Count; i++)
        {
            Character currentCharacter = m_CharacterTypes[i];
            EPlayerType currentCharacterType = currentCharacter.CharacterType;

            EPlayerType characterTypeToMove = characterType;

            if (currentCharacterType == characterTypeToMove)
            {
                m_CurrentCharacter = currentCharacter;
                currentCharacter.EnableDrag();
                currentCharacter.ValidPieceRing.SetActive(true);
            }
            else
            {
                currentCharacter.DisableDrag();
                currentCharacter.ValidPieceRing.SetActive(false);
            }
        }
    }

    private void DisableAllCharacters()
    {
        for (int i = 0; i < m_CharacterTypes.Count; i++)
        {
            Character currentCharacter = m_CharacterTypes[i];
            currentCharacter.DisableDrag();
            currentCharacter.ValidPieceRing.SetActive(false);
        }
    }

    private void OnCharacterTypeChanged(EPlayerType characterType, CharacterSlot characterSlot)
    {
        if (m_CurrentRoundOfMoves == 0)
        {
            if (characterType == EPlayerType.Human && characterSlot.PlayerColour == EPlayerColour.Blue)
            {
                StartCoroutine(BeginEndMove());
            }
        }

        if (m_CurrentRoundOfMoves == 1)
        {
            if (characterType == EPlayerType.BasicAI && characterSlot.PlayerColour == EPlayerColour.Red)
            {
                StartCoroutine(BeginEndMove());
            }
        }
    }

    private IEnumerator BeginEndMove()
    {
        yield return new WaitForEndOfFrame();
        EndMove();
    }

    private void EndMove()
    {
        m_TutorialScreen.SetActive(false);

        if (m_CurrentRoundOfMoves == 0)
        {
            m_RedCharacterSlot.GetComponentInParent<BoxCollider>().enabled = true;
            DisableAllCharacters();
        }
        else if (m_CurrentRoundOfMoves == 1)
        {
            m_BlueCharacterSlot.GetComponentInParent<BoxCollider>().enabled = true;
            DisableAllCharacters();
        }
        else
        {
            return;
        }

        m_CurrentRoundOfMoves++;
        StartTutorialTurn();
    }

    private void OnDestroy()
    {
        CharacterSlot.OnCharacterTypeChanged -= OnCharacterTypeChanged;
    }
}
