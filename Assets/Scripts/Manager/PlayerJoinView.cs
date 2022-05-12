using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class PlayerJoinView : MonoBehaviour
{
    public GameObject firstSelected = null;
    public GameObject playerRoot = null;
    public Image backgroundImage = null;
    public GameObject characterSelectPanel = null;
    public TMPro.TextMeshProUGUI joinText = null;
    public TMPro.TextMeshProUGUI continueText = null;
    public UnityEvent OnJoinEvent = null;
}
