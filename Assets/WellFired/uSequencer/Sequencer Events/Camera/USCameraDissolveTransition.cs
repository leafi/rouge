using UnityEngine;
using System.Collections;

namespace WellFired
{
	/// <summary>
	/// A custom event that will dissolve one camera into another camera.
	/// </summary>
	[USequencerFriendlyName("Dissolve Transition")]
	[USequencerEvent("Camera/Transition/Dissolve")]
	[ExecuteInEditMode]
	public class USCameraDissolveTransition : USEventBase
	{
		private Vector2 MainGameViewSize
		{
			get
			{
				if(Application.isEditor)
				{
					var T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
					var GetMainGameView = WellFired.Shared.PlatformSpecificFactory.ReflectionHelper.GetNonPublicStaticMethod(T, "GetMainGameView");
					var ShownResolution = WellFired.Shared.PlatformSpecificFactory.ReflectionHelper.GetNonPublicInstanceField(T, "m_ShownResolution");
					var MainGameView = GetMainGameView.Invoke(null, null);
					var Res = ShownResolution.GetValue(MainGameView);
					return (Vector2)Res;
				}
				else
				{
					return new Vector2(Screen.width, Screen.height);
				}
			}
			set { ; }
		}

		private RenderTexture IntroRenderTexture 
		{
			get
			{
				if(introRenderTexture == null)
					introRenderTexture = new RenderTexture((int)MainGameViewSize.x, (int)MainGameViewSize.y, 24);

				return introRenderTexture;
			}
			set { ; }
		}

		private RenderTexture OutroRenderTexture 
		{
			get
			{
				if(outroRenderTexture == null)
					outroRenderTexture = new RenderTexture((int)MainGameViewSize.x, (int)MainGameViewSize.y, 24);
				
				return outroRenderTexture;
			}
			set { ; }
		}

		[SerializeField]
		private Camera introCamera;

		[SerializeField]
		private Camera outroCamera;
		
		[SerializeField]
		private Material renderMaterial;

		private RenderTexture introRenderTexture;
		private RenderTexture outroRenderTexture;
		private bool shouldRender;
		private bool prevIntroCameraState;
		private bool prevOutroCameraState;

		private float alpha;

		private void OnGUI()
		{
			if(!shouldRender)
				return;

			renderMaterial.SetTexture("_SecondTex", OutroRenderTexture);
			renderMaterial.SetFloat("_Alpha", alpha);
			Graphics.Blit(IntroRenderTexture, default(RenderTexture), renderMaterial);
		}
		
		public override void FireEvent()
		{
			Debug.Log("Fire");

			if(introCamera)
				prevIntroCameraState = introCamera.enabled;
			
			if(outroCamera)
				prevOutroCameraState = outroCamera.enabled;
		}

		public override void ProcessEvent(float deltaTime)
		{
			Debug.Log("Process");

			if(introCamera)
				introCamera.enabled = false;

			if(outroCamera)
				outroCamera.enabled = false;

			if(introCamera)
			{
				introCamera.targetTexture = IntroRenderTexture;
				introCamera.Render();
			}

			if(outroCamera)
			{
				outroCamera.targetTexture = OutroRenderTexture;
				outroCamera.Render();
			}

			alpha = 1.0f - (deltaTime / Duration);
			shouldRender = true;
		}

		public override void EndEvent()
		{
			Debug.Log("End");
			shouldRender = false;

			if(outroCamera)
			{
				outroCamera.enabled = true;
				outroCamera.targetTexture = null;
			}

			if(introCamera)
			{
				introCamera.enabled = false;
				introCamera.targetTexture = null;
			}
		}

		public override void StopEvent()
		{
			Debug.Log("Stop");
			UndoEvent();
		}
		
		public override void UndoEvent()
		{
			Debug.Log("Undo");
			if(introCamera)
			{
				introCamera.enabled = prevIntroCameraState;
				introCamera.targetTexture = null;
			}

			if(outroCamera)
			{
				outroCamera.enabled = prevOutroCameraState;
				outroCamera.targetTexture = null;
			}

			RenderTexture.DestroyImmediate(IntroRenderTexture);
			RenderTexture.DestroyImmediate(OutroRenderTexture);

			shouldRender = false;
		}
	}
}