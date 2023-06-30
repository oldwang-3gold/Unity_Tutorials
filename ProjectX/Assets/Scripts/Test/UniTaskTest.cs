using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class UniTaskTest : MonoBehaviour
{
    public bool test = false;

    private void Start()
    {
        TestFunc();
    }

    async UniTask TestFunc()
    {
        await UniTask.Delay(2000);
        while (true)
        {
            bool result = CheckIsTrue();
            if (result)
            {
                Debug.Log("Exit while");
                break;
            }
            Debug.Log("Wait");
            await UniTask.Delay(100);
        }
    }

    bool CheckIsTrue()
    {
        return test;
    }
}
