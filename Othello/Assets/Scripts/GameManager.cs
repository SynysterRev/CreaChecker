using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject parentCase;
    Button[,] cases;
    GameObject[,] cases3D;
    [SerializeField] Sprite[] sprites;
    [SerializeField] GameObject prefabButton;
    [SerializeField] Text winner;
    [SerializeField] Text scoreWhite;
    [SerializeField] Text scoreBlack;
    [SerializeField] GameObject board3D;
    [SerializeField] GameObject prefabCube;
    [SerializeField] GameObject[] prefab3DPiece;
    [SerializeField] GameObject board2D;
    [SerializeField] GameObject filterEnd;
    Data data;
    bool isNewTurn;
    int cptTurn;
    int nbSimulation;
    bool gameOver;
    int cpt = 0;
    bool isTwoPlayers;
    bool is3D;
    int maxDepth;
    // Use this for initialization
    void Start()
    {
        is3D = false;
        ChangeVisual();
        cptTurn = 0;
        gameOver = false;
        cases = new Button[8, 8];
        cases3D = new GameObject[8, 8];
        isNewTurn = true;
        filterEnd.gameObject.SetActive(false);
        isTwoPlayers = PlayerPrefs.GetInt("IsTwoPlayers", 0) != 0;
        maxDepth = PlayerPrefs.GetInt("Difficulty", 0) * 2 + 1;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {

                GameObject tmp = Instantiate(prefabButton);

                cases[i, j] = tmp.GetComponent<Button>();

                cases[i, j].transform.SetParent(parentCase.transform);
                cases[i, j].name = "Case : " + i + " , " + j;
                int idColumn = i;
                int idLine = j;

                cases[i, j].onClick.AddListener(() => Play(idColumn, idLine));
                RectTransform rectTrans = cases[i, j].GetComponent<RectTransform>();
                rectTrans.pivot = Vector3.up;
                rectTrans.anchoredPosition = new Vector2(i * 128.0f - 512.0f, 512.0f - j * 128.0f);
                rectTrans.sizeDelta = new Vector2(128.0f, 128.0f);
                rectTrans.transform.localScale = Vector3.one;
            }
        }
        data = new Data();
        data.InitBoard();
        UpdateVisual();
    }

    public void ResetGame()
    {
        SoundManager.instance.PlayButtonSound();
        cptTurn = 0;
        gameOver = false;
        isNewTurn = true;
        data.isOpponent = true;
        filterEnd.gameObject.SetActive(false);
        CleanBoard();
        data.InitBoard();
        UpdateVisual();
    }

    void CleanBoard()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                cases[i, j].image.sprite = sprites[2];
                if (cases3D[i, j] != null)
                {
                    Destroy(cases3D[i, j]);
                    cases3D[i, j] = null;
                }
            }
        }
    }

    public void AI_Play()
    {
        data.children.Clear();
        data.isOpponent = false;
        data.depth = 0;
        data.value = 0;
        nbSimulation = 0;
        Simulate(data);
        //  Debug.Log("NB_sim = " + nbSimulation);
        int value = data.Minmax();
        //Debug.Log("minmax : " + value);
        for (int i = 0; i < data.children.Count; i++)
        {
            if (data.children[i].value == value)
            {
                data = data.children[i];
                Debug.Log("value column " + data.valueColumnAI + " " + data.valueLineAI);
                data.numberOfPieces++;
                i = data.children.Count;
            }
        }
        UpdateVisual();
        isNewTurn = true;
    }

    //to change et opti
    void Simulate(Data dataSim)
    {
        nbSimulation++;
        if (dataSim.depth < maxDepth)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (dataSim.CanIPlayAI(i, j))
                    {
                        Data tmpData = new Data();
                        Array.Copy(dataSim.board, tmpData.board, 64);
                        tmpData.stateAI = data.stateAI;
                        tmpData.stateP1 = data.stateP1;
                        tmpData.valueColumnAI = i;
                        tmpData.valueLineAI = j;
                        tmpData.isOpponent = !dataSim.isOpponent;
                        tmpData.isAiPlaying = !dataSim.isOpponent;
                        tmpData.numberOfPieces = dataSim.numberOfPieces;
                        tmpData.depth = dataSim.depth + 1;
                        tmpData.directionPiece = dataSim.tmpDirectionPiece;
                        if (dataSim.isOpponent)
                        {
                            tmpData.board[i, j] = data.stateP1;

                            tmpData.TransformOpponentPiece(i, j, data.stateP1);
                        }
                        else
                        {
                            tmpData.board[i, j] = data.stateAI;
                            tmpData.TransformOpponentPiece(i, j, data.stateAI);
                        }
                        Simulate(tmpData);
                        dataSim.children.Add(tmpData);
                    }
                }
            }
        }
    }
    public void Play(int column, int line)
    {
        if (data.CanIPlay(column, line))
        {
            if (data.isOpponent)
            {
                data.board[column, line] = data.stateP1;

                data.TransformOpponentPiece(column, line, data.stateP1);
            }
            else
            {
                data.board[column, line] = data.stateAI;

                data.TransformOpponentPiece(column, line, data.stateAI);
            }
            data.numberOfPieces++;
            data.isOpponent = !data.isOpponent;

            UpdateVisual();
            isNewTurn = true;
        }
    }

    public void UpdateVisual()
    {
        bool hasDestroyedPiece = false;
        bool hasCreatedPiece = false;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (data.board[i, j] == Data.STATE.BLACK)
                {
                    cases[i, j].image.sprite = sprites[1];
                    if (cases3D[i, j] != null)
                    {
                        if (cases3D[i, j].tag == "White")
                        {
                            Destroy(cases3D[i, j]);
                            hasDestroyedPiece = true;
                            InstantiatePiece(i, j, 1);
                        }
                    }
                    else
                    {
                        InstantiatePiece(i, j, 1);
                    }
                    hasCreatedPiece = true;
                }
                else if (data.board[i, j] == Data.STATE.WHITE)
                {
                    cases[i, j].image.sprite = sprites[0];
                    if (cases3D[i, j] != null)
                    {
                        if (cases3D[i, j].tag == "Black")
                        {
                            Destroy(cases3D[i, j]);
                            hasDestroyedPiece = true;
                            InstantiatePiece(i, j, 0);
                        }

                    }
                    else
                    {
                        InstantiatePiece(i, j, 0);
                    }
                    hasCreatedPiece = true;
                }
            }
        }
        if (hasDestroyedPiece)
        {
            SoundManager.instance.PlaySound(SoundManager.TYPESOUND.DESTROYPIECE);
        }
        if (hasCreatedPiece)
        {
            SoundManager.instance.PlaySound(SoundManager.TYPESOUND.CREATEPIECE);
        }
    }


    void ChangeVisual()
    {
        is3D = !is3D;
        if (is3D)
        {
            board2D.gameObject.SetActive(false);
            board3D.gameObject.SetActive(true);
        }
        else
        {
            board2D.gameObject.SetActive(true);
            board3D.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameOver)
        {
            if (isNewTurn && !data.IsPossibilityToPlay())
            {
                data.isOpponent = !data.isOpponent;
                cptTurn++;
                Debug.Log("je passe le tour");
                if (cptTurn == 2)
                {
                    Debug.Log("game over");
                    data.CalculateFinaleScore();
                    gameOver = true;
                    filterEnd.gameObject.SetActive(true);
                    scoreWhite.text = "White : \n" + (data.stateP1 == Data.STATE.WHITE ? data.scoreP1.ToString() : data.scoreAI.ToString());
                    scoreBlack.text = "Black : \n" + (data.stateP1 == Data.STATE.BLACK ? data.scoreP1.ToString() : data.scoreAI.ToString());
                    winner.text = "Winner is " + data.winner;
                    return;
                }
            }
            else if (isNewTurn)
            {
                isNewTurn = false;
                cptTurn = 0;
                if (!isTwoPlayers && !data.isOpponent)
                {
                    StartCoroutine(WaitAIToPlay());
                }
            }
        }
        if ((isTwoPlayers || data.isOpponent) && Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                Play((int)hit.point.x, (8 - (int)hit.point.z));
            }
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            ChangeVisual();
        }


    }

    IEnumerator WaitAIToPlay()
    {
        yield return new WaitForSeconds(1.0f);
        AI_Play();
    }

    void InstantiatePiece(int i, int j, int sprite)
    {
        cases3D[i, j] = Instantiate(prefab3DPiece[sprite], new Vector3(i + 0.5f, 0.5f, 8 - j + 0.5f), Quaternion.identity);
        cases3D[i, j].transform.Rotate(new Vector3(0.0f, 180.0f, 0.0f));
        cases3D[i, j].transform.SetParent(board3D.transform);
    }
}
