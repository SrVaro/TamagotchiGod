using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoopText : MonoBehaviour
{
    public float spaceBetweenLoop = 20;
    public TextMeshProUGUI textComponent;
    public float scrollSpeed;

    private RectTransform m_textRecttransform;

    private TextMeshProUGUI m_CloneTextObject;
    private RectTransform m_textRectTransform;


    // Start is called before the first frame update
    void Awake()
    {
        m_textRecttransform = textComponent.GetComponent<RectTransform>();
        m_textRecttransform.anchoredPosition = new Vector2(m_textRecttransform.anchoredPosition.x + textComponent.preferredWidth / 2, m_textRecttransform.anchoredPosition.y);
        m_CloneTextObject = Instantiate(textComponent) as TextMeshProUGUI;
        RectTransform cloneRectTransform = m_CloneTextObject.GetComponent<RectTransform>();
        cloneRectTransform.SetParent(m_textRecttransform);
        cloneRectTransform.anchoredPosition = new Vector2(m_textRecttransform.anchoredPosition.x + textComponent.preferredWidth + spaceBetweenLoop, m_textRecttransform.anchoredPosition.y);
        cloneRectTransform.anchorMin = m_textRecttransform.anchorMin;
        cloneRectTransform.localScale = m_textRecttransform.localScale;
    }

     IEnumerator Start()
    {
        float width = textComponent.preferredWidth;
        Vector3 startPosition = m_textRecttransform.anchoredPosition;

        float scrollPosition = startPosition.x;
        Debug.Log("Width" + width);

        while (true)
        {
            float remaider = scrollPosition % width;
            m_textRecttransform.anchoredPosition = new Vector3(-remaider, 0);

            scrollPosition += scrollSpeed * 20 * Time.deltaTime;

            yield return null;
        }

    }
}
