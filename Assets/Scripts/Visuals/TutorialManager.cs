using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;


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

    private string[] m_TextLines;
    private int m_CurrentRoundOfMoves;
    private List<Character> m_CharacterTypes;
    private Character m_CurrentCharacter;
    private bool m_HasCompletedStep;

    public Character CurrentTutorialCharacter => m_CurrentCharacter;

    public void StartTutorial()
    {
        m_CharacterTypes = m_MenuManager.CharacterTypes;

        CharacterSlot.OnCharacterTypeChanged += OnCharacterTypeChanged;

        m_CharacterTypes.ForEach(character => { character.OnCharacterReleased += OnCharacterReleased; });

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

        if (m_TutorialTextFile != null)
        {
            m_TextLines = m_TutorialTextFile.text.Split("\r\n\r\n");
        }

        if (m_CurrentRoundOfMoves >= m_TextLines.Length)
        {
            yield break;
        }

        string currentLine = m_TextLines[m_CurrentRoundOfMoves];

        for (int i = 0; i < currentLine.Length; i++)
        {
            string currentText = currentLine.Substring(0, i);
            m_TutorialText.text = currentText;
            yield return new WaitForSeconds(0.01f);
        }
    }

    private void FirstTutorialStep()
    {
        m_HasCompletedStep = false;
        // Enable only human character and the blue slot
        EnableCharacter(EPlayerType.Human);

        m_RedCharacterSlot.GetComponentInParent<BoxCollider>().enabled = false;
        m_RedRingParticleSystem.enableEmission = false;
    }

    private void SecondTutorialStep()
    {
        m_HasCompletedStep = false;

        // Enable only basicAI character and the red slot
        m_RedRingParticleSystem.enableEmission = true;
        EnableCharacter(EPlayerType.BasicAI);

        m_BlueCharacterSlot.GetComponentInParent<BoxCollider>().enabled = false;
        m_BlueRingParticleSystem.enableEmission = false;
    }

    private void ThirdTutorialStep()
    {
        m_HasCompletedStep = false;

        // Don't enable any characters just wait fot the player to press play
        m_BlueRingParticleSystem.enableEmission = false;
        m_RedRingParticleSystem.enableEmission = false;
        DisableAllCharacters();
    }

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
                m_HasCompletedStep = true;
                return;
            }
        }

        if (m_CurrentRoundOfMoves == 1)
        {
            if (characterType == EPlayerType.BasicAI && characterSlot.PlayerColour == EPlayerColour.Red)
            {
                m_HasCompletedStep = true;
                return;
            }
        }

        m_HasCompletedStep = false;
    }

    private void OnCharacterReleased()
    {
        if (m_HasCompletedStep)
        {
            StartCoroutine(BeginEndMove());
        }
    }

    private IEnumerator BeginEndMove()
    {
        yield return new WaitForEndOfFrame();
        EndMove();
    }

    private void OnDestroy()
    {
        CharacterSlot.OnCharacterTypeChanged -= OnCharacterTypeChanged;
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
}

