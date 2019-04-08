	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using System;
	using System.Runtime.Serialization.Formatters.Binary;
	using System.IO;

	// Store and persist the input group ID and player ID
	public class PlayerData : MonoBehaviour
	{
		public static PlayerData playerdata;

		public string playerID;
		public string groupID;
        public int level;

		// Singleton method. If there is no instance of this object, persist it to the scene. If there is already an instance of this object, destroy it and use this instance.
		void Awake ()
		{
			if (playerdata == null) {
				DontDestroyOnLoad (gameObject);
				playerdata = this;
			} else if (playerdata != this) {
				Destroy (gameObject);
			}
		}
	}
