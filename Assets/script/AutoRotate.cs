using UnityEngine;
using System.Collections;

public class AutoRotate : MonoBehaviour
{
	public Vector3 rotation = new Vector3(0, -30, 0);
    public bool autoRotate = true;
	
	void Awake()
	{
        if (this.transform != null)
        {
            Shader.SetGlobalVector("_SGameShadowParams", new Vector4(-0.486f, -0.271f, 0.831f, 0.5f));
            //Shader.SetGlobalVector("_SGameShadowParams", new Vector4(-0.376f, -0.2997f, 0.8767f, 0.5f));

            Renderer smr = this.transform.gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
            if (smr != null)
            {
                BoxCollider c = smr.gameObject.AddComponent<BoxCollider>();
                c.size = new Vector3(2* c.size.x, 2*c.size.y, 2*c.size.z);
                //Material mat = smr.material;
                //mat.SetVector("_SGameShadowParams", new Vector4(0, 0, 0, 0));
                //Debug.LogFormat("set params {0}", 1);
            }
        }
	}
	
	void Update ()
	{
        if (autoRotate)
        {
            this.transform.Rotate(rotation * Time.deltaTime, Space.World);
        }
	}
}
