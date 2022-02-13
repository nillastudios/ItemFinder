using UnityEngine;

namespace NillaStudios
{
    public class DragObject : MonoBehaviour
    {
        public float forceAmount = 500;
        public LayerMask detectLayer;
        public Vector3 offset = new Vector3(0f, 1.5f, 0f);
        public float maxHeight = 2f;

        private Rigidbody selectedRigidbody;
        private Camera targetCamera;
        private Vector3 originalScreenTargetPosition;
        private Vector3 originalRigidbodyPos;
        private float selectionDistance;

        // Start is called before the first frame update
        void Start()
        {
            targetCamera = GetComponent<Camera>();
        }

        void Update()
        {
            if (!targetCamera)
                return;

            if (Input.GetMouseButtonDown(0))
            {
                //Check if we are hovering over Rigidbody, if so, select it
                selectedRigidbody = GetRigidbodyFromMouseClick();
            }
            if (Input.GetMouseButtonUp(0) && selectedRigidbody)
            {
                //Release selected Rigidbody if there any
                selectedRigidbody = null;
            }
        }

        void FixedUpdate()
        {
            // If we have a rigidbody then move it
            if (selectedRigidbody)
            {
                // calculatye mouse movement
                Vector3 mousePositionOffset = (selectedRigidbody.position.y < maxHeight ? offset : Vector3.zero) 
                                            + targetCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, selectionDistance)) - originalScreenTargetPosition;
                
                // Assign velocity to rigidbody towards calculated mouse position
                selectedRigidbody.velocity = (originalRigidbodyPos + mousePositionOffset - selectedRigidbody.transform.position) * forceAmount * Time.deltaTime;
            }
        }

        Rigidbody GetRigidbodyFromMouseClick()
        {
            RaycastHit hitInfo = new RaycastHit();
            Ray ray = targetCamera.ScreenPointToRay(Input.mousePosition);
            bool hit = Physics.Raycast(ray, out hitInfo, 100f, detectLayer);
            if (hit)
            {
                if (hitInfo.collider.gameObject.GetComponent<Rigidbody>())
                {
                    selectionDistance = Vector3.Distance(ray.origin, hitInfo.point);
                    originalScreenTargetPosition = targetCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, selectionDistance));
                    originalRigidbodyPos = hitInfo.collider.transform.position;
                    return hitInfo.collider.gameObject.GetComponent<Rigidbody>();
                }
            }

            return null;
        }

        public void ClearSelectedRigidbody()
        {
            selectedRigidbody = null;
        }
    }
}
