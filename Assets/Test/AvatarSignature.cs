using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class AvatarSignature : MonoBehaviour
{
    public TMP_Text NickText;

    public void SetName(string nick)
    {
        NickText.text = nick;

        NickText.transform.rotation =
            Quaternion.Euler(
                0,
                0,
                Random.Range(-8f, 8f));

        NickText.alpha =
            0.85f;
    }
}