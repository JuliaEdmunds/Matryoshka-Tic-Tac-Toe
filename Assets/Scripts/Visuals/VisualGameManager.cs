using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class VisualGameManager : MonoBehaviour
{
    [Header("Pieces")]
    [SerializeField] private List<Piece> m_PlayerBluePieces = new();
    [SerializeField] private List<Piece> m_PlayerRedPieces = new();

    [Header("Board")]
    [SerializeField] private List<Dropzone> m_Dropzones = new();

    private GameLogic m_GameLogic = new();

    private void Start()
    {
        m_PlayerBluePieces.ForEach(piece => { piece.OnGridOccupied += OnGridOccupied; piece.OnPieceGrabbed += OnPieceGrabbed; });
        m_PlayerRedPieces.ForEach(piece => { piece.OnGridOccupied += OnGridOccupied; piece.OnPieceGrabbed += OnPieceGrabbed; });

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

            if (currentPlayerActivePieces.Contains(currentPieceType))
            {
                currentPiece.EnableDrag();
            }
            else
            {
                currentPiece.DisableDrag();
            }
        }
    }

    private void DisablePieces(List<Piece> all2ndPlayerPieces)
    {
        for (int i = 0; i < all2ndPlayerPieces.Count; i++)
        {
            Piece currentPiece = all2ndPlayerPieces[i];
            currentPiece.DisableDrag();
        }
    }

    private void OnPieceGrabbed(Piece piece)
    {
        EPiece pieceType = piece.PieceType;
        List<EGrid> validTiles = m_GameLogic.CheckValidTiles(pieceType);
        DisableTiles(validTiles);
    }

    private void DisableTiles(List<EGrid> validTiles)
    {
        // Grab all the tiles and disable the ones that are not in the validTiles
        for (int i = 0; i < m_Dropzones.Count; i++)
        {
            Dropzone currentTile = m_Dropzones[i];
            EGrid currentTilePos = currentTile.GridID;

            currentTile.enabled = validTiles.Contains(currentTilePos);
        }
    }

    private void OnGridOccupied(Piece piece, Dropzone targetZone)
    {
        m_GameLogic.SetPieceOnBoard(piece.PieceType, targetZone.GridID);

        targetZone.NeutralCube.SetActive(false);
        targetZone.RedCube.SetActive(piece.PlayerID == EPlayer.Red);
        targetZone.BlueCube.SetActive(piece.PlayerID == EPlayer.Blue);
    }

    private void OnGameEnded(EPlayer winner)
    {
        Debug.Log($"{winner} won");
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
