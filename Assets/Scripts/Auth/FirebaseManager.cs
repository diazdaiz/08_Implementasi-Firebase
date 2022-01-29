using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Firebase;
using Firebase.Auth;
using UnityEngine.SceneManagement;
using System;

public class FirebaseManager : MonoBehaviour {
    public static FirebaseManager Instance;

    [Header("Firebase")]
    [SerializeField] FirebaseAuth auth;
    [SerializeField] public FirebaseUser user;

    [Header("Login")]
    [SerializeField] TMP_InputField loginEmail;
    [SerializeField] TMP_InputField loginPassword;
    [SerializeField] TMP_Text loginOutputText;

    [Header("Register")]
    [SerializeField] TMP_InputField registerUsername;
    [SerializeField] TMP_InputField registerEmail;
    [SerializeField] TMP_InputField registerPassword;
    [SerializeField] TMP_InputField registerConfirmPassword;
    [SerializeField] TMP_Text registerOutputText;

    private void Awake() {
        DontDestroyOnLoad(gameObject);
        if (Instance == null) {
            Instance = this;
        }
        else if (Instance != null) {
            Destroy(gameObject);
            Instance = this;
        }
    }

    private void Start() {
        StartCoroutine(CheckAndFixDependencies());
    }

    private IEnumerator CheckAndFixDependencies() {
        var checkAndFixDependencyTask = FirebaseApp.CheckAndFixDependenciesAsync();
        yield return new WaitUntil(predicate: () => checkAndFixDependencyTask.IsCompleted);

        var dependecyStatus = checkAndFixDependencyTask.Result;
        if(dependecyStatus == DependencyStatus.Available) {
            InitializeFirebase();
        }
        else {
            Debug.LogError($"Firebase not resolve all dependencies : {DependencyStatus}");
        }
    }

    private void InitializeFirebase() {
        auth = FirebaseAuth.DefaultInstance;
        StartCoroutine(CheckAutoLogin());

        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    private IEnumerator CheckAutoLogin() {
        yield return new WaitForEndOfFrame();
        if(user!= null) {
            var reloadUserTask = user.ReloadAsync();
            yield return new WaitUntil(predicate: () => reloadUserTask.IsCompleted);
            AutoLogin();
        }
        else {
            AuthUIManager.Instance.LoginScreen();
        }
    }

    private void AutoLogin() {
        if(user != null) {
            if (user.IsEmailVerified) {
                SceneManager.LoadScene(1);
            }
            else {
                StartCoroutine(SendVerificationEmail());
            }
        }
        else {
            AuthUIManager.Instance.LoginScreen();
        }
    }

    private void AuthStateChanged(object sender, EventArgs e) {
        if(auth.CurrentUser != user) {
            bool SignedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if(!SignedIn && user != null) {
                Debug.Log("Signed out");
            }

            user = auth.CurrentUser;
            if (SignedIn) {
                Debug.Log($"Signed In: {user.DisplayName}");
            }
        }
    }

    public void ClearOutputs() {
        loginOutputText.text = "";
        registerOutputText.text = "";
    }

    public void Logout() {
        auth.SignOut();
        user.DeleteAsync();
        Application.Quit();
    }

    private IEnumerator SendVerificationEmail() {
        if(user != null) {
            var emailTask = user.SendEmailVerificationAsync();
            yield return new WaitUntil(predicate: () => emailTask.IsCompleted);

            if (emailTask.Exception != null) {
                user.DeleteAsync();
                FirebaseException firebaseException = (FirebaseException)emailTask.Exception.GetBaseException();
                AuthError error = (AuthError)firebaseException.ErrorCode;
                string output = "Unknown error, mohon coba lagi";

                switch (error) {
                    case AuthError.Cancelled:
                        output = "Verifikasi dibatalkan";
                        break;
                    case AuthError.InvalidRecipientEmail:
                        output = "Email invalid";
                        break;
                    case AuthError.TooManyRequests:
                        output = "Request terlalu banyak";
                        break;
                }
                AuthUIManager.Instance.AwaitVerification(false, user.Email, output);
            }
            else {
                AuthUIManager.Instance.AwaitVerification(true, user.Email, null);
                Debug.Log("Email berhasil dikirim");
            }
        }
    }

    public void LoginButton() {
        StartCoroutine(LoginLogic(loginEmail.text, loginPassword.text));
    }

    public void RegisterButton() {
        StartCoroutine(RegisterLogic(registerUsername.text, registerEmail.text, registerPassword.text, registerConfirmPassword.text));
    }

    private IEnumerator LoginLogic(string email, string password) {
        Credential credential = EmailAuthProvider.GetCredential(email, password);

        var loginTask = auth.SignInAndRetrieveDataWithCredentialAsync(credential);
        //yield return new WaitUntil

        //if(logintask)
    }


    private string RegisterLogic(string username, string email, string password, string confirmPassword) {
        if(username == "") {
            registerOutputText.text = "Mohon masukkan username";
        }
        else if (password == "") {
            registerOutputText.text = "Mohon masukkan password";
        }
        else if (password != confirmPassword) {
            registerOutputText.text = "Konfirmasi password tidak sesuai";
        }
        else {
            var registerTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);
            yield return new WaitUntil
        }
    }
}
