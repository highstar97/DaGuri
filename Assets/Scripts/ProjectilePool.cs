using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// Photon에서 사용할 커스텀 프리팹 풀.
/// 지정된 프리팹(FireBall, Arrow, Sword)만 풀링 대상으로 하며,
/// 나머지는 기본 Instantiate 방식으로 처리.
/// </summary>
public class ProjectilePool : IPunPrefabPool
{
    #region Variables
    private readonly Dictionary<string, Transform> prefabRoots = new();     // prefabId별 루트 Transform

    private readonly HashSet<string> pooledPrefabs = new() { "FireBall", "Arrow", "Sword" };    // 풀링 대상으로 지정한 프리팹 이름들

    private Transform poolRoot;                                             // 비활성화된 오브젝트들을 담아 둘 부모 트랜스폼
    #endregion

    public ProjectilePool()
    {
        // 풀링된 오브젝트들을 씬에서 보기 쉽게 루트 트랜스폼 생성
        poolRoot = new GameObject("ProjectilePools").transform;
    }

    /// <summary>
    /// PhotonNetwork.Instantiate() 내부에서 호출됨. 네트워크 오브젝트 생성 시 풀에 있으면 꺼내고, 없으면 새로 생성.
    /// </summary>
    public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    {
        if (!pooledPrefabs.Contains(prefabId))  // 풀링 대상이 아닌 프리팹은 그냥 새로 생성해서 반환
        {
            GameObject prefab = Resources.Load<GameObject>(prefabId);
            GameObject gameObject = GameObject.Instantiate(prefab, position, rotation);
            gameObject.SetActive(false);    // Photon이 내부에서 활성화 처리함
            return gameObject;
        }

        // 해당 prefabId의 루트가 없다면 생성 
        if (!prefabRoots.ContainsKey(prefabId))
        {
            GameObject prefabRoot = new GameObject(prefabId + "_Pool");
            prefabRoot.transform.SetParent(poolRoot);
            prefabRoots[prefabId] = prefabRoot.transform;
        }

        Transform parent = prefabRoots[prefabId];
        
        // 자식 중에서 비활성화된 오브젝트 찾기
        for (int i = 0; i < parent.childCount; ++i)
        {
            Transform child = parent.GetChild(i);
            if (!child.gameObject.activeSelf)   // 사용되지 않은 오브젝트
            {
                GameObject gameObject = child.gameObject;
                gameObject.transform.SetPositionAndRotation(position, rotation);
                gameObject.SetActive(false);
                return gameObject;
            }
        }

        // 사용 가능한 오브젝트가 없으면 새로 생성
        GameObject prefabToInstantiate = Resources.Load<GameObject>(prefabId);
        if (prefabToInstantiate == null)
        {
            Debug.LogError($"[CustomPrefabPool] Cannot load prefab: {prefabId}");
            return null;
        }
        GameObject newGameObject = Object.Instantiate(prefabToInstantiate, position, rotation);
        newGameObject.transform.SetParent(parent);  // 프리팹 ID별 루트에 등록
        newGameObject.SetActive(false);
        return newGameObject;
    }

    /// <summary>
    /// PhotonNetwork.Destroy() 시 내부적으로 호출됨.
    /// 오브젝트를 비활성화하고 다시 풀에 반환.
    /// </summary>
    public void Destroy(GameObject gameObject)
    {
        string prefabId = gameObject.name.Replace("(Clone)", "").Trim();

        if (!pooledPrefabs.Contains(prefabId))  // 풀링 대상이 아니라면 그냥 파괴
        {
            Object.Destroy(gameObject);
            return;
        }

        // 풀링 대상이라면
        gameObject.SetActive(false);

        if (prefabRoots.TryGetValue(prefabId, out Transform parent))
        {
            gameObject.transform.SetParent(parent);
        }
        else
        {
            gameObject.transform.SetParent(poolRoot);
        }
    }
}