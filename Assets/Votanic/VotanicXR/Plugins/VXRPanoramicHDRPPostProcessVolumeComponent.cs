using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using Votanic.vXR.vCast.Core;
using UCamera = UnityEngine.Camera;

namespace Votanic.vXR.vCast
{
	/// <summary>
	/// Cylinderical Unwrapper transforms a normal perspective render to a cylinderical render. It utilizes
	/// an additional post process render pass through an intermediate material in the transformation.
	/// The class must be attached to the same game object with the target camera.
	/// This is the HDRP implementation for cylinderical unwrap.
	/// </summary>
	[RequireComponent(typeof(UCamera))]
	[Serializable, VolumeComponentMenu("Post-processing/PanoramicHDRP")]
	public sealed class VXRPanoramicHDRPPostProcessVolumeComponent : CustomPostProcessVolumeComponent, IPostProcessComponent
	{
		// ========================================================= Variables and Properties =========================================================

		private Material m_Material;

		public BoolParameter m_enable = new BoolParameter(false);

		// ========================================================= CustomPostProcessVolumeComponent Methods =========================================================

		public bool IsActive() => m_Material != null && m_enable.value;

		public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

		public override void Setup()
		{
			if (Shader.Find("Hidden/VotanicXR/vCastPanoramicPostProcess") != null)
			{
				m_Material = new Material(Shader.Find("Hidden/VotanicXR/vCastPanoramicPostProcess"));
			}
		}

		public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
		{
			if (m_Material == null)
			{
				return;
			}

			TransferVariables(camera);
			cmd.Blit(source, destination, m_Material, 0);
		}

		public override void Cleanup() => CoreUtils.Destroy(m_Material);

		// ========================================================= Functionality =========================================================

		/// <summary>
		/// Transfer variables from unwrap controller to shader
		/// </summary>
		public void TransferVariables(HDCamera camera)
		{
			// notify shader the values
			PanoramicHDRP unwrapController = camera.camera.GetComponent<PanoramicHDRP>();
			bool enable = m_enable.value && unwrapController != null && unwrapController.enabled;
			m_Material.SetFloat("_Enable", enable ? 1f : 0f);

			if (enable)
			{
				m_Material.SetFloat("_RenderSpan", unwrapController._ShaderRenderSpan);
				m_Material.SetFloat("_DisplaySpan", unwrapController._ShaderDisplaySpan);
				m_Material.SetFloat("_KeepAspectRatio", unwrapController._ShaderKeepAspectRatio);
				m_Material.SetFloat("_DisplayRectWidth", unwrapController._ShaderDisplayRectWidth);
				m_Material.SetFloat("_DisplayRectHeight", unwrapController._ShaderDisplayRectHeight);
			}
		}
	}
}