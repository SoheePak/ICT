using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class LogInSystem : MonoBehaviour
{
    [Header("Input")]
    public InputField email;
    public InputField password;
    [Header("ErrorMessage")]
    public Text errorMessageText;
    public GameObject errorMessage;
    [Header("Scene")]
    public int nextScene;
    public LoadScene loadScene;
    private bool isNewUser = false;     // 신규 유저인지
    private bool isLoggingIn = false;   // 로그인 중인지

    #region Messages
    private const string ERROR_EMPTY_FIELDS = "이메일과 비밀번호를 입력해주세요.";
    private const string ERROR_INVALID_EMAIL = "유효한 이메일 주소를 입력해주세요.";
    private const string ERROR_SIGNUP_FAIL = "회원가입에 실패했습니다. 다시 시도해주세요.";
    private const string ERROR_LOGIN_FAIL = "로그인에 실패했습니다. 다시 시도해주세요.";
    private const string MESSAGE_SIGNUP_SUCCESS = "회원가입 성공! 다시 로그인해주세요.";
    #endregion

    void Start()
    {
        FirebaseAuthManager.Instance.LoginState += OnChangedState;
        FirebaseAuthManager.Instance.Init();
        password.contentType = InputField.ContentType.Password;
        errorMessage.SetActive(false);
    }

    private void OnChangedState(bool sign)
    {
        if (sign)
        {
            if (isNewUser)
            {
                // 회원가입을 하면 자동으로 로그인되므로 강제 로그아웃
                ShowMessage(MESSAGE_SIGNUP_SUCCESS, false);
                LogOut();
                isNewUser = false;
            }
            else if (isLoggingIn)
            {
                DBManager.Instance.userUid = FirebaseAuthManager.Instance.UserId;
                DBManager.Instance.InitAsync();
                loadScene.FadeOutNextScene(nextScene);
            }
        }
        // 입력 창을 비움
        ClearInputFields();
    }

    public void CreaetBtn()
    {
        Create();
    }
    public async Task Create()
    {
        if (!ValidateInput()) return;

        try
        {
            isNewUser = true;
            bool success = await FirebaseAuthManager.Instance.Create(email.text, password.text);
            if (success)
            {
                ShowMessage("Sign up successful! Please log in again.", false);
                LogOut();
            }
            else
            {
                ShowMessage("Sign up failed. Please try again.", true);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error during sign up: {e.Message}");
            ShowMessage("An error occurred. Please try again.", true);
        }
        finally
        {
            isNewUser = false;
        }
    }

    public void LogInBtn()
    {
        LogIn();
    }

    public async Task LogIn()
    {
        if (!ValidateInput()) return;

        try
        {
            isLoggingIn = true;
            bool success = await FirebaseAuthManager.Instance.LogIn(email.text, password.text);
            if (!success)
            {
                ShowMessage("Login failed. Please check your credentials and try again.", true);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error during login: {e.Message}");
            ShowMessage("An error occurred. Please try again.", true);
        }
        finally
        {
            isLoggingIn = false;
        }
    }

    private bool ValidateInput()
    {
        if (string.IsNullOrEmpty(email.text) || string.IsNullOrEmpty(password.text))
        {
            ShowMessage(ERROR_EMPTY_FIELDS, true);
            return false;
        }
        if (!IsValidEmail(email.text))
        {
            ShowMessage(ERROR_INVALID_EMAIL, true);
            return false;
        }
        return true;
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private void ShowMessage(string message, bool isError)
    {
        if (isError)
        {
            AudioManager.Instance.PlayOneShot("Error");
        }
        errorMessageText.text = message;
        errorMessage.SetActive(true);
    }

    public void LogOut()
    {
        FirebaseAuthManager.Instance.LogOut();
    }

    private void ClearInputFields()
    {
        email.text = string.Empty;
        password.text = string.Empty;
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void SoundPlay(string name)
    {
        AudioManager.Instance.PlayOneShot(name);
    }
}