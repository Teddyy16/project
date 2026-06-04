#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class CreateAxolotlMemoryScene
{
    [MenuItem("Tools/Axolotl Memory/Create Ready Phone Scene")]
    public static void CreateReadyPhoneScene()
    {
        SetSpritesForUnity();

        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        GameObject game = new GameObject("AxolotlMemoryGame");
        game.AddComponent<AxolotlMemoryGame>();

        string folder = "Assets/AxolotlMemoryGame/Scenes";
        if (!AssetDatabase.IsValidFolder(folder))
        {
            AssetDatabase.CreateFolder("Assets/AxolotlMemoryGame", "Scenes");
        }

        EditorSceneManager.SaveScene(scene, folder + "/AxolotlMemoryGame.unity");
        Selection.activeGameObject = game;
        Debug.Log("Ready phone scene created: Assets/AxolotlMemoryGame/Scenes/AxolotlMemoryGame.unity. Press Play.");
    }

    private static void SetSpritesForUnity()
    {
        string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/AxolotlMemoryGame/Resources/AxolotlCards" });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) continue;

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.maxTextureSize = 2048;
            importer.SaveAndReimport();
        }

        AssetDatabase.Refresh();
    }
}
#endif
