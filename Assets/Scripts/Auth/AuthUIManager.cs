using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AuthUIManager : MonoBehaviour {
    public static AuthUIManager Instance;

    [SerializeField] GameObject checkingForAccount;
    [SerializeField] GameObject loginUI;
    [SerializeField] GameObject registerUI;
    [SerializeField] GameObject verifyEmailUI;

    private TMP_Text verifyEmailText;
    private void Awake() {
        if(Instance == null) {
            Instance = this;
        }
        else if(Instance != null) {
            Destroy(gameObject);
        }
    }

    private void ClearUI() {
        checkingForAccount.SetActive(false);
        loginUI.SetActive(false);
        registerUI.SetActive(false);
        verifyEmailUI.SetActive(false);
        //FirebaseManager.Instance.ClearOutputs();
    }

    public void LoginScreen() {
        ClearUI();
        loginUI.SetActive(true);
    }

    public void RegisterScreen() {
        ClearUI();
        registerUI.SetActive(true);
    }

    public void AwaitVerification(bool emailSent, string email, string output) {
        ClearUI();
        if (emailSent) {

        }
    }
}
