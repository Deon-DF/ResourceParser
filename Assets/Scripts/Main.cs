using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using LogReader;

public class Main : MonoBehaviour {
	
	public static Main id;


	string logfolder = "";
	string logpath = "";
	StreamReader file;

	List<string> tasklog = new List<string>();
	List<string> namelog = new List<string>();

	List<Task> tasklist = new List<Task>();
	List<Proxy> proxylist = new List<Proxy>();

	public void LoadLogFromPath ()
	{

		logfolder = UI.id.inputField.text;

		if (!(logfolder.EndsWith ("\\"))) {
			Debug.Log ("The log folder does not end with " + "\\");
			logfolder += @"\";
		}

		Debug.Log ("Log folder selected: " + logfolder);
		logpath = logfolder + "svc.veeambackup.log";
		Log.readFromTextFile (logpath, tasklog, namelog);

		Parser.parseResourceRequests (tasklog, tasklist, proxylist);

		//UI.id.redrawDropdownResourceMenu (resourcelist);
		//Parser.nameAllTasks(namelog, tasklist);
		Debug.Log("Done!");

	}
	/*
	public void listTasksForResourceName ()
	{
		string id = "";
		string name = UI.id.dropdown.options [UI.id.dropdown.value].text;

		Debug.Log ("Listing all tasks for resource: " + name);
		foreach (Resource resource in resourcelist) {
			if (name == resource.name) {
				id = resource.id;
			}
		}
		Debug.LogWarning("Resource name: " + name + ", resource id: " + id);

		Parser.findAllTasksByResourceID (id, tasklist);
	}*/

	public void Awake () {
		if (id == null) {
			id = this;
		} else {			
			Debug.LogError ("Another instance of ::UI.cs:: is already running!");
		}
	}
}
