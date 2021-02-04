using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TextureAtlasUnity.Model
{
    public class SpriteResourceData : IComparable<SpriteResourceData>
    {
        /*
        <summary>
            Name of Sprite with index included
        </summary>
        */
        public string SpriteName { get; }

        /*
        <summary>
            Name of the sprite
        </summary>
        */
        public string BaseName { get; }

        /*
        <summary>
            The path to the image in the asset directory.
        </summary>
        */
        public string ImageResourcePath { get; }

        /*
        <summary>
            Index of the sprite in an animation
        </summary>
        */
        public int Index { get; }

        /*
        <summary>
            The Unity Sprite Class
        </summary>
        */
        public Sprite Sprite { get; set; }

        /*
        <summary>
            Frames Per Second if provided by first index.
        </summary>
        */
        public Nullable<float> FPS {get; set;}

        /*
        <summary>
            Duration if provided else frame time will be calculated by FPS.
        </summary>
        */
        public Nullable<int> Duration { get; set; }

        public SpriteResourceData(int index, string baseName, string imageResourcePath, Nullable<float> fps, Nullable<int> duration)
        {
            this.Index = index;
            this.BaseName = baseName;
            this.SpriteName = GenerateImageMetaName(baseName, index);
            this.ImageResourcePath = imageResourcePath;
            Sprite = null;
            FPS = fps;
            Duration = duration;
        }

        /*
        <summary>
            Implement the generic CompareTo method with the SpriteResourceData
            class as the Type parameter.
        </summary>
        */
        public int CompareTo(SpriteResourceData other)
        {
            if (BaseName != other.BaseName)
            { 
                throw new ArgumentException(String.Format("BaseName is not the same for {0} and other {1}",this, other));
            }

            return Index.CompareTo(other.Index);
        }

        /*
        <summary>
            Generate Image name with index.
        </summary>
        */
        public static string GenerateImageMetaName(string name, int index)
        {
            return name + "_" + index.ToString();
        }
    }
}
