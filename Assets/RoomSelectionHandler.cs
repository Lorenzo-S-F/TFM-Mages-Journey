using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomSelectionHandler : MonoBehaviour
{
    [SerializeField]
    private List<Image> m_FadeImages;
    [SerializeField]
    private Image m_RoomTypeImage;
    [SerializeField]
    private TMPro.TextMeshProUGUI m_TextDescription;
    [SerializeField]
    private Button m_ButtonDescription;

    public IEnumerator Initialize(int roomSelection, Sprite image, string text)
    {
        m_RoomTypeImage.sprite = image;
        m_TextDescription.text = string.Empty;

        float t = 0;
        while (t < 1)
        {
            foreach (var img in m_FadeImages)
                img.color = new Color(img.color.r, img.color.g, img.color.b, t);

            t += Time.deltaTime * 2;
            yield return null;
        }

        foreach (var img in m_FadeImages)
            img.color = new Color(img.color.r, img.color.g, img.color.b, 1);
        m_RoomTypeImage.color = new Color(m_RoomTypeImage.color.r, m_RoomTypeImage.color.g, m_RoomTypeImage.color.b, 1);

        m_ButtonDescription.onClick.RemoveAllListeners();
        m_ButtonDescription.onClick.AddListener(() => GameplayManagers.Instance.LoadNextRoomCall(roomSelection));
        m_TextDescription.text = text;
    }
}
