using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLevel_06 : PersistableObject
{
    public static GameLevel_06 Current { get; private set; }

    public Vector3 SpawnPoint
    {
        get { return spawnZone.SpawnPoint; }
    }

    [SerializeField]
    SpawnZone spawnZone;

    [SerializeField]
    PersistableObject[] persistableObjects;

    void OnEnable()
    {
        Current = this;
        if (persistableObjects == null)
        {
            persistableObjects = new PersistableObject[0];
        }
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(persistableObjects.Length);
        for (int i = 0; i < persistableObjects.Length; i++)
        {
            persistableObjects[i].Save(writer);
        }
    }

    public override void Load(GameDataReader reader)
    {
        int savedCount = reader.ReadInt();
        for (int i = 0; i < savedCount; i++)
        {
            persistableObjects[i].Load(reader);
        }
    }
}
