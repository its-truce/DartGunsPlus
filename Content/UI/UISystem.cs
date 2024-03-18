using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace DartGunsPlus.Content.UI
{ // Taken from verveiene
	[Autoload(Side = ModSide.Client)]
	public class UISystem : ModSystem
	{
		private static float _uiScale = -1f;
		private static bool _ignoreHotbarScroll;

		private static readonly List<ModUIState> UIStates = new();
		private static readonly List<UserInterface> UserInterfaces = new();

		public static readonly RasterizerState OverflowHiddenRasterizerState = new()
		{
			CullMode = CullMode.None,
			ScissorTestEnable = true
		};

		public static void RequestIgnoreHotbarScroll()
			=> _ignoreHotbarScroll = true;

		public static T GetUIState<T>() where T : ModUIState
			=> UIStates.FirstOrDefault(i => i is T) as T;

		private static void OnResolutionChanged(Vector2 screenSize)
		{
			foreach (var uiState in UIStates)
			{
				uiState.OnResolutionChanged((int)screenSize.X, (int)screenSize.Y);
			}
		}

		private static void ModifyScrollHotbar(On_Player.orig_ScrollHotbar orig, Player player, int offset)
		{
			if (_ignoreHotbarScroll) return;

			orig(player, offset);
		}

		private static void ResetVariables(GameTime _)
		{
			_ignoreHotbarScroll = false;
		}

		// ...

		public override void Load()
		{
			foreach (Type type in Mod.Code.GetTypes())
			{
				if (!type.IsSubclassOf(typeof(ModUIState))) continue;

				var uiState = (ModUIState)Activator.CreateInstance(type, null);
				UIStates.Add(uiState);
			}

			UIStates.Sort((x, y) => x.Priority.CompareTo(y.Priority));

			foreach (var uiState in UIStates)
			{
				uiState.Activate();

				var userInterface = new UserInterface();
				userInterface.SetState(uiState);
				UserInterfaces.Add(userInterface);
			}

			On_Player.ScrollHotbar += ModifyScrollHotbar;
			Main.OnResolutionChanged += OnResolutionChanged;
			Main.OnPreDraw += ResetVariables;
		}

		public override void Unload()
		{
			Main.OnPreDraw -= ResetVariables;
			Main.OnResolutionChanged -= OnResolutionChanged;
			On_Player.ScrollHotbar -= ModifyScrollHotbar;

			foreach (var uiState in UIStates)
			{
				uiState.Deactivate();
				uiState.Unload();
			}

			UIStates.Clear();
			UserInterfaces.Clear();
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			for (int i = 0; i < UserInterfaces.Count; i++)
			{
				var uiState = UIStates[i];
				var userInterface = UserInterfaces[i];

				var index = uiState.InsertionIndex(layers);
				if (index < 0) continue;

				layers.Insert(index, new LegacyGameInterfaceLayer(
					name: $"{Mod.Name}: {uiState.Name}",
					drawMethod: () =>
					{
						if (uiState.Visible)
						{
							userInterface.Draw(Main.spriteBatch, Main._drawInterfaceGameTime);
						}
						return true;
					},
					scaleType: uiState.ScaleType)
				);
			}
		}

		public override void UpdateUI(GameTime gameTime)
		{
			if (_uiScale != Main.UIScale)
			{
				_uiScale = Main.UIScale;

				foreach (var uiState in UIStates)
				{
					uiState.OnUIScaleChanged();
				}
			}

			if (Main.mapFullscreen) return;

			foreach (var userInterface in UserInterfaces)
			{
				userInterface.Update(gameTime);
			}
		}
	}
}