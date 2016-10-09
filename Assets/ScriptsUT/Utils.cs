#if DEBUG
//#define DEBUG_PRINT
#endif
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.IO;
using CT;

namespace Utility {

	//delegate bool CompareAnimals(Animal animalA, Animal animalB);

	public class Utils {


		public static Sprite LoadSpriteSpecRes(string name, Sprite currSprite) {
			return Utils.CreateSprite(Utils.LoadTextureSpecRes(name, currSprite != null ? currSprite.texture : null));
		}

		public static Sprite LoadSpriteMainRes(string name) {
			return Utils.CreateSprite(Utils.LoadTextureMainRes(name));
		}


		public static Texture2D LoadTextureSpecRes(string name, Texture2D currTexture) {
#if UNITY_WP8 || UNITY_WP_8_1
			return Resources.Load(AppControl.resFolder.ToString()+"/"+name) as Texture2D;
#else
			if (currTexture == null)
				return Resources.Load(Multires.Folder.ToString() + "/" + name) as Texture2D;
			else {
				try {
					return Resources.Load(Multires.Folder.ToString() + "/" + name) as Texture2D;
				} catch (Exception e) {
					Utils.Log("LoadTextureSpecRes: " + name + " Exception:" + e.Message);
					return currTexture;
				}
			}
#endif

		}

		public static Texture2D LoadTextureMainRes(string name) {
			return Resources.Load(name) as Texture2D;
		}

		public static Sprite CreateSprite(Texture2D texture) {
			Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
			sprite.name = texture.name;
			return sprite;
		}

		public static GameObject CreateSpriteGameObject(Vector3 size, Texture2D texture, int sortingOrder, GameObject parent, string name) {
			GameObject go = new GameObject(name);
			Sprite sprite = CreateSprite(texture);
			SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
			sr.sprite = sprite;
			sr.sortingOrder = sortingOrder;
			if (parent != null) {
				go.transform.SetParent(parent.transform);
			}
			go.transform.localPosition = Vector3.zero;

			Vector3 origSize = sprite.bounds.size;
			go.transform.localScale = new Vector3(size.x / origSize.x, size.y / origSize.y, 1.0f);

			return go;
		}

		public static GameObject CreateRoundRectSprite(Vector2 size, float R, float posZ, string name) {
			int sortingOrder = 0;
			GameObject parent = new GameObject(name);
			parent.transform.localPosition = new Vector3(0, 0, 0);

			Texture2D middleTexture = Utils.LoadTextureMainRes("okno_srodek");
			Texture2D cornerTexture = Utils.LoadTextureMainRes("okno_rog");
			Texture2D sideTexture = Utils.LoadTextureMainRes("okno_bok_hori");
			//Utils.Log("textures = "+middleTexture+" "+cornerTexture+" "+sideTexture);

			Vector3 middleSize = new Vector3(size.x - 2 * R, size.y - 2 * R, 1.0f);
			GameObject middleGO = CreateSpriteGameObject(middleSize, middleTexture, sortingOrder, parent, "Middle");

			Vector3 sideHoriSize = new Vector3(size.x - 2 * R, R, 1.0f);
			GameObject sideGO = CreateSpriteGameObject(sideHoriSize, sideTexture, sortingOrder, parent, "Side");

			Vector3 cornerSize = new Vector3(R, R, 1.0f);
			GameObject cornerGO = CreateSpriteGameObject(cornerSize, cornerTexture, sortingOrder, parent, "Corner");

			float colorBightnss = 0.9f;
			Color color = new Color(colorBightnss, colorBightnss, colorBightnss);
			Utils.MlpSpriteColor(middleGO, color);
			Utils.MlpSpriteColor(sideGO, color);
			Utils.MlpSpriteColor(cornerGO, color);

			float[] cornerAngles = { 180, 90, 270, 0 };
			float[] sideAngles = { 180, 0, 270, 90 };

			int i = 0;
			for (int y = -1; y < 2; y += 2) {
				for (int x = -1; x < 2; x += 2) {
					GameObject corner = GameObject.Instantiate(cornerGO);
					corner.transform.SetParent(parent.transform);
					corner.transform.localPosition = new Vector3(x * (-size.x + R) / 2, y * (size.y - R) / 2, 0.0f);
					corner.transform.localRotation = Quaternion.Euler(0, 0, cornerAngles[i]);
					i++;
				}
			}
			i = 0;
			for (int y = -1; y < 2; y += 2) {
				GameObject side = GameObject.Instantiate(sideGO);
				side.transform.SetParent(parent.transform);
				side.transform.localPosition = new Vector3(0.0f, y * (size.y - R) / 2, 0.0f);
				side.transform.localRotation = Quaternion.Euler(0, 0, sideAngles[i]);
				i++;
			}

			Vector3 sideVertiSize = new Vector3(size.y - 2 * R, R, 1.0f);
			Sprite sideSprite = sideGO.GetComponent<SpriteRenderer>().sprite;
			Vector3 origSize = sideSprite.bounds.size;
			sideGO.transform.localScale = new Vector3(sideVertiSize.x / origSize.x, sideVertiSize.y / origSize.y, 1.0f);

			for (int x = -1; x < 2; x += 2) {
				GameObject side = GameObject.Instantiate(sideGO);
				side.transform.SetParent(parent.transform);
				side.transform.localPosition = new Vector3(x * (-size.x + R) / 2, 0.0f, 0.0f);
				side.transform.localRotation = Quaternion.Euler(0, 0, sideAngles[i]);
				i++;
			}

			GameObject.Destroy(cornerGO);
			GameObject.Destroy(sideGO);

			return parent;
		}

		public static GameObject CreateRoundRectWithMeshModels(Vector2 size, float R, float posZ, Color color, Color borderColor, float border, string name) {
			if (R < border) {
				Debug.LogError("Utils.CreateRoundRectWithMeshModels: R<border");
				return null;
			}

			GameObject externalModel = CreateRoundRectWithMeshModels(size, R, 0, borderColor, "External");
			Vector2 internalSize = new Vector2(size.x - 2 * border, size.y - 2 * border);
			float r = R - border;
			GameObject internalModel = CreateRoundRectWithMeshModels(internalSize, r, -0.1f, color, "Internal");

			GameObject container = new GameObject(name);
			container.transform.localPosition = Vector3.zero;
			container.transform.localRotation = Quaternion.identity;

			externalModel.transform.SetParent(container.transform);
			internalModel.transform.SetParent(container.transform);

			container.transform.localPosition = new Vector3(0, 0, posZ);

			return container;
		}


		public static GameObject CreateRoundRectWithMeshModels(Vector2 size, float radius, float posZ, Color color, string name) {
			//Vector2 camHalfSize = Utils.GetCameraHalfSize(camera);
			GameObject circle = Resources.Load("Circle") as GameObject;
			GameObject[] parts = new GameObject[7];
			int partsCounter = 0;
			GameObject[] circles = new GameObject[4];
			//Vector2 size = new Vector2(4, 4);//new Vector2(camHalfSize.x/2, camHalfSize.y/2);
			//float radius = size.y/8;
			float origRadius = 1.0f;
			float circleScale = radius / origRadius;

			Vector3 circleScaleVecc = new Vector3(circleScale, circleScale, 1.0f);
			//float posZ = 0;
			Quaternion circleRotation = Quaternion.Euler(new Vector3(180, 0, 0));
			int circleCounter = 0;
			for (int j = -1; j < 2; j += 2) {
				for (int i = -1; i < 2; i += 2) {
					Vector3 pos = new Vector3(i * (size.x / 2 - radius), j * (size.y / 2 - radius), 0);
					circles[circleCounter] = GameObject.Instantiate(circle, pos, circleRotation) as GameObject;
					circles[circleCounter].transform.localScale = circleScaleVecc;
					parts[partsCounter++] = circles[circleCounter];
					circleCounter++;

				}
			}
			Vector2 quadOrigSize = Vector2.one;
			GameObject centerQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
			centerQuad.transform.position = Vector3.zero;
			centerQuad.transform.rotation = Quaternion.identity;
			Vector3 cRectScaleVec = new Vector3((size.x - 2 * radius) / quadOrigSize.x, size.y / quadOrigSize.y, 1.0f);
			centerQuad.transform.localScale = cRectScaleVec;

			GameObject sideQuadL = GameObject.CreatePrimitive(PrimitiveType.Quad);
			GameObject sideQuadR = GameObject.CreatePrimitive(PrimitiveType.Quad);

			Vector3 sideRectScaleVec = new Vector3((2 * radius) / quadOrigSize.x, (size.y - 2 * radius) / quadOrigSize.y, 1.0f);

			sideQuadL.transform.position = new Vector3(-(size.x / 2 - radius), 0, 0);
			sideQuadL.transform.localScale = sideRectScaleVec;
			sideQuadR.transform.position = new Vector3((size.x / 2 - radius), 0, 0);
			sideQuadR.transform.localScale = sideRectScaleVec;

			parts[partsCounter++] = centerQuad;
			parts[partsCounter++] = sideQuadL;
			parts[partsCounter++] = sideQuadR;

			GameObject container = new GameObject(name);
			container.transform.localPosition = Vector3.zero;
			container.transform.localRotation = Quaternion.identity;

			Renderer circleRenderer = parts[0].GetComponent<Renderer>();
			//			circleRenderer.sharedMaterial.SetFloat("_Shininess",0.5f);
			//			Utils.Log("_Shininess="+circleRenderer.sharedMaterial.GetFloat("_Shininess"));
			//			MaterialPropertyBlock mpb = new MaterialPropertyBlock();
			//			circleRenderer.GetPropertyBlock(mpb);
			//			Shader circleShader = circleRenderer.material.shader;
			//			circleRenderer.material.color = color;
			//			circleRenderer.material.SetColor("_Color", color);
			Texture texture = CreateTextureWithColor(4, 4, color);
			circleRenderer.material.SetTexture("_MainTex", texture);
			for (int i = 0; i < partsCounter; i++) {
				GameObject part = parts[i];
				Renderer renderer = part.GetComponent<Renderer>();
				//				Utils.Log(" "+part+" "+renderer.material.shader);
				//				renderer.SetPropertyBlock(mpb);
				renderer.material = circleRenderer.material;
				//				renderer.material.shader = circleShader;
				//				Utils.Log (" "+part+" "+renderer.material.GetTexture("_MainTex")) ;
				part.transform.SetParent(container.transform);
			}
			container.transform.localPosition = new Vector3(0, 0, posZ);
			return container;
		}

		public static void ScaleChildren(GameObject go, Vector3 scaleVec) {
			for (int i = 0; i < go.transform.childCount; i++) {
				go.transform.GetChild(i).localScale = scaleVec;
			}
		}

		public static Texture2D CreateTextureWithColor(int width, int height, Color color) {
			Texture2D texture = new Texture2D(width, height);
			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					texture.SetPixel(x, y, color);
				}
			}
			texture.Apply();
			return texture;

		}

		public static void DrawGUIQuad(Rect position, Color color) {
			Texture2D texture = CreateTextureWithColor(1, 1, color);
			GUI.skin.box.normal.background = texture;
			GUI.Box(position, GUIContent.none);
		}

		public static float GetSpritesAlpha(GameObject spriteObject) {
			SpriteRenderer sr = spriteObject.GetComponent<SpriteRenderer>();
			if (sr) {
				Color c = sr.color;
				return c.a;
			} else
				return 0.0f;
		}

		public static void MlpSpriteColor(GameObject spriteObject, Color color) {
			SpriteRenderer sr = spriteObject.GetComponent<SpriteRenderer>();
			if (sr) {
				Color c = sr.color;
				sr.color = new Color(c.r * color.r,
									 c.g * color.g,
									 c.b * color.b,
									 c.a);
			}
		}
		public static void SetSpriteAlpha(GameObject spriteObject, float alpha) {
			SpriteRenderer sr = spriteObject.GetComponent<SpriteRenderer>();
			if (sr) {
				Color c = sr.color;
				sr.color = new Color(c.r, c.g, c.b, alpha);
			}
		}
		public static void SetImageAlpha(Image image, float alpha) {
			Color c = image.color;
			c.a = alpha;
			image.color = c;
		}
		public static void SetRendererMaterialsAlpha(Renderer renderer, float alpha) {
			//			Utils.Log("SetRendererMaterialAlpha");
			if (renderer.materials != null || renderer.material) {
				foreach (Material material in renderer.materials) {
					if (material.HasProperty("_Color")) {
						Color c = material.color;
						//						Utils.Log(Utils.GetColorString(c));
						material.color = new Color(c.r, c.g, c.b, alpha);
					}
				}
			}

		}

		public static Vector2 GetCameraHalfSize(Camera camera) {
			if (camera.orthographic) {
				return new Vector2(camera.orthographicSize * camera.aspect, camera.orthographicSize);
			} else {
				return Vector2.zero;
			}
		}

		public static Vector3 GetLocalSpaceAnchorPointOfCamera(int anchor, Camera camera) {
			if (camera.orthographic) {
				float halfHeight = camera.orthographicSize;
				float halfWidth = halfHeight * camera.aspect;
				Vector3 localPoint = Vector3.zero;

				switch (anchor) {
					case (int)(Const.Anchor.Top | Const.Anchor.Left):
					localPoint = new Vector3(-halfWidth, halfHeight, 0.0f);
					break;
					case (int)(Const.Anchor.Top | Const.Anchor.Middle):
					localPoint = new Vector3(0.0f, halfHeight, 0.0f);
					break;
					case (int)(Const.Anchor.Top | Const.Anchor.Right):
					localPoint = new Vector3(halfWidth, halfHeight, 0.0f);
					break;

					case (int)(Const.Anchor.Middle | Const.Anchor.Left):
					localPoint = new Vector3(-halfWidth, 0.0f, 0.0f);
					break;
					case (int)(Const.Anchor.Middle):
					localPoint = new Vector3(0.0f, 0.0f, 0.0f);
					break;
					case (int)(Const.Anchor.Middle | Const.Anchor.Right):
					localPoint = new Vector3(halfWidth, 0.0f, 0.0f);
					break;

					case (int)(Const.Anchor.Bottom | Const.Anchor.Left):
					localPoint = new Vector3(-halfWidth, -halfHeight, 0.0f);
					break;
					case (int)(Const.Anchor.Bottom | Const.Anchor.Middle):
					localPoint = new Vector3(0.0f, -halfHeight, 0.0f);
					break;
					case (int)(Const.Anchor.Bottom | Const.Anchor.Right):
					localPoint = new Vector3(halfWidth, -halfHeight, 0.0f);
					break;
					default:
					localPoint = Vector3.zero;
					break;
				}
				return localPoint;
			} else {
				return Vector3.zero;
			}
		}

		public static Vector3 GetWorldSpaceAnchorPointOfCamera(int anchor, Camera camera) {
			//			camera.pixelWidth
			//			camera.ScreenToWorldPoint(camScreenVec);

			if (camera.orthographic) {
				Vector3 localPoint = GetLocalSpaceAnchorPointOfCamera(anchor, camera);
				return camera.transform.position + (camera.transform.up * localPoint.y + camera.transform.right * localPoint.x);
			} else {
				return Vector3.zero;
			}
		}

		//static bool CompareAnimalsSpeed(Animal animalA, Animal animalB) {
		//	return animalA.Speed > animalB.Speed;
		//}
		//static bool CompareAnimalsMass(Animal animalA, Animal animalB) {
		//	return animalA.Mass > animalB.Mass;
		//}
		//static bool CompareAnimalsLifespan(Animal animalA, Animal animalB) {
		//	return animalA.Lifespan > animalB.Lifespan;
		//}

		//public static void SortAnimalsByQuestionKey(Animal[] animals, QuestionKey qkey, int arrayLength) {
		//	CompareAnimals compareProperty;

		//	switch (qkey) {
		//		case QuestionKey.QuestionWeightLighter:
		//		case QuestionKey.QuestionWeightHeavier:
		//		compareProperty = CompareAnimalsMass;
		//		break;
		//		case QuestionKey.QuestionAirSpeedFaster:
		//		case QuestionKey.QuestionAirSpeedSlower:
		//		case QuestionKey.QuestionGroundSpeedSlower:
		//		case QuestionKey.QuestionGroundSpeedFaster:
		//		compareProperty = CompareAnimalsSpeed;
		//		break;
		//		case QuestionKey.QuestionLifespanLonger:
		//		case QuestionKey.QuestionLifespanShorter:
		//		compareProperty = CompareAnimalsLifespan;
		//		break;
		//		default:
		//		compareProperty = CompareAnimalsMass;
		//		break;
		//	}
		//	int j;
		//	Animal v;
		//	for (int i = 1; i < arrayLength; i++) {
		//		j = i;
		//		v = animals[i];
		//		//animals[j-1].Mass>v.Mass
		//		while (j > 0 && compareProperty(animals[j - 1], v)) {
		//			animals[j] = animals[j - 1];
		//			j--;
		//		}
		//		animals[j] = v;
		//	}
		//}

		public static string GetAllocatedMemory() {
			long bytes = System.GC.GetTotalMemory(false);
			int mb = (int)(bytes / 1000000);
			int bytesInMB = mb * 1000000;
			int kb = (int)((bytes - bytesInMB) / 1000);
			int bytesInKB = kb * 1000;
			int b = (int)(bytes - bytesInMB - bytesInKB);
			string kbString = (kb < 10 ? "00" + kb : (kb < 100 ? "0" + kb : "" + kb));
			string bString = (b < 10 ? "00" + b : (b < 100 ? "0" + b : "" + b));
			return mb + " " + kbString + " " + bString;
			//			return bytes.ToString();
		}
#if DEBUG
		public static void Log(string s) {
#if UNITY_EDITOR
			s = "[" + Time.time.ToString("F4") + "] " + s;
			Debug.Log(s);
#elif UNITY_ANDROID
			Debug.Log ("# "+s);
#elif UNITY_IOS
			Debug.Log ("# "+s);
#elif UNITY_WP8 || UNITY_WP_8_1
			System.Diagnostics.Debug.WriteLine("# "+s);
			//		Console.WriteLine(s);
			//		print(s);
#endif
		}
#else
		public static void Log (string s){}
#endif

		public static void PrintSystemInfo() {
			Utils.Log(
				SystemInfo.deviceModel + "\n" +
				SystemInfo.deviceName + "\n" +
				SystemInfo.deviceType + "\n" +
				SystemInfo.deviceUniqueIdentifier
			);
		}

		public static string GetFileContent(string filePath) {
			string text = null;
			try {
				using (StreamReader sr = File.OpenText(filePath)) {
					text = sr.ReadToEnd();
					Utils.Log(text);
#if UNITY_WP_8_1
#else
					sr.Close();
#endif
				}
			} catch (IOException e) {

				Utils.Log(e.Message);
			}
			return text;
		}

		public static RenderTexture RenderToTexture2D(Texture2D destTexture, Camera cam, RenderTexture renderTexture) {
			//Texture2D destTexture = destSprite.texture as Texture2D;
			if (renderTexture == null) {
				renderTexture = new RenderTexture(destTexture.width, destTexture.height, 24, RenderTextureFormat.ARGB32);
				renderTexture.Create();
			}
			RenderTexture active = RenderTexture.active;
			RenderTexture.active = renderTexture;

			RenderTexture camTargetTexture = cam.targetTexture;
			cam.targetTexture = renderTexture;
			cam.Render();
			//Texture2D destTexture = new Texture2D (renderTexture.width, renderTexture.height);
			destTexture.ReadPixels(new Rect(0.0f, 0.0f, renderTexture.width, renderTexture.height), 0, 0);
			destTexture.Apply();

			RenderTexture.active = active;
			cam.targetTexture = camTargetTexture;
			return renderTexture;
		}


		public static void ResizeCameraToSprite(GameObject gobject, Camera camera) {
			SpriteRenderer spRen = gobject.GetComponent<SpriteRenderer>();
			Sprite sprite = spRen.sprite;

#if DEBUG_PRINT
			Utils.Log("sprite.textureRect "+sprite.textureRect.width+ " "+sprite.textureRect.height
			          +"\nsprite.texture "+sprite.texture.width+ " "+sprite.texture.height
			          +"\nsprite.rect "+sprite.rect.width+ " "+sprite.rect.height
			          +"\nsprite.bounds.size "+sprite.bounds.size.x+ " "+sprite.bounds.size.y
			          + "\nsprite.pixelsPerUnit "+sprite.pixelsPerUnit
			          );
			Utils.Log("camera.orthographicSize="+camera.orthographicSize);
#endif
			gobject.transform.localScale = new Vector3(1, 1, 1);

			float width = sprite.bounds.size.x;
			float height = sprite.bounds.size.y;

			if (camera.orthographic) {
				if (1.0f * Screen.width / Screen.height > width / height) {
					/**
					 cameraWidth = width;
					 cameraWidth/cameraHeight = ScreenWidth/ScreenHeight
					 cameraHeight = cameraWidth * ScreenHeight/ScreenWidth
					 camera.orthographicSize = height / 2;
					*/
					camera.orthographicSize = (width * Screen.height / Screen.width) / 2;
					//					Utils.Log("Resize "+Screen.width+" "+Screen.height);
				} else {
					camera.orthographicSize = height / 2;
				}
			} else {
				// change camera fov or move camera towards sprite
			}
#if DEBUG_PRINT
			Utils.Log("camera.orthographicSize="+camera.orthographicSize);
#endif
		}


		public static void ResizeSpriteToScreen(GameObject spriteGO, Camera camera, Const.Axis alongAxis) {
			SpriteRenderer sr = spriteGO.GetComponent<SpriteRenderer>();
			Sprite sprite = sr.sprite;

			spriteGO.transform.localScale = Vector3.one;

			float width = sprite.bounds.size.x;
			float height = sprite.bounds.size.y;

			float scale = 1;

			if (camera.orthographic) {
				float cameraHeight = (float)(camera.orthographicSize * 2.0);
				float cameraWidth = (float)(cameraHeight * Screen.width / Screen.height);
				if (alongAxis == Const.Axis.None) {
					if (cameraWidth / cameraHeight > width / height) {
						scale = cameraWidth / width;
					} else {
						scale = cameraHeight / height;
					}
				} else if (alongAxis == Const.Axis.Hori) {
					scale = cameraWidth / width;
				} else if (alongAxis == Const.Axis.Verti) {
					scale = cameraHeight / height;
				}

			} else {

				float distFromCamera = spriteGO.transform.localPosition.z;
				spriteGO.transform.parent = camera.transform;
				spriteGO.transform.localPosition = new Vector3(0, 0, distFromCamera);

				//Utils.Log("Resize sprite "+1.0f*Screen.width/Screen.height+" "+1.0f*width/height);

				if ((alongAxis == Const.Axis.None && 1.0f * Screen.width / Screen.height > width / height) || alongAxis == Const.Axis.Hori) {
					//Mathf.Tan(camera.aspectRatio*camera.fieldOfView/2) = destHalfWidth / distFromCamera
					float destHalfWidth = distFromCamera * camera.aspect * Mathf.Tan(camera.fieldOfView * Mathf.Deg2Rad / 2);
					float currHalfWidth = sprite.bounds.size.x * spriteGO.transform.localScale.x / 2;
					scale = destHalfWidth / currHalfWidth;
				} else {
					//Mathf.Tan(camera.fieldOfView/2) = destHalfHeight / distFromCamera
					float destHalfHeight = distFromCamera * Mathf.Tan(camera.fieldOfView * Mathf.Deg2Rad / 2);
					float currHalfHeight = sprite.bounds.size.y * spriteGO.transform.localScale.y / 2;
					scale = destHalfHeight / currHalfHeight;
				}

			}

			spriteGO.transform.localScale = new Vector3(scale, scale, 1);
		}

		public static Collider2D GetTouchedCollider2D(Vector3 touchPos) {
			Vector3 worldPos3 = Camera.main.ScreenToWorldPoint(touchPos);
			Vector2 worldPos2 = new Vector2(worldPos3.x, worldPos3.y);

			Collider2D coll2D = Physics2D.OverlapPoint(worldPos2);
			RaycastHit2D hit2D = Physics2D.Raycast(worldPos2, Vector2.zero);

			if (coll2D)
				return coll2D;
			else if (hit2D)
				return hit2D.collider;
			return null;
		}

		public static GameObject GetTouchedGameObject(Vector3 touchPos) {
			Collider2D coll2D = GetTouchedCollider2D(touchPos);
			if (coll2D)
				return coll2D.gameObject;

			return null;
		}

		public static bool GameObjectTouched(GameObject gameObject, Vector3 touchPos) {
			Collider2D coll2D = gameObject.GetComponent<Collider2D>();
			if (coll2D) {
				Vector3 worldPos3 = Camera.main.ScreenToWorldPoint(touchPos);
				Vector2 worldPos2 = new Vector2(worldPos3.x, worldPos3.y);
				if (coll2D.OverlapPoint(worldPos2)) {
					return true;
				}
			} else {
				Collider coll = gameObject.GetComponent<Collider>();
				if (!coll) {
					coll = gameObject.GetComponentInChildren<Collider>();
				}
				if (coll) {
					const float rayLength = 100;
					Ray ray = Camera.main.ScreenPointToRay(touchPos);
					RaycastHit hit;
					if (coll.Raycast(ray, out hit, rayLength)) {
						//					Utils.Log(gameObject+" was hit at time="+Time.time);
						return true;
					}
				}
			}
			return false;
		}

		public static Color32 Color32Zero {
			get {
				return new Color32(0, 0, 0, 0);
			}
		}


		public static Color32 Color32White {
			get {
				return new Color32(255, 255, 255, 255);
			}
		}

		public static string GetVector3String(Vector3 v, int digits) {
			string digitTag = "F" + digits.ToString();
			if (digits >= 0) {
				return "(" + v.x.ToString(digitTag) + ", " + v.y.ToString(digitTag) + ", " + v.z.ToString(digitTag) + ")";
			} else {
				return GetVector3String(v);
			}
		}

		public static string GetVector3String(Vector3 v) {
			return "(" + v.x + "," + v.y + "," + v.z + ")";
		}

		public static string GetVector2String(Vector2 v, int digits) {
			string digitTag = "F" + digits.ToString();
			if (digits >= 0) {
				return "(" + v.x.ToString(digitTag) + "," + v.y.ToString(digitTag) + ")";
			} else {
				return GetVector2String(v);
			}
		}

		public static string GetVector2String(Vector2 v) {
			return "(" + v.x + "," + v.y + ")";
		}

		public static int GetColor32Int(Color32 color) {
			//			unchecked {
			//				int d  = unchecked((int)0xff000000);
			//
			//			}
			int c =
				(color.a << 24) |
				(color.r << 16) |
				(color.g << 8) |
				(color.b << 0);
			return c;
		}

		public static Color32 GetColor32FromInt(uint c) {
			return new Color32((byte)((c >> 16) & 0xff),
							   (byte)((c >> 8) & 0xff),
							   (byte)((c >> 0) & 0xff),
							   (byte)((c >> 24) & 0xff));
		}


		public static string GetColor32HexString(Color32 color) {
			return GetColor32Int(color).ToString("X");
		}

		public static string GetColor32String(Color32 color) {
			return "color=(" + color.a + " " + color.r + " " + color.g + " " + color.b + ")";
		}

		public static string GetColorString(Color color) {
			return "color=(" + color.a + " " + color.r + " " + color.g + " " + color.b + ")";
		}


		public static bool ByteMajor4BitsEqual(byte b1, byte b2) {
			return (b1 >> 4) == (b2 >> 4);
		}


		public static bool Color32ComponentMajor4BitsEqual(Color32 c1, Color32 c2) {
			return (
				ByteMajor4BitsEqual(c1.r, c2.r) &&
				ByteMajor4BitsEqual(c1.g, c2.g) &&
				ByteMajor4BitsEqual(c1.b, c2.b) &&
				ByteMajor4BitsEqual(c1.a, c2.a)
					);
		}

		public static bool Color32ComponentMajor4BitsEqualWithoutAlpha(Color32 c1, Color32 c2) {
			return (
				ByteMajor4BitsEqual(c1.r, c2.r) &&
				ByteMajor4BitsEqual(c1.g, c2.g) &&
				ByteMajor4BitsEqual(c1.b, c2.b)
				);
		}


		public static bool Colors32EqualWithoutAlpha(Color32 c1, Color32 c2) {
			return (c1.r == c2.r &&
					c1.g == c2.g &&
					c1.b == c2.b
					);
		}


		public static bool Colors32Equal(Color32 c1, Color32 c2) {
			return (c1.r == c2.r &&
				c1.g == c2.g &&
				c1.b == c2.b &&
				c1.a == c2.a);
		}

#if UNITY_EDITOR
		public static TouchPhase GetMouseTouchPhaseForButton(int buttonIndex) {
			if (Input.GetMouseButtonDown(buttonIndex)) {
				return TouchPhase.Began;
			} else if (Input.GetMouseButton(buttonIndex)) {
				return TouchPhase.Moved;
			} else if (Input.GetMouseButtonUp(buttonIndex)) {
				return TouchPhase.Ended;
			} else {
				return TouchPhase.Stationary;
			}
		}

		public static bool GetMouseButtonForTouchPhase(int buttonIndex, TouchPhase phase, out Vector3 outPos) {
			bool actionPosFound = false;
			outPos = Vector3.zero;
			switch (phase) {
				case TouchPhase.Began:
				actionPosFound = Input.GetMouseButtonDown(buttonIndex);
				break;
				case TouchPhase.Moved:
				actionPosFound = Input.GetMouseButton(buttonIndex);
				break;
				case TouchPhase.Ended:
				actionPosFound = Input.GetMouseButtonUp(buttonIndex);
				break;
				case TouchPhase.Stationary:
				actionPosFound = Input.GetMouseButton(buttonIndex);
				break;
				case TouchPhase.Canceled:
				return false;
				default:
				return false;
			}
			if (actionPosFound) {
				outPos = Input.mousePosition;
				return true;
			} else {
				return false;
			}
		}
#endif
		public static bool GetFirstTouchAtPhase(TouchPhase phase, out Vector3 outPos) {
			outPos = Vector3.zero;
			if (Input.touchCount > 0) {
				//Touch touch = Input.GetTouch (0);
				foreach (Touch touch in Input.touches) {
					//Utils.Log(" "+GetVector2String(touch.position)+" "+GetVector2String(touch.rawPosition));
					if (touch.phase == phase) {
						outPos = touch.position;
						return true;
					}
				}
				return false;
			} else {
#if UNITY_EDITOR
				if (Utils.GetMouseTouchPhaseForButton(0) == phase) {
					outPos = Input.mousePosition; ;
					return true;
				} else
					return false;
#else
				return false;
#endif
			}

		}


		//		public static Texture2D CreateRoundedRectTexture(int texWidth, int texHeight, Color c, int R) {
		//
		//			const int blurPixels = 2;
		//
		//			Texture2D texture =  new Texture2D(texWidth, texHeight);
		//			int RSq = R*R;
		//			float alphaR0 = R-blurPixels;
		//			float alphaMin = 0.0f;
		//			float alphaA = (1-alphaMin)/(alphaR0-R);
		//			float alphaB = 1-alphaA*alphaR0;
		//			
		//			for (int y = 0; y < texHeight; y++) {
		//				for (int x = 0; x < texWidth; x++) {
		//					texture.SetPixel(x,y,Color.clear);
		//				}
		//			}
		//			
		//			int begX, endX;
		//			int begY, endY;
		//			Color currColor;
		//			// draw circles
		//			{
		//				const int dr = 1;
		//				for (int j = 0; j < 2; j++) {
		//					begY = j * (texHeight - 2 * R);
		//					endY = begY + 2 * R;
		//					int middleY = begY + R + (j * 2 - 1) * dr;
		//					for (int i = 0; i < 2; i++) {
		//						begX = i * (texWidth - 2 * R);
		//						endX = begX + 2 * R;
		//						int middleX = begX + R + (i * 2 - 1) * dr;
		//						
		//						for (int y = begY; y <= endY; y++) {
		//							for (int x = begX; x <= endX; x++) {
		//								//							texture.SetPixel(x,y,Color.red);
		//								int rSq = Maths.PowI (x - middleX, 2) + Maths.PowI (y - middleY, 2);
		//								if (rSq < RSq) {
		//									currColor = texture.GetPixel(x,y);
		//									c.a = Mathf.Clamp (alphaA * Mathf.Sqrt (rSq) + alphaB, alphaMin, 1.0f);
		//									c.a = Mathf.Max(c.a,currColor.a);
		//									texture.SetPixel (x, y, c);
		//								} 
		//							}
		//						}
		//					}
		//				}
		//			}
		//			// draw rects
		//			{
		//				c.a = 1.0f;
		//				begX = R;
		//				endX = texWidth - R;
		//				begY = 0;
		//				endY = texHeight;
		//				for (int y = begY; y < endY; y++) {
		//					for (int x = begX; x < endX; x++) {
		//						texture.SetPixel (x, y, c);
		//					}
		//				}
		//				begX = 0;
		//				endX = 2 * R;
		//				begY = R;
		//				endY = texHeight - R;
		//				for (int i = 0; i < 2; i++) {
		//					begX = i * (texWidth - 2 * R);
		//					endX = begX + 2 * R;
		//					for (int y = begY; y < endY; y++) {
		//						for (int x = begX; x < endX; x++) {
		//							texture.SetPixel (x, y, c);
		//						}
		//					}
		//				}
		//			}
		//			//adjust borders alpha
		//			{
		//				float borderAlpha = 0.5f;
		//				
		//				
		//				int borderPixels = Mathf.Max( blurPixels-2,1 );
		////				int borderPixels = blurPixels;
		//				for (int j = 0; j < 2; j++) {
		//					int dy = j*(texHeight-borderPixels);
		//					for (int y = dy; y < dy+borderPixels; y++) {
		//						if (borderPixels>1) {
		//							int yP = y-dy;
		//							if (j==0) {
		//								borderAlpha = yP*1.0f/borderPixels; 
		//							} else {
		//								borderAlpha = 1.0f-yP*1.0f/borderPixels; 
		//							}
		//						}
		//						for (int x = 0; x < texWidth; x++) {
		//							currColor = texture.GetPixel(x,y);
		//							c.a = Mathf.Min(borderAlpha, currColor.a);
		//							texture.SetPixel(x,y,c);
		//						}
		//					}
		//				}
		//				borderAlpha = 0.5f;
		//				for (int i = 0; i < 2; i++) {
		//					int dx = i*(texWidth-borderPixels);
		//					for (int x = dx; x < dx+borderPixels; x++) {
		//						if (borderPixels>1) {
		//							int xP = x-dx;
		//							if (i==0) {
		//								borderAlpha = xP*1.0f/borderPixels; 
		//							} else {
		//								borderAlpha = 1.0f-xP*1.0f/borderPixels; 
		//							}
		//						}
		//						for (int y = 0; y < texHeight; y++) {
		//							currColor = texture.GetPixel(x,y);
		//							c.a = Mathf.Min(borderAlpha, currColor.a);
		//							texture.SetPixel(x,y,c);
		//						}			
		//					}
		//				}
		//			}
		//			texture.Apply();
		//			return texture;
		//		}
		//
		//
		//
		//		public static GameObject CreateRoundedRectSprite(int texWidth, int texHeight, Color c, int R, string name, int sortingOrder) {
		//			Rect rect  = new Rect(0,0,texWidth,texHeight);
		//			Texture2D texture = CreateRoundedRectTexture(texWidth, texHeight, c, R);
		//			GameObject go = new GameObject(name);
		//			SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
		//			sr.sortingOrder = sortingOrder;
		//			sr.sprite = Sprite.Create(texture, rect, new Vector2(0.5f,0.5f));
		//			return go;
		//		}
		//
		//		public static GameObject CreateRoundedRectSpriteWithBorder(int texWidth, int texHeight, int R, int border, Color c1, Color c2, string name, int sortingOrder) {
		//			GameObject empty1 = Utils.CreateRoundedRectSprite(texWidth,texHeight,c1,R,"external",sortingOrder);
		//			GameObject empty2 = Utils.CreateRoundedRectSprite(texWidth-2*border,texHeight-2*border,c2,R-border,"internal",sortingOrder+1);
		//			GameObject empty = new GameObject(name);
		//			empty1.transform.SetParent(empty.transform);
		//			empty2.transform.SetParent(empty.transform);
		//			return empty;
		//		}

	}
}

