using UnityEngine;
using Photon.Pun;

public class SwordTransformSync : MonoBehaviourPun, IPunObservable
{
    private Vector3 networkPosition;
    private Quaternion networkRotation;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) // 내가 이 오브젝트 소유자면 → 내 위치 전송
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else // 다른 사람이라면 → 그 위치 받아서 저장
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
        }
    }

    void Update()
    {
        if (!photonView.IsMine) // 내 오브젝트가 아니면 보간해서 따라감
        {
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * 10);
            transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.deltaTime * 10);
        }
    }
}