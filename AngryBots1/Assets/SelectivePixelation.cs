using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
//[AddComponentMenu("Image Effects/Blur/Blur")]
public class SelectivePixelation : MonoBehaviour {

	public Shader pixelationShader = null;	

	public float effectStartDistance = 0.2f;
	public float effectMaxDistance = 0.5f;
	public float downsampleFactor = 2f;

	void Update()
	{
		material.SetFloat("_EffectStartDistance", effectStartDistance);
		material.SetFloat("_EffectMaxDistance", effectMaxDistance);
		material.SetFloat("_DownSampleFactor", downsampleFactor);
	}

	static Material m_Material = null;
	protected Material material {
		get {
			if (m_Material == null) {
				m_Material = new Material(pixelationShader);
				m_Material.hideFlags = HideFlags.DontSave;
			}
			return m_Material;
		} 
	}

	private Vector2 GetNormalizedFocus()
	{
		// For now, use mouse pos:
		return GetNormalizedMousePosition();
	}

	private Vector2 GetNormalizedMousePosition()
	{
		float mouseX = Input.mousePosition.x / Screen.width;
		float mouseY = Input.mousePosition.y / Screen.height;

		return new Vector2(mouseX, mouseY);
	}

	void OnRenderImage(RenderTexture source, RenderTexture dest)
	{
		Vector2 focus = GetNormalizedFocus();

		// Establish temp buffer for lo-res frame 
		RenderTexture loResBuffer = RenderTexture.GetTemporary(
							(int)(source.width / downsampleFactor), 
							(int)(source.height / downsampleFactor), 0);

		// We want low-quality filtering: Point!
		loResBuffer.filterMode = FilterMode.Point;
		Graphics.Blit(source, loResBuffer);

		// Send lo-res buffer to shader
		material.SetTexture("_LoResTex", loResBuffer);

		material.SetFloat("_FocusX", focus.x);
		material.SetFloat("_FocusY", focus.y);

		// Draw composite to dest
		Graphics.Blit(source, dest, material);

		RenderTexture.ReleaseTemporary(loResBuffer);
	}
}