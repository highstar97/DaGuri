using Photon.Pun.Demo.Asteroids;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class FireBallSpawner : MonoBehaviour
{
    public const float lifeTime = 5.0f;

    [SerializeField] FireBall fireBallPrefab;

    ObjectPool<FireBall> fireBallPool;

    private void Awake()
    {
        fireBallPool = new ObjectPool<FireBall>(
            createFunc: () =>
            {
                FireBall fireBall = Instantiate(fireBallPrefab, this.transform);
                fireBall.OnFireBallCollapsed += Release;
                return fireBall;
            },
            actionOnGet: (fireBall) => fireBall.gameObject.SetActive(true),
            actionOnRelease: (fireBall) => fireBall.gameObject.SetActive(false),
            actionOnDestroy: (fireBall) => Destroy(fireBall.gameObject),
            collectionCheck: false,
            defaultCapacity: 10,
            maxSize: 100
        );
    }

    public void SpawnFireBall(Vector3 position, Vector3 direction, GameObject owner)
    {
        FireBall fireBall = fireBallPool.Get();
        fireBall.transform.position = position;
        fireBall.transform.forward = direction;
        fireBall.ownerStat = owner.GetComponent<StatComponent>();
    }

    public void Release(FireBall fireBall)
    {
        fireBallPool.Release(fireBall);
    }
}