namespace VRTK.Examples.Archery
{
    using UnityEngine;

    public class Follow : MonoBehaviour
    {
        public bool followPosition;
        public bool followRotation;
        public Transform target;

        private void Update()
        {
            if (target != null)
            {
                if (followRotation)
                {
                    //transform.rotation = target.rotation;
                    //transform.rotation.SetEulerRotation(-1.0f, 0.0f, 0.0f);
                }

                if (followPosition)
                {
                    transform.position = target.position;
                }
            }
            else
            {
                Debug.LogError("No follow target defined!");
            }
        }
    }
}