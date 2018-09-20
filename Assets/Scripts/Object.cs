using System.Collections;
using System.Collections.Generic;

public class Task  {
	
	public string taskname = "PH_Tname";
	public string id = "PH_TID";

	public List<Proxy> proxies;

	public string comment = "Empty comment";

	public static void addTask(string in_id, List<Task> in_tasks, List<Proxy> in_proxies) {
		Task newtask = new Task ();
		newtask.id = in_id;
		newtask.comment = "";
		newtask.proxies = in_proxies;
		in_tasks.Add (newtask);
	}
}


public class Resource {
	public string name = "PH_Rname";
	public string id = "PH_RID";
	public string usage_start_time = "00:00:00";
	public string usage_end_time = "00:00:00";
}

public class Proxy {
	public string name = "PH_Rname";
	public string id = "PH_RID";
	public string usage_start_time = "00:00:00";
	public string usage_end_time = "00:00:00";

	public Proxy (string in_name, string in_id) {
		id = in_id;
		name = in_name;
	}

}