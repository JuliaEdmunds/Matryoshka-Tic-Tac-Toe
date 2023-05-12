using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using TMPro;

public class VisualGameManager : MonoBehaviour
{
    private const string MENU_SCENE = "Menu";

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

    private void Start()
    {
        m_PlayerBluePieces.ForEach(piece => { piece.OnGridOccupied += OnGridOccupied; piece.OnPieceGrabbed += OnPieceGrabbed; piece.OnPieceReleased += OnPieceReleased; });
        m_PlayerRedPieces.ForEach(piece => { piece.OnGridOccupied += OnGridOccupied; piece.OnPieceGrabbed += OnPieceGrabbed; piece.OnPieceReleased += OnPieceReleased; });

        m_GameLogic.OnTurnStarted += OnTurnStarted;
        m_GameLogic.OnGameEnded += OnGameEnded;

        m_GameLogic.StartGame();
    }

    private void OnDestroy()
    {
        // Pieces are gonna be destroyed by reloading the scene but keeping unsubscription for good practice
        m_PlayerBluePieces.ForEach(piece => { piece.OnGridOccupied -= OnGridOccupied; piece.OnPieceGrabbed -= OnPieceGrabbed; });
        m_PlayerRedPieces.ForEach(piece => { piece.OnGridOccupied -= OnGridOccupied; piece.OnPieceGrabbed -= OnPieceGrabbed; });

        m_GameLogic.OnTurnStarted -= OnTurnStarted;
        m_GameLogic.OnGameEnded -= OnGameEnded;
    }

    private void OnTurnStarted(EPlayer currentPlayer, List<EPiece> currentValidPieces)
    {
        m_CurrentActivePieces.Clear();

        if (currentPlayer == EPlayer.Blue)
        {
            EnableActivePieces(m_PlayerBluePieces, currentValidPieces);
            DisablePieces(m_PlayerRedPieces);
        }
        else
        {
            EnableActivePieces(m_PlayerRedPieces, currentValidPieces);
            DisablePieces(m_PlayerBluePieces);
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

    private void OnPieceGrabbed(Piece piece)
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

    private void OnGridOccupied(Piece piece, Dropzone targetZone)
    {
        m_GameLogic.SetPieceOnBoard(piece.PieceType, targetZone.GridID);

        targetZone.NeutralCube.SetActive(false);
        targetZone.RedCube.SetActive(piece.PlayerID == EPlayer.Red);
        targetZone.BlueCube.SetActive(piece.PlayerID == EPlayer.Blue);
    }

    private void OnPieceReleased()
    {
        ResetVisualAids();
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

    private void OnGameEnded(EPlayer winner)
    {
        StartCoroutine(ShowGameOverScreen(winner));
    }

    private IEnumerator ShowGameOverScreen(EPlayer winner)
    {
        if (winner != EPlayer.Invalid)
        {
            m_GameOverText.text = $"{winner} won";
            m_GameOverText.color = winner == EPlayer.Blue ? Color.blue : Color.red;
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
