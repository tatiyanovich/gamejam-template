using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Code.Gameplay.Input.Behaviours;
using Code.Gameplay.Neighbours.Behaviours;
using Code.Gameplay.Teacher;
using Code.Gameplay.Teacher.Behaviours;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Code.Editor.Art
{
	public static class CopycatArtBuilder
	{
		private const string Content = "Assets/AddressableResources/Content/";
		private const string Shared = "CopycatShared";
		private const string UserInterface = "UI/Copycat";
		private static readonly string ArtRoot = Path.GetFullPath(Path.Combine(Application.dataPath, "../../../art"));
		private static readonly string[] Folders =
		{
			"Classroom", "Characters/Kitten", "Characters/Teacher", "Characters/Neighbours",
			"Papers", "Duck", UserInterface
		};

		[MenuItem("COPYCAT/Art/Build D11")]
		public static void Build()
		{
			if (EditorApplication.isPlaying)
				throw new InvalidOperationException("Stop Play Mode before building art.");

			ImportTextures();
			ImportFonts();
			BuildPrefabs();
		}

		[MenuItem("COPYCAT/Art/Rebuild D11 Prefabs")]
		public static void BuildPrefabs()
		{
			if (EditorApplication.isPlaying)
				throw new InvalidOperationException("Stop Play Mode before building art.");

			BuildClassroom();
			BuildKitten();
			BuildTeacher();
			BuildNeighbours();
			BuildPapers();
			BuildDuck();
			BuildUserInterface();
			RegisterAddressables();
			BuildScene();
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			Debug.Log("D11: built classroom, rigs, papers, duck and UI prefabs.");
		}

		[MenuItem("COPYCAT/Art/Build E3 Neighbour Views")]
		public static void BuildNeighbourViews()
		{
			if (EditorApplication.isPlaying)
				throw new InvalidOperationException("Stop Play Mode before building neighbour views.");

			ConfigureNeighbourPrefab("Whiskerstein", "nerd_head");
			ConfigureNeighbourPrefab("Fluffy", "fluffy_head");
			AssetDatabase.SaveAssets();
			Debug.Log("E3: configured both neighbour views.");
		}

		private static JObject Read(int day)
		{
			string file = day >= 3 && day <= 5 ? "rig.json" : "layout.json";
			return JObject.Parse(File.ReadAllText(Path.Combine(ArtRoot, $"src/d{day}/{file}")));
		}

		private static Vector2 Point(JToken value)
		{
			return new Vector2((float)value[0], (float)value[1]);
		}

		private static Vector3 Local(Vector2 pixels)
		{
			return new Vector3(pixels.x / 100f, -pixels.y / 100f, 0f);
		}

		private static Vector3 World(Vector2 pixels)
		{
			return Local(pixels - new Vector2(960f, 540f));
		}

		private static void ImportTextures()
		{
			for (int day = 2; day <= 8; day++)
			{
				string destination = Content + Folders[day - 2];
				Directory.CreateDirectory(destination);
				foreach (string source in Directory.GetFiles(Path.Combine(ArtRoot, $"exports/d{day}"), "*.png"))
					File.Copy(source, Path.Combine(destination, Path.GetFileName(source)), true);
			}

			AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

			for (int day = 2; day <= 8; day++)
			{
				JObject layout = Read(day);
				List<JToken> assets = (layout[day == 2 ? "layers" : "assets"]).ToList();
				if (day == 6)
				{
					foreach (string direction in new[] { "up", "right", "down", "left" })
					foreach (string state in new[] { "normal", "done", "wrong" })
					{
						JObject glyph = (JObject)layout["glyphAsset"].DeepClone();
						glyph["name"] = $"glyph_arrow_{direction}_{state}";
						assets.Add(glyph);
					}
				}

				foreach (JToken asset in assets)
					ImportTexture(Folders[day - 2], asset);
			}
		}

		private static void ImportTexture(string folder, JToken asset)
		{
			string path = Content + folder + "/" + (string)asset["name"] + ".png";
			TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(path);
			bool icon = (string)asset["textureType"] == "Default";
			importer.textureType = icon ? TextureImporterType.Default : TextureImporterType.Sprite;
			importer.spriteImportMode = SpriteImportMode.Single;
			importer.spritePixelsPerUnit = (float?)asset["pixelsPerUnit"] ?? 200f;
			importer.filterMode = FilterMode.Bilinear;
			importer.mipmapEnabled = false;
			importer.wrapMode = TextureWrapMode.Clamp;
			importer.textureCompression = TextureImporterCompression.Uncompressed;
			importer.maxTextureSize = 4096;
			importer.npotScale = TextureImporterNPOTScale.None;
			importer.alphaIsTransparency = icon == false;
			importer.sRGBTexture = true;
			TextureImporterSettings settings = new();
			importer.ReadTextureSettings(settings);
			settings.spriteMeshType = SpriteMeshType.FullRect;
			settings.spriteAlignment = (int)SpriteAlignment.Custom;
			Vector2 pivot = Point(asset["pivotPixels"]);
			Vector2 size = Point(asset["size"]);
			settings.spritePivot = new Vector2(pivot.x / size.x, 1f - pivot.y / size.y);
			JToken border = asset["unityBorderPixels"];
			settings.spriteBorder = border == null ? Vector4.zero :
				new Vector4((float)border[0], (float)border[1], (float)border[2], (float)border[3]);
			importer.SetTextureSettings(settings);
			importer.SaveAndReimport();
		}

		private static void ImportFonts()
		{
			Directory.CreateDirectory(Content + Shared + "/Fonts");
			foreach (string name in new[] { "PatrickHand-Regular", "Nunito-Bold", "LuckiestGuy" })
			{
				string path = Content + Shared + "/Fonts/" + name;
				File.Copy(Path.Combine(ArtRoot, "fonts/" + name + ".ttf"), path + ".ttf", true);
				AssetDatabase.ImportAsset(path + ".ttf", ImportAssetOptions.ForceSynchronousImport);
				if (AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path + "-SDF.asset") != null)
					continue;

				TMP_FontAsset font = TMP_FontAsset.CreateFontAsset(AssetDatabase.LoadAssetAtPath<Font>(path + ".ttf"));
				font.name = name + "-SDF";
				font.atlasPopulationMode = AtlasPopulationMode.Dynamic;
				font.isMultiAtlasTexturesEnabled = true;
				AssetDatabase.CreateAsset(font, path + "-SDF.asset");
				AssetDatabase.AddObjectToAsset(font.material, font);
				foreach (Texture2D atlas in font.atlasTextures)
					AssetDatabase.AddObjectToAsset(atlas, font);
				string characters = new(Enumerable.Range(32, 95).Select(value => (char)value).ToArray());
				font.TryAddCharacters(characters + "—…’", out string missing);
				if (missing.Length > 0)
					throw new InvalidOperationException("Missing font glyphs: " + missing);
				EditorUtility.SetDirty(font);
			}
			foreach (string license in Directory.GetFiles(Path.Combine(ArtRoot, "fonts"), "*.txt"))
				File.Copy(license, Content + Shared + "/Fonts/" + Path.GetFileName(license), true);
		}

		private static Transform Node(Transform parent, string name, Vector3 position)
		{
			Transform node = new GameObject(name).transform;
			node.SetParent(parent, false);
			node.localPosition = position;
			return node;
		}

		private static Sprite Sprite(string folder, string name)
		{
			return AssetDatabase.LoadAssetAtPath<Sprite>(Content + folder + "/" + name + ".png");
		}

		private static SpriteRenderer Layer(Transform parent, string folder,
			(string Name, Vector3 Position, int Order) layer)
		{
			SpriteRenderer renderer = Node(parent, layer.Name, layer.Position).gameObject.AddComponent<SpriteRenderer>();
			renderer.sprite = Sprite(folder, layer.Name);
			if (renderer.sprite == null)
				throw new InvalidOperationException("Missing sprite: " + folder + "/" + layer.Name);
			renderer.sortingOrder = layer.Order;
			return renderer;
		}

		private static GameObject Save(Transform root, string folder)
		{
			GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root.gameObject, Content + folder + "/" + root.name + ".prefab");
			Object.DestroyImmediate(root.gameObject);
			return prefab;
		}

		private static TMP_FontAsset Font(string name)
		{
			return AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(Content + Shared + "/Fonts/" + name + "-SDF.asset");
		}

		private static Color Color(string name)
		{
			JObject palette = JObject.Parse(File.ReadAllText(Path.Combine(ArtRoot, "palette.json")));
			ColorUtility.TryParseHtmlString((string)palette[name], out Color color);
			return color;
		}

		private static TextMeshPro Text(Transform parent, string name,
			(string Value, Vector2 Baseline, float Size, float Width, int Order, string Font, string Color) data)
		{
			TextMeshPro label = Node(parent, name, Local(data.Baseline)).gameObject.AddComponent<TextMeshPro>();
			label.font = Font(data.Font);
			label.text = data.Value;
			label.fontSize = data.Size / 10f;
			label.color = Color(data.Color);
			label.rectTransform.pivot = new Vector2(0f, 0.5f);
			label.rectTransform.sizeDelta = new Vector2(data.Width / 100f, data.Size * 2f / 100f);
			label.alignment = TextAlignmentOptions.BaselineLeft;
			label.textWrappingMode = TextWrappingModes.NoWrap;
			label.GetComponent<MeshRenderer>().sortingOrder = data.Order;
			return label;
		}

		private static void BuildClassroom()
		{
			JObject layout = Read(2);
			Transform root = Node(null, "Classroom", Vector3.zero);
			foreach (JToken layer in layout["layers"])
			{
				Vector2 position = new((float)layer["x"], (float)layer["y"]);
				SpriteRenderer renderer = Layer(root, "Classroom",
					((string)layer["name"], World(position + Point(layer["pivotPixels"])), (int)layer["sortingOrder"]));
				renderer.transform.localEulerAngles = new Vector3(0f, 0f, -(float)layer["rotation"]);
				renderer.transform.localScale = Vector3.one * (float)layer["scale"];
			}
			Transform board = Node(root, "BlackboardText", World(Point(layout["boardText"]["origin"])));
			foreach (JToken line in layout["boardText"]["lines"])
			{
				string font = (string)line["font"] == "Luckiest Guy" ? "LuckiestGuy" : "PatrickHand-Regular";
				TextMeshPro label = Text(board, "Rule", ((string)line["text"],
					new Vector2((float)line["x"], (float)line["baseline"]), (float)line["fontSize"], 810f, 5, font, "PAPER"));
				label.rectTransform.pivot = new Vector2(0.5f, 0.5f);
				label.alignment = TextAlignmentOptions.Baseline;
			}
			Save(root, "Classroom");
		}

		private static void BuildKitten()
		{
			JObject rig = Read(3);
			Transform root = Node(null, "Kitten", Vector3.zero);
			root.gameObject.AddComponent<SortingGroup>().sortingOrder = 34;
			Vector2 pivot = Point(rig["rootPivot"]);
			List<Transform> poses = new();
			foreach (JProperty pose in ((JObject)rig["poses"]).Properties())
			{
				Transform state = Node(root, pose.Name, Local(new Vector2((float)pose.Value["x"], 0f)));
				poses.Add(state);
				state.localEulerAngles = new Vector3(0f, 0f, -(float)pose.Value["rotation"]);
				JToken profile = rig["headProfiles"][(string)pose.Value["profile"]];
				JToken expression = rig["expressions"][(string)pose.Value["expression"]];
				Transform head = Layer(state, "Characters/Kitten",
					((string)profile["asset"], Local(new Vector2(235f, 247f) - pivot), 6)).transform;
				head.name = "Head";
				foreach (JToken node in rig["nodes"])
				{
					if ((string)node["name"] == "head")
						continue;
					bool onHead = (string)node["parent"] == "head";
					Vector2 position = Point(node["position"]);
					if (onHead)
						position += Point(profile["earOffsets"][(string)node["name"] == "earLeft" ? 0 : 1]);
					else
						position -= pivot;
					Transform child = Layer(onHead ? head : state, "Characters/Kitten",
						((string)node["asset"], Local(position), (int)node["order"])).transform;
					child.name = (string)node["name"];
				}
				foreach (JToken eye in profile["eyes"])
				{
					Vector2 position = Point(eye) - new Vector2(235f, 247f);
					Transform white = Layer(head, "Characters/Kitten",
						((string)expression["eyeAsset"], Local(position), 7)).transform;
					white.localScale = Vector3.one * (float)expression["eyeScale"];
					Transform pupil = Layer(head, "Characters/Kitten",
						("kitten_pupil", Local(position + Point(profile["gaze"])), 8)).transform;
					pupil.localScale = Vector3.one * (float)expression["pupilScale"];
				}
				state.gameObject.SetActive(pose.Name == "idle");
			}
			root.gameObject.AddComponent<KittenView>().Configure(poses.ToArray());
			Save(root, "Characters/Kitten");
		}

		private static void BuildTeacher()
		{
			JObject rig = Read(4);
			Transform root = Node(null, "Teacher", Vector3.zero);
			Vector2 pivot = Point(rig["rootPivot"]);
			foreach (JProperty pose in ((JObject)rig["poses"]).Properties())
			{
				bool atDesk = pose.Name == "watching" || pose.Name == "staring" || pose.Name == "alerted";
				Transform state = Node(root, pose.Name, atDesk ? Local(new Vector2(0f, 297.5f)) : Vector3.zero);
				state.localScale = Vector3.one * (float)pose.Value["scale"];
				state.localEulerAngles = new Vector3(0f, 0f, -(float)pose.Value["rotation"]);
				state.gameObject.AddComponent<SortingGroup>().sortingOrder = atDesk ? 12 : 8;
				JToken profile = rig["profiles"][(string)pose.Value["profile"]];
				Layer(state, "Characters/Teacher", ((string)profile["body"], Vector3.zero, 0)).name = "Body";
				Transform head = Layer(state, "Characters/Teacher",
					((string)profile["head"], Local(Point(rig["placements"]["head"]) - pivot), 3)).transform;
				head.name = "Head";
				if ((bool)profile["chalk"])
				{
					Transform arm = Layer(state, "Characters/Teacher",
						("teacher_arm_chalk", Local(Point(rig["placements"]["armChalk"]) - pivot), 2)).transform;
					arm.localEulerAngles = new Vector3(0f, 0f, -((float?)pose.Value["armRotation"] ?? 0f));
				}
				if ((bool)profile["pointer"])
				{
					float scale = (float?)profile["pointerScaleX"] ?? 1f;
					Vector2 position = Point(rig["placements"]["pointer"]);
					position.x = position.x * scale + ((float?)profile["pointerOffsetX"] ?? 0f);
					Transform pointer = Layer(state, "Characters/Teacher",
						("teacher_pointer", Local(position - pivot), 1)).transform;
					pointer.localScale = new Vector3(scale, 1f, 1f);
				}
				foreach (JToken eye in profile["eyes"])
				{
					Vector2 position = Point(eye) - Point(rig["placements"]["head"]);
					float scaleX = (float?)profile["faceScaleX"] ?? 1f;
					float scaleY = (float?)profile["eyeScaleY"] ?? 1f;
					Layer(head, "Characters/Teacher", ("teacher_eye_white", Local(position), 4))
						.transform.localScale = new Vector3(scaleX, scaleY, 1f);
					Vector2 offset = Point(profile["pupilOffset"]);
					Layer(head, "Characters/Teacher", ("teacher_pupil", Local(position + offset), 5))
						.transform.localScale = new Vector3(scaleX, scaleY, 1f);
				}
				if ((bool)profile["glasses"])
				{
					float scale = (float)profile["faceScaleX"];
					Vector2 position = Point(rig["placements"]["glasses"]);
					position.x = position.x * scale + (float)profile["faceOffsetX"];
					Layer(head, "Characters/Teacher",
						("teacher_glasses", Local(position - Point(rig["placements"]["head"])), 6))
						.transform.localScale = new Vector3(scale, 1f, 1f);
				}
				state.gameObject.SetActive(pose.Name == "writing");
			}

			ConfigureTeacherView(root);
			Save(root, "Characters/Teacher");
		}

		private static void ConfigureTeacherView(Transform root)
		{
			TeacherView view = root.gameObject.AddComponent<TeacherView>();
			SerializedObject serialized = new(view);
			SerializedProperty poses = serialized.FindProperty("poses");
			string[] names = Enum.GetNames(typeof(TeacherAttention));
			poses.arraySize = names.Length;
			for (int index = 0; index < names.Length; index++)
				poses.GetArrayElementAtIndex(index).objectReferenceValue = root.Find(names[index].ToLowerInvariant());

			serialized.ApplyModifiedPropertiesWithoutUndo();
		}

		private static void BuildNeighbours()
		{
			JObject rig = Read(5);
			foreach (JProperty character in ((JObject)rig["characters"]).Properties())
			{
				Transform root = Node(null, (string)character.Value["displayName"], Vector3.zero);
				Vector2 pivot = Point(rig["rootPivot"]);
				Layer(root, "Characters/Neighbours", (character.Name + "_body", Vector3.zero, 18));
				Transform head = Layer(root, "Characters/Neighbours",
					(character.Name + "_head", Local(Point(rig["placements"]["head"]) - pivot), 24)).transform;
				Transform paw = Layer(root, "Characters/Neighbours",
					(character.Name + "_paw_cover", Local(Point(rig["placements"]["paw"]) - pivot), 23)).transform;
				paw.name = "Paw";
				foreach (JToken eye in character.Value["eyes"])
				{
					Vector2 position = Point(eye) - Point(rig["placements"]["head"]);
					Vector2 scale = Point(character.Value["eyeScale"]);
					Layer(head, "Characters/Neighbours", ("eye_white", Local(position), 25))
						.transform.localScale = new Vector3(scale.x, scale.y, 1f);
					Layer(head, "Characters/Neighbours",
						("pupil", Local(position + Point(character.Value["pupilOffset"])), 26));
				}
				ConfigureNeighbourView(root, paw, head);
				Save(root, "Characters/Neighbours");
			}
		}

		private static void ConfigureNeighbourPrefab(string characterName, string headName)
		{
			string path = Content + "Characters/Neighbours/" + characterName + ".prefab";
			GameObject root = PrefabUtility.LoadPrefabContents(path);
			try
			{
				ConfigureNeighbourView(root.transform, root.transform.Find("Paw"), root.transform.Find(headName));
				PrefabUtility.SaveAsPrefabAsset(root, path);
			}
			finally
			{
				PrefabUtility.UnloadPrefabContents(root);
			}
		}

		private static void ConfigureNeighbourView(Transform root, Transform paw, Transform head)
		{
			NeighbourView view = root.GetComponent<NeighbourView>();
			if (view == null)
				view = root.gameObject.AddComponent<NeighbourView>();

			view.Configure(paw, head);
		}

		private static void BuildPapers()
		{
			JObject layout = Read(6);
			Transform player = Node(null, "PlayerPaper", Vector3.zero);
			player.gameObject.AddComponent<SortingGroup>().sortingOrder = 32;
			Layer(player, "Papers", ("paper_player", Local(Point(layout["playerPaper"]["pivotPixels"])), 0));
			foreach (JProperty slot in ((JObject)layout["playerPaper"]["text"]).Properties())
			{
				JToken data = slot.Value;
				float baseline = (float?)data["baseline"] ?? (float)data["baselines"][0];
				string value = (string)data["value"] ?? "";
				if (slot.Name == "student")
					value = "Student:";
				Text(player, slot.Name, (value, new Vector2((float)data["x"], baseline),
					(float)data["fontSize"], 540f, 1, "PatrickHand-Regular", "PENCIL_INK"));
			}
			Text(player, "Answer", ("____", new Vector2(206f, 336f), 44f, 380f, 1,
				"PatrickHand-Regular", "PENCIL_INK"));
			Transform stamp = Layer(player, "Papers",
				("stamp_copied", Local(Point(layout["playerPaper"]["stamp"]["center"])), 2)).transform;
			stamp.localEulerAngles = new Vector3(0f, 0f, 12f);
			stamp.gameObject.SetActive(false);
			Node(player, "AnswerGlyphs", Local(Point(layout["playerPaper"]["answerRow"]["origin"])));
			Save(player, "Papers");

			Transform neighbour = Node(null, "NeighbourPaper", Vector3.zero);
			neighbour.gameObject.AddComponent<SortingGroup>().sortingOrder = 22;
			Layer(neighbour, "Papers", ("paper_neighbour", Local(Point(layout["neighbourPaper"]["pivotPixels"])), 0));
			Text(neighbour, "StudentName", ("", new Vector2(96f, 116f), 32f, 360f, 1,
				"PatrickHand-Regular", "PENCIL_INK"));
			TextMeshPro word = Text(neighbour, "Word", ("", new Vector2(246f, 272f), 70f, 420f, 1,
				"PatrickHand-Regular", "PENCIL_INK"));
			word.rectTransform.pivot = new Vector2(0.5f, 0.5f);
			word.alignment = TextAlignmentOptions.Baseline;
			for (int index = 0; index < 4; index++)
			{
				TextMeshPro option = Text(neighbour, "Pick" + (index + 1), ("",
					Point(layout["neighbourPaper"]["pick"]["cells"][index]), 34f, 180f, 1,
					"PatrickHand-Regular", "PENCIL_INK"));
				option.rectTransform.pivot = new Vector2(0.5f, 0.5f);
				option.alignment = TextAlignmentOptions.Center;
			}
			Node(neighbour, "StrokeGlyphs", Local(new Vector2(240f, 246f)));
			Transform circle = Layer(neighbour, "Papers", ("glyph_pick_circle", Vector3.zero, 2)).transform;
			circle.localScale = Vector3.one * 0.9f;
			circle.gameObject.SetActive(false);
			Save(neighbour, "Papers");

			Transform ringRoot = Node(null, "PawTimer", Vector3.zero);
			RectTransform ring = Rectangle(ringRoot, "Canvas", new Rect(0f, 0f, 160f, 160f));
			ring.pivot = new Vector2(0.5f, 0.5f);
			Canvas canvas = ring.gameObject.AddComponent<Canvas>();
			canvas.renderMode = RenderMode.WorldSpace;
			canvas.sortingOrder = 27;
			ring.localScale = Vector3.one / 100f;
			Image track = Picture(ring, ("Papers", "ring_timer_track"), new Rect(0f, 0f, 160f, 160f));
			Image fill = Picture(ring, ("Papers", "ring_timer"), new Rect(0f, 0f, 160f, 160f));
			fill.type = Image.Type.Filled;
			fill.fillMethod = Image.FillMethod.Radial360;
			fill.fillOrigin = (int)Image.Origin360.Top;
			fill.fillClockwise = true;
			fill.fillAmount = 0f;
			GameplayWindowBuilder.ConfigurePawTimer(ringRoot.gameObject);
			Save(ringRoot, "Papers");
		}

		private static void BuildDuck()
		{
			Transform root = Node(null, "Duck", Vector3.zero);
			foreach (string name in new[] { "duck_idle", "duck_fly_1", "duck_fly_2", "duck_sad" })
			{
				SpriteRenderer frame = Layer(root, "Duck", (name, Vector3.zero, 33));
				frame.gameObject.SetActive(name == "duck_idle");
			}
			Save(root, "Duck");
		}

		private static RectTransform Rectangle(Transform parent, string name, Rect rectangle)
		{
			RectTransform transform = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
			transform.SetParent(parent, false);
			transform.anchorMin = new Vector2(0f, 1f);
			transform.anchorMax = new Vector2(0f, 1f);
			transform.pivot = new Vector2(0f, 1f);
			transform.anchoredPosition = new Vector2(rectangle.x, -rectangle.y);
			transform.sizeDelta = rectangle.size;
			return transform;
		}

		private static Image Picture(Transform parent, (string Folder, string Name) sprite, Rect rectangle)
		{
			Image image = Rectangle(parent, sprite.Name, rectangle).gameObject.AddComponent<Image>();
			image.sprite = Sprite(sprite.Folder, sprite.Name);
			image.type = image.sprite != null && image.sprite.border != Vector4.zero ? Image.Type.Sliced : Image.Type.Simple;
			image.raycastTarget = false;
			return image;
		}

		private static TextMeshProUGUI Label(Transform parent, string value, Rect rectangle)
		{
			TextMeshProUGUI label = Rectangle(parent, "Label", rectangle).gameObject.AddComponent<TextMeshProUGUI>();
			label.font = Font("Nunito-Bold");
			label.fontSize = 36f;
			label.color = Color("INK");
			label.text = value;
			label.alignment = TextAlignmentOptions.Center;
			label.raycastTarget = false;
			return label;
		}

		private static void BuildUserInterface()
		{
			RectTransform panel = Rectangle(null, "PaperPanel", new Rect(0f, 0f, 720f, 350f));
			Picture(panel, (UserInterface, "panel_paper_9slice"), new Rect(0f, 0f, 720f, 350f));
			Save(panel, UserInterface);

			Image buttonImage = Picture(null, (UserInterface, "button_yellow_9slice"), new Rect(0f, 0f, 460f, 100f));
			buttonImage.name = "YellowButton";
			buttonImage.raycastTarget = true;
			Button button = buttonImage.gameObject.AddComponent<Button>();
			button.targetGraphic = buttonImage;
			button.transition = Selectable.Transition.SpriteSwap;
			button.spriteState = new SpriteState
			{
				highlightedSprite = Sprite(UserInterface, "button_yellow_9slice_hover"),
				selectedSprite = Sprite(UserInterface, "button_yellow_9slice_hover"),
				pressedSprite = Sprite(UserInterface, "button_yellow_9slice_pressed"),
				disabledSprite = Sprite(UserInterface, "button_yellow_9slice")
			};
			Label(button.transform, "", new Rect(32f, 16f, 396f, 64f));
			Save(button.transform, UserInterface);

			RectTransform suspicion = Rectangle(null, "SuspicionMeter", new Rect(0f, 0f, 420f, 100f));
			Image track = Picture(suspicion, (UserInterface, "bar_fill"), new Rect(15f, 50f, 390f, 36f));
			track.name = "Track";
			track.sprite = null;
			track.color = Color("PAPER_SHADE");
			Image fill = Picture(suspicion, (UserInterface, "bar_fill"), new Rect(15f, 50f, 390f, 36f));
			fill.type = Image.Type.Filled;
			fill.fillMethod = Image.FillMethod.Horizontal;
			fill.fillOrigin = (int)Image.OriginHorizontal.Left;
			fill.fillAmount = 0f;
			Picture(suspicion, (UserInterface, "bar_frame"), new Rect(0f, 36f, 420f, 64f));
			TextMeshProUGUI suspicionLabel = Label(suspicion, "SUSPICION", new Rect(0f, 0f, 420f, 32f));
			suspicionLabel.font = Font("LuckiestGuy");
			suspicionLabel.fontSize = 28f;
			Save(suspicion, UserInterface);

			RectTransform meow = Rectangle(null, "MeowMeter", new Rect(0f, 0f, 240f, 240f));
			Picture(meow, (UserInterface, "meow_circle"), new Rect(0f, 0f, 240f, 240f));
			Image meowFill = Picture(meow, (UserInterface, "meow_fill"), new Rect(24f, 24f, 192f, 192f));
			meowFill.type = Image.Type.Filled;
			meowFill.fillMethod = Image.FillMethod.Vertical;
			meowFill.fillOrigin = (int)Image.OriginVertical.Bottom;
			meowFill.fillAmount = 0f;
			meowFill.color = Color("OK");
			Picture(meow, (UserInterface, "meow_threshold_line"), new Rect(24f, 114f, 192f, 12f));
			Label(meow, "MEOW", new Rect(0f, 96f, 240f, 52f));
			Save(meow, UserInterface);

			Image chip = Picture(null, (UserInterface, "chip_hud"), new Rect(0f, 0f, 456f, 88f));
			chip.name = "HudChip";
			Label(chip.transform, "", new Rect(24f, 12f, 408f, 64f));
			Save(chip.transform, UserInterface);
			Image vignette = Picture(null, (UserInterface, "vignette_radial"), new Rect(0f, 0f, 1920f, 1080f));
			vignette.name = "DangerVignette";
			vignette.color = new Color(1f, 0.6f, 0.24f, 0f);
			vignette.rectTransform.anchorMax = Vector2.one;
			vignette.rectTransform.anchorMin = Vector2.zero;
			vignette.rectTransform.sizeDelta = Vector2.zero;
			Save(vignette.transform, UserInterface);
			Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>(Content + UserInterface + "/icon_app.png");
			PlayerSettings.SetIcons(NamedBuildTarget.Unknown, new[] { icon }, IconKind.Any);
		}

		private static void RegisterAddressables()
		{
			AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
			foreach (string folder in new[] { "Classroom", "Characters", "Papers", "Duck", UserInterface, Shared })
			{
				string name = folder == UserInterface ? "UI" : folder == Shared ? "Shared" : folder;
				AddressableAssetGroup group = settings.FindGroup("Copycat_" + name);
				if (group == null)
					group = settings.CreateGroup("Copycat_" + name, false, false, true, null,
						typeof(BundledAssetGroupSchema), typeof(ContentUpdateGroupSchema));
				BundledAssetGroupSchema schema = group.GetSchema<BundledAssetGroupSchema>();
				schema.BuildPath.SetVariableByName(settings, AddressableAssetSettings.kLocalBuildPath);
				schema.LoadPath.SetVariableByName(settings, AddressableAssetSettings.kLocalLoadPath);
				schema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackTogether;
				AddressableAssetEntry entry = settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(Content + folder), group);
				entry.address = "copycat_" + name.ToLowerInvariant() + "_folder";
				foreach (string path in Directory.GetFiles(Content + folder, "*.prefab", SearchOption.AllDirectories))
				{
					AddressableAssetEntry prefab = settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(path), group);
					string key = System.Text.RegularExpressions.Regex.Replace(Path.GetFileNameWithoutExtension(path),
						"([a-z])([A-Z])", "$1_$2").ToLowerInvariant();
					prefab.address = "copycat_" + key + "_prefab";
				}
			}
			settings.SetDirty(AddressableAssetSettings.ModificationEvent.BatchModification, null, true);
		}

		private static Transform Instantiate(Transform parent, string path, Vector3 position)
		{
			GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(Content + path + ".prefab");
			GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(asset, parent);
			instance.transform.localPosition = position;
			return instance.transform;
		}

		private static void BuildScene()
		{
			UnityEngine.SceneManagement.Scene scene = EditorSceneManager.OpenScene("Assets/Scenes/Gameplay.unity");
			foreach (GameObject existing in scene.GetRootGameObjects())
			{
				if (existing.name == "CopycatArt" || existing.name == "------------Enviroment---------------")
					Object.DestroyImmediate(existing);
			}
			Transform root = Node(null, "CopycatArt", Vector3.zero);
			Transform classroom = Instantiate(root, "Classroom/Classroom", Vector3.zero);
			JObject kitten = Read(3);
			Instantiate(root, "Characters/Kitten/Kitten", World(Point(kitten["sceneOrigin"]) + Point(kitten["rootPivot"])));
			JObject teacher = Read(4);
			Transform teacherInstance = Instantiate(root, "Characters/Teacher/Teacher",
				World(Point(teacher["sceneOrigins"]["writing"]) + Point(teacher["rootPivot"]) * (float)teacher["sceneScale"]));
			teacherInstance.localScale = Vector3.one * (float)teacher["sceneScale"];
			JObject neighbour = Read(5);
			float scale = (float)neighbour["sceneScale"];
			Vector2 left = Point(neighbour["sceneOrigin"]) + Point(neighbour["rootPivot"]) * scale;
			Transform nerd = Instantiate(root, "Characters/Neighbours/Whiskerstein", World(left));
			nerd.localScale = Vector3.one * scale;
			Transform fluffy = Instantiate(root, "Characters/Neighbours/Fluffy", World(new Vector2(1920f - left.x, left.y)));
			fluffy.localScale = new Vector3(-scale, scale, 1f);
			JObject papers = Read(6);
			JToken playerPlacement = papers["playerPaper"]["placement"];
			Transform player = Instantiate(root, "Papers/PlayerPaper",
				World(new Vector2((float)playerPlacement["x"], (float)playerPlacement["y"])));
			player.localScale = Vector3.one * (float)playerPlacement["scale"];
			player.localEulerAngles = new Vector3(0f, 0f, -(float)playerPlacement["rotation"]);
			foreach (string side in new[] { "left", "right" })
			{
				JToken placement = papers["neighbourPaper"]["placements"][side];
				Transform desk = classroom.Find((string)placement["desk"]);
				Transform paper = Instantiate(desk, "Papers/NeighbourPaper",
					Local(new Vector2((float)placement["x"] - 310f, (float)placement["y"] - 130f)));
				paper.localScale = Vector3.one * (float)placement["scale"];
				TextMeshPro studentName = paper.Find("StudentName").GetComponent<TextMeshPro>();
				studentName.text = side == "left" ? "Whiskerstein" : "Fluffy";
				PrefabUtility.RecordPrefabInstancePropertyModifications(studentName);
				JToken ring = papers["ringTimer"]["placements"][side];
				Transform timer = Instantiate(root, "Papers/PawTimer", World(new Vector2((float)ring["x"], (float)ring["y"])));
				timer.localScale = Vector3.one * (float)ring["scale"];
			}
			JObject duck = Read(7);
			JToken duckPlacement = duck["desk"]["duck"];
			Instantiate(root, "Duck/Duck", World(new Vector2((float)duckPlacement["x"], (float)duckPlacement["y"])));
			PrefabUtility.RecordPrefabInstancePropertyModifications(teacherInstance);
			foreach (Transform transform in root.GetComponentsInChildren<Transform>(true))
			{
				if (PrefabUtility.IsPartOfPrefabInstance(transform))
					PrefabUtility.RecordPrefabInstancePropertyModifications(transform);
			}
			EditorSceneManager.SaveScene(scene);
			ConfigureCamera();
		}

		private static void ConfigureCamera()
		{
			string path = Content + "3D/Camera/CameraView.prefab";
			GameObject cameraRoot = PrefabUtility.LoadPrefabContents(path);
			try
			{
				Camera camera = cameraRoot.GetComponentInChildren<Camera>();
				camera.orthographic = true;
				camera.orthographicSize = 5.4f;
				camera.backgroundColor = Color("INK");
				PrefabUtility.SaveAsPrefabAsset(cameraRoot, path);
			}
			finally
			{
				PrefabUtility.UnloadPrefabContents(cameraRoot);
			}
		}
	}
}
