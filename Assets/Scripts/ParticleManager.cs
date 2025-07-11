using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

//RaisEvent사용
/*
 * 효율적으로 사용이 가능하다는 특징이 있으면서
 * photon안에서 사용할수 있는 같은 기능이다. 다른 구조랑 해치치 않으면서 장점이 있어서 사용
 */

//EventType enum 설정

//파티클 타입 Enum
public enum JobParticle
{
    WarriorBasicAttack, WarriorSkill,
    MageBasicAttack, MageSkill,
    ArcherBasicAttack, ArcherSkill,
    BossBasicAttack, BossSkill,
}

//RaiseEvent를 사용하기 위해서는 Code가 반드시 필요하기 때문에(그 코드는 byte로 되어야 하며) enum처리함
// 현재 사용하고 있는건(100이라는 숫자) 아무의미는 없지만, 그래도 추후 사용할때 저장이 필요할것 같아 표기함.
public enum PhotonEventCode : byte
{
    ParticlePlay = 100
}

public class ParticleManager : MonoBehaviour, IOnEventCallback
{
    public static ParticleManager instance;

    [System.Serializable] //Dictionaly를 캡슐화 하기 위해 사용됨
    public struct ParticleData
    {
        public JobParticle type; // 파티클이벤트의 종류
        public ParticleSystem particlePrefab; //파티클 프리팹
    }

    [Header("파티클 프리팹 연결")]
    public List<ParticleData> paricleList; // 이 리스트를 인스펙터에서 채웁니다.
    private Dictionary<JobParticle, ParticleSystem> particleDict = new(); // 런타임에 사용될 딕셔너리

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        // Dictionary 초기화: 인스펙터에서 설정한 paricleList를 기반으로 딕셔너리를 채웁니다.
        foreach (var data in paricleList)
        {
            particleDict[data.type] = data.particlePrefab; // 프리팹 자체를 저장합니다.
        }
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);      // 활성화일때 photon 이벤트 수신자로 등록함
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);   //비활성화일때 수신대상에서 제거
    }
    //OnEnable, OnDisable모두 RaiseEvent를 사용하기 위해서 사용

    public ParticleSystem GetParticleSystem(JobParticle type)
    {
        return particleDict[type];
    }

    public void PlayParticle(JobParticle type, Vector3 tramsform, Quaternion rotation)
    {
        object[] data = new object[] {(int)type, tramsform, rotation};
        PhotonNetwork.RaiseEvent(
            (byte)PhotonEventCode.ParticlePlay,
            data,                                                    //파티클 타입이나 위치 정보를 보낸다
            new RaiseEventOptions { Receivers = ReceiverGroup.All }, //모두에게 보내기 위해서 사용됨
            SendOptions.SendUnreliable                               //전송방식이라고 한다. 전송이 빠른걸 우선시 하며, 약간의 손실은 발생하지만 파티클에서는 쓸만하다고 한다.
            );
        
    }

    public void OnEvent(EventData photonEvent)
    {
        if(photonEvent.Code == (byte)PhotonEventCode.ParticlePlay)
        {
            object[] data = (object[])photonEvent.CustomData;
            JobParticle type = (JobParticle)(int)data[0];

            Vector3 position = (Vector3)(data[1]);
            Quaternion rotation = (Quaternion)(data[2]);

            if(particleDict.TryGetValue(type, out ParticleSystem ps)) //paricleSystem약자임. 
            {
                ParticleSystem instance = Instantiate(ps, position, rotation);
                ps.Play();

                Destroy(instance.gameObject, instance.main.duration + instance.main.startLifetime.constantMax);
            }
        }
    }
    


}
