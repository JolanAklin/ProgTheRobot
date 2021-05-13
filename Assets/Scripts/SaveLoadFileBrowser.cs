using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using SimpleFileBrowser;
using System;

// start tpi

/// <summary>
/// Inspired from there <see cref="https://github.com/yasirkula/UnitySimpleFileBrowser"/>
/// This class handle the file browser for saving and loading project
/// </summary>
public class SaveLoadFileBrowser : MonoBehaviour
{

	public static SaveLoadFileBrowser instance;
	private bool isBrowserOpened;

    private void Awake()
    {
		instance = this;
    }

    // Warning: paths returned by FileBrowser dialogs do not contain a trailing '\' character
    // Warning: FileBrowser can only show 1 dialog at a time

    /// <summary>
    /// Show a open file dialog
    /// </summary>
    /// <param name="onSuccess">Action done if a file is saved</param>
    /// <param name="onCancel">Action done if the dialogue is canceled</param>
    /// <returns>True if the dialogue has been opened</returns>
    public bool ShowLoadFileDialog(Action<string[]> onSuccess, Action onCancel)
    {
		if (isBrowserOpened)
			return false;
		isBrowserOpened = true;

		// Set filters (optional)
		// It is sufficient to set the filters just once (instead of each time before showing the file browser dialog), 
		// if all the dialogs will be using the same filters
		FileBrowser.SetFilters(true, new FileBrowser.Filter("Prog The Robot", ".pr"));


		// Set default filter that is selected when the dialog is shown (optional)
		// Returns true if the default filter is set successfully
		// In this case, set Images filter as the default filter
		FileBrowser.SetDefaultFilter(".pr");

		// Add a new quick link to the browser (optional) (returns true if quick link is added successfully)
		// It is sufficient to add a quick link just once
		// Name: Users
		// Path: C:\Users
		// Icon: default (folder icon)
		FileBrowser.AddQuickLink("Users", "C:\\Users", null);

        FileBrowser.ShowLoadDialog(
			(paths) => { onSuccess.Invoke(paths); isBrowserOpened = false; },
            () => { onCancel.Invoke(); isBrowserOpened = false; },
            FileBrowser.PickMode.Files, false, null, null, "Sélectionner un fichier à charger", "Sélectionner"
		);
		return true;
	}

    /// <summary>
    /// Show a save file dialog
    /// </summary>
    /// <param name="onSuccess">Action done if a file is selected</param>
    /// <param name="onCancel">Action done if the dialogue is canceled</param>
    /// <returns>True if the dialogue has been opened</returns>
    public bool ShowSaveFileDialog(Action<string[]> onSuccess, Action onCancel)
    {
        if (isBrowserOpened)
            return false;
        isBrowserOpened = true;

        // Set filters (optional)
        // It is sufficient to set the filters just once (instead of each time before showing the file browser dialog), 
        // if all the dialogs will be using the same filters
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Prog The Robot", ".pr"));


        // Set default filter that is selected when the dialog is shown (optional)
        // Returns true if the default filter is set successfully
        // In this case, set Images filter as the default filter
        FileBrowser.SetDefaultFilter(".pr");

        // Add a new quick link to the browser (optional) (returns true if quick link is added successfully)
        // It is sufficient to add a quick link just once
        // Name: Users
        // Path: C:\Users
        // Icon: default (folder icon)
        FileBrowser.AddQuickLink("Users", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), null);
        FileBrowser.AddQuickLink("Prog The Robot Projects", SaveManager.instance.savePath, null);

        FileBrowser.ShowSaveDialog( 
            (paths) => { onSuccess.Invoke(paths); isBrowserOpened = false; }, () => { onCancel.Invoke(); isBrowserOpened = false; }, FileBrowser.PickMode.Files, false, SaveManager.instance.savePath, "project.pr", "Sauvegarder", "Sauvegarder");;

        return true;
    }
}

// end tpi