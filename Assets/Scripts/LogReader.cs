using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace LogReader {

	public class Log {

		public static void readFromTextFile(string file_path, List<string> tasklog, List<string> namelog)
		{
			if (System.IO.File.Exists (file_path)) {
				StreamReader file = new StreamReader (file_path);

				Debug.Log ("Loading from the file path: " + file_path);
				while (!file.EndOfStream) {
					string input_line = file.ReadLine ();
					if (input_line.Contains("Capturing resource" ) || input_line.Contains("Releasing resource")) {
						tasklog.Add (input_line);
					} else if (input_line.Contains("Mark task as response")) {
						namelog.Add (input_line);
					}
				}

				Debug.Log ("The log file is loaded into a string array.");

				file.Close ();
			} else {
				Debug.LogError ("File not found: " + file_path);
			}
		}
	}

	public class Parser {
		public static void parseLogForTasksStart(List<string> tasklog, List<Task> tasks, List<Resource> resources) {

			List<string> resource_names = new List<string> ();

			foreach (string line in tasklog) {
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
		}


		public static void nameAllTasks (List<string> namelog, List<Task> tasklist)
		{
			foreach (string line in namelog) {

				string pattern = "^\\[(.*?)\\].*\\[(.*?)\\].*response given, task '(.*?) from job \"(.*?)\".*task id:(.*?),.*";
				Match namematch = Regex.Match (line, pattern);

				string found_name = "Job: " + namematch.Groups [4].ToString () + ", Task: " + namematch.Groups [3].ToString ();
				string found_id = namematch.Groups [5].ToString();

				foreach (Task task in tasklist) {
					if (task.task_id == found_id) {
						task.taskname = found_name;
						//Debug.Log (found_id + " = " + found_name);
					}
				}
			}
		}

		public static void findAllTasksByResourceID (string resource_id, List<Task> tasks)
		{

			List<Task> filtered_list = new List<Task> ();

			UI.id.output_text.text = "";

			foreach (Task task in tasks) {
				if (task.resource_id == resource_id) {
					filtered_list.Add (task);
				}				
			}

			foreach (Task task in filtered_list) {
				if (!task.taskname.Contains ("VSS Proxy")) {
					UI.id.output_text.text += (task.taskname + ". Start time: " + task.starttime + ". End time: " + task.endtime + "\n");
				}
			}
		}


	}
}
