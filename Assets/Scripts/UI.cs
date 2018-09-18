using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UI : MonoBehaviour {

	public static UI id;

	public InputField inputField;
	public Dropdown dropdown;

	public void redrawDropdownResourceMenu (List<Resource> resources) {

		List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();

		foreach (Resource resource in resources) {
			Dropdown.OptionData option = new Dropdown.OptionData ();
			option.text = resource.name;
			options.Add(option);
		}

		dropdown.AddOptions (options);
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
