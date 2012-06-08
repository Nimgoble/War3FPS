using UnityEngine;
using System.Collections;
using System;

namespace nimgoble
{
	namespace gui
	{
		public delegate void DrawGUI();
		public delegate void GUIDone();
		public interface BaseGUI 
		{
			DrawGUI GetDrawGUIDelegate();
			void Start(GUIDone doneDelegate);
			void Stop();
		}
	}
}