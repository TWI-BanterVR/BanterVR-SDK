using UnityEditor;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.VersionControl;
using System.Xml.Linq;

public class BanterBuilderWindow : EditorWindow
{
	private const string packageName = "com.bantervr.sdk";
	
    private string scenePath;
    private BuildTarget[] buildTargets = new BuildTarget[] { BuildTarget.Android, BuildTarget.StandaloneWindows };
    private bool[] buildTargetFlags = new bool[] { true, true };
    private VisualElement m_DropArea;

    //bool showAllShadersValue;

    bool selectAllShadersValue;

    bool selectNoneShadersValue;

    BanterBuilderBundleMode mode = BanterBuilderBundleMode.None;

    private string DEFAULT_SCENE_PATH = "Not set. Drag a scene file over the image above to begin.";

    [MenuItem("Banter/Banter Builder")]
    static void OpenWindow() {
        BanterBuilderWindow window = GetWindow<BanterBuilderWindow>();
        window.titleContent = new GUIContent("Banter Builder");
        window.Init();
        
    }

    private void OnEnable()
    {
        // UnityEditor.PackageManager.Client.Add("com.unity.xr.management");
        // UnityEditor.PackageManager.Client.Add("com.unity.xr.oculus");
        // PlayerSettings.stereoRenderingPath = StereoRenderingPath.SinglePass;
        rootVisualElement.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>($"Packages/{packageName}/Editor/dnd.uss"));
        SetupDropArea();
        SetupOptions();
    }
    
    private void Init() {
        // Project level prefs would be better here, or else the scene name travel across different projects which is no bueno. Possible solution - https://github.com/TheAllenChou/unity-project-prefs
        mode = (BanterBuilderBundleMode)EditorPrefs.GetInt("BanterBuilder_Mode", 0);
        if (mode == BanterBuilderBundleMode.Scene) {
            scenePath = EditorPrefs.GetString("BanterBuilder_ScenePath", "");
        } else {
            scenePath = ""; 
        }

        if (mode == BanterBuilderBundleMode.Scene && string.IsNullOrWhiteSpace(scenePath)) {
            mode = BanterBuilderBundleMode.None;
        }
        
        buildTargetFlags[0] = EditorPrefs.GetBool("BanterBuilder_BuildTarget_Android", true);
        buildTargetFlags[1] = EditorPrefs.GetBool("BanterBuilder_BuildTarget_Windows", true);
       // showAllShadersValue = EditorPrefs.GetBool("BanterBuilder_ShowAllShaders", false);
        includeAndroid.value = buildTargetFlags[0];
        includeWindows.value = buildTargetFlags[1];
        sceneName.text = scenePath;
    }


    void SetupDropArea() {
        
        m_DropArea = new VisualElement();
        
        m_DropArea.AddToClassList("droparea");
        
        rootVisualElement.Add(m_DropArea);

        m_DropArea.RegisterCallback<DragUpdatedEvent>(OnDragUpdate);
        m_DropArea.RegisterCallback<DragPerformEvent>(OnDragPerform);

        DragAndDrop.objectReferences = new UnityEngine.Object[] {};
    }

    UnityEngine.UIElements.Label sceneName;
    List<string> warningList;
    ListView listView;
    //List<Shader> shaderList;
    ListView kitListView;

    List<KitObjectAndPath> kitObjectList = new List<KitObjectAndPath>();

    //void SetSelectedItems() {
    //    List<string> previouslySelected = new List<string>();
    //    if (EditorPrefs.HasKey("BanterBuilder_SelectedShaders")) {
    //        previouslySelected = EditorPrefs.GetString("BanterBuilder_SelectedShaders").Split(',').ToList();
    //    }
    //    List<int> selectedIndices = new List<int>();
    //    if(selectNoneShadersValue) {
    //        selectNoneShadersValue = false;
    //    }else if(selectAllShadersValue) {
    //        for(int i = 0; i < shaderList.Count; i++) {
    //            selectedIndices.Add(i);
    //        }
    //        selectAllShadersValue = false;
    //    }else{
    //        for(int i = 0; i < shaderList.Count; i++) {
    //            if(previouslySelected.IndexOf(shaderList[i].name) > -1) {
    //                selectedIndices.Add(i);
    //            }
    //        }   
    //    }
    //    shaderListView.SetSelection(selectedIndices);
    //}

    //void HandleSelectedItems(ListView shaderListView) {
    //    shaderListView.onSelectionChange += objects => {
    //        EditorPrefs.SetString("BanterBuilder_SelectedShaders", String.Join(",", objects.Select(shader => ((Shader)shader).name).ToArray()));
    //    };
    //    SetSelectedItems();
    //}
    
    UnityEngine.UIElements.Toggle includeWindows;
    UnityEngine.UIElements.Toggle includeAndroid;
    UnityEngine.UIElements.Toggle showAllShaders;
    UnityEngine.UIElements.Button removeSelected;
    UnityEngine.UIElements.Button selectNoneShaders;
    UnityEngine.UIElements.Button buildButton;
    //void SetShaderList() {
    //    if(showAllShadersValue) {
    //        shaderList = new List<Shader>(ShaderUtil.GetAllShaderInfo()
    //        .Where(x => {
    //                var path = AssetDatabase.GetAssetPath(Shader.Find(x.name));
    //                return path.StartsWith("Assets/") && path.EndsWith(".shader");
    //            }
    //        ).Select(x => Shader.Find(x.name)));
    //    }else{
    //        shaderList = new List<Shader>();
    //        foreach(Transform t in GameObject.FindObjectsOfType<Transform>(true)) {
    //            if(t != null) {
    //                var renderer = t.GetComponent<Renderer>();
    //                if(renderer != null) {
    //                    var materials = renderer.sharedMaterials;  
    //                    for( int i = 0; i < materials.Length; i++)
    //                    {
    //                        if(materials[i] != null) {
    //                            var path = AssetDatabase.GetAssetPath(materials[i].shader);
    //                            var isInAssetsFolder = path.StartsWith("Assets/") && path.EndsWith(".shader");
    //                            if(isInAssetsFolder && !shaderList.Contains(materials[i].shader)) {
    //                                shaderList.Add(materials[i].shader);
    //                            }
    //                        }
    //                    }
    //                }
    //            }

    //        }
    //    }
    //    if(shaderListView != null) {
    //        shaderListView.itemsSource = shaderList;
    //        shaderListView.Rebuild();
    //        SetSelectedItems();
    //    }
    //}
    void LoadKitObjectList()
    {
        kitObjectList.Clear();
        if (mode == BanterBuilderBundleMode.Kit)
        {
            
            if (EditorPrefs.HasKey("BanterBuilder_SelectedKitObjects"))
            {
                var kitPaths = EditorPrefs.GetString("BanterBuilder_SelectedKitObjects").Split(',').ToList();

                foreach (var kitPath in kitPaths)
                {
                    var obj = GetKitObject(kitPath); 
                    if (obj == null) { continue; }
                    if (!kitObjectList.Any(x => x.path == kitPath))
                    {
                        kitObjectList.Add(new KitObjectAndPath() { obj = obj, path = kitPath });
                    }
                }
            }
        }
    }
    UnityEngine.UIElements.Label mainTitle;
    UnityEngine.UIElements.Label shaderTitle;
    UnityEngine.UIElements.Label sceneTitle;
    //VisualElement containerCustomShaders;

    void SetupOptions() {
        var container = new VisualElement();
        var toggleContainer = new VisualElement();

        mainTitle = new UnityEngine.UIElements.Label();
        mainTitle.text = "<b></b>";
        mainTitle.AddToClassList("scene-title");

        rootVisualElement.Add(mainTitle);

        sceneName = new UnityEngine.UIElements.Label();
        sceneName.AddToClassList("scene-name");

        rootVisualElement.Add(sceneName);

        includeAndroid = new UnityEngine.UIElements.Toggle();
        includeAndroid.text = "Build for Android";
        includeAndroid.AddToClassList("toggle-box");
        includeAndroid.value = buildTargetFlags[0];

        includeWindows = new UnityEngine.UIElements.Toggle();
        includeWindows.text = "Build for Windows";
        includeWindows.AddToClassList("toggle-box");
        includeWindows.value = buildTargetFlags[1];
        
        includeAndroid.RegisterCallback<MouseUpEvent>(IncludeAndroid);
        includeWindows.RegisterCallback<MouseUpEvent>(IncludeWindows);

        toggleContainer.Add(includeAndroid);
        toggleContainer.Add(includeWindows);

        container.Add(toggleContainer);
        buildButton = new UnityEngine.UIElements.Button();
        buildButton.AddToClassList("build-button");
        container.Add(buildButton);
        buildButton.text = "Build";
        buildButton.RegisterCallback<MouseUpEvent>(BuildAssetBundles);
        
        container.AddToClassList("horizontal");

        rootVisualElement.Add(container);

        shaderTitle = new UnityEngine.UIElements.Label();
        shaderTitle.text = "<b>Objects included in Kit Bundle:</b>";
        shaderTitle.AddToClassList("warning-title");

        rootVisualElement.Add(shaderTitle);

        
        //UnityEngine.UIElements.Label shaderDesc = new UnityEngine.UIElements.Label();
        //shaderDesc.AddToClassList("toggle-box");
        //shaderDesc.text = "Select Custom Shaders to Pack with this bundle (Multi-select with Shift/Ctrl). \nOnly shaders in the Assets folder are supported.";

        //rootVisualElement.Add(shaderDesc);

        //containerCustomShaders = new VisualElement();
        //showAllShaders = new UnityEngine.UIElements.Toggle();
        //showAllShaders.text = "Show All";
        //showAllShaders.AddToClassList("toggle-box");

        //showAllShaders.RegisterCallback<MouseUpEvent>(ShowAllShaders);
        //showAllShaders.value = showAllShadersValue;
        //containerCustomShaders.Add(showAllShaders);

        removeSelected = new UnityEngine.UIElements.Button();
        removeSelected.text = "Remove Selected Objects";
        removeSelected.AddToClassList("toggle-box");

        removeSelected.RegisterCallback<MouseUpEvent>(RemoveSelectedObjects);
        rootVisualElement.Add(removeSelected);

        //selectNoneShaders = new UnityEngine.UIElements.Button();
        //selectNoneShaders.text = "Select None";
        //selectNoneShaders.AddToClassList("toggle-box");

        //selectNoneShaders.RegisterCallback<MouseUpEvent>(SelectNoneShaders);
        //containerCustomShaders.Add(selectNoneShaders);

       // containerCustomShaders.AddToClassList("horizontal-no-stretch");
       // rootVisualElement.Add(containerCustomShaders);

        //SetShaderList();
        LoadKitObjectList();
        const int itemHeight = 16;
        
        kitListView = new ListView(kitObjectList, itemHeight, () => new Label(), (e, i) => (e as Label).text = i + ". " + kitObjectList[i].obj.name);
        kitListView.selectionType = SelectionType.Multiple;

        // HandleSelectedItems(shaderListView);

        kitListView.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
        // shaderListView.showFoldoutHeader = true;
        // shaderListView.headerTitle = "Select Custom Shaders to Pack (Multi-select with Shift/Ctrl)";
        kitListView.reorderMode = ListViewReorderMode.Simple;
        kitListView.AddToClassList("toggle-box");
        rootVisualElement.Add(kitListView);

        UnityEngine.UIElements.Label warningTitle = new UnityEngine.UIElements.Label();
        warningTitle.text = "<b>Messages:</b>";
        warningTitle.AddToClassList("warning-title");

        rootVisualElement.Add(warningTitle);

        warningList = new List<string>();

        listView = new ListView(warningList, itemHeight, () => new Label(), (e, i) => (e as Label).text = i + ". " + warningList[i]);

        listView.selectionType = SelectionType.None;

        // listView.onItemsChosen += obj => Debug.Log(obj);
        // listView.onSelectionChange += objects => Debug.Log(objects);

        listView.style.flexGrow = 1.0f;
        rootVisualElement.Add(listView);
        RefreshView();
    }

    //void SelectNoneShaders(MouseUpEvent _) {
    //    selectNoneShadersValue = true;
    //    SetShaderList();
    //}

    void RemoveSelectedObjects(MouseUpEvent _)
    {
        foreach (var sel in kitListView.selectedItems.Cast<KitObjectAndPath>())
        {
            kitObjectList.Remove(sel);
        }
        RefreshView();
       // selectAllShadersValue = true;
       // SetShaderList();
    }

    //void ShowAllShaders(MouseUpEvent _) {
    //    EditorPrefs.SetBool("BanterBuilder_ShowAllShaders", showAllShaders.value);
    //    showAllShadersValue = showAllShaders.value;
    //    SetShaderList();
    //}
    void IncludeAndroid(MouseUpEvent _) {
        EditorPrefs.SetBool("BanterBuilder_BuildTarget_Android", includeAndroid.value);
        buildTargetFlags[0] = includeAndroid.value;
    }

    void IncludeWindows(MouseUpEvent _) {
        EditorPrefs.SetBool("BanterBuilder_BuildTarget_Windows", includeWindows.value);
        buildTargetFlags[1] = includeWindows.value;
    }

    void OnDisable() {
        if(m_DropArea != null) {
            m_DropArea.UnregisterCallback<DragUpdatedEvent>(OnDragUpdate);
            m_DropArea.UnregisterCallback<DragPerformEvent>(OnDragPerform);
        }
        includeAndroid.UnregisterCallback<MouseUpEvent>(IncludeAndroid);
        includeWindows.UnregisterCallback<MouseUpEvent>(IncludeWindows);
      //  showAllShaders.UnregisterCallback<MouseUpEvent>(ShowAllShaders);
    }

    void AddStatus(string text) {
        warningList.Add( text);
        if(warningList.Count > 300) {
            warningList = warningList.GetRange(0, 300);
        }
        listView.Rebuild();
    } 

    private void BuildAssetBundles(MouseUpEvent _) {
        if (mode == BanterBuilderBundleMode.None)
        {
            AddStatus("Nothing to build...");
            return;
        }
        if (mode == BanterBuilderBundleMode.Scene && string.IsNullOrEmpty(scenePath)) {
            AddStatus("No scene selected...");
            return;
        }
        if (mode == BanterBuilderBundleMode.Kit && kitObjectList.Count < 1)
        {
            AddStatus("No objects selected...");
            return;
        }
        if (EditorUtility.DisplayDialog("Are you sure?", "This will clear any files in the output folder.", "Continue", "Cancel")) {

            string assetBundleDirectory = "BanterAssetBundles";
            AddStatus("Build started...");

            if (Directory.Exists(assetBundleDirectory)) {
                Directory.Delete(assetBundleDirectory, true);
            }
            
            Directory.CreateDirectory(assetBundleDirectory);
            if (mode == BanterBuilderBundleMode.None)
            {
                throw new Exception("Nothing to build!");
            } else if (mode == BanterBuilderBundleMode.Scene && string.IsNullOrWhiteSpace(scenePath))
            {
                throw new Exception("No scene to build!");
            } else if (mode == BanterBuilderBundleMode.Kit && kitObjectList.Count < 1)
            {
                throw new Exception("No kit objects to build!");
            }

            List<string> names = new List<string>();
            for (int i = 0; i < buildTargets.Length; i++) {
                if (buildTargetFlags[i]) {
                    string newAssetBundleName = "bundle";
                    string platform = buildTargets[i].ToString().ToLower();
                    AssetBundleBuild abb = new AssetBundleBuild();

                    if (mode == BanterBuilderBundleMode.Scene) {
                        string[] parts = scenePath.Split("/");
                        newAssetBundleName = parts[parts.Length - 1].Split(".")[0].ToLower() + "_" + platform; // + "_" + DateTime.Now.ToString("dd-MM-yyyy_hh-mm-ss")
                        AddStatus("Building: " + newAssetBundleName);
                        
                        AssetImporter.GetAtPath(scenePath).SetAssetBundleNameAndVariant(newAssetBundleName, string.Empty);
                        abb.assetNames = new[] { scenePath }; 
                    } else if (mode == BanterBuilderBundleMode.Kit) {
                        newAssetBundleName = "kitbundle_" + platform;
                        AddStatus("Building: " + newAssetBundleName);
                        abb.assetNames = kitObjectList.Select(x => x.path).ToArray();
                    } else
                    {
                        continue;
                    }
                    abb.assetBundleName = newAssetBundleName;
                    BuildPipeline.BuildAssetBundles(assetBundleDirectory, new[] { abb }, BuildAssetBundleOptions.None, buildTargets[i]);
                    names.Add(newAssetBundleName);
                    if(File.Exists(assetBundleDirectory + "/" + newAssetBundleName + ".manifest")){
                        File.Delete(assetBundleDirectory + "/" + newAssetBundleName + ".manifest");
                    }
                }
            }
            if(File.Exists(assetBundleDirectory + "/" + assetBundleDirectory + ".manifest")){
                File.Delete(assetBundleDirectory + "/" + assetBundleDirectory + ".manifest");
            }
            if(File.Exists(assetBundleDirectory + "/" + assetBundleDirectory )){
                File.Delete(assetBundleDirectory + "/" + assetBundleDirectory );
            }
            if(names.Count > 0) {
                EditorUtility.RevealInFinder(assetBundleDirectory + "/" + names[0]);
            }
            AddStatus("Build finished: " + DateTime.Now.ToString("dd-MM-yyyy_hh:mm:ss"));
        }
    }


    private static List<Type> ALLOWED_KIT_TYPES = new List<Type>()
    {
        typeof(GameObject),
        typeof(Material),
        typeof(Shader)
    };

    void OnDragUpdate(DragUpdatedEvent _) {
        DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
    }
    void OnDragPerform(DragPerformEvent _) {
        if (DragAndDrop.paths.Length < 1) {
            return;
        }
        var sceneFileDrop = DragAndDrop.paths.FirstOrDefault(x => x.EndsWith(".unity"));
        bool isScene = sceneFileDrop != null;
        if (isScene)
        {
            sceneName.text = scenePath = sceneFileDrop;
            mode = BanterBuilderBundleMode.Scene;
        } else
        {
            sceneName.text = scenePath = "";
            foreach (var dropped in DragAndDrop.paths)
            {
                var obj = GetKitObject(dropped);
                if (obj == null) {
                    continue;
                }
                if (!kitObjectList.Any(x=> x.path == dropped))
                {
                    kitObjectList.Add(new KitObjectAndPath() { obj = obj, path = dropped });
                }
            }
            if (kitObjectList.Count > 0)
            {
                mode = BanterBuilderBundleMode.Kit;
            }
            
        }

        EditorPrefs.SetString("BanterBuilder_SelectedKitObjects", String.Join(",", kitObjectList.Select(ko => ko.path).ToArray()));
        EditorPrefs.SetString("BanterBuilder_ScenePath", scenePath);

        RefreshView();
        //if(!string.IsNullOrEmpty(DragAndDrop.paths[0])) {
        //    var sceneDropped = DragAndDrop.paths[0].EndsWith(".unity");
        //    if (sceneDropped && mode == BanterBuilderBundleMode.Kit)
        //    {
        //        ClearKitList();
        //    } else
        //    {
        //        sceneName.text = scenePath = Dra
        //    }
        //    sceneName.text = scenePath = DragAndDrop.paths[0];
        //    EditorPrefs.SetString("BanterBuilder_ScenePath", scenePath);
        //} else {
        //    AddStatus("Is this a scene file?? " + DragAndDrop.paths[0]);
        //    Debug.LogError("Is this a scene file?? " + DragAndDrop.paths[0]);
        //}
    }

    private UnityEngine.Object GetKitObject(string path)
    {
        var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
        if (obj == null)
        {
            Debug.LogWarning("Couldn't load asset at path " + path);
            return null;
        }
        if (!ALLOWED_KIT_TYPES.Contains(obj.GetType()))
        {
            Debug.LogWarning($"Asset at path {path} isn't a valid kit bundle object type, it is {obj.GetType().Name}.  Allowed types are: {string.Join(", ", ALLOWED_KIT_TYPES.Select(x => x.Name))}");
            return null;
        }
        return obj;
    }


    private void RefreshView()
    {
        shaderTitle.visible = false;
        sceneName.visible = false;
        kitListView.visible = false;
        removeSelected.visible = false;
        if (mode == BanterBuilderBundleMode.Kit)
        {
            mainTitle.text = "<b>Building a Banter Kit Bundle</b>";
            shaderTitle.visible = true;
            removeSelected.visible = true;
            kitListView.visible= true;
            
            kitListView.itemsSource = kitObjectList;
            kitListView.Rebuild();
            removeSelected.visible = (kitObjectList.Count > 0);
            
        } else if (mode == BanterBuilderBundleMode.Scene)
        {
            mainTitle.text = "<b>Building a Banter Scene Bundle</b>";
            sceneName.visible = true;
            sceneName.text = scenePath;
        } else
        {
            mainTitle.text = "<b></b>";
        }
        //????
        
    }
}

public enum BanterBuilderBundleMode
{
    None = 0,
    Scene = 1,
    Kit = 2    
}
public class KitObjectAndPath
{
    public UnityEngine.Object obj;
    public string path;
}