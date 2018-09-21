using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using LogReader;

public class Main : MonoBehaviour {
	
	public static Main id;
	public int numproxies;


	string logfolder = "";
	string logpath = "";
	StreamReader file;

	List<string> tasklog = new List<string>();
	List<string> namelog = new List<string>();

	public List<Task> tasklist = new List<Task>();
	public List<ProxyUsage> proxyUsageList = new List<ProxyUsage>();
	public List<Proxy> proxyList = new List<Proxy> ();

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

		Parser.parseResourceRequests (tasklog, tasklist, proxyUsageList);

		findProxies ();
		UI.id.redrawDropdownResourceMenu (proxyList);
		Debug.Log("Done!");

		int num = 0;

		foreach (Task task in tasklist) {
			foreach (ProxyUsage proxyusage in task.proxyusagelist) {
				num++;
			}
		}
		Debug.Log ("Number of proxy registartions for usage: " + num.ToString());

	}

	public void findProxies () {

		foreach (Task task in tasklist) {
			foreach (ProxyUsage proxyUsage in task.proxyusagelist) {
				bool alreadyExists = false;
				foreach (Proxy proxy in proxyList) {
					if (proxy.id == proxyUsage.id) {
						alreadyExists = true;
					}
				}
				if (!alreadyExists) {
					proxyList.Add (new Proxy (proxyUsage.id, proxyUsage.name));
				}
				//Debug.LogWarning ("Adding a new proxy to the proxy list from proxy usage list");
			}
		}
		string foundProxies = "";

		foreach (Proxy proxy in proxyList) {
			foundProxies += "[" + proxy.name + ":" + proxy.id + "]";
		}

		Debug.Log ("Proxies found: " + foundProxies);
	}

	public void Awake () {
		if (id == null) {
			id = this;
		} else {			
			Debug.LogError ("Another instance of ::UI.cs:: is already running!");
		}
	}
}
