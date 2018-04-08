using UnityEngine;
using System.Collections;

/* Best practises
 * 1. AssetPostprocessor: OnPreprocessTexture, OnPreprocessModel { ((ModelImporter)AssetPostprocessor.assetImporter).isReadable = false / .SaveAndReimport(); }
 * 2. Textures: make sure Read/Write is disabled, disable mipmaps if possible, make sure textures are compressed, sizes: 1024 or 2048 for atlases, 512 or smaller for model textures
 * 3. Models: make sure Read/Write is disabled, disable rig on non-character models, copy avatars for characters with shared rigs, enable mesh compression
 * 4. Instiantiating models with MeshRenderer at runtime: Casting/Receiving shadows, Light/Reflection probes, MotionVectors are on, turn them off if not necessary
 * 5. Audio: Force mono, set bitrate as low as possible, use MP3 compression for iOS, Vorbis for Android
 * 6. Avoid temporary allocations, GarbageCollector running once per minute is bad for framerate
 * 7. Tracking managed memory allocations: use unity profiler, sort by "GC alloc" column. Avoid allocations during user interaction.
 * 8. Reuse collections (Lists, HashSets)
 * 9. Avoid string concatenation, reuse StringBuilders to compose strings
 * 10. Avoid closures and anomymous methods
 * 11. Avoid boxing, which happens when using enums as Dictionary keys. Enum key is beeing boxed. Implement IEqualityComparer class and pass it to the dictionary's constructor
 * 12. Foreach are half as slow as for loops. They allocate Enumerator when loop begins. Use them only for types, when it makes sense.
 * 13. When unity returns array, it allocates a new copy.
 * 14. Input.touches should be referenced before loop through the touches, to be allocated only once: Touch[] touches = Input.touches; for(;i<touches.Length;) { Touch t = touches[i]; }
 * 15. OR int touchCount = Input.touchCountl for(;i<touchCount;) { Touch t = GetTouch(i); } <- no allocations at all.
 * 16. Avoid parsing text and parsers built on Reflection, use JsonUtility class;
 * 17. Bake text data to binary - use ScriptableObject - useful for immutable data, like game design patterns
 * 18. Large amount of resources: move them form Resources folder to Asset Bundles
 * 19. Never address Material, Shader, or Animator properties by name, use cached, hash integers: static readonly int foo = Shader.PropertyToID("_Color") / Animator.StringToHash("attack"); material.SetColor(foo,Color.white) / animator.SetTrigger(foo);
 * 20. Avoid regular expressions whenever possible
 * 21. compare strings with ordinal comparison: string1.Equals(string2, StringComparison.Ordinal)
 * 22. Do not use Property getters and setters in loops if they are plain integers, each get and set is costs performance, property accessors are always method calls
 * 23. Vector3. zero or Quaternion.Identity are method calls, avoid them, for simple types make a const, for complex types: static readonly
 * 24. Quaternon.Set, Vector.Scale - cost of a function call, do the math and assign variables
 * 25. Transform.Translate, Transform.Rotate - cost of a function call, assign position and rotation
 */
namespace SurvivalShooter {
	public class CameraFollow : MonoBehaviour {
		public Transform target;            // The position that that camera will be following.
		public float smoothing = 5f;        // The speed with which the camera will be following.
		//[Tooltip("AudioListener.volume"), Range(0f, 1f)]
		//public float soundVolume = 1f;

		Vector3 offset;                     // The initial offset from the target.


		void Start() {
			// Calculate the initial offset.
			offset = transform.position - target.position;
		}


		void FixedUpdate() {
			// Set AudioListener's volume
#if UNITY_EDITOR
			//if (AudioListener.volume != soundVolume) {
			//	AudioListener.volume = soundVolume;
			//}
#endif
			// Create a postion the camera is aiming for based on the offset from the target.
			Vector3 targetCamPos = target.position + offset;

			// Smoothly interpolate between the camera's current position and it's target position.
			transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
		}
	}
}