using UnityEngine;
using UnityEditor.Experimental.AssetImporters;
using System.IO;
using UnityEditor;
using System.Collections.Generic;
using LibGDXAtlasExtender.Model;
using TextureAtlasUnity.Model;
using System;
using System.Linq;
using UnityEditor.Animations;
using TextureAtlasUnity.Utility;

namespace AtlasTextureImporter
{
    [ScriptedImporter(1, "atlas")]
    public class UnityAtlasTextureImporter : ScriptedImporter
    {
        /*
        <summary>
            Make an Animation for a group of SpriteResourceData. This will generate a basic Animation controller of the same name
            and animation. The animation will also be added as a prefab asset.
        </summary>
        */
        private void MakeAnimation(AssetImportContext ctx, string animationName, List<SpriteResourceData> spriteAnimationData)
        {
            AnimationClip animClip = new AnimationClip();
            // First you need to create e Editor Curve Binding
            EditorCurveBinding curveBinding = new EditorCurveBinding();
            GameObject spriteGameObject=null;
            Animator animator=null;
            float adjustedFps = animClip.frameRate;

            // I want to change the sprites of the sprite renderer, so I put the typeof(SpriteRenderer) as the binding type.
            curveBinding.type = typeof(SpriteRenderer);
            // Regular path to the gameobject that will be changed (empty string means root)
            //curveBinding.path = "";
            curveBinding.path = "";
            // This is the property name to change the sprite of a sprite renderer
            curveBinding.propertyName = "m_Sprite";

            List<Sprite[]> SpriteListArray = new List<Sprite[]>();
            List<string> ImagePaths = new List<string>();

            // An array to hold the object keyframes
            ObjectReferenceKeyframe[] keyFrames = new ObjectReferenceKeyframe[spriteAnimationData.Count];
            for (int i = 0; i < spriteAnimationData.Count; i++)
            {

                if (!ImagePaths.Contains(spriteAnimationData[i].ImageResourcePath))
                {
                    ImagePaths.Add(spriteAnimationData[i].ImageResourcePath);
                    //AssetDatabase.LoadAssetAtPath
                    //Sprite[] sprites = Resources.LoadAll<Sprite>(spriteAnimationData[i].ImageResourcePath);
                    //Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(spriteAnimationData[i].ImageResourcePath).OfType<Sprite>().ToArray();
                    Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(spriteAnimationData[i].ImageResourcePath).OfType<Sprite>().ToArray();
                    if (sprites == null || sprites.Length == 0)
                    {
                        Debug.LogWarning(String.Format("Failed to load Sprites {0}", spriteAnimationData[i].ImageResourcePath));
                    }
                    SpriteListArray.Add(sprites);
                    for (int j = 0; j < sprites.Length; j++)
                    {
                        Debug.LogWarning(String.Format("SpriteName[{0}]: {1}", j, sprites[j].name));
                        if (sprites[j].name == spriteAnimationData[i].SpriteName)
                        {
                            spriteAnimationData[i].Sprite = sprites[j];
                        }
                    }
                    Debug.LogWarning(String.Format("Sprite Name: '{0}' index: {1} Object: {2}", spriteAnimationData[i].SpriteName, spriteAnimationData[i].Index, spriteAnimationData[i].Sprite));
                }
                else
                {
                    int spriteIdx = ImagePaths.IndexOf(spriteAnimationData[i].ImageResourcePath);
                    Sprite[] sprites = SpriteListArray[spriteIdx];
                    for (int j = 0; j < sprites.Length; j++)
                    {
                        if (sprites[j].name == spriteAnimationData[i].SpriteName)
                        {
                            spriteAnimationData[i].Sprite = sprites[j];
                        }
                    }
                    Debug.LogWarning(String.Format("Sprite Name: '{0}' index: {1} Object: {2}", spriteAnimationData[i].SpriteName, spriteAnimationData[i].Index, spriteAnimationData[i].Sprite));
                }
                if (i == 0)
                {
                    if (spriteAnimationData[i].FPS != null)
                    {
                        Debug.LogWarning(String.Format("Framerate: {0}", spriteAnimationData[i].FPS.Value));
                        animClip.frameRate = spriteAnimationData[i].FPS.Value;
                        adjustedFps = animClip.frameRate / spriteAnimationData[i].FPS.Value;
                    }
                    //Create GameObject
                    spriteGameObject = new GameObject(animationName, typeof(SpriteRenderer), typeof(Animator));
                    SpriteRenderer spriteRenderer = spriteGameObject.GetComponent<SpriteRenderer>();
                    spriteRenderer.sprite = spriteAnimationData[i].Sprite;
                    animator = spriteGameObject.GetComponent<Animator>();
                }
                keyFrames[i] = new ObjectReferenceKeyframe();
                // set the time
                if (spriteAnimationData[i].Duration != null)
                {
                    keyFrames[i].time = adjustedFps*spriteAnimationData[i].Duration.Value;
                }
                else
                {
                    keyFrames[i].time = ((float)i) * adjustedFps / animClip.frameRate;
                }
                // set reference for the sprite you want
                keyFrames[i].value = spriteAnimationData[i].Sprite;
                Debug.LogWarning(spriteAnimationData[i].Sprite);
            }
            AnimationUtility.SetObjectReferenceCurve(animClip, curveBinding, keyFrames);
            AssetDatabase.CreateAsset(animClip, "Assets/Animation/"+ animationName + ".anim");

            if (animator != null && spriteGameObject != null)
            {
                AnimatorController animController = AnimatorController.CreateAnimatorControllerAtPathWithClip("Assets/Animation/" + animationName + ".controller", animClip);
                animator.runtimeAnimatorController = animController;
                PrefabUtility.SaveAsPrefabAsset(spriteGameObject, "Assets/Prefabs/" + animationName + ".prefab");
                DestroyImmediate(spriteGameObject);
            }
        }

        public override void OnImportAsset(AssetImportContext ctx)
        {
            Dictionary<string, List<SpriteResourceData>> animations = new Dictionary<string, List<SpriteResourceData>>();

            TextureAtlasFile textureAtlasFile = TextureAtlasParserImporter.ImportFromFile(ctx.assetPath);
            string currentDirectory = UnityStringUtilities.RemoveFileName(ctx.assetPath);
            string resourcePath = "";
            Debug.Log("assetPath: " + ctx.assetPath);
            Debug.Log("currentDirectory: " + currentDirectory);

            foreach (var texture in textureAtlasFile.Textures)
            {
                //Update the texture based off of the atlas
                Debug.Log("texture.ImagePath: " + currentDirectory+texture.ImagePath);

                resourcePath = currentDirectory + texture.ImagePath;
                TextureImporter textureImporter = (TextureImporter)AssetImporter.GetAtPath(resourcePath);
                textureImporter.textureType = TextureImporterType.Sprite;
                textureImporter.spriteImportMode = SpriteImportMode.Multiple;
                textureImporter.mipmapEnabled = false;
                textureImporter.filterMode = FilterMode.Point;
                //Remove Compression to maintain better quality
                textureImporter.textureCompression = TextureImporterCompression.Uncompressed;

                List<SpriteMetaData> metas = new List<SpriteMetaData>();
                int yPos = 0;
                foreach (var subtexture in texture.Subtexture)
                {
                    SpriteMetaData meta = new SpriteMetaData();
                    // Convert y from top to Unity's bottom
                    yPos = texture.Height - subtexture.Height;
                    yPos = yPos - subtexture.Y;
                    meta.rect = new Rect(subtexture.X, yPos, subtexture.Width, subtexture.Height);
                    if (subtexture.Index != -1)
                    {
                        //meta.name = subtexture.Name + "_" + subtexture.Index.ToString();
                        meta.name = SpriteResourceData.GenerateImageMetaName(subtexture.Name, subtexture.Index);
                        if(animations.ContainsKey(subtexture.Name))
                        {
                            animations[subtexture.Name].Add( new SpriteResourceData(subtexture.Index, subtexture.Name, resourcePath, subtexture.FPS, subtexture.Duration) );
                        }
                        else
                        {
                            animations.Add(subtexture.Name, new List<SpriteResourceData>() { new SpriteResourceData(subtexture.Index, subtexture.Name, resourcePath, subtexture.FPS, subtexture.Duration) });
                        }
                    }
                    else
                    {
                        meta.name = subtexture.Name;
                    }
                    metas.Add(meta);
                }

                textureImporter.spritesheet = metas.ToArray();
                EditorUtility.SetDirty(textureImporter);
                textureImporter.SaveAndReimport();
            }

            foreach (var animation in animations)
            {
                Debug.Log("animation["+ animation.Key + "]="+animation.Value);
                animation.Value.Sort();

                if (animation.Value.Count > 0)
                {
                    MakeAnimation(ctx, animation.Key, animation.Value);
                    animation.Value.Clear();
                }
            }
        }
    }
}