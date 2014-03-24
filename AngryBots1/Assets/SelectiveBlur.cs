// SelectiveBlur.cs
using UnityEngine;
using System.Collections;

public class SelectiveBlur : MonoBehaviour {

	public Shader blurShader = null;	

	public float maxSampleRadius = 1.25f;
	public float effectStartDistance = 0.2f;
	public float effectMaxDistance = 0.5f;
	public float blurSpread = 0.5f;

	void Update()
	{
		material.SetFloat("_MaxSampleRadius", maxSampleRadius);
		material.SetFloat("_EffectStartDistance", effectStartDistance);
		material.SetFloat("_EffectMaxDistance", effectMaxDistance);
		material.SetFloat("_BlurSpread", blurSpread);
	}

	static Material m_Material = null;
	protected Material material {
		get {
			if (m_Material == null) {
				m_Material = new Material(blurShader);
				m_Material.hideFlags = HideFlags.DontSave;
			}
			return m_Material;
		} 
	}

	void OnRenderImage(RenderTexture source, RenderTexture dest)
	{
		float mouseX = Input.mousePosition.x / Screen.width;
		float mouseY = Input.mousePosition.y / Screen.height;
		//Debug.Log(mouseX + ", " + mouseY);
		material.SetFloat("_FocusX", mouseX);
		material.SetFloat("_FocusY", mouseY);
		Graphics.Blit(source, dest, material);
	}
}