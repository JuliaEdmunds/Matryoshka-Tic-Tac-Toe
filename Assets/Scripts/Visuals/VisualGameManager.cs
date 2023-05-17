using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using TMPro;
using System;
using System.IO;

public class VisualGameManager : MonoBehaviour
{
    [Header("Pieces")]
    [SerializeField] private List<Piece> m_PlayerBluePieces = new();
    [SerializeField] private List<Piece> m_PlayerRedPieces = new();

    [Header("Board")]
    [SerializeField] private List<Dropzone> m_Dropzones = new();

    [Header("Tutorial")]
    [SerializeField] private GameObject m_TutorialScreen;
    public GameObject TutorialScreen => m_TutorialScreen;
    [SerializeField] private TextAsset m_TutorialTextFile;
    [SerializeField] private TextMeshProUGUI m_TutorialText;

    [Header("Game Over")]
    [SerializeField] private GameObject m_GameOverScreen;
    [SerializeField] private TextMeshProUGUI m_GameOverText;

    private GameLogic m_GameLogic = new();
    private List<Piece> m_CurrentActivePieces = new();
    private string[] m_TextLines;

    private APlayer m_BluePlayer;
    private APlayer m_RedPlayer;
    private bool m_HasPlayedTutorial; //TODO: update later on to rely on PlayerPrefs

    private void Start()
    {
        m_GameLogic.OnTurnStarted += OnTurnStarted;
        m_GameLogic.OnTurnEnded += OnTurnEnded;
        m_GameLogic.OnGameEnded += OnGameEnded;

        if (!m_HasPlayedTutorial)
        {
            GameSettings.BluePlayer = EPlayerType.Tutorial;
            GameSettings.RedPlayer = EPlayerType.Tutorial;
        }

        m_BluePlayer = PlayerFactory.CreatePlayer(GameSettings.BluePlayer);
        m_RedPlayer = PlayerFactory.CreatePlayer(GameSettings.RedPlayer);

        m_BluePlayer.Init(this, m_GameLogic, EPlayerColour.Blue);
        m_RedPlayer.Init(this, m_GameLogic, EPlayerColour.Red);

        m_GameLogic.StartGame();
    }

    private void OnDestroy()
    {
        m_GameLogic.OnTurnStarted -= OnTurnStarted;
        m_GameLogic.OnTurnEnded -= OnTurnEnded;
        m_GameLogic.OnGameEnded -= OnGameEnded;
    }

    int m_CurrentLineIndex = 0;
    public IEnumerator ShowNextTutorialText()
    {
        m_TutorialScreen.SetActive(true);

        if (m_TutorialTextFile != null)
        {
            m_TextLines = m_TutorialTextFile.text.Split("\r\n\r\n");
        }

        if (m_CurrentLineIndex >= m_TextLines.Length)
        {
            yield break;
        }

        string currentLine = m_TextLines[m_CurrentLineIndex];

        for (int i = 0; i < currentLine.Length; i++)
        {
            string currentText = currentLine.Substring(0, i);
            m_TutorialText.text = currentText;
            yield return new WaitForSeconds(0.01f);
        }

        m_CurrentLineIndex++;
    }

    private void OnTurnStarted(EPlayerColour currentPlayer, List<EPiece> currentValidPieces)
    {
        m_CurrentActivePieces.Clear();
        SetActivePiecesForCurrentPlayer(currentValidPieces);

        if (currentPlayer == EPlayerColour.Blue)
        {     
            DisablePieces(m_PlayerRedPieces);
            m_BluePlayer.StartTurn(m_CurrentActivePieces);
        }
        else
        {
            DisablePieces(m_PlayerBluePieces);
            m_RedPlayer.StartTurn(m_CurrentActivePieces);
        }
    }

    public void SetActivePiecesForCurrentPlayer(List<EPiece> currentPlayerActivePieces)
    {
        List<Piece> allCurrentPlayerPieces = m_GameLogic.CurrentPlayer == EPlayerColour.Blue
            ? m_PlayerBluePieces
            : m_PlayerRedPieces;

        for (int i = 0; i < allCurrentPlayerPieces.Count; i++)
        {
            Piece currentPiece = allCurrentPlayerPieces[i];
            EPiece currentPieceType = currentPiece.PieceType;

            bool isActivePiece = currentPlayerActivePieces.Contains(currentPieceType);
            if (isActivePiece)
            {
                currentPiece.EnableDrag();
                m_CurrentActivePieces.Add(currentPiece);
            }
            else
            {
                currentPiece.DisableDrag();
            }

            currentPiece.ValidPieceRing.SetActive(isActivePiece);
        }
    }

    public void DisablePieces(List<Piece> all2ndPlayerPieces)
    {
        for (int i = 0; i < all2ndPlayerPieces.Count; i++)
        {
            Piece currentPiece = all2ndPlayerPieces[i];
            currentPiece.ValidPieceRing.SetActive(false);
            currentPiece.DisableDrag();
        }
    }

    public void DisableTiles(List<EGrid> validTiles)
    {
        // Grab all the tiles and disable the ones that are not in the validTiles
        for (int i = 0; i < m_Dropzones.Count; i++)
        {
            Dropzone currentTile = m_Dropzones[i];
            EGrid currentTilePos = currentTile.GridID;

            bool isValidTile = validTiles.Contains(currentTilePos);

            currentTile.enabled = isValidTile;
            currentTile.ValidZoneRing.SetActive(isValidTile);
        }
    }

    public Dropzone GetDropzoneFromGridID(EGrid gridID)
    {
        for (int i = 0; i < m_Dropzones.Count; i++)
        {
            EGrid currentGridID = m_Dropzones[i].GridID;

            if (currentGridID == gridID)
            {
                return m_Dropzones[i];
            }
        }

        throw new NotSupportedException($"GridID: {gridID} does not match any Dropzones");
    }

    public Piece GetPieceFromPieceType(EPiece pieceType)
    {
        for (int i = 0; i < m_CurrentActivePieces.Count; i++)
        {
            EPiece currentPieceType = m_CurrentActivePieces[i].PieceType;

            if (currentPieceType == pieceType)
            {
                return m_CurrentActivePieces[i];
            }
        }

        throw new NotSupportedException($"GridID: {pieceType} does not match any CurrentActivePieces");
    }

    public void RequestStartMove(Piece piece)
    {
        // Check valid tiles for selected piece & highlight valid ones
        EPiece pieceType = piece.PieceType;
        List<EGrid> validTiles = m_GameLogic.CheckValidTiles(pieceType);
        DisableTiles(validTiles);

        ResetVisualAidsOnActivePieces(false);
    }

    // Set piece on board and adjust the occupied cube colour
    public void RequestFinishMove(Piece piece, Dropzone targetZone)
    {
        Rigidbody pieceRb = piece.GetComponent<Rigidbody>();
        pieceRb.position = targetZone.transform.position;

        targetZone.NeutralCube.SetActive(false);
        targetZone.RedCube.SetActive(piece.PlayerID == EPlayerColour.Red);
        targetZone.BlueCube.SetActive(piece.PlayerID == EPlayerColour.Blue);

        ResetDropzoneVisualAids();
        ResetVisualAidsOnActivePieces(false);

        m_GameLogic.SetPieceOnBoard(piece.PieceType, targetZone.GridID);
    }

    // If the player dropped the piece outside of the board (or on invalid slot)
    public void RequestCancelMove()
    {
        ResetDropzoneVisualAids();
        ResetVisualAidsOnActivePieces(true);
    }

    private void OnTurnEnded(EPlayerColour currentPlayer)
    {
        //ResetVisualAidsOnActivePieces(false);

        if (currentPlayer == EPlayerColour.Blue)
        {
            m_BluePlayer.EndTurn();
        }
        else
        {
            m_RedPlayer.EndTurn();
        }
    }

    private void ResetDropzoneVisualAids()
    {
        for (int i = 0; i < m_Dropzones.Count; i++)
        {
            Dropzone currentDropzone = m_Dropzones[i];

            currentDropzone.ValidZoneRing.SetActive(false);
        }
    }

    private void ResetVisualAidsOnActivePieces(bool turnOn)
    {
        for (int i = 0; i < m_CurrentActivePieces.Count; i++)
        {
            Piece currentPiece = m_CurrentActivePieces[i];
            currentPiece.ValidPieceRing.SetActive(turnOn);
        }
    }

    private void OnGameEnded(EPlayerColour winner)
    {
        ResetVisualAidsOnActivePieces(false);
        StartCoroutine(ShowGameOverScreen(winner));
    }

    private IEnumerator ShowGameOverScreen(EPlayerColour winner)
    {
        yield return new WaitForSeconds(2);

        if (winner != EPlayerColour.Invalid)
        {
            m_GameOverText.text = $"{winner} won";
            m_GameOverText.color = winner == EPlayerColour.Blue ? Color.blue : Color.red;
        }
        else
        {
            m_GameOverText.text = "It's a draw...";
        }
        
        m_GameOverScreen.SetActive(true);

        yield return new WaitForSeconds(2);

        BackToMenu();
    }

    public void BackToMenu()
    {
        SceneController.ChangeScene(EScene.Menu);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
