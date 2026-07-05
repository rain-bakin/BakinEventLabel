using System;
using System.Text.RegularExpressions;
using Yukar.Engine;
using Microsoft.Xna.Framework;

namespace Bakin
{
    public class EventLabel : BakinObject 
    {
        private string label = "";
        private bool showRequested = false;
        
        [BakinFunction(Description="キャスト名を頭上に表示する（キャスト名内に <LB:xxx> があればその文字列を優先）")]
        public void Show()
        {
            string castName = mapChr?.rom?.name ?? "";
            label = ExtractLabelText(castName);
            showRequested = true;
        }

        // キャスト名から表示ラベルを決める。<LB:xxx> があれば xxx を、なければ名前全体を使う
        private string ExtractLabelText(string castName)
        {
            if (string.IsNullOrEmpty(castName)) return "";
            var match = Regex.Match(castName, @"<LB:(.*?)>");
            return match.Success ? match.Groups[1].Value : castName;
        }

        // 毎フレーム呼ばれる
        public override void Update()
        {
            if (!showRequested || mapChr == null) return;

            // キャラの座標をスクリーンの座標に変換。ちょっとだけ上にずらす
            int screenX, screenY;
            mapScene.GetCharacterScreenPos(
                mapChr,
                out screenX, out screenY,
                MapScene.EffectPosType.Head,
                new Vector3 (0, mapChr.getHeight()/5, 0)
            );

            // ラベル自身のサイズを測って中央ぞろえ
            Vector2 textSize = Graphics.MeasureString(0, label);
            Vector2 drawPos = new Vector2 (
                screenX - textSize.X / 2f,  // 文字幅の半分をずらして中央ぞろえ
                screenY - textSize.Y        // テキスト幅の分上にずらす
            );

            // 背景（半透明ダークグレー）— 白文字とのコントラストで視認性を上げる
            int padX = 6;
            int padY = 2;
            Graphics.DrawFillRect(
                (int)drawPos.X - padX,
                (int)drawPos.Y - padY,
                (int)textSize.X + padX * 2,
                (int)textSize.Y + padY * 2,
                50, 50, 50, 160,   // R, G, B, α（α=160 で約63%不透明）
                false,             // useClip
                1                  // zOrder（テキスト zOrder=0 より奥）
            );

            Graphics.DrawString(
                0,
                label,
                drawPos,
                new Color(255, 255, 255), 0
            );

            // 次フレームで Show() が呼ばれなければ非表示になる
            showRequested = false;
        }
    }
}