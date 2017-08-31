using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathDefs {

    public const int CONST_MAX_MOVE_DIRECTION = 4;

    public  enum AI_Direction
    {
        MOVE_NONE = 0,
        MOVE_RIGHT = 1,
        MOVE_LEFT = 2,
        MOVE_UP = 4,
        MOVE_DOWN = 8,
    }

    public enum AI_PATH_TYPE
    {
        PATH_LINEAR,
        PATH_RANDOM,
        PATH_FOLLOW,
        PATH_STEADY,
        PATH_RUNAWAY,
        PATH_RABBIT,
    }
}
