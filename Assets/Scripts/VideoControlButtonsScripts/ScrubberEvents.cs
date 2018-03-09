namespace GoogleVR.VideoDemo
{
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class ScrubberEvents : MonoBehaviour
    {
        public float lookTime = 3f;
        private bool isLooking = false;
        private float timer = 0f;
        public GameObject MagnetButton;

        private GameObject newPositionHandle;

        private Vector3[] corners;
        private Slider slider;

        private VideoControlsManager mgr;

        public VideoControlsManager ControlManager
        {
            set
            {
                mgr = value;
            }
        }

        void Start()
        {
            foreach (Image im in GetComponentsInChildren<Image>(true))
            {
                if (im.gameObject.name == "newPositionHandle")
                {
                    newPositionHandle = im.gameObject;
                    break;
                }
            }

            corners = new Vector3[4];
            GetComponent<Image>().rectTransform.GetWorldCorners(corners);
            slider = GetComponentInParent<Slider>();
        }

        void Update()
        {
            bool setPos = false;
            if (GvrPointerInputModule.Pointer != null)
            {
                RaycastResult r = GvrPointerInputModule.Pointer.CurrentRaycastResult;
                if (r.gameObject != null)
                {
                    newPositionHandle.transform.position = new Vector3(
                        r.worldPosition.x,
                        newPositionHandle.transform.position.y,
                        newPositionHandle.transform.position.z);
                    setPos = true;
                    //If Person is looking at slider and pressed the magnet button.
                    if (isLooking)
                    {
                        if (MagnetButton.GetComponent<MagnetSensor>().isPressedMagnetButton)
                            OnPointerClick(null);
                        timer += Time.deltaTime;
                        if (timer >= lookTime)
                        {
                            OnPointerClick(null);
                        }
                    }
                }
            }
            if (!setPos)
            {
                newPositionHandle.transform.position = slider.handleRect.transform.position;
            }
        }

        public void OnPointerEnter(BaseEventData data)
        {
            timer = 0f;
            isLooking = true;
            if (GvrPointerInputModule.Pointer != null)
            {
                RaycastResult r = GvrPointerInputModule.Pointer.CurrentRaycastResult;
                if (r.gameObject != null)
                {
                    newPositionHandle.transform.position = new Vector3(
                        r.worldPosition.x,
                        newPositionHandle.transform.position.y,
                        newPositionHandle.transform.position.z);
                }
            }
            newPositionHandle.SetActive(true);
        }

        public void OnPointerExit(BaseEventData data)
        {
            resetTheGaze();
            newPositionHandle.SetActive(false);
        }

        public void OnPointerClick(BaseEventData data)
        {
            timer = 0f;
            float minX = corners[0].x;
            float maxX = corners[3].x;

            float pct = (newPositionHandle.transform.position.x - minX) / (maxX - minX);

            if (mgr != null)
            {
                long p = (long)(slider.maxValue * pct);
                mgr.Player.CurrentPosition = p;
            }
        }

        private void resetTheGaze()
        {
            timer = 0f;
            isLooking = false;
        }
    }
}
