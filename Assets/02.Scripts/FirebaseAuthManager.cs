using System;
using System.Threading.Tasks;
using Firebase.Auth;
using UnityEngine;

public class FirebaseAuthManager
{
    private static FirebaseAuthManager instance = null;
    public static FirebaseAuthManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new FirebaseAuthManager();
            }
            return instance;
        }
    }

    private FirebaseAuth auth;
    private FirebaseUser user;
    public string UserId => user?.UserId;
    public Action<bool> LoginState;
    private bool isNewUser = false;

    public void Init()
    {
        auth = FirebaseAuth.DefaultInstance;
        if (auth.CurrentUser != null)
            LogOut();
        auth.StateChanged += AuthStateChanged;
    }

    private void AuthStateChanged(object sender, EventArgs e)
    {
        if (auth.CurrentUser != user)
        {
            bool signed = (auth.CurrentUser != null);
            if (!signed && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
                LoginState?.Invoke(false);
            }
            user = auth.CurrentUser;
            if (signed && !isNewUser)
            {
                Debug.Log("Signed in " + user.UserId);
                LoginState?.Invoke(true);
            }
        }
    }

    public async Task<bool> Create(string email, string password)
    {
        try
        {
            isNewUser = true;
            AuthResult result = await auth.CreateUserWithEmailAndPasswordAsync(email, password);
            FirebaseUser newUser = result.User;
            Debug.Log($"Account created: {newUser.DisplayName} ({newUser.UserId})");
            LoginState?.Invoke(true);
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Sign up failed: {e.Message}");
            return false;
        }
        finally
        {
            isNewUser = false;
        }
    }

    public async Task<bool> LogIn(string email, string password)
    {
        try
        {
            AuthResult result = await auth.SignInWithEmailAndPasswordAsync(email, password);
            FirebaseUser newUser = result.User;
            Debug.Log($"Login successful: {newUser.DisplayName} ({newUser.UserId})");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Login failed: {e.Message}");
            return false;
        }
    }

    public void LogOut()
    {
        auth.SignOut();
        Debug.Log("·Î±×¾Æ¿ô");
    }
}