using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Color originalColor;
    private Image image;

    void Awake()
    {
        image = GetComponent<Image>();
        if (image != null)
            originalColor = image.color;
    }

    // 컨트롤러 레이 포인터가 버튼에 진입했을 때 호출
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (image != null)
            image.color = Color.yellow;  // 예: 노란색으로 강조 표시
    }

    // 컨트롤러 레이 포인터가 버튼에서 벗어났을 때 호출
    public void OnPointerExit(PointerEventData eventData)
    {
        if (image != null)
            image.color = originalColor;  // 원래 색으로 복원
    }
}
