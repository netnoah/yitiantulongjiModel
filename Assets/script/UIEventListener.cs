using UnityEngine;

public class UIEventListener : MonoBehaviour
{
    public Vector3 oldMousePos;
    public bool isPressed;
    private Material[] allMaterials = new Material[0];

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isPressed = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            isPressed = false;
        }
        if (isPressed)
        {
            Vector3 delta = Input.mousePosition - oldMousePos;
            RotateHitGameObject(Input.mousePosition, new Vector2(delta.x, delta.y));
        }
        oldMousePos = Input.mousePosition;
    }


    public void RotateHitGameObject(Vector3 pos, Vector2 delta)
    {
        if (Camera.main == null)
        {
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(pos);
        RaycastHit[] array = Physics.RaycastAll(ray, float.PositiveInfinity);
        for (int i = 0; i < array.Length; i++)
        {
            if (!(array[i].collider == null) && !(array[i].collider.gameObject == null))
            {
                GameObject go = array[i].collider.gameObject;
                if (go.transform.parent != null)
                {
                    go.transform.parent.Rotate(Vector3.up, delta.x, Space.World);
                }
                else
                {
                    go.transform.Rotate(Vector3.up, delta.x, Space.World);
                }
                return;
            }
        }
    }

    public void OnGUI()
    {
        float start = 100f;
        float delta = 50f;
        int i = 0;
        if (GUI.Button(new Rect(50, start + delta * i, 200, 30), "显示正常效果"))
        {
            Shader.DisableKeyword("SHOW_MASK_X");
            Shader.DisableKeyword("SHOW_MASK_Y");
            Shader.DisableKeyword("SHOW_MASK_Z");
            Shader.DisableKeyword("SHOW_RAW_COLOR");
            Shader.EnableKeyword("OPEN_SPECULAR_LIGHT");
            Shader.EnableKeyword("OPEN_MATCAP_LIGHT");
            Shader.EnableKeyword("OPEN_REFLECT_LIGHT");
        }
        i += 1;
        if (GUI.Button(new Rect(50, start + delta * i, 200, 30), "去掉高光效果"))
        {
            Shader.DisableKeyword("SHOW_MASK_X");
            Shader.DisableKeyword("SHOW_MASK_Y");
            Shader.DisableKeyword("SHOW_MASK_Z");
            Shader.DisableKeyword("SHOW_RAW_COLOR");
            Shader.DisableKeyword("OPEN_SPECULAR_LIGHT");
            Shader.EnableKeyword("OPEN_MATCAP_LIGHT");
            Shader.EnableKeyword("OPEN_REFLECT_LIGHT");
        }
        i += 1;
        if (GUI.Button(new Rect(50, start + delta * i, 200, 30), "去掉轮廓光效果"))
        {
            Shader.DisableKeyword("SHOW_MASK_X");
            Shader.DisableKeyword("SHOW_MASK_Y");
            Shader.DisableKeyword("SHOW_MASK_Z");
            Shader.DisableKeyword("SHOW_RAW_COLOR");
            Shader.EnableKeyword("OPEN_SPECULAR_LIGHT");
            Shader.DisableKeyword("OPEN_MATCAP_LIGHT");
            Shader.EnableKeyword("OPEN_REFLECT_LIGHT");
        }
        i += 1;
        if (GUI.Button(new Rect(50, start + delta * i, 200, 30), "去掉反光效果"))
        {
            Shader.DisableKeyword("SHOW_MASK_X");
            Shader.DisableKeyword("SHOW_MASK_Y");
            Shader.DisableKeyword("SHOW_MASK_Z");
            Shader.DisableKeyword("SHOW_RAW_COLOR");
            Shader.EnableKeyword("OPEN_SPECULAR_LIGHT");
            Shader.EnableKeyword("OPEN_MATCAP_LIGHT");
            Shader.DisableKeyword("OPEN_REFLECT_LIGHT");
        }
        i += 1;
        if (GUI.Button(new Rect(50, start + delta * i, 200, 30), "显示原始贴图效果"))
        {
            Shader.DisableKeyword("SHOW_MASK_X");
            Shader.DisableKeyword("SHOW_MASK_Y");
            Shader.DisableKeyword("SHOW_MASK_Z");
            Shader.EnableKeyword("SHOW_RAW_COLOR");
            Shader.EnableKeyword("OPEN_SPECULAR_LIGHT");
            Shader.EnableKeyword("OPEN_MATCAP_LIGHT");
            Shader.EnableKeyword("OPEN_REFLECT_LIGHT");
        }
        i += 1;
        if (GUI.Button(new Rect(50, start + delta * i, 200, 30), "显示镜面高光通道"))
        {
            Shader.EnableKeyword("SHOW_MASK_X");
            Shader.DisableKeyword("SHOW_MASK_Y");
            Shader.DisableKeyword("SHOW_MASK_Z");
            Shader.DisableKeyword("SHOW_RAW_COLOR");
            Shader.EnableKeyword("OPEN_SPECULAR_LIGHT");
			Shader.EnableKeyword("OPEN_MATCAP_LIGHT");
            Shader.EnableKeyword("OPEN_REFLECT_LIGHT");
        }
        i += 1;
        if (GUI.Button(new Rect(50, start + delta * i, 200, 30), "显示流光通道"))
        {
            Shader.DisableKeyword("SHOW_MASK_X");
            Shader.EnableKeyword("SHOW_MASK_Y");
            Shader.DisableKeyword("SHOW_MASK_Z");
            Shader.DisableKeyword("SHOW_RAW_COLOR");
            Shader.EnableKeyword("OPEN_SPECULAR_LIGHT");
			Shader.EnableKeyword("OPEN_MATCAP_LIGHT");
            Shader.EnableKeyword("OPEN_REFLECT_LIGHT");
        }
        i += 1;
        if (GUI.Button(new Rect(50, start + delta * i, 200, 30), "显示反射光通道"))
        {
            Shader.DisableKeyword("SHOW_MASK_X");
            Shader.DisableKeyword("SHOW_MASK_Y");
            Shader.EnableKeyword("SHOW_MASK_Z");
            Shader.DisableKeyword("SHOW_RAW_COLOR");
            Shader.EnableKeyword("OPEN_SPECULAR_LIGHT");
			Shader.EnableKeyword("OPEN_MATCAP_LIGHT");
            Shader.EnableKeyword("OPEN_REFLECT_LIGHT");
        }
        i += 1;
    }
}
