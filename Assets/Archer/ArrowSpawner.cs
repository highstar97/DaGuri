using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Pool;

public class ArrowSpawner : MonoBehaviour
{
    public const float shotTime = 1f;
    [SerializeField]
    Arrow arrowPrefabs;
    [SerializeField]
    Arrow skillarrowPrefabs;

    ObjectPool<Arrow> arrowPool;
    ObjectPool<Arrow> SkillarrowPool;

    private void Awake()
    {
        arrowPool = new ObjectPool<Arrow>(
            createFunc: () =>
            {
                Arrow arrow = Instantiate(arrowPrefabs, this.transform);
                arrow.OnArrowCollapsed += Release;
                return arrow;
            },
            actionOnGet: (arrow) => arrow.gameObject.SetActive(true),
            actionOnRelease: (arrow) => arrow.gameObject.SetActive(false),
            actionOnDestroy: (arrow) => Destroy(arrow.gameObject),
            collectionCheck: false,
            defaultCapacity: 10,
            maxSize: 100
            );


        SkillarrowPool = new ObjectPool<Arrow>(
          createFunc: () =>
          {
              Arrow arrow = Instantiate(skillarrowPrefabs, this.transform);
              arrow.OnArrowCollapsed += ReleaseSkillArrow;
              return arrow;
          },
          actionOnGet: (arrow) => arrow.gameObject.SetActive(true),
          actionOnRelease: (arrow) => arrow.gameObject.SetActive(false),
          actionOnDestroy: (arrow) => Destroy(arrow.gameObject),
          collectionCheck: false,
          defaultCapacity: 10,
          maxSize: 100
          );
    }

    public void SpawnArrow(Vector3 position, Vector3 direction, GameObject owner)
    {
        Arrow arrow = arrowPool.Get();
        arrow.transform.position = position;
        arrow.transform.forward = direction;
        arrow.ownerStart = owner.GetComponent<StartComponent>();
    }
    public void SpawnSkillArrow(Vector3 position, Vector3 direction, GameObject owner)
    {
        Arrow arrow = SkillarrowPool.Get();
        arrow.transform.position = position;
        arrow.transform.forward = direction;
        arrow.ownerStart = owner.GetComponent<StartComponent>();
    }

    public void Release(Arrow arrow)
    {
        arrowPool.Release(arrow);
    }
    void ReleaseSkillArrow(Arrow arrow)
    {
        SkillarrowPool.Release(arrow);
    }
}
