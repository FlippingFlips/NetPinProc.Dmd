using NetPinProc.Domain;
using System;
using System.Collections.Generic;

namespace NetPinProc.Dmd
{
    /// <summary>
    /// A _mode that provides a DMD layer containing a generic 1-4 player score display.
    /// To use 'ScoreDisplay' simply instantiate it and Add it to the _mode queue. A low priority is recommended.
    /// <para/>
    /// When the layer is asked for its NextFrame() the DMD Frame is built based on the player score and the ball information
    /// contained in the GameController
    /// <para/>
    /// 'ScoreDisplay' uses a number of fonts, the defaults of which are included in the shared DMD resources folder. If a font
    /// cannot be found then the score may not display properly in some states.
    /// </summary>
    public class ScoreDisplay : Mode
    {
        Font font_09x5;
        Font font_09x6;
        Font font_09x7;
        Font font_14x10;
        Font font_14x8;
        Font font_14x9;
        Font font_18x10;
        Font font_18x11;
        Font font_18x12;

        /// <summary>
        /// Font used for the bottom status line text (BALL 1 FREE PLAY) -- defaults to Font07x5.dmd
        /// </summary>
        Font font_common;
        FontJustify[] score_justs;
        Dictionary<bool, List<Pair<int, int>>> score_posns;
        /// <summary>
        /// initializes font .dmd files
        /// </summary>
        /// <param name="game"></param>
        /// <param name="priority"></param>
        /// <param name="left_players_justify"></param>
        public ScoreDisplay(IGameController game, int priority, FontJustify left_players_justify = FontJustify.Right)
            : base(game, priority)
        {
            this.Layer = new ScoreLayer(128, 32, this);
            this.font_common = FontManager.instance.FontName("Font07x5.dmd");
            this.font_18x12 = FontManager.instance.FontName("Font18x12.dmd");
            this.font_18x11 = FontManager.instance.FontName("Font18x11.dmd");
            this.font_18x10 = FontManager.instance.FontName("Font18x10.dmd");
            this.font_14x10 = FontManager.instance.FontName("Font14x10.dmd");
            this.font_14x9 = FontManager.instance.FontName("Font14x9.dmd");
            this.font_14x8 = FontManager.instance.FontName("Font14x8.dmd");
            this.font_09x5 = FontManager.instance.FontName("Font09x5.dmd");
            this.font_09x6 = FontManager.instance.FontName("Font09x6.dmd");
            this.font_09x7 = FontManager.instance.FontName("Font09x7.dmd");

            this.score_posns = new Dictionary<bool, List<Pair<int, int>>>();

            this.SetLeftPlayersJustify(left_players_justify);
        }

        /// <summary>
        /// Returns the font to be used for displaying the given numeric score in a 2,3, or 4 player game
        /// </summary>
        public Font FontForScore(long score, bool is_active_player)
        {
            if (is_active_player)
            {
                if (score < 1e7)
                    return this.font_14x10;
                if (score < 1e8)
                    return this.font_14x9;
                else
                    return this.font_14x8;
            }
            else
            {
                if (score < 1e7)
                    return this.font_09x7;
                if (score < 1e8)
                    return this.font_09x6;
                else
                    return this.font_09x5;
            }
        }

        /// <summary>
        /// Returns the font to be used for displaying the given numeric score value in a single-player game
        /// </summary>
        public Font FontForScoreSingle(long score)
        {
            if (score < 1e10)
                return this.font_18x12;
            else if (score < 1e11)
                return this.font_18x11;
            else
                return this.font_18x10;
        }

        /// <summary>
        /// Returns a string representation of the given score value
        /// </summary>
        public string FormatScore(long score)
        {
            if (score == 0) return "00";
            return score.ToString("#,##0");
        }

        /// <inheritdoc/>
        public override void ModeStarted() { }

        /// <summary>
        /// Call to set the justification of the left-hand players' scores in a multiplayer game.
        /// Valid values for left_player_justify are FontJustify.Left and FontJustify.Right
        /// </summary>
        public void SetLeftPlayersJustify(FontJustify left_players_justify)
        {
            List<Pair<int, int>> position_entries;
            this.score_posns.Clear();
            if (left_players_justify == FontJustify.Left)
            {
                position_entries = new List<Pair<int, int>>();
                position_entries.Add(new Pair<int, int>(0, 0));
                position_entries.Add(new Pair<int, int>(128, 0));
                position_entries.Add(new Pair<int, int>(0, 11));
                position_entries.Add(new Pair<int, int>(128, 11));
                this.score_posns.Add(true, position_entries);
                position_entries.Clear();
                position_entries.Add(new Pair<int, int>(0, -1));
                position_entries.Add(new Pair<int, int>(128, -1));
                position_entries.Add(new Pair<int, int>(0, 16));
                position_entries.Add(new Pair<int, int>(128, 16));
                this.score_posns.Add(false, position_entries);
            }
            else
            {
                position_entries = new List<Pair<int, int>>();
                position_entries.Add(new Pair<int, int>(75, 0));
                position_entries.Add(new Pair<int, int>(128, 0));
                position_entries.Add(new Pair<int, int>(75, 11));
                position_entries.Add(new Pair<int, int>(128, 11));
                this.score_posns.Add(true, position_entries);
                position_entries.Clear();
                position_entries.Add(new Pair<int, int>(52, -1));
                position_entries.Add(new Pair<int, int>(128, -1));
                position_entries.Add(new Pair<int, int>(52, 16));
                position_entries.Add(new Pair<int, int>(128, 16));
                this.score_posns.Add(false, position_entries);
            }
            this.score_justs = new FontJustify[4] { left_players_justify, FontJustify.Right, left_players_justify, FontJustify.Right };
        }
        /// <summary>
        /// Called by the layer to Update the score layer for the present game state.
        /// </summary>
        public void UpdateLayer()
        {
            ((ScoreLayer)this.Layer).layers.Clear();
            if (this.Game.Players.Count <= 1)
                this.UpdateLayer1p();
            else
                this.UpdateLayer4p();

            // Common: Add the ball X ... FREE PLAY footer
            TextLayer common = new TextLayer(128 / 2, 32 - 6, this.font_common, FontJustify.Center);
            if (this.Game.Ball == 0)
                common.SetText("FREE PLAY");
            else
                common.SetText(String.Format("BALL {0}            FREE PLAY", this.Game.Ball));

            ((ScoreLayer)this.Layer).layers.Add(common);
        }

        FontJustify JustifyForPlayer(int player_index) => this.score_justs[player_index];
        Pair<int, int> PosForPlayer(int player_index, bool is_active_player) => this.score_posns[is_active_player][player_index];

        void UpdateLayer1p()
        {
            long score;
            if (this.Game.CurrentPlayer() == null)
                score = 0; // Small hack to make something show up on startup
            else
                score = this.Game.CurrentPlayer().Score;

            TextLayer layer = new TextLayer(128 / 2, 5, this.FontForScoreSingle(score), FontJustify.Center);
            layer.SetText(this.FormatScore(score));
            ((ScoreLayer)this.Layer).layers.Add(layer);
        }

        void UpdateLayer4p()
        {
            long score;
            bool is_active_player;
            Font font;
            Pair<int, int> pos;
            FontJustify justify;
            TextLayer layer;
            for (int i = 0; i < Game.Players.Count; i++)
            {
                score = Game.Players[i].Score;
                is_active_player = (this.Game.Ball > 0) && (i == this.Game.CurrentPlayerIndex);
                font = this.FontForScore(score, is_active_player);
                pos = this.PosForPlayer(i, is_active_player);
                justify = this.JustifyForPlayer(i);
                layer = new TextLayer(pos.First, pos.Second, font, justify);
                layer.SetText(this.FormatScore(score));
                ((ScoreLayer)this.Layer).layers.Add(layer);
            }
        }
    }
}
