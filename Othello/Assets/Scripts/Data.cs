using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//add corner and around
//or small move in the beginning
public class Data
{
    public enum STATE { EMPTY, BLACK, WHITE };

    public enum DIRECTION { RIGHT, DOWNRIGHT, DOWN, DOWNLEFT, LEFT, UPLEFT, UP, UPRIGHT };

    public enum AIDIFFICULTY { EASY, MEDIUM, HARD };

    public short[] directionPiece = { -1, -1, -1, -1, -1, -1, -1, -1 };

    public short[] tmpDirectionPiece = { -1, -1, -1, -1, -1, -1, -1, -1 };

    public int value = 0;

    public int numberOfPieces = 0;

    bool hasStrategicValue = false;

    public STATE stateAI;

    public STATE stateP1;

    public int scoreP1;

    public int scoreAI;

    public string winner;

    public STATE[,] board = new STATE[8, 8];

    public bool isOpponent = true;

    public int valueColumnAI;

    public int valueLineAI;

    public List<Data> children = new List<Data>();

    public AIDIFFICULTY difficulty;

    public int depth;

    public bool isAiPlaying;

    public void InitBoard()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                board[i, j] = STATE.EMPTY;
            }
        }
        board[3, 3] = STATE.WHITE;
        board[4, 4] = STATE.WHITE;

        board[3, 4] = STATE.BLACK;
        board[4, 3] = STATE.BLACK;
        stateP1 = STATE.BLACK;
        stateAI = STATE.WHITE;
        numberOfPieces = 4;
        difficulty = (AIDIFFICULTY)PlayerPrefs.GetInt("Difficulty", 0);
        depth = 0;
    }

    public STATE GetCurrentPlayerState()
    {
        if (isOpponent)
        {
            return stateP1;
        }
        else
        {
            return stateAI;
        }
    }

    public STATE GetOpponentState()
    {
        if (!isOpponent)
        {
            return stateP1;
        }
        else
        {
            return stateAI;
        }
    }

    public bool IsTwoPiecesAligned(int column, int line, short[] directionPiece)
    {
        STATE playerState = GetCurrentPlayerState();
        bool isFinishedLeft = false;
        bool isFinishedRight = false;
        bool isFinishedUp = false;
        bool isFinishedDown = false;


        bool isFinishedUpLeft = false;
        bool isFinishedUpRight = false;
        bool isFinishedDownLeft = false;
        bool isFinishedDownRight = false;
        for (short i = 1; i < 8; i++)
        {
            //left
            if (IsInTheGrid(column - i, line) && board[column - i, line] != STATE.EMPTY && !isFinishedLeft)
            {
                if (board[column - i, line] == playerState)
                {
                    directionPiece[(int)DIRECTION.LEFT] = i;
                    isFinishedLeft = true;
                }
            }
            else
            {
                isFinishedLeft = true;
            }
            //right
            if (IsInTheGrid(column + i, line) && board[column + i, line] != STATE.EMPTY && !isFinishedRight)
            {
                if (board[column + i, line] == playerState)
                {
                    directionPiece[(int)DIRECTION.RIGHT] = i;
                    isFinishedRight = true;
                }
            }
            else
            {
                isFinishedRight = true;
            }
            //up
            if (IsInTheGrid(column, line - i) && board[column, line - i] != STATE.EMPTY && !isFinishedUp)
            {
                if (board[column, line - i] == playerState)
                {
                    directionPiece[(int)DIRECTION.UP] = i;
                    isFinishedUp = true;
                }
            }
            else
            {
                isFinishedUp = true;
            }
            //down
            if (IsInTheGrid(column, line + i) && board[column, line + i] != STATE.EMPTY && !isFinishedDown)
            {
                if (board[column, line + i] == playerState)
                {
                    directionPiece[(int)DIRECTION.DOWN] = i;
                    isFinishedDown = true;
                }
            }
            else
            {
                isFinishedDown = true;
            }


            //UpLeft
            if (IsInTheGrid(column - i, line - i) && board[column - i, line - i] != STATE.EMPTY && !isFinishedUpLeft)
            {
                if (board[column - i, line - i] == playerState)
                {
                    directionPiece[(int)DIRECTION.UPLEFT] = i;
                    isFinishedUpLeft = true;
                }
            }
            else
            {
                isFinishedUpLeft = true;
            }
            //UpRight
            if (IsInTheGrid(column + i, line - i) && board[column + i, line - i] != STATE.EMPTY && !isFinishedUpRight)
            {
                if (board[column + i, line - i] == playerState)
                {
                    directionPiece[(int)DIRECTION.UPRIGHT] = i;
                    isFinishedUpRight = true;
                }
            }
            else
            {
                isFinishedUpRight = true;
            }
            //DownLeft
            if (IsInTheGrid(column - i, line + i) && board[column - i, line + i] != STATE.EMPTY && !isFinishedDownLeft)
            {
                if (board[column - i, line + i] == playerState)
                {
                    directionPiece[(int)DIRECTION.DOWNLEFT] = i;
                    isFinishedDownLeft = true;
                }
            }
            else
            {
                isFinishedDownLeft = true;
            }
            //downRight
            if (IsInTheGrid(column + i, line + i) && board[column + i, line + i] != STATE.EMPTY && !isFinishedDownRight)
            {
                if (board[column + i, line + i] == playerState)
                {
                    directionPiece[(int)DIRECTION.DOWNRIGHT] = i;
                    isFinishedDownRight = true;
                }
            }
            else
            {
                isFinishedDownRight = true;
            }
        }

        if (directionPiece[(int)DIRECTION.DOWN] != -1 ||
             directionPiece[(int)DIRECTION.RIGHT] != -1 ||
             directionPiece[(int)DIRECTION.UP] != -1 ||
             directionPiece[(int)DIRECTION.LEFT] != -1 ||
             directionPiece[(int)DIRECTION.UPLEFT] != -1 ||
             directionPiece[(int)DIRECTION.UPRIGHT] != -1 ||
             directionPiece[(int)DIRECTION.DOWNLEFT] != -1 ||
             directionPiece[(int)DIRECTION.DOWNRIGHT] != -1)
        {
            return true;
        }

        return false;
    }

    public bool IsCaseEmpty(int column, int line)
    {
        if (board[column, line] == STATE.EMPTY)
        {
            return true;
        }
        return false;
    }

    public bool IsOpponentPieceNextTo(int column, int line, short[] directionPiece)
    {
        STATE stateOpponent = GetOpponentState();
        if ((IsInTheGrid(column - 1, line) && board[column - 1, line] == stateOpponent && directionPiece[(int)DIRECTION.LEFT] != -1) ||
            (IsInTheGrid(column + 1, line) && board[column + 1, line] == stateOpponent && directionPiece[(int)DIRECTION.RIGHT] != -1) ||
            (IsInTheGrid(column, line - 1) && board[column, line - 1] == stateOpponent && directionPiece[(int)DIRECTION.UP] != -1) ||
           (IsInTheGrid(column, line + 1) && board[column, line + 1] == stateOpponent && directionPiece[(int)DIRECTION.DOWN] != -1) ||
           (IsInTheGrid(column - 1, line - 1) && board[column - 1, line - 1] == stateOpponent && directionPiece[(int)DIRECTION.UPLEFT] != -1) ||
            (IsInTheGrid(column + 1, line - 1) && board[column + 1, line - 1] == stateOpponent && directionPiece[(int)DIRECTION.UPRIGHT] != -1) ||
            (IsInTheGrid(column - 1, line + 1) && board[column - 1, line + 1] == stateOpponent && directionPiece[(int)DIRECTION.DOWNLEFT] != -1) ||
           (IsInTheGrid(column + 1, line + 1) && board[column + 1, line + 1] == stateOpponent && directionPiece[(int)DIRECTION.DOWNRIGHT] != -1))
        {
            return true;
        }
        return false;
    }

    public bool CanIPlay(int column, int line)
    {
        for (int k = 0; k < 8; k++)
        {
            directionPiece[k] = -1;
        }
        if (IsCaseEmpty(column, line) && IsTwoPiecesAligned(column, line, directionPiece) && IsOpponentPieceNextTo(column, line, directionPiece))
        {
            return true;
        }
        return false;
    }

    public bool IsPossibilityToPlay()
    {
        short[] directionPieceTmp = new short[8] { -1, -1, -1, -1, -1, -1, -1, -1 };
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (IsCaseEmpty(i, j) && IsTwoPiecesAligned(i, j, directionPieceTmp) && IsOpponentPieceNextTo(i, j, directionPieceTmp))
                {
                    return true;
                }
                for(int k = 0; k < 8; k++)
                {
                    directionPieceTmp[k] = -1;
                }
            }
        }
        return false;
    }

    public bool CanIPlayAI(int column, int line)
    {
        for (int k = 0; k < 8; k++)
        {
            tmpDirectionPiece[k] = -1;
        }

        if (IsCaseEmpty(column, line) && IsTwoPiecesAligned(column, line, tmpDirectionPiece) && IsOpponentPieceNextTo(column, line, tmpDirectionPiece))
        {
            return true;
        }
        return false;
    }

    bool IsInTheGrid(int column, int line)
    {
        if (column >= 0 && column < 8 && line >= 0 && line < 8)
        {
            return true;
        }
        return false;
    }

    public void TransformOpponentPiece(int column, int line, STATE playerState)
    {
        for (short i = 0; i < 8; i++)
        {
            //left
            if (i < directionPiece[(int)DIRECTION.LEFT])
            {
                board[column - i, line] = playerState;

            }
            //right
            if (i < directionPiece[(int)DIRECTION.RIGHT])
            {
                board[column + i, line] = playerState;
            }

            //up
            if (i < directionPiece[(int)DIRECTION.UP])
            {
                board[column, line - i] = playerState;
            }
            //down
            if (i < directionPiece[(int)DIRECTION.DOWN])
            {
                board[column, line + i] = playerState;
            }

            //UpLeft
            if (i < directionPiece[(int)DIRECTION.UPLEFT])
            {
                board[column - i, line - i] = playerState;
            }
            //UpRight
            if (i < directionPiece[(int)DIRECTION.UPRIGHT])
            {
                board[column + i, line - i] = playerState;
            }

            //DownLeft
            if (i < directionPiece[(int)DIRECTION.DOWNLEFT])
            {
                board[column - i, line + i] = playerState;
            }

            //downRight
            if (i < directionPiece[(int)DIRECTION.DOWNRIGHT])
            {
                board[column + i, line + i] = playerState;
            }
        }
    }

    public int Minmax()
    {
        if (children.Count == 0)
        {
            value = Evaluate();
            return value;
        }
        else if (isOpponent)
        {
            int minValue = 100000;
            for (int i = 0; i < children.Count; i++)
            {
                int tmpValue = children[i].Minmax();
                if (tmpValue < minValue)
                    minValue = tmpValue;
            }
            value = minValue;
            return minValue;
        }
        else
        {
            int maxValue = -100000;
            bool strategic = false;
            for (int i = 0; i < children.Count; i++)
            {
                int tmpValue = children[i].Minmax();
                if (difficulty == AIDIFFICULTY.HARD)
                {
                    if (tmpValue > maxValue - 3 && hasStrategicValue)
                    {
                        strategic = true;
                        maxValue = tmpValue;
                    }
                    else if (tmpValue > maxValue && !strategic)
                        maxValue = tmpValue;
                    else if (tmpValue > maxValue + 3)
                        maxValue = tmpValue;
                }
                else if (tmpValue > maxValue)
                    maxValue = tmpValue;
            }
            value = maxValue;
            return maxValue;
        }
    }

    bool HasStrategicValue()
    {
        if ((valueColumnAI == 0 && valueLineAI == 0) ||
            (valueColumnAI == 0 && valueLineAI == 7) ||
            (valueColumnAI == 7 && valueLineAI == 0) ||
                (valueColumnAI == 7 && valueLineAI == 7))
        {
            return true;
        }
        return false;
    }
    //rajouter un ptit truc pour le mode hard
    public int Evaluate()
    {
        STATE statePlayer = isAiPlaying ? stateAI : stateP1;
        int tmpScore = 0;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (board[i, j] == statePlayer)
                {
                    tmpScore++;
                }

            }
        }
        if (difficulty == AIDIFFICULTY.HARD)
            hasStrategicValue = HasStrategicValue();
        return tmpScore;
    }

    public void CalculateFinaleScore()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (board[i, j] == stateP1)
                {
                    scoreP1++;
                }
                else if (board[i, j] == stateAI)
                {
                    scoreAI++;
                }
            }
        }
        if (scoreAI == scoreP1)
        {
            string tmp = "No one";
            winner = tmp;
        }
        else if (scoreAI > scoreP1)
        {
            string tmp = stateAI == STATE.WHITE ? "white" : "black";
            winner = tmp;
        }
        else
        {
            string tmp = stateP1 == STATE.WHITE ? "white" : "black";
            winner = tmp;
        }
    }
}
