using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

// A variant for acquiring multi resource / Releasing resource 'Repository / Releasing resource 'Vi proxy resource

namespace LogReader {

	public class Log {



		public static void readFromTextFile(string file_path, List<string> tasklog, List<string> namelog)
		{
			if (System.IO.File.Exists (file_path)) {
				StreamReader file = new StreamReader (file_path);

				Debug.Log ("Loading from the file path: " + file_path);
				while (!file.EndOfStream) {
					string input_line = file.ReadLine ();
					if (input_line.Contains("Acquiring multi resource" ) || input_line.Contains("Releasing resource 'Vi proxy resource") || input_line.Contains("Releasing resource 'Repository")) {
						tasklog.Add (input_line);
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

		public static string listProxies (List<Proxy> in_proxies) {
			string output = "";

			foreach (Proxy proxy in in_proxies) {
				output += @"[""" + proxy.name + @""":""" + proxy.id + @"""]";
			}

			return output;
		}

		public static void parseResourceRequests(List<string> tasklog, List<Task> tasks, List<Proxy> all_proxies) {

			foreach (string line in tasklog) {
				string pattern = @"^\[(.*?)\].*\[RTS\].*ProxyResourceRequest: (.*?),  RepositoryResourceRequest : (.*?) for task.*\[(.*?) from job \""(.*?)\"" \(task id:(.*?),.*$";
				Match startmatch = Regex.Match (line, pattern);

				if (startmatch.Success) {

					string start_time = startmatch.Groups [1].ToString ();

					string proxies_raw = startmatch.Groups [2].ToString ();
					string proxy_pattern = @"\[Vi proxy resource \[id=(.*?) : srv name=(.*?) :";

					MatchCollection proxy_ids = Regex.Matches (proxies_raw, proxy_pattern);

					List<Proxy> task_proxies = new List<Proxy> ();

					foreach (Match match in proxy_ids) {
						string proxy_id = match.Groups [1].Value;
						string proxy_name = match.Groups [2].Value;
						task_proxies.Add(new Proxy(proxy_name, proxy_id));
					}

					string task_id = startmatch.Groups[6].ToString();

					int matched_index = -1; // -1 means that such task was not found in the task list yet; used to match an id from the log with existing ids in the task list

					if (tasks.Count <= 0) {
						Debug.LogWarning ("No tasks in the list yet!");

						Task.addTask (task_id, tasks, task_proxies);

						Debug.Log ("[NEW] Added task with ID: " + task_id + ", proxies: " + listProxies(task_proxies));

					} else {

						// here we check for an existing task with found task_id in the task list; matched_index will help us determine what to do later
						for (int index = 0; index < tasks.Count; index++) {
							if (tasks [index].id == task_id) { // if task with such ID already exists as tasks[index]
								if (matched_index == -1) { // if we haven't found a task with such ID yet, set the id of the index to the matched_index
									matched_index = index; // here we set the matched_index
									Debug.Log("[EXISTS] The task list already has a task with id " + task_id);
								} else {
									Debug.LogError ("Duplicate tasks found in the task list!"); // if the index was already matched in the task list
								}								
							}
						}

						if (matched_index == -1) { // if we haven't found a task in the task list
							Task.addTask (task_id, tasks, task_proxies);
							Debug.Log ("[NEW] Added task with ID: " + task_id + ", proxies: "  + listProxies(task_proxies));
						} else {				// we need to add extra resource assignment here to the matched task !TODO!
						}
					}
				}

				/*
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

				} */
			}
		}


		public static void findAllTasksByResourceID (string resource_id, List<Task> tasks)
		{

			List<Task> filtered_list = new List<Task> ();
			
			UI.id.output_text.text = "";
			
			/*
			foreach (Task task in tasks) {
				if (task.resource_id == resource_id) {
					filtered_list.Add (task);
				}				
			}

			foreach (Task task in filtered_list) {
				if (!task.taskname.Contains ("VSS Proxy")) {
					UI.id.output_text.text += (task.taskname + ". Start time: " + task.starttime + ". End time: " + task.endtime + "\n");
				}
			}*/
		}


	}
}
