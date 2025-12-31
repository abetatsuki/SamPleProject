using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class CreateCustomScript
{
    private const string MenuItemPath = "Assets/Create/Custom C# Script";
    private const int MenuItemPriority = 80;
    private const string DefaultScriptName = "NewScript.cs";
    private const string TemplatePath = "Assets/Editor/CustomScriptTemplates/CustomScriptTemplate.txt";
    private const string ScriptsFolderPath = "Assets/Scripts/";

    [MenuItem(MenuItemPath, false, MenuItemPriority)]
    public static void CreateNewScript()
    {
        string selectedPath = GetSelectedPath();
        string filePath = ShowSaveFilePanel(selectedPath);

        if (string.IsNullOrEmpty(filePath))
        {
            // User cancelled the save operation
            return;
        }

        string scriptName = Path.GetFileNameWithoutExtension(filePath);
        string namespaceName = GetNamespaceFromPath(selectedPath);

        CreateScriptFile(filePath, scriptName, namespaceName);
        AssetDatabase.Refresh();
    }

    private static string GetSelectedPath()
    {
        string path = "Assets";
        foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                path = Path.GetDirectoryName(path);
                break;
            }
        }
        return path;
    }

    private static string ShowSaveFilePanel(string initialPath)
    {
        return EditorUtility.SaveFilePanelInProject(
            "Save Script",
            DefaultScriptName,
            "cs",
            "Please enter a file name to save the script to.",
            initialPath
        );
    }

    private static string GetNamespaceFromPath(string assetPath)
    {
        // Assets から始まらないパスは保険としてデフォルト namespace を返す
        if (!assetPath.StartsWith("Assets"))
        {
            return "Project";
        }

        //int pass = assetPath.Length;
        //if (pass <= 6)
        //{
        //     Debug.LogError("Asset直下なので作れません");
        //    return "Error";
        //}


        // Assets/ を除いた相対パスを取得する
        string relativePath = assetPath.Substring("Assets/".Length);

        // パス区切りを namespace 区切りに変換する
        string namespaceName = relativePath
            .Replace(Path.DirectorySeparatorChar, '.')
            .Replace(Path.AltDirectorySeparatorChar, '.')
            .Trim('.');

        // フォルダが Assets 直下だった場合に空になるのを防ぐ
        return string.IsNullOrEmpty(namespaceName)
            ? "Project"
            : namespaceName;
    }



    private static void CreateScriptFile(string filePath, string scriptName, string namespaceName)
    {
        string templateContent = File.ReadAllText(TemplatePath);
        templateContent = templateContent.Replace("#NAMESPACE#", namespaceName);
        templateContent = templateContent.Replace("#SCRIPTNAME#", scriptName);

        File.WriteAllText(filePath, templateContent);
    }
}