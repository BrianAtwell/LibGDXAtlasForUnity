using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace TextureAtlasUnity.Utility
{
    /*
        <summary>
            Contains Unity Animation Utility functions
        </summary>
    */
    static class UnityAnimationUtilities
    {
        /*
        <summary>
            Prints out a test animation
        </summary>
        */
        public static void PrintTestObject(string path, string animName)
        {
            EditorCurveBinding[] curveBindings;
            AnimationClip animClip;
            ObjectReferenceKeyframe[] keyFrames = null;
            // path+animName should be something like "Assets/Animation/Stickman_0.anim"
            animClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path+animName+".anim");
            curveBindings = AnimationUtility.GetObjectReferenceCurveBindings(animClip);
            if (curveBindings.Length > 0)
            {
                keyFrames = AnimationUtility.GetObjectReferenceCurve(animClip, curveBindings[0]);
            }
            using (StreamWriter sw = new StreamWriter(animName+"anim_out.txt"))
            {
                sw.WriteLine("Animation Name: " + animClip.name);
                foreach (var curveBinding in curveBindings)
                {
                    sw.WriteLine("Path: " + curveBinding.path);
                    sw.WriteLine("PropertyName: " + curveBinding.propertyName);
                    sw.WriteLine("Type: " + curveBinding.type.ToString());
                }

                if (keyFrames != null)
                {
                    sw.WriteLine("Keyframes:");
                    int count = 0;
                    foreach (var frame in keyFrames)
                    {
                        sw.WriteLine("Keyframe[" + count + "]: time: " + frame.time);
                        count++;
                    }
                }
            }

        }
    }
}
