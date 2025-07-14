using Photon.Pun;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

public class GameEndUIController : MonoBehaviourPunCallbacks
{
    public GameObject panel;
    public TextMeshProUGUI titleText;

   

    // UI를 플레이어 카메라 앞 fixedPosition 거리만큼 띄움
    public Vector3 offsetFromCamera = new Vector3(0f, 0f, 2f);
    public static GameEndUIController Instance;


    [SerializeField] 
    private XRRayInteractor rightRay;

    private void Start()
    {
        if (!photonView.IsMine)
        {
            gameObject.SetActive(false);
            return;
        }

        ShowResult(false); // 테스트용s

    }

    void SetUIInteractionMode(bool enabled)
    {
        // UI용 레이 인터랙션 활성화
        rightRay.gameObject.SetActive(enabled);

    }
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
    //    panel.SetActive(false);
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

            panel.transform.Rotate(0, 180, 0);
        }
    }



    public void ShowResult(bool isWin)
    {
        panel.SetActive(true);

        if (isWin)
        {
            titleText.text = " Win! ";
        }
        else
        {
            titleText.text = " Lose!";
        }

        SetUIInteractionMode(true); // UI 상호작용 모드로 전환

    }


    public void OnClickExit()
    {
        Debug.Log("OnClickExit() 함수 호출됨! 포톤 룸을 나가려고 시도합니다.");
        PhotonNetwork.LeaveRoom(); // 이후 OnLeftRoom에서 씬 이동
        Application.Quit();
    }

    public void OnClickRetry()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(SceneManager.GetActiveScene().name);
        }
    }
}