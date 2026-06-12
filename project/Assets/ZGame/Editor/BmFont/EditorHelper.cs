using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
namespace ZGame.Editor.BmFont
{
    public class EditorHelper : MonoBehaviour
    { 
        [MenuItem("Assets/ZGame/BatchCreateArtistFont")]
        static public void BatchCreateArtistFont()
        {
            ArtistFont.BatchCreateArtistFont();
        }
    }
}