using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace LogReader {

	public class Log {

		public static void readFromTextFile(string file_path, List<string> log, bool logfilecontents)
		{
			if (System.IO.File.Exists (file_path)) {
				StreamReader file = new StreamReader (file_path);

				Debug.Log ("Loading from the file path: " + file_path);
				while (!file.EndOfStream) {
					string input_line = file.ReadLine ();
					if (input_line.Contains("Capturing resource" ) || input_line.Contains("Releasing resource")) {
						log.Add (input_line);
					}
				}

				if (logfilecontents) {
					Debug.LogWarning ("The file contents:\n");
					foreach (string line in log) {
						Debug.Log (line);
					}
				} else {
					Debug.Log ("The log file is loaded into a string array.");
				}

				file.Close ();
			} else {
				Debug.LogError ("File not found: " + file_path);
			}
		}
	}

	public class Parser {
		public static void parseLogForTasksStart(List<string> log, List<Task> tasks, List<Resource> resources) {

			List<string> resource_names = new List<string> ();

			foreach (string line in log) {
				string pattern = @"^\[(.*?)\].*Capturing resource '(.*?)'.*id '(.*?)'.*for task '(.*?)'.*";
				Match taskmatch = Regex.Match (line, pattern);

				if (taskmatch.Success) {
					Task newtask = new Task ();

					resource_names.Clear();

					foreach (Resource resource in resources) {
						resource_names.Add (resource.name);
					}

					newtask.starttime = taskmatch.Groups [1].ToString();
					newtask.resourcename = taskmatch.Groups [2].ToString();
					newtask.resource_id = taskmatch.Groups [3].ToString();
					if (!(resource_names.Contains(newtask.resourcename))) {
						Resource newresource = new Resource();
						newresource.name = newtask.resourcename;
						newresource.ID = newtask.resource_id;
						resources.Add(newresource);
					}
					newtask.task_id = taskmatch.Groups [4].ToString();
					//Debug.Log (newtask.resourcename + "(" + newtask.resource_id + ")" + ": New task added: " + newtask.task_id + ". Start time: " + newtask.starttime);
					tasks.Add (newtask);
				}

				string endpattern = @"^\[(.*?)\].* '(.*?)'.*id '(.*?)'.*from task '(.*?)'.*";
				Match endtaskmatch = Regex.Match (line, endpattern);

				if (endtaskmatch.Success) {

					string foundID = endtaskmatch.Groups [4].ToString();
					string endTime = endtaskmatch.Groups [1].ToString();

					foreach (Task task in tasks) {
						if (task.task_id == foundID) {
							task.endtime = endTime;
						}
					}

				}
			}

			/*
			foreach (Task task in tasks) {
				Debug.Log (task.resourcename + "(" + task.resource_id + ")" + ". Task ID: " + task.task_id + ". Start time: " + task.starttime + ". End time: " + task.endtime);
			}*/

			/*
			string temp_resourcelist = "";
			foreach (Resource resource in resources) {
				temp_resourcelist += "[" + resource.name + ", " + resource.ID + "]";
			}
			Debug.LogWarning ("Registered resources: " + temp_resourcelist);*/

		}


		public static void findAllTasksByResourceID (string resource_id, List<Task> tasks) {

			List<Task> filtered_list = new List<Task>();

			foreach (Task task in tasks) {
				if (task.resource_id == resource_id) {
					filtered_list.Add(task);
				}				
			}

			Debug.Log ("Listing all tasks for resource id " + resource_id);
			foreach (Task task in filtered_list) {
				Debug.Log ("Task ID: " + task.task_id + ". Start time: " + task.starttime + ". End time: " + task.endtime);
			}
		}


	}
}
