using UnityEngine;

public class ConsoleToGUI : MonoBehaviour
{
	#if !UNITY_EDITOR
	static string myLog = "";
	private string output;
	private string stack;
	private bool showConsole;

	void OnEnable()
	{
		Application.logMessageReceived += Log;
	}

	void OnDisable()
	{
		Application.logMessageReceived -= Log;
	}

	public void Log(string logString, string stackTrace, LogType type)
	{
		output = logString;
		stack = stackTrace;
		myLog = output + "\n\n" + myLog;
		if (myLog.Length > 5000)
		{
			myLog = myLog.Substring(0, 4000);
		}
	}

	void OnGUI()
	{
		showConsole = GUI.Toggle(new Rect(Screen.width - 30, Screen.height - 30, 30, 30), showConsole, "");

		if (showConsole)
		{
			myLog = GUI.TextArea(new Rect(10, Screen.height / 2, Screen.width - 20, Screen.height), myLog);
		}
	}
	#endif
}