using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CameraThrough : MonoBehaviour
{
    private WebCamTexture webCamTexture;
    public RawImage imgCam;

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
    }


    public void AutoFocus()
    {

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

        //flip preview on iOS
#if UNITY_IOS
        imgCam.gameObject.GetComponent<RectTransform>().localScale=new Vector3(1,-1,1);
        Debug.Log ("ios - flipping camera preview");
#endif
    }
}