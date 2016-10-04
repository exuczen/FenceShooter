using CT;
using CT.DEBUG;
using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

namespace CT {
	/// <summary>Add this component to game object with ParticleSystem for "multires" support.</summary>
	[RequireComponent(typeof(ParticleSystem)), DisallowMultipleComponent]
	public class MultiresParticleSystemMaterialComponent : MonoBehaviour {
		/// <summary>NOTE: Unity event</summary>
		public void Awake() {
			if (!Multires.IsDefault) {
				ParticleSystemRenderer particleSystemRenderer = GetComponent<ParticleSystem>().GetComponent<ParticleSystemRenderer>();
				var originalMaterial = particleSystemRenderer.material;
				particleSystemRenderer.material = originalMaterial.mainTexture.name.LoadMultires<Material>();
			}
		}
	}
}
