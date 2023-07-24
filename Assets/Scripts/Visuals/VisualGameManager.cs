using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;


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
    private Dictionary<KeyValuePair<EPlayerColour, EPiece>, Piece> m_PieceMappings = new();
    private List<Piece> m_CurrentActivePieces = new();

    private APlayer m_BluePlayer;
    private APlayer m_RedPlayer;

    private void Start()
    {
        m_GameLogic.OnTurnStarted += OnTurnStarted;
        m_GameLogic.OnTurnEnded += OnTurnEnded;
        m_GameLogic.OnGameEnded += OnGameEnded;

        if (!TutorialHelper.HasCompletedTutorial)
        {
            GameSettings.BluePlayer = EPlayerType.Tutorial;
            GameSettings.RedPlayer = EPlayerType.Tutorial;
        }

        //Init m_PieceMappings dictionaries
        for (int i = 0; i < m_PlayerBluePieces.Count; i++)
        {
            Piece currentPiece = m_PlayerBluePieces[i];
            KeyValuePair<EPlayerColour, EPiece> key = new(EPlayerColour.Blue, currentPiece.PieceType);
            m_PieceMappings[key] = currentPiece;
        }

        for (int i = 0; i < m_PlayerRedPieces.Count; i++)
        {
            Piece currentPiece = m_PlayerRedPieces[i];
            KeyValuePair<EPlayerColour, EPiece> key = new(EPlayerColour.Red, currentPiece.PieceType);
            m_PieceMappings[key] = currentPiece;
        }

        m_BluePlayer = PlayerFactory.CreatePlayer(GameSettings.BluePlayer);
        m_RedPlayer = PlayerFactory.CreatePlayer(GameSettings.RedPlayer);

        m_BluePlayer.Init(this, m_GameLogic, EPlayerColour.Blue);
        m_RedPlayer.Init(this, m_GameLogic, EPlayerColour.Red);

        m_GameLogic.StartGame();
    }

    int m_CurrentLineIndex = 0;
    public IEnumerator ShowNextTutorialText()
    {
        m_TutorialScreen.SetActive(true);
        string currentLine = string.Empty;

        if (m_CurrentLineIndex >= 5)
        {
            yield break;
        }

        var op = LocalizationSettings.StringDatabase.GetLocalizedStringAsync("TextTable", $"GameplayInstruction{m_CurrentLineIndex}");

        while (!op.IsDone)
        {
            yield return null;
        }

        currentLine = op.Result;

        for (int i = 0; i <= currentLine.Length; i++)
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

    public void EnableValidTiles(List<EGrid> validTiles)
    {
        // Grab all the tiles, disable the ones that are not in the validTiles, otherwise set as valid
        for (int i = 0; i < m_Dropzones.Count; i++)
        {
            Dropzone currentTile = m_Dropzones[i];
            EGrid currentTilePos = currentTile.GridID;

            bool isValidTile = validTiles.Contains(currentTilePos);
            currentTile.enabled = isValidTile;

            if (isValidTile)
            {
                currentTile.DropzoneRingHelper.ValidRingOn();
            }
            else
            {
                currentTile.DropzoneRingHelper.RingOff();
            }  
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
        EnableValidTiles(validTiles);

        ResetVisualAidsOnActivePieces(false);
    }

    // Set piece on board and adjust the occupied cube colour
    public void RequestFinishMove(Piece piece, Dropzone targetZone)
    {
        StartCoroutine(DoRequestFinishMove(piece, targetZone));
    }

    private IEnumerator DoRequestFinishMove(Piece piece, Dropzone targetZone)
    {
        yield return MovePiece(piece, targetZone);

        targetZone.NeutralCube.SetActive(false);
        targetZone.RedCube.SetActive(piece.PlayerID == EPlayerColour.Red);
        targetZone.BlueCube.SetActive(piece.PlayerID == EPlayerColour.Blue);

        ResetVisualAidsOnActivePieces(false);
        ResetDropzoneVisualAids();

        yield return null;

        m_GameLogic.SetPieceOnBoard(piece.PieceType, targetZone.GridID);
    }

    private IEnumerator MovePiece(Piece piece, Dropzone targetZone)
    {
        GameplayAudioManager.Instance.PlayCharacterMoving();
        bool isTaken = false;

        Vector3 startPos = piece.transform.position;
        Vector3 targetPos = targetZone.transform.position;
        Vector3 endPos = targetPos;

        Rigidbody pieceRb = piece.GetComponent<Rigidbody>();

        if (!m_GameLogic.IsTileEmpty(targetZone.GridID))
        {
            Vector3 offest = new(0, 5, 0);
            targetPos += offest;
            isTaken = true;
        }

        float arcHeight = 7.5f;
        float minParabolaDuration = 0.75f;

        // Move on or above the dropzone
        yield return DoParabollicMovement(pieceRb, startPos, targetPos, arcHeight, minParabolaDuration);

        // If crashing then lower slowly and play crashing animation
        float duration = 0.5f;

        if (isTaken)
        {
            piece.GetReadyToCrash();
            float timeElapsed = 0;
            startPos = pieceRb.position;

            while (timeElapsed < duration)
            {
                pieceRb.position = Vector3.Lerp(startPos, endPos, timeElapsed / duration);
                timeElapsed += Time.deltaTime;

                yield return null;
            }
        }    

        pieceRb.position = targetZone.transform.position;
        piece.ResetAnimation();
    }

    private IEnumerator DoParabollicMovement(Rigidbody pieceRb, Vector3 startPos, Vector3 targetPos, float arcHeight, float minDuration)
    {
        float duration;
        float timeElapsed = 0;

        float x0 = startPos.x;
        float x1 = targetPos.x;
        float dist = x1 - x0;

        float z0 = startPos.z;
        float z1 = targetPos.z;

        float y0 = startPos.y;
        float y1 = targetPos.y;

        //Duration scaler vars
        float startDistanceToIncrease = 15;
        float endDistanceToIncrease = 30;
        float startDistanceValue = minDuration;
        float endDistanceValue = 1.25f;

        // Assign duration of the move depending on the distance
        if (Math.Abs(dist) < startDistanceToIncrease)
        {
            duration = startDistanceValue;
        }
        else if (Math.Abs(dist) > endDistanceToIncrease)
        {
            duration = endDistanceValue;
        }
        else
        {
            float distanceRange = endDistanceToIncrease - startDistanceToIncrease;
            float normalisedTimePassed = (Math.Abs(dist) - startDistanceToIncrease) / distanceRange;

            float valueRange = endDistanceValue - startDistanceValue;

            duration = (valueRange * normalisedTimePassed) + startDistanceValue;
        }

        while (timeElapsed < duration)
        {
            float normalisedTime = timeElapsed / duration;

            float nextX = Mathf.Lerp(x0, x1, normalisedTime);
            float nextZ = Mathf.Lerp(z0, z1, normalisedTime);
            float baseY = Mathf.Lerp(y0, y1, normalisedTime); //check how it works with normalisedTime

            float arc = arcHeight * (nextX - x0) * (nextX - x1) / (-0.25f * dist * dist);
            Vector3 nextPos = new(nextX, baseY + arc, nextZ);

            pieceRb.position = nextPos;

            timeElapsed += Time.deltaTime;

            yield return null;
        }

        pieceRb.position = targetPos;
    }

    // If the player dropped the piece outside of the board (or on invalid slot)
    public void RequestCancelMove()
    {
        StartCoroutine(DoRequestCancelMove());
    }

    private IEnumerator DoRequestCancelMove()
    {
        // Ensure that the piece exists the trigger zone before reseting
        yield return null;

        ResetVisualAidsOnActivePieces(true);

        yield return null;

        ResetDropzoneVisualAids();
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

    public void ResetDropzoneVisualAids()
    {
        for (int i = 0; i < m_Dropzones.Count; i++)
        {
            Dropzone currentDropzone = m_Dropzones[i];
            currentDropzone.DropzoneRingHelper.RingOff();
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
        yield return new WaitForSeconds(1.5f);

        if (winner != EPlayerColour.Invalid)
        {
            string winnerKey = winner.ToString();
            var op = LocalizationSettings.StringDatabase.GetLocalizedStringAsync("TextTable", $"{winnerKey}PlayerWon");
            if (op.IsDone)
            {
                m_GameOverText.text = $"{op.Result}";
            }
            else
            {
                op.Completed += OnGameOverLocalisedStringProcessed;
            }
            m_GameOverText.color = winner == EPlayerColour.Blue ? Color.blue : Color.red;
        }
        else
        {
            var op = LocalizationSettings.StringDatabase.GetLocalizedStringAsync("TextTable", "Draw");
            if (op.IsDone)
            {
                m_GameOverText.text = $"{op.Result}";
            }
            else
            {
                op.Completed += OnGameOverLocalisedStringProcessed;
            }
        }

        m_GameOverScreen.SetActive(true);

        yield return new WaitForSeconds(2);

        BackToMenu();
    }

    private void OnGameOverLocalisedStringProcessed(AsyncOperationHandle<string> op)
    {
        m_GameOverText.text = $"{op.Result}";
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

    private void OnDestroy()
    {
        StopAllCoroutines();

        m_GameLogic.OnTurnStarted -= OnTurnStarted;
        m_GameLogic.OnTurnEnded -= OnTurnEnded;
        m_GameLogic.OnGameEnded -= OnGameEnded;
    }
}
