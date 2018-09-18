using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using LogReader;

public class Main : MonoBehaviour {
	
	public static Main id;

	public bool LogFileContents = false;

	string logfolder = "";
	string logpath = "";
	StreamReader file;

	List<string> log = new List<string>();
	List<Task> tasklist = new List<Task>();
	List<Resource> resourcelist = new List<Resource>();

	public void LoadLogFromPath () {

		logfolder = UI.id.inputField.text;
		Debug.Log ("Log path selected: " + logfolder);
		logpath = logfolder + "svc.veeambackup.log";
		Log.readFromTextFile (logpath, log, LogFileContents);
		//Debug.Log (file.ReadToEnd());


		Parser.parseLogForTasksStart (log, tasklist, resourcelist);

		UI.id.redrawDropdownResourceMenu (resourcelist);

		Parser.findAllTasksByResourceID ("9ecacd23-b7cc-4a48-a468-f9ee65fd10d7", tasklist);

	}

	public void Awake () {
		if (id == null) {
			id = this;
		} else {			
			Debug.LogError ("Another instance of ::UI.cs:: is already running!");
		}
	}
}
