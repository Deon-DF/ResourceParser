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

		public static System.DateTime str_convertVeeamtimeToDatetime(string input) {
			string pattern = @"^([0-9]{2}).([0-9]{2}).([0-9]{4}) ([0-9]{2}):([0-9]{2}):([0-9]{2})$";
			Match timematch = Regex.Match (input, pattern);

			string day = timematch.Groups [1].ToString();
			string month = timematch.Groups [2].ToString();
			string year = timematch.Groups [3].ToString();

			string hour = timematch.Groups [4].ToString();
			string minute = timematch.Groups [5].ToString();
			string second = timematch.Groups [6].ToString();

			return System.DateTime.Parse(year + "/" + month + "/" + day + " " + hour + ":" + minute + ":" + second);
		}

		public static void registerProxyUsage(Match startmatch, List<Task> tasks) {
			System.DateTime start_time = str_convertVeeamtimeToDatetime(startmatch.Groups [1].ToString ());
			//Debug.Log ("The task start time: " + start_time);

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			/// prepare a list of proxies involved, parsed from the regex
			/// We do not add them to a task yet, we just create a list in memory

			string proxies_raw = startmatch.Groups [2].ToString ();
			string proxy_pattern = @"\[Vi proxy resource \[id=(.*?) : srv name=(.*?) :";

			MatchCollection proxy_ids = Regex.Matches (proxies_raw, proxy_pattern);

			List<ProxyUsage> task_proxies = new List<ProxyUsage> (); // list of proxies from the "acquiring multi resource" string for one task

			foreach (Match match in proxy_ids) {
				string proxyusage_id = match.Groups [1].Value;
				string proxyusage_name = match.Groups [2].Value;
				Main.id.numproxies++;
				task_proxies.Add(new ProxyUsage(proxyusage_name, proxyusage_id, start_time));
			}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			/// here we decide if we want to add the acquired proxy usage list to an existing task, or allocate a new task if a task with such ID doesnt exist yet

			string task_name = startmatch.Groups[4].ToString();
			string job_name = startmatch.Groups[5].ToString();
			string task_id = startmatch.Groups[6].ToString();

			int matched_index = -1; // -1 means that such task was not found in the task list yet; used to match an id from the log with existing ids in the task list

			if (tasks.Count <= 0) {
				Debug.LogWarning ("No tasks in the list yet!");

				Task.addTask (task_id, task_name, job_name, tasks, task_proxies, start_time);

			} else {
				
				for (int index = 0; index < tasks.Count; index++)
				{
					if (tasks [index].id == task_id)
					{ // checking if task with such ID already exists as tasks[index]
						if (matched_index == -1)
						{ // if we haven't found a task with such ID yet, set the id of the index to the matched_index
							matched_index = index; // here we set the matched_index
							//Debug.Log("[EXISTS] The task list already has a task with id " + task_id);
						} 
						else
						{
							Debug.LogError ("Duplicate tasks found in the task list!"); // if the index was already matched in the task list
						}								
					}
				}

				// if we haven't found a task with the same task_id in the task list, create a new task
				if (matched_index == -1) { //
					Task.addTask (task_id, task_name, job_name, tasks, task_proxies, start_time);
				// otherwise assign the proxy usage from the parsed string to an existing task with matched_index
				} else {
					List<ProxyUsage> added_proxies = new List<ProxyUsage> ();
					string str_added_proxies = "";

					foreach (ProxyUsage proxyusage in task_proxies) {
						Main.id.numproxies++;
						added_proxies.Add (proxyusage);
						tasks [matched_index].proxyusagelist.Add (proxyusage);
					}

					foreach (ProxyUsage proxyusage in added_proxies) {
						str_added_proxies += ("[" + proxyusage.name + "]");
					}

					Debug.Log ("[" + start_time + "]" + " Added new proxies: " + str_added_proxies + " to the task: " + tasks [matched_index].id);
				}


			}
		}

		public static void unregisterProxyUsage(Match endmatch, List<Task> tasks) {
			System.DateTime end_time = str_convertVeeamtimeToDatetime(endmatch.Groups [1].ToString ());
			string proxy_id = endmatch.Groups [2].ToString ();
			string task_id =  endmatch.Groups [3].ToString ();


			//////////////////// DOES NOT WORK WITHOUT TASK LOGS
		}

		public static void parseResourceRequests(List<string> tasklog, List<Task> tasks, List<ProxyUsage> all_proxies) {

			foreach (string line in tasklog) {
				string startpattern = @"^\[(.*?)\].*\[RTS\].*ProxyResourceRequest: (.*?),  RepositoryResourceRequest : (.*?) for task.*\[(.*?) from job \""(.*?)\"" \(task id:(.*?),.*$";
				string endpattern = @"\[(.*?)\].*Vi proxy resource \[id=(.*?) :.*from task '(.*?)'";
				Match startmatch = Regex.Match (line, startpattern);

				if (startmatch.Success) {
					registerProxyUsage (startmatch, tasks);					
				} else {
					Match endmatch = Regex.Match (line, endpattern);
					if (endmatch.Success) {
						unregisterProxyUsage (endmatch, tasks);
					}
				}
			}
		}


		public static List<Task> findAllTasksByProxyID (string proxy_id, List<Task> tasks)
		{
			List<Task> filtered_list = new List<Task> ();			
			UI.id.output_text.text = "";


			foreach (Task task in tasks) {
				foreach (ProxyUsage proxyUsage in task.proxyusagelist) {
					if (proxyUsage.id == proxy_id) {
						filtered_list.Add (task);
					}
				}
			}

			return filtered_list;
		}


	}
}
