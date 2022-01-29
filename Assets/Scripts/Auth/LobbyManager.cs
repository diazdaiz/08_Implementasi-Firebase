using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

public class LobbyManager : MonoBehaviour {
    public static LobbyManager Instance;

    [Header("UI")]
    [SerializeField] GameObject profileUI;
    [SerializeField] GameObject changePPUI;
    [SerializeField] GameObject actionSuccessPanelUI;

    [Header("Info")]
    [SerializeField] TMP_Text usernameText;
    [SerializeField] TMP_Text emailText;

    [Header("PP")]
    [SerializeField] Image profilePicture;
    [SerializeField] TMP_InputField profilePictureLink;
    [SerializeField] TMP_Text outputText;

    [Header("Action Success Panel")]
    private TMP_Text actionSuccessText;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else if (Instance != null) {
            Destroy(gameObject);
        }
    }

    private void Start() {
        LoadProfile();
    }

    private void LoadProfile() {
        if(FirebaseManager.Instance.user != null) {
            Uri.photoUrl = FirebaseManager.Instance.user.PhotoUrl;
            string name = FirebaseManager.Instance.user.DisplayName;
            string email = FirebaseManager.Instance.user.DisplayEmail;
            StartCoroutine(LoadImage(photoUrl.ToString()));
            usernameText.text = name;
            emailText.text = email;
        }
    }

    private IEnumerator LoadImage(string photoUrl) {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(photoUrl);
        yield return request.SendWebRequest();
        if(request != null) {
            string output = "Error, coba lagi";
            if (request.isHttpError) {
                output = "Link salah atau image tidak di support!";
            }
            Output(output);
        }
        else {
            Texture2D image = ((DownloadHandlerTexture)request.downloadHandler).texture;
            profilePicture.sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), Vector2.zero);
        }
    }

    private void Output(string output) {
        outputText.text = output;
    }

    private void ClearUI() {
        profileUI.SetActive(false);
        changePPUI.SetActive(false);
        actionSuccessPanelUI.SetActive(false);
    }

    public void ProfileUI() {
        ClearUI();
        profileUI.SetActive(true);
        LoadProfile();
    }

    public void ChangePPUI() {
        ClearUI();
        changePPUI.SetActive(true);
    }

    public void ChangePPSuccess() {
        ClearUI();
        actionSuccessPanelUI.SetActive(true);
        actionSuccessText.text = "Foto profil berhasil diganti";
    }

    public void SubmitProfileImageButton() {
        FirebaseManager.Instance.UpdateProfilePicture(profilePictureLink.text);
    }

    public void SignOut() {

    }
}
