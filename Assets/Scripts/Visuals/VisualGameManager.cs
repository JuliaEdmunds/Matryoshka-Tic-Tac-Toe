using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using TMPro;
using System;

public class VisualGameManager : MonoBehaviour
{
    [Header("Pieces")]
    [SerializeField] private List<Piece> m_PlayerBluePieces = new();
    [SerializeField] private List<Piece> m_PlayerRedPieces = new();

    [Header("Board")]
    [SerializeField] private List<Dropzone> m_Dropzones = new();

    [Header("Game Over")]
    [SerializeField] private GameObject m_GameOverScreen;
    [SerializeField] private TextMeshProUGUI m_GameOverText;

    private GameLogic m_GameLogic = new();
    private List<Piece> m_CurrentActivePieces = new();
    
    private APlayer m_BluePlayer;
    private APlayer m_RedPlayer;

    private void Start()
    {
        m_GameLogic.OnTurnStarted += OnTurnStarted;
        m_GameLogic.OnTurnEnded += OnTurnEnded;
        m_GameLogic.OnGameEnded += OnGameEnded;

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

    private void OnTurnStarted(EPlayerColour currentPlayer, List<EPiece> currentValidPieces)
    {
        m_CurrentActivePieces.Clear();

        if (currentPlayer == EPlayerColour.Blue)
        {
            EnableActivePieces(m_PlayerBluePieces, currentValidPieces);
            DisablePieces(m_PlayerRedPieces);
            m_BluePlayer.StartTurn(m_CurrentActivePieces);
        }
        else
        {
            EnableActivePieces(m_PlayerRedPieces, currentValidPieces);
            DisablePieces(m_PlayerBluePieces);
            m_RedPlayer.StartTurn(m_CurrentActivePieces);
        }
    }

    private void EnableActivePieces(List<Piece> allCurrentPlayerPieces, List<EPiece> currentPlayerActivePieces)
    {
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

    private void DisablePieces(List<Piece> all2ndPlayerPieces)
    {
        for (int i = 0; i < all2ndPlayerPieces.Count; i++)
        {
            Piece currentPiece = all2ndPlayerPieces[i];
            currentPiece.ValidPieceRing.SetActive(false);
            currentPiece.DisableDrag();
        }
    }

    private void DisableTiles(List<EGrid> validTiles)
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

    public void RequestStartMove(Piece piece)
    {
        // Check valid tiles for selected piece & highlight valid ones
        EPiece pieceType = piece.PieceType;
        List<EGrid> validTiles = m_GameLogic.CheckValidTiles(pieceType);
        DisableTiles(validTiles);

        for (int i = 0; i < m_CurrentActivePieces.Count; i++)
        {
            Piece currentPiece = m_CurrentActivePieces[i];
            currentPiece.ValidPieceRing.SetActive(false);
        }
    }

    // Set piece on board and adjust the occupied cube colour
    public void RequestFinishMove(Piece piece, Dropzone targetZone)
    {
        Rigidbody pieceRb = piece.GetComponent<Rigidbody>();
        pieceRb.position = targetZone.transform.position;
        m_GameLogic.SetPieceOnBoard(piece.PieceType, targetZone.GridID);

        targetZone.NeutralCube.SetActive(false);
        targetZone.RedCube.SetActive(piece.PlayerID == EPlayerColour.Red);
        targetZone.BlueCube.SetActive(piece.PlayerID == EPlayerColour.Blue);

        ResetVisualAids();
    }

    // If the player dropped the piece outside of the board (or on invalid slot)
    public void RequestCancelMove()
    {
        ResetVisualAids();
    }

    private void OnTurnEnded(EPlayerColour currentPlayer)
    {
        if (currentPlayer == EPlayerColour.Blue)
        {
            m_BluePlayer.EndTurn();
        }
        else
        {
            m_RedPlayer.EndTurn();
        }
    }

    private void ResetVisualAids()
    {
        for (int i = 0; i < m_Dropzones.Count; i++)
        {
            Dropzone currentDropzone = m_Dropzones[i];

            currentDropzone.ValidZoneRing.SetActive(false);
        }

        for (int i = 0; i < m_CurrentActivePieces.Count; i++)
        {
            Piece currentPiece = m_CurrentActivePieces[i];
            currentPiece.ValidPieceRing.SetActive(true);
        }
    }

    private void OnGameEnded(EPlayerColour winner)
    {
        StartCoroutine(ShowGameOverScreen(winner));
    }

    private IEnumerator ShowGameOverScreen(EPlayerColour winner)
    {
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
