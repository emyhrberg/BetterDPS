﻿using System;
using System.Collections.Generic;
using System.Linq;
using log4net.Repository.Hierarchy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using static DPSPanel.MainCode.Panel.BossDamageTrackerSP;

namespace DPSPanel.MainCode.Panel
{
    public class Panel : UIPanel
    {
        /* -------------------------------------------------------------
         * Variables
         * -------------------------------------------------------------
         */
        // Variables for dragging the panel
        private Vector2 offset;
        private bool dragging;
        private bool clickStartedInsidePanel;
        private readonly bool IS_DRAGGABLE = false; // Debug option: Set to true to enable dragging

        // Panel items
        private readonly float padding;
        private float currentYOffset = 0;
        private const float ItemHeight = 40f;

        // Slider items
        private Dictionary<string, PanelSlider> sliders = [];
        private readonly Asset<Texture2D> sliderEmpty;
        private readonly Asset<Texture2D> sliderFull;

        // Header
        private NPC currentBoss;
        private const float headerHeight = 16f;

        private readonly Color[] colors =
        [
            Color.Red,
            Color.Green,
            Color.Blue,
            Color.Yellow,
            Color.Purple,
            Color.Orange,
            Color.Cyan,
            Color.Pink,
            Color.LightGreen,
            Color.LightBlue,
            Color.LightCoral,
            Color.LightGoldenrodYellow,

        ];

        /* -------------------------------------------------------------
         * Panel Constructor
         * -------------------------------------------------------------
         */
        public Panel(float padding)
        {
            this.padding = padding;
            SetPadding(padding);
            sliderEmpty = ModContent.Request<Texture2D>("DPSPanel/MainCode/Assets/SliderEmpty");
            sliderFull = ModContent.Request<Texture2D>("DPSPanel/MainCode/Assets/SliderFull");
        }

        /* -------------------------------------------------------------
         * Panel content
         * -------------------------------------------------------------
         */
        public void AddBossTitle(string bossName = "Boss Name", NPC npc = null)
        {
            currentBoss = npc;
            UIText bossTitle = new(bossName, 1.0f);
            bossTitle.HAlign = 0.5f;
            Append(bossTitle);

            currentYOffset = headerHeight + padding * 2; // Adjust Y offset for the next element
            ResizePanelHeight();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch); // Ensure the panel is drawn

            // Draw boss head icon if a boss is active
            if (currentBoss != null && currentBoss.active && currentBoss.boss && currentBoss.life > 0)
            {
                int headIndex = currentBoss.GetBossHeadTextureIndex();
                if (headIndex >= 0 && headIndex < TextureAssets.NpcHeadBoss.Length &&
                    TextureAssets.NpcHeadBoss[headIndex]?.IsLoaded == true)
                {
                    // Get the boss head texture
                    Texture2D bossHeadTexture = TextureAssets.NpcHeadBoss[headIndex].Value;

                    // Get the dimensions of the panel and boss title
                    CalculatedStyle dims = GetDimensions();
                    Vector2 panelCenter = new Vector2(dims.X + dims.Width / 2, dims.Y + 10f); // Panel center
                    float iconHeight = bossHeadTexture.Height;
                    float textHeight = 16f; // Approximate height of the text
                    float totalHeight = iconHeight + textHeight + 4f; // Space between icon and text

                    // Calculate icon position to center it horizontally with the text
                    Vector2 iconPosition = new Vector2(
                        panelCenter.X - bossHeadTexture.Width / 2, // Center icon horizontally
                        panelCenter.Y - totalHeight / 2            // Offset vertically to center text + icon
                    );

                    // Draw the boss head icon
                    spriteBatch.Draw(bossHeadTexture, iconPosition, Color.White);
                }
            }
        }

        public void CreateSlider(string sliderName="Name")
        {
            // Check if the slider already exists
            if (!sliders.ContainsKey(sliderName))
            {
                // Create a new slider
                PanelSlider slider = new(sliderEmpty, sliderFull)
                {
                    Width = new StyleDimension(0, 1.0f), // Fill the width of the panel
                    Height = new StyleDimension(ItemHeight, 0f), // Set height
                    Top = new StyleDimension(currentYOffset, 0f),
                    HAlign = 0.5f, // Center horizontally
                };
                Append(slider);
                currentYOffset += ItemHeight + padding * 2; // Adjust Y offset for the next element
                ResizePanelHeight();

                // Add to the dictionary
                sliders[sliderName] = slider;
            }
        }

        public void UpdateSliders(List<Weapon> weapons)
        {
            // Reset vertical offset.
            currentYOffset = headerHeight + padding * 2;

            // Sort weapons by descending damage.
            weapons = weapons.OrderByDescending(w => w.damage).ToList();
            int highest = weapons.FirstOrDefault()?.damage ?? 1;

            for (int i = 0; i < weapons.Count; i++)
            {
                var wpn = weapons[i];
                Color color = colors[i % colors.Length];

                // Get the slider for this weapon.
                PanelSlider slider = sliders[wpn.weaponName];

                // Update the slider with the current data.
                slider.UpdateSlider(highest, wpn.weaponName, wpn.damage, color, wpn.itemID, wpn.itemType);
                slider.Top.Set(currentYOffset, 0f);
                currentYOffset += ItemHeight + padding * 2;
            }
            ResizePanelHeight();
        }

        public void ClearPanelAndAllItems()
        {
            RemoveAllChildren();
            sliders = []; // reset sliders
        }

        private void ResizePanelHeight()
        {
            Height.Set(currentYOffset + padding, 0f);
            Recalculate();
        }

        /* -------------------------------------------------------------
         * Dragging functionality
         * -------------------------------------------------------------
         */

        public override void LeftMouseDown(UIMouseEvent evt)
        {

            if (!IS_DRAGGABLE) return; 

            // When the mouse is pressed, start dragging the panel
            base.LeftMouseDown(evt);
            if (evt.Target == this)
            {
                DragStart(evt);
                clickStartedInsidePanel = true;

                // Prevent other UI elements from interacting
                Main.LocalPlayer.mouseInterface = true;
            }
        }

        public override void LeftMouseUp(UIMouseEvent evt)
        {
            if (!IS_DRAGGABLE) return;

            // When the mouse is released, stop dragging the panel
            base.LeftMouseUp(evt);
            if (clickStartedInsidePanel)
            {
                DragEnd(evt);
            }
            clickStartedInsidePanel = false; // default to false
        }

        private void DragStart(UIMouseEvent evt)
        {
            if (!IS_DRAGGABLE) return;

            // Start dragging the panel
            offset = new Vector2(evt.MousePosition.X - Left.Pixels, evt.MousePosition.Y - Top.Pixels);
            dragging = true;
        }

        private void DragEnd(UIMouseEvent evt)
        {
            if (!IS_DRAGGABLE) return;

            // Stop dragging the panel
            dragging = false;
            Vector2 endMousePosition = evt.MousePosition;
            Left.Set(endMousePosition.X - offset.X, 0f);
            Top.Set(endMousePosition.Y - offset.Y, 0f);
            Recalculate();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!IS_DRAGGABLE) return;

            // If the mouse is inside the panel, set the mouse interface to true to prevent other UI elements from interacting
            if (dragging || clickStartedInsidePanel && ContainsPoint(Main.MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            // Drag the panel
            if (dragging)
            {
                Left.Set(Main.mouseX - offset.X, 0f);
                Top.Set(Main.mouseY - offset.Y, 0f);
                Recalculate();
            }

            // Keep the panel within bounds
            var parentSpace = Parent.GetDimensions().ToRectangle();
            if (!GetDimensions().ToRectangle().Intersects(parentSpace))
            {
                Left.Pixels = Utils.Clamp(Left.Pixels, 0, parentSpace.Right - Width.Pixels);
                Top.Pixels = Utils.Clamp(Top.Pixels, 0, parentSpace.Bottom - Height.Pixels);
                Recalculate();
            }
        }
    }
}
