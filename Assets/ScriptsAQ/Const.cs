using System;
using UnityEngine;
namespace Utility {


	public class Const {

		public enum ResFolder
		{
			Res256,
			Res512,
			Res1024,
		}

		public enum ButtonName
		{
			ButtonOK,
			ButtonYes,
			ButtonNo,
			Whatever
		}

		public enum DialogId
		{
			Info,
			Restart,
			Licences
		}

		public enum Axis
		{
			None,
			Hori,
			Verti
		}

		public enum Anchor
		{
			Bottom = 1 << 1,
			Top = 1 << 2,
			Left = 1 << 3,
			Right = 1 << 4,
			Middle = 1 << 5,
		}

		public enum ScreenName
		{
			GameScene,
			CupScreen,
			MainMenu
		}

		public enum AppState
		{
			None,
			MainMenu,
			InGame,
			CupPuzzle,
			CupShow
		}

		public enum ScreenState
		{
			FadeIn,
			Loading,
			Ready,
			FadeOut,
			InterstitialAd,
		}

	}
}

