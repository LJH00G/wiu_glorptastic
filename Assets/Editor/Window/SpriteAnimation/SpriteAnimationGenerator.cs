using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Editor.SpriteAnimation
{
    public class SpriteAnimationGenerator : EditorWindow
    {

        Texture2D spriteSheet;
        string folderPath;
        SpriteAnimationFormatSO animationFormat;
        bool canGenerate;
        string validateSpriteAnimDataMsg = "";
        bool folderPathExists;
        string validateFolderPathMsg = "";

        const string DEFAULT_FOLDER_PATH = "Assets/Animations";
        GUIStyle invalidStyle_label;
        GUIStyle validStyle_label;


        [MenuItem("Tools/Glorptastic_Sunk/Sprite Animation Generator")]
        public static void ShowWindow()
        {
            GetWindow<SpriteAnimationGenerator>("Sprite Animation Generator");
        }


        List<Sprite> GetSpritesFromSpriteSheet()
        {
            var sprites = AssetDatabase.LoadAllAssetsAtPath(
                AssetDatabase.GetAssetPath(spriteSheet)
            );

            List<Sprite> spriteList = new();

            foreach (var obj in sprites)
            {
                if (obj is Sprite sprite)
                    spriteList.Add(sprite);
            }

            return spriteList;
        }

        void Generate()
        {
            Debug.Log("Generating...");


            var spriteList = GetSpritesFromSpriteSheet();
            spriteList.Sort(
                (a, b) =>
                {
                    int result = b.rect.y.CompareTo(a.rect.y);
                    return result == 0 ? a.rect.x.CompareTo(b.rect.x) : result;
                }
            );


            if (animationFormat.RepetitionSuffixes.Count == 0)
                CreateAnimationAsset();
            else
                for (int i = 0; i < animationFormat.RepetitionSuffixes.Count; i++)
                    CreateAnimationAsset(i);


            Debug.Log("Generation completed");


            void CreateAnimationAsset(int repetitionIndex = 0)
            {
                // create animation clips
                AnimationClip clip = new()
                {
                    frameRate = animationFormat.FrameRate
                };

                AnimationUtility.SetAnimationClipSettings(
                    clip,
                    new AnimationClipSettings
                    {
                        loopTime = animationFormat.Loop
                    }
                );


                // make key frames with keyDinitions
                int keyFrameCount = animationFormat.TotalKeyFrameCount();
                ObjectReferenceKeyframe[] keys = new ObjectReferenceKeyframe[keyFrameCount];

                float timePerFrame = 1 / animationFormat.FrameRate;

                int keyDefinitionIndex = 0;
                var currentKeyDefinition = animationFormat.KeyDefinitions[keyDefinitionIndex];
                int keyDefinitionFramesLeft = currentKeyDefinition.frames;
                int repetitionSpriteIndexOffset = animationFormat.RepetitionSpriteIndexActualOffset * repetitionIndex;

                for (int i = 0; i < keyFrameCount; i++)
                {
                    if (keyDefinitionFramesLeft <= 0)
                    {
                        keyDefinitionIndex++;
                        currentKeyDefinition = animationFormat.KeyDefinitions[keyDefinitionIndex];
                        keyDefinitionFramesLeft = currentKeyDefinition.frames;
                    }

                    keys[i] = new ObjectReferenceKeyframe
                    {
                        time = i * timePerFrame,
                        value = spriteList[currentKeyDefinition.spriteIndex + repetitionSpriteIndexOffset]
                    };

                    keyDefinitionFramesLeft--;
                }


                // set binding of keyframes
                EditorCurveBinding binding =
                    EditorCurveBinding.PPtrCurve(
                        "Sprite",
                        typeof(SpriteRenderer),
                        "m_Sprite"
                    );


                /// bind keyframes to clip with binding
                AnimationUtility.SetObjectReferenceCurve(
                    clip,
                    binding,
                    keys
                );


                string suffix = "";
                if (animationFormat.AnimationSuffix != "")
                    suffix += "_" + animationFormat.AnimationSuffix;
                if (animationFormat.RepetitionSuffixes.Count != 0)
                    suffix += "_" + animationFormat.RepetitionSuffixes[repetitionIndex];

                string filePath = $"{DEFAULT_FOLDER_PATH}";
                if (folderPath != "")
                    filePath += $"/{folderPath}";
                filePath += $"/{spriteSheet.name}{suffix}.anim";

                if (AssetDatabase.LoadAssetAtPath<AnimationClip>(filePath))
                {
                    if (!EditorUtility.DisplayDialog(
                        "Animation Already Exists",
                        $"\"{filePath}\" already exists.\nOverwrite it?",
                        "Overwrite",
                        "Cancel"))
                    {
                        return;
                    }

                    AssetDatabase.DeleteAsset(filePath);
                }

                AssetDatabase.CreateAsset(
                    clip,
                    filePath
                );

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Debug.Log($"Generated asset at \"{filePath}\"");
            }
        }


        void ValidateSpriteAnimationData()
        {

            if (!animationFormat)
            {
                canGenerate = false;
                validateSpriteAnimDataMsg = "Animation Format must not be left empty";
                return;
            }


            if (!spriteSheet)
            {
                canGenerate = false;
                validateSpriteAnimDataMsg = "Sprite Sheet must not be left empty";
                return;
            }


            var spriteList = GetSpritesFromSpriteSheet();

            int totalSpritesRequired = animationFormat.TotalSpritesRequired();
            canGenerate = totalSpritesRequired == spriteList.Count;

            if (!canGenerate)
                validateSpriteAnimDataMsg = $"Sprite Sheet must contain required amount of sprites for this format, required sprites count: {totalSpritesRequired}";
            else
                validateSpriteAnimDataMsg = "animation can be generated";

        }


        void ValidateFolderPath()
        {
            if (!spriteSheet || !animationFormat)
            {
                validateFolderPathMsg = "Sprite Sheet and Animation Format must exist before folder path can be validated";
                return;
            }

            string path = $"{DEFAULT_FOLDER_PATH}";
            if (folderPath != "")
                path += $"/{folderPath}";

                folderPathExists = AssetDatabase.IsValidFolder(path);

            string suffix = "";
            if (animationFormat.AnimationSuffix != "")
                suffix += "_" + animationFormat.AnimationSuffix;

            if (folderPathExists)
                validateFolderPathMsg = $"the asset will be created at \"{path}/{spriteSheet.name}{suffix}[_possible_suffix].anim\"";
            else
                validateFolderPathMsg = $"\"{path}\" does not exist, please make the folder first";
        }


        private void OnGUI()
        {
            
            GUILayout.Label(
                "Sprite Animation Generator",
                EditorStyles.boldLabel
            );



            EditorGUI.BeginChangeCheck();

            spriteSheet = (Texture2D)EditorGUILayout.ObjectField(
                "Sprite Sheet",
                spriteSheet,
                typeof(Texture2D),
                false
            );

            animationFormat = (SpriteAnimationFormatSO)EditorGUILayout.ObjectField(
                "Animation Format",
                animationFormat,
                typeof(SpriteAnimationFormatSO),
                false
            );

            if (EditorGUI.EndChangeCheck())
            {
                ValidateSpriteAnimationData();
                ValidateFolderPath();
            }

            EditorGUILayout.LabelField(
                validateSpriteAnimDataMsg,
                canGenerate ? validStyle_label : invalidStyle_label
            );



            EditorGUI.BeginChangeCheck();

            folderPath = EditorGUILayout.TextField(
                "Folder Path",
                folderPath
                );

            if (EditorGUI.EndChangeCheck())
                ValidateFolderPath();

            EditorGUILayout.LabelField(
                validateFolderPathMsg,
                folderPathExists ? validStyle_label : invalidStyle_label
            );



            if (GUILayout.Button("Generate") && canGenerate && folderPathExists)
                Generate();

        }




        private void OnEnable()
        {
            invalidStyle_label = new GUIStyle(EditorStyles.label);
            invalidStyle_label.normal.textColor = Color.red;

            validStyle_label = new GUIStyle(EditorStyles.label);
            validStyle_label.normal.textColor = Color.green;

            ValidateSpriteAnimationData();
            ValidateFolderPath();
        }
    }
}