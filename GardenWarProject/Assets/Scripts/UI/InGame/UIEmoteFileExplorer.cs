using System.Collections;
using System.IO;
using System.Linq;
using SimpleFileBrowser;
using UnityEngine;
using UnityEngine.UI;

public class UIEmoteFileExplorer : MonoBehaviour
{
    [SerializeField] private RawImage image;
    [SerializeField] private Button button;
    private EmotesManager manager => EmotesManager.instance;
    private byte index;

    private string path;
    private string filename;

    public void Init(byte newIndex)
    {
        this.index = newIndex;
        image.texture = manager.EmotesTexture2Ds[index];

        button.onClick.AddListener(OpenExplorer);
        
        // Set filters (optional)
        // It is sufficient to set the filters just once (instead of each time before showing the file browser dialog), 
        // if all the dialogs will be using the same filters
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Images", ".jpg", ".png"));

        // Set default filter that is selected when the dialog is shown (optional)
        // Returns true if the default filter is set successfully
        // In this case, set Images filter as the default filter
        FileBrowser.SetDefaultFilter(".jpg");

        // Set excluded file extensions (optional) (by default, .lnk and .tmp extensions are excluded)
        // Note that when you use this function, .lnk and .tmp extensions will no longer be
        // excluded unless you explicitly add them as parameters to the function
        FileBrowser.SetExcludedExtensions(".lnk", ".tmp", ".zip", ".rar", ".exe");

        // Add a new quick link to the browser (optional) (returns true if quick link is added successfully)
        // It is sufficient to add a quick link just once
        // Name: Users
        // Path: C:\Users
        // Icon: default (folder icon)
        FileBrowser.AddQuickLink("Desktop", "C:\\Users\\Desktop", null);
    }

    IEnumerator ShowLoadDialogCoroutine()
    {
        // Show a load file dialog and wait for a response from user
        // Load file/folder: both, Allow multiple selection: true
        // Initial path: default (Documents), Initial filename: empty
        // Title: "Load File", Submit button text: "Load"
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders, false, null, null,
            "Load Files and Folders", "Load");

        // Dialog is closed
        // Print whether the user has selected some files/folders or cancelled the operation (FileBrowser.Success)
        Debug.Log(FileBrowser.Success);

        if (FileBrowser.Success)
        {
            // Print paths of the selected files (FileBrowser.Result) (null, if FileBrowser.Success is false)
            string path = FileBrowser.Result.Aggregate(string.Empty, (current, t) => current + t);
            
            Debug.Log($"path : {path}");
            
            if (File.Exists(path))
            {
                byte[]fileData = File.ReadAllBytes(path);
                
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(fileData);
                
                image.texture = tex;
                GameSettingsManager.SetEmoteTexture(index,fileData);
            }
            else
            {
                Debug.Log("File not found");
            }
        }
    }

    private void OpenExplorer()
    {
        StartCoroutine(ShowLoadDialogCoroutine());
    }
}