using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;



public class AppManager : MonoBehaviour
{

    private WebCamTexture webCamTexture;
    public RawImage imgCam;
    public Texture imgCam2;
    public GameObject sButton;
    public GameObject backButton;
    public GameObject hashtagButton;
    public GameObject menuPanel;
    public GameObject muralesImage;
    public GameObject instructionZoom;
    public GameObject camPanel;
    public GameObject hashtagText;
    public GameObject confMess;
    public GameObject instagramPanel;

    private static AndroidJavaObject _activity;

    private const string MediaStoreImagesMediaClass = "android.provider.MediaStore$Images$Media";

    private string destination;

    public void StartCamera()
    {
        menuPanel.SetActive(false);
        StartCoroutine(ActivateCamera());
    }


    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (!menuPanel.activeInHierarchy)
            {
                BackButton();
            } else
            {
                Application.Quit();
            }
        }
    }


    public IEnumerator ActivateCamera()
    {
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam | UserAuthorization.Microphone);
        if (Application.HasUserAuthorization(UserAuthorization.WebCam | UserAuthorization.Microphone))
        {
            Debug.Log("Start");
            Show(true);
        }
        else
        {
            Debug.Log("None");
        }
        yield return new WaitForSeconds(1f);
        muralesImage.SetActive(true);
    }


    public void BackButton()
    {
        menuPanel.SetActive(true);
        muralesImage.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        muralesImage.SetActive(false);
        Show(false);
    }


    public void Show(bool bShow)
    {
        //gameObject.SetActive(bShow);

        if (webCamTexture == null)
            InitCam();

        if (bShow)
            webCamTexture.Play();
        else
            webCamTexture.Stop();
    }


    void InitCam()
    {
        webCamTexture = new WebCamTexture();

        Debug.Log("Camera devices:");

        WebCamDevice[] devices = WebCamTexture.devices;

        int i = 0;
        while (i < devices.Length)
        {
            Debug.Log(devices[i].name);
            i++;
        }

        imgCam.texture = webCamTexture;
        //imgCam2 = webCamTexture;
        //camPanel.GetComponent<Renderer>().material.mainTexture = imgCam2;

#if UNITY_IOS
        imgCam.gameObject.GetComponent<RectTransform>().localScale=new Vector3(1,-1,1);
        Debug.Log ("ios - flipping camera preview");
#endif
    }


    public void TakeScreenshot()
    {
        CopyHashTag();
        StartCoroutine(ShareScreenshot());
    }


    public IEnumerator ShareScreenshot()
    {
        this.gameObject.GetComponent<AudioSource>().Play();
        Debug.Log("Screenshot Call");
        sButton.SetActive(false);
        backButton.SetActive(false);
        //hashtagButton.SetActive(false);
        instagramPanel.SetActive(false);
        hashtagText.SetActive(false);
        instructionZoom.SetActive(false);
        
        // wait for graphics to render
        yield return new WaitForEndOfFrame();

        // prepare texture with Screen and save it
        Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, true);
        texture.ReadPixels(new Rect(0f, 0f, Screen.width, Screen.height), 0, 0);
        texture.Apply();

        // save to persistentDataPath File
        byte[] data = texture.EncodeToPNG();

        destination = Path.Combine(Application.persistentDataPath,
                                          System.DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + ".png");
#if !UNITY_EDITOR && UNITY_ANDROID
        //destination = Path.Combine("/mnt/sdcard/DCIM/Images/", System.DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + ".png");
        SaveImageToGallery(texture, System.DateTime.Now.ToString("yyyy-MM-dd-HHmmss"), ".png");
#endif
        File.WriteAllBytes(destination, data);

        sButton.SetActive(true);
        backButton.SetActive(true);
        //hashtagButton.SetActive(true);
        instagramPanel.SetActive(true);
        hashtagText.SetActive(true);
        instructionZoom.SetActive(true);

        if (!Application.isEditor)
        {
#if UNITY_ANDROID
            //string body = "#bloop #biokip #mural #ino #uncensored #ibiza #festival";
            //string subject = "Subject of message";

            AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
            AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
            intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
            AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
            AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "file://" + destination);
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);
            //intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), body);
            //intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), subject);
            intentObject.Call<AndroidJavaObject>("setType", "image/jpeg");
            AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");

            // run intent from the current Activity
            currentActivity.Call("startActivity", intentObject);
#endif
        }
    }


    public static string SaveImageToGallery(Texture2D texture2D, string title, string description)
    {
        using (var mediaClass = new AndroidJavaClass(MediaStoreImagesMediaClass))
        {
            using (var cr = Activity.Call<AndroidJavaObject>("getContentResolver"))
            {
                var image = Texture2DToAndroidBitmap(texture2D);
                var imageUrl = mediaClass.CallStatic<string>("insertImage", cr, image, title, description);
                return imageUrl;
            }
        }
    }


    public static AndroidJavaObject Texture2DToAndroidBitmap(Texture2D texture2D)
    {
        byte[] encoded = texture2D.EncodeToPNG();
        using (var bf = new AndroidJavaClass("android.graphics.BitmapFactory"))
        {
            return bf.CallStatic<AndroidJavaObject>("decodeByteArray", encoded, 0, encoded.Length);
        }
    }


    public static AndroidJavaObject Activity
    {
        get
        {
            if (_activity == null)
            {
                var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                _activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            }
            return _activity;
        }
    }


    public void BloopProgram()
    {
        Application.OpenURL("http://www.biokiplabs.com/bloopfestival2017/");
    }


    public void MuralStory()
    {
        Application.OpenURL("http://www.widewalls.ch/ino-mural-bloop-festival-2016/");
    }


    public void CopyHashTag()
    {
        UniPasteBoard.SetClipBoardString("#bloopfestival #uncensoredapp #ino #funkthepower #santantoni #eivissa #streetart #openairgallery");
        //confMess.SetActive(true);
        //StartCoroutine(DisableMessage());      
    }


    public IEnumerator DisableMessage()
    {
        yield return new WaitForSeconds(1f);
        confMess.SetActive(false);
    }


    public void FacebookLike()
    {

#if UNITY_ANDROID //&& !UNITY_EDITOR
        if (checkPackageAppIsPresent("com.facebook.katana"))
        {
            Application.OpenURL("fb://page/288570284546156"); //there is Facebook app installed so let's use it
        }
        else
        {
            Application.OpenURL("https://www.facebook.com/bloopfestival/"); // no Facebook app - use built-in web browser
        }
#endif

    }


#if UNITY_ANDROID //&& !UNITY_EDITOR
    private bool checkPackageAppIsPresent(string package)
    {
        AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject ca = up.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject packageManager = ca.Call<AndroidJavaObject>("getPackageManager");

        //take the list of all packages on the device
        AndroidJavaObject appList = packageManager.Call<AndroidJavaObject>("getInstalledPackages", 0);
        int num = appList.Call<int>("size");
        for (int i = 0; i < num; i++)
        {
            AndroidJavaObject appInfo = appList.Call<AndroidJavaObject>("get", i);
            string packageNew = appInfo.Get<string>("packageName");
            if (packageNew.CompareTo(package) == 0)
            {
                return true;
            }
        }
        return false;
    }
#endif


    public void ExitButton()
    {
        Application.Quit();
    }

}
