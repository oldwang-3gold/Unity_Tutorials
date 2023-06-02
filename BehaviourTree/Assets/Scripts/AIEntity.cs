using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GGGBT;

public class AIBBConst
{
    public static string TARGET_MOVING_POSITION = "TargetMovingPosition";
}

public class AIEntity : MonoBehaviour
{
    private BTAction _behaviourTree;
    private TBlackBoard _blackBoard;
    private AIEntityWorkingData _behaviourWorkingData;
    private float _movingSpeed;
    public float MovingSpeed { 
        get 
        { 
            return _movingSpeed; 
        } 
        set 
        { 
            _movingSpeed = value; 
        } 
    }

    public GameObject targetObject;
    public AIEntity Init()
    {
        _behaviourTree = Demo1.CreateTree();

        _behaviourWorkingData = new AIEntityWorkingData();
        _behaviourWorkingData.Entity = this;
        _behaviourWorkingData.EntityTF = transform;

        _blackBoard = new TBlackBoard();

        _movingSpeed = 0.5f;

        return this;
    }

    private void Awake()
    {
        Init();
    }

    public T GetBBValue<T>(string key, T defaultValue)
    {
        return _blackBoard.GetValue(key, defaultValue);
    }

    public int UpdateAI(float gameTime, float deltaTime)
    {
        return 0;
    }

    public int UpdateRequest(float gameTime, float deltaTime)
    {
        _behaviourTree.Transition(_behaviourWorkingData);
        return 0;
    }

    public int UpdateBehaviour(float gameTime, float deltaTime)
    {
        _behaviourWorkingData.DeltaTime = deltaTime;

        _blackBoard.SetValue(AIBBConst.TARGET_MOVING_POSITION, targetObject.transform.position);
        if (_behaviourTree.Evaluate(_behaviourWorkingData))
        {
            _behaviourTree.Update(_behaviourWorkingData);
        }
        else
        {
            _behaviourTree.Transition(_behaviourWorkingData);
        }
        return 0;
    }
}
