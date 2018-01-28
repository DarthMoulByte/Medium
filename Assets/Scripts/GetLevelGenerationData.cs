using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class GetLevelGenerationData : MonoBehaviour
{
	public string address = "8.8.8.8";
	public TracertFetcher.DataType dataType = TracertFetcher.DataType.Ping;
	public TextAsset premadeData;

	public TracertOutput GetTheData ()
	{
		string output;
		if (dataType == TracertFetcher.DataType.Premade)
		{
			output = premadeData.text;
		}
		else
		{
			TracertFetcher.GetData(dataType, address, out output);
		}

		return new TracertOutput(output);
	}

	public class TracertOutput
	{
		public string startInfo;
		public List<Hop> tries = new List<Hop>();
		public string endInfo;

		public class Hop
		{
			public string index;
			public string ip;
			public string hostName;
			public string try1;
			public string try2;
			public string try3;
		}

		public TracertOutput (string input)
		{
			var lines = input.Split('\n');

			tries = new List<Hop>();

			for (var i = 0; i < lines.Length; i++)
			{
				var line = lines[i];

//				Debug.Log("Line " + i + ": " + line);

				if (i == 1)
				{
					startInfo = line;
				}

				if (i > 2 && i < lines.Length - 3)
				{
					string index = line.Substring(0, 3);
					string try1 = line.Substring(4, 8);
					string try2 = line.Substring(13, 8);
					string try3 = line.Substring(22, 8);
					string hostName = line.Substring(32, line.Length - 32);

					var hop = new Hop()
					{
						index = index,
						hostName = hostName,
						ip = hostName,
						try1 = try1,
						try2 = try2,
						try3 = try3,
					};

					tries.Add(hop);

					// TODO: properly parse hostname if present

//					Debug.Log("index: " + index);
//					Debug.Log("try1: " + try1);
//					Debug.Log("try2: " + try2);
//					Debug.Log("try3: " + try3);
//					Debug.Log("hostName: " + hostName);
				}

				if ( i == lines.Length - 1)
				{
					endInfo = line;
				}

			}
		}
	}

	private void Start()
	{
		Debug.Log(GetTheData());
	}
}
