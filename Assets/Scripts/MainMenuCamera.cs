using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
    [SerializeField] private float maxSize = 350.0f;
    [SerializeField] private float zoomInSpeed = 50.0f;
    [SerializeField] private float zoomOutSpeed = 10.0f;
    [SerializeField] private float timeToZoomOut = 10.0f;

    private Camera  mainCamera;
    private float   defaultSize;
    private float   timeOnClick;

    void Start()
    {
        mainCamera = GetComponent<Camera>();
        defaultSize = mainCamera.orthographicSize;
        mainCamera.orthographicSize = maxSize;

        timeOnClick = -timeToZoomOut;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            timeOnClick = Time.time;
        }

        float elapsedTime = Time.time - timeOnClick;

        if (elapsedTime < timeToZoomOut)
        {
            mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize - zoomInSpeed * Time.deltaTime, defaultSize, maxSize);
        }
        else
        {
            mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize + zoomOutSpeed * Time.deltaTime, defaultSize, maxSize);
        }
    }
}
