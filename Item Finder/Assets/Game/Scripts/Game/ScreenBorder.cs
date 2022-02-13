using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NillaStudios
{
    // Makes screen as border by adding box colliders
    public class ScreenBorder : MonoBehaviour
    {
        public GameObject topRightBorder;
        public GameObject bottomLeftBorder;
        private Camera mainCam;

        private void Awake()
        {
            mainCam = Camera.main;
        }

        private void Start()
        {
            Vector3 placePoint;

            // Place Top Right border
            placePoint = mainCam.ScreenToWorldPoint(new Vector3(0f, 0f, mainCam.nearClipPlane));
            bottomLeftBorder.transform.position = placePoint;

            // Place bottom left border
            placePoint = mainCam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCam.nearClipPlane));
            topRightBorder.transform.position = placePoint;
        }
    }
}
