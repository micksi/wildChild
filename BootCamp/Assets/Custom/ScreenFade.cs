using UnityEngine;
using System.Collections;
using System;

public class ScreenFade : MonoBehaviour {

	public Shader fadeShader = null;	
	public Color fadeColour = new Color(0,0,0,1);

	private int blinkFramesToGo = 0;
	private int fadeOutFramesToGo = 0;
	private int fadeOutFramesTotal = 1;
	private int fadeInFramesToGo = 0;
	private int fadeInFramesTotal = 1;

	static Material m_Material = null;
	protected Material material {
		get {
			if (m_Material == null) {
				m_Material = new Material(fadeShader);
				m_Material.hideFlags = HideFlags.DontSave;
			}
			return m_Material;
		} 
	}

	void Update()
	{
		// TODO move this functionality AWAY from this class!
		if(Input.GetKey("o"))
		{
			Blink(1);
		}	
		else if(Input.GetKey("i"))
		{
			Blink(6);
		}
		else if(Input.GetKey("p"))
		{
			FadeOutIn(24,24);
		}	
	}

	public void Blink(int noOfFrames)
	{
		blinkFramesToGo = noOfFrames;
	}

	public void FadeIn(int noOfFrames)
	{
		fadeInFramesToGo = noOfFrames;
		fadeInFramesTotal = noOfFrames;
	}

	public void FadeOut(int noOfFrames)
	{
		fadeOutFramesToGo = noOfFrames;
		fadeOutFramesTotal = noOfFrames;
	}

	public void FadeOutIn(int noOfFramesOut, int noOfFramesIn)
	{
		FadeOut(noOfFramesOut);
		FadeIn(noOfFramesIn);
	}

	void OnRenderImage(RenderTexture source, RenderTexture dest)
	{	
		float fadeAmount = 0;
		if(blinkFramesToGo-- > 0)
		{
			fadeAmount = 1;
		}
		else if(fadeOutFramesToGo-- > 0)
		{
			fadeAmount = 1.0f - (float)(fadeOutFramesToGo) / fadeOutFramesTotal;
		}
		else if(fadeInFramesToGo-- > 0)
		{
			fadeAmount = (float)(fadeInFramesToGo) / fadeInFramesTotal;
		}

		material.SetColor("_Colour", fadeColour);
		material.SetFloat("_FadeAmount", fadeAmount);

		Graphics.Blit(source, dest, material);
	}
}