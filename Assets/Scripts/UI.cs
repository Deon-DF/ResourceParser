using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using LogReader;

public class UI : MonoBehaviour {

	public static UI id;

	public InputField inputField;
	public Dropdown dropdown;
	public InputField output_text;

	public void redrawDropdownResourceMenu (List<Proxy> proxies) {

		List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();

		foreach (Proxy proxy in proxies) {
			Dropdown.OptionData option = new Dropdown.OptionData ();
			option.text = proxy.name;
			options.Add(option);
		}

		dropdown.AddOptions (options);
	}



	public void listTasksForProxy () {
		string id = "";
		string name = UI.id.dropdown.options [UI.id.dropdown.value].text;

		Debug.Log ("Listing all tasks for the proxy: " + name);
		foreach (Proxy proxy in Main.id.proxyList) {
			if (name == proxy.name) {
				id = proxy.id;
			} 
		}
		Debug.LogWarning("Selected proxy: " + name + ", id: " + id);

		List<Task> filtered_list = Parser.findAllTasksByProxyID (id, Main.id.tasklist);

		foreach (Task task in filtered_list) {
			output_text.text += "Job: [" + task.job_name + "] VM: [" + task.task_name + "]" + " [" + task.task_start_time + "]" + "\n";
		}
	}

	public void Awake () {
		if (id == null) {
			id = this;

			initialize ();
		} else {			
			Debug.LogError ("Another instance of ::UI.cs:: is already running!");
		}
	}

	public void initialize () {
		dropdown.ClearOptions ();
	}
}
