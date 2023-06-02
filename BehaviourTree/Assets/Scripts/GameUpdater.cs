using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUpdater : MonoBehaviour
{
    public List<AIEntity> entities;

    private float gameTime = 0f;
    private float deltaTime = 0f;

    void Update()
    {
        deltaTime = Time.deltaTime * Time.timeScale;
        gameTime += deltaTime;
        for (int i = 0; i < entities.Count; i++)
        {
            entities[i].UpdateBehaviour(gameTime, deltaTime);
        }
    }
}
