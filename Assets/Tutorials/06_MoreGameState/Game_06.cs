using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game_06 : PersistableObject
{
    [SerializeField] ShapeFactory_04 shapeFactory;
    [SerializeField] PersistentStorage storage;
    public KeyCode createKey = KeyCode.C;
    public KeyCode destroyKey = KeyCode.X;
    public KeyCode newGameKey = KeyCode.N;
    public KeyCode saveKey = KeyCode.S;
    public KeyCode loadKey = KeyCode.L;

    public float CreationSpeed { get; set; }
    public float DestructionSpeed { get; set; }
    float creationProgress, destructionProgress;
    [SerializeField] Slider creationSpeedSlider;
    [SerializeField] Slider destructionSpeedSlider;

    private List<Shape> shapes;
    const int saveVersion = 3;

    [SerializeField] int levelCount;
    int loadedLevelBuildIndex;

    public int quickCreateCount = 10;

    [SerializeField] bool reseedOnLoad;
    Random.State mainRandomState;

    void Start()
    {
        // https://docs.unity3d.com/ScriptReference/Random-state.html
        mainRandomState = Random.state;

        shapes = new List<Shape>();

        if (Application.isEditor)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene loadedScene = SceneManager.GetSceneAt(i);
                if (loadedScene.name.Contains("Level "))
                {
                    SceneManager.SetActiveScene(loadedScene);
                    loadedLevelBuildIndex = loadedScene.buildIndex;
                    return;
                }
            }
        }

        BeginNewGame();
        StartCoroutine(LoadLevel(1));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(createKey))
        {
            for (int i = 0; i < quickCreateCount; i++)
            {
                CreateShape();
            }
        }
        else if (Input.GetKeyDown(destroyKey))
        {
            DestroyShape();
        }
        else if (Input.GetKeyDown(newGameKey))
        {
            BeginNewGame();
            StartCoroutine(LoadLevel(loadedLevelBuildIndex));
        }
        else if (Input.GetKeyDown(saveKey))
        {
            storage.Save(this);
        }
        else if (Input.GetKeyDown(loadKey))
        {
            BeginNewGame();
            storage.Load(this);
        }
        else
        {
            for (int i = 1; i <= levelCount; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha0 + i))
                {
                    BeginNewGame();
                    StartCoroutine(LoadLevel(i));
                    return;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        creationProgress += Time.deltaTime * CreationSpeed;
        while (creationProgress >= 1f)
        {
            creationProgress -= 1f;
            CreateShape();
        }

        destructionProgress += Time.deltaTime * DestructionSpeed;
        while (destructionProgress >= 1f)
        {
            destructionProgress -= 1f;
            DestroyShape();
        }
    }

    void CreateShape()
    {
        Shape instance = shapeFactory.GetRandom();
        Transform t = instance.transform;
        t.localPosition = GameLevel_06.Current.SpawnPoint;
        t.localRotation = Random.rotation;
        t.localScale = Vector3.one * Random.Range(0.1f, 1f);
        instance.SetColor(Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.25f, 1f, 1f, 1f));
        shapes.Add(instance);
    }

    void BeginNewGame()
    {
        Random.state = mainRandomState;
        int seed = Random.Range(0, int.MaxValue) ^ (int)Time.unscaledTime;
        mainRandomState = Random.state;
        Random.InitState(seed);

        creationSpeedSlider.value = CreationSpeed = 0;
        destructionSpeedSlider.value = DestructionSpeed = 0;

        for (int i = 0; i < shapes.Count; i++)
        {
            shapeFactory.Reclaim(shapes[i]);
        }
        shapes.Clear();
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(saveVersion);
        writer.Write(shapes.Count);
        writer.Write(Random.state);
        writer.Write(CreationSpeed);
        writer.Write(creationProgress);
        writer.Write(DestructionSpeed);
        writer.Write(destructionProgress);
        writer.Write(loadedLevelBuildIndex);
        GameLevel_06.Current.Save(writer);
        for (int i = 0; i < shapes.Count; i++)
        {
            //writer.Write(shapes[i].ShapeId);
            //writer.Write(shapes[i].MaterialId);
            shapes[i].Save(writer);
        }
    }

    public override void Load(GameDataReader reader)
    {
        int version = reader.ReadInt();
        if (version > saveVersion)
        {
            Debug.LogError("Unsupported future save version" + version);
            return;
        }
        StartCoroutine(LoadGame(reader, version));
    }

    void DestroyShape()
    {
        if (shapes.Count > 0)
        {
            int index = Random.Range(0, shapes.Count);
            shapeFactory.Reclaim(shapes[index]);
            int lastIndex = shapes.Count - 1;
            shapes[index] = shapes[lastIndex];
            shapes.RemoveAt(lastIndex);
        }
    }

    IEnumerator LoadLevel(int levelBuildIndex)
    {
        enabled = false;
        if (loadedLevelBuildIndex > 0)
        {
            yield return SceneManager.UnloadSceneAsync(loadedLevelBuildIndex);
        }
        yield return SceneManager.LoadSceneAsync(
            levelBuildIndex, LoadSceneMode.Additive
        );
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(levelBuildIndex));
        loadedLevelBuildIndex = levelBuildIndex;
        enabled = true;
    }

    IEnumerator LoadGame (GameDataReader reader, int version)
    {
        int count = reader.ReadInt();
        if (version >= 3)
        {
            Random.State state = reader.ReadRandomState();
            if (!reseedOnLoad)
            {
                Random.state = state;
            }
            creationSpeedSlider.value = CreationSpeed = reader.ReadFloat();
            creationProgress = reader.ReadFloat();
            destructionSpeedSlider.value = DestructionSpeed = reader.ReadFloat();
            destructionProgress = reader.ReadFloat();
        }
        yield return StartCoroutine(LoadLevel(version < 2 ? 1 : reader.ReadInt()));
        if (version >= 3)
        {
            GameLevel_06.Current.Load(reader);
        }
        for (int i = 0; i < count; i++)
        {
            int shapeId = reader.ReadInt();
            int materialId = reader.ReadInt();
            Shape instance = shapeFactory.Get(shapeId, materialId);
            instance.Load(reader);
            shapes.Add(instance);
        }
    }
}
