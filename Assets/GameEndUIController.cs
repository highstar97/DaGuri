using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class GameEndUIController : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI titleText;

   

    // UI를 플레이어 카메라 앞 fixedPosition 거리만큼 띄움
    public Vector3 offsetFromCamera = new Vector3(0f, 0f, 2f);
    public static GameEndUIController Instance;

    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // 필요하다면 씬 전환 시 파괴되지 않도록 설정
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // 이미 인스턴스가 존재하면 현재 오브젝트 파괴
        }
    }

    void LateUpdate()
    {
        // UI 패널이 활성화되어 있고, 메인 카메라가 존재하는 경우에만 처리
        if (panel.activeSelf && Camera.main != null)
        {
            // UI 패널의 위치를 카메라 앞에 설정합니다.
            // 카메라의 현재 위치에서 카메라의 정면 방향(forward)으로 offsetFromCamera.z 만큼 이동합니다.
            panel.transform.position = Camera.main.transform.position + Camera.main.transform.forward * offsetFromCamera.z;

            // UI 패널이 카메라를 바라보도록 회전합니다.
            panel.transform.LookAt(Camera.main.transform);

            // panel.transform.Rotate(0, 180, 0);
        }
    }



    public void ShowResult(bool isWin)
    {
        panel.SetActive(true);

        if (isWin)
        {
            titleText.text = " 승리!";
        }
        else
        {
            titleText.text = " 패배!";
        }

       
    }
    // 카메라 앞에 UI 위치 및 회전 조정
    

    public void OnClickExit()
    {
        PhotonNetwork.LeaveRoom(); // 이후 OnLeftRoom에서 씬 이동
    }

    public void OnClickRetry()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(SceneManager.GetActiveScene().name);
        }
    }
}