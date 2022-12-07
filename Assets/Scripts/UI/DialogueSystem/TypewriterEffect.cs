using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TypewriterEffect : MonoBehaviour
{
    public float typewriterSpeed = 50f;

    private Coroutine typewriterCoroutine = null;

    public Coroutine Run(string textToType, TMP_Text textLabel)
    {
        if (typewriterCoroutine == null)
        {
            typewriterCoroutine = StartCoroutine(TypeText(textToType, textLabel));
        }
        else
        {
            ////debug.Log("Parando la corutina del texto anterior a: " + dialogueArray);
            StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = StartCoroutine(TypeText(textToType, textLabel));
        }

        return typewriterCoroutine;
    }

    private IEnumerator TypeText(string textToType, TMP_Text textLabel)
    {
        float t = 0;
        int charIndex = 0;
        //debug.Log("Texto: " + textToType);
        while (charIndex < textToType.Length)
        {
            //debug.Log("caracter pintado ");
            t += Time.deltaTime * typewriterSpeed;
            charIndex = Mathf.FloorToInt(t);
            charIndex = Mathf.Clamp(charIndex, 0, textToType.Length);

            textLabel.text = textToType.Substring(0, charIndex);

            yield return null;
        }

        textLabel.text = textToType;
    }
}
