using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Task  {
	
	public string task_name = "PH_Tname";
	public string job_name = "PH_Jname";
	public string id = "PH_TID";
	public System.DateTime task_start_time;
	public System.DateTime task_end_time;

	public List<ProxyUsage> proxyusagelist;

	public static string listProxies (List<ProxyUsage> in_proxyusages) {
		string output = "";

		foreach (ProxyUsage proxyusage in in_proxyusages) {
			output += "[" + proxyusage.name + "]";
		}

		return output;
	}

	public static void addTask(string in_id, string in_taskname, string in_jobname, List<Task> in_tasks, List<ProxyUsage> in_proxyusage, DateTime in_start_time) {
		Task newtask = new Task ();
		newtask.id = in_id;
		newtask.task_name = in_taskname;
		newtask.job_name = in_jobname;
		newtask.proxyusagelist = in_proxyusage;
		newtask.task_start_time = in_start_time;
		in_tasks.Add (newtask);
		Debug.Log ("[" + in_start_time + "]" + " New task with ID: " + in_id + ", proxies: " + listProxies(in_proxyusage));
	}
}

public class Proxy {
	public string name = "PH_Prname";
	public string id = "PH_PrID";

	public Proxy(string in_id, string in_name) {
		id = in_id;
		name = in_name;
	}
}

public class ProxyUsage {
	public string name = "PH_PUname";
	public string id = "PH_PrID";
	public System.DateTime usage_start_time;
	public System.DateTime usage_end_time;

	public ProxyUsage (string in_name, string in_id, System.DateTime in_start_time ) {
		id = in_id;
		name = in_name;
		usage_start_time = in_start_time;
	}

}