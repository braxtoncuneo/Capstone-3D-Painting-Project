using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FollowCameraObj : MonoBehaviour
{
    public float Objx;
    public float Objy;
    public float Objz;
    public bool selected = false;

    public float OffsetVertRotation;
    MeshRenderer m_Renderer;
    Color m_SelectedColor;
    Color m_UnselectedColor;
    Color color;
    Material m_SelectedMaterial;

    public Vector3 targetPos;
    // Update is called once per frame
    public void Start()
    {
        targetPos = new Vector3(Objx, Objy, Objz);
        m_Renderer = this.GetComponent<MeshRenderer>();
        m_SelectedColor = new Color(1f, 1f, 1f, 1f);
        m_UnselectedColor = new Color(1f, 1f, 1f, 0.2f);
    }

    private void LateUpdate()
    {
        /*
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        pos.x = Mathf.Clamp01(pos.x);
        pos.y = Mathf.Clamp01(pos.y);
        transform.position = Camera.main.ViewportToWorldPoint(pos);
        */
        transform.localPosition = Camera.main.ViewportToWorldPoint(targetPos);
        transform.localRotation = Camera.main.transform.rotation;
        Vector3 rotV = new Vector3(0, OffsetVertRotation, 0);
        transform.Rotate(rotV);

        if (selected)
        {
            //m_Renderer.material.color = m_SelectedColor;
            this.GetComponent<MeshRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 1f); ;
        }
        else
        {
            this.GetComponent<MeshRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.5f); ;
        }
    }

    public void SetHeight(float inp)
    {
        Objy = inp;
        targetPos = new Vector3(Objx, Objy, Objz);
    }

    public void SetHorizontal(float inp)
    {
        Objx = inp;
        targetPos = new Vector3(Objx, Objy, Objz);
    }

    public void SetDepth(float inp)
    {
        Objz = inp;
        targetPos = new Vector3(Objx, Objy, Objz);
    }

    public void SetSelected(bool inp)
    {
        selected = inp;
    }
}
