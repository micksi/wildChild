using UnityEngine;
using System.Collections;

public class SelectiveDecolouring : MonoBehaviour {

	public Shader decolourShader = null;	

	public float effectStartDistance = 0.2f;
	public float effectMaxDistance = 0.5f;

	void Update()
	{
		material.SetFloat("_EffectStartDistance", effectStartDistance);
		material.SetFloat("_EffectMaxDistance", effectMaxDistance);
	}

	static Material m_Material = null;
	protected Material material {
		get {
			if (m_Material == null) {
				m_Material = new Material(decolourShader);
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

		material.SetFloat("_FocusX", focus.x);
		material.SetFloat("_FocusY", focus.y);
		
		Graphics.Blit(source, dest, material);
	}
}