﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SkiaSharp;

namespace ChemNotation.DiagramObjects
{
    class Atom : DiagramObject
    {
        /// <summary>
        /// Type ID. Used for casting.
        /// </summary>
        public override ObjectTypeID ObjectID { get; } = ObjectTypeID.Atom;

        private static readonly string[] PropertyKeys =
        {
            "X", "Y", "Symbol", "FontFamily", "FontSize", "Colour", "Charge", "LoneElectronCount", "ElectronAngle"
        };

        public float X { get; private set; }
        public float Y { get; private set; }
        public string Symbol { get; private set; }
        public string FontFamily { get; private set; }
        public float FontSize { get; private set; }
        public int LoneElectronCount { get; private set; }
        public float ElectronAngle { get; private set; }
        public SKColor Colour { get; private set; }
        public int Charge { get; private set; }

        public Atom() : this("C", 0f, 0f, null, "Arial", 16, 0) { }

        /// <summary>
        /// Creates a new atom object. Default parameters used if arguments omitted.
        /// </summary>
        /// <param name="symbol">Symbol, default <code>C</code>C/param>
        /// <param name="x">Pixel x-coordinate of object</param>
        /// <param name="y">Pixel y-coordinate of object</param>
        /// <param name="fontFamily">Font family of text</param>
        /// <param name="fontSize">Font size of text</param>
        /// <param name="colour">Font colour</param>
        public Atom(string symbol = "C", float x = 0, float y = 0, SKColor? colour = null, string fontFamily = "Arial", float fontSize = 16, int charge = 0, int loneElectronCount = 0, float electronAngle = 0f)
        {
            DiagramID = Program.DForm.CurrentDiagram.NextFreeID();
            X = x;
            Y = y;
            Symbol = symbol;
            FontFamily = fontFamily;
            FontSize = fontSize;
            Colour = (colour == null ? DefaultColour : colour.Value);
            Charge = charge;
            LoneElectronCount = loneElectronCount;
            ElectronAngle = electronAngle;
        }

        public override void Draw(Diagram diagram)
        {
            SKPaint paint = new SKPaint
            {
                Color = Colour,
                IsAntialias = true,
                Typeface = SKTypeface.FromFamilyName(FontFamily),
                TextAlign = SKTextAlign.Center,
                TextSize = FontSize
            };

            if (Charge != 0)
            {
                string chargeText = $"{(Math.Abs(Charge) > 1 ? (Math.Sign(Charge) * Charge).ToString() : string.Empty)}";
                if (Charge < 0) chargeText += '-';
                else if (Charge > 0) chargeText += '+';
                float chargeFontSize = FontSize / 2f;

                SKPaint secondPaint = paint.Clone();
                secondPaint.TextSize = chargeFontSize;
                secondPaint.TextAlign = SKTextAlign.Left;

                diagram.DiagramSurface.Canvas.DrawText(chargeText, X + FontSize / 2.25f, Y, secondPaint);
            }

            diagram.DiagramSurface.Canvas.DrawText(Symbol, X, Y + FontSize / 2.25f, paint);

            if (LoneElectronCount > 0)
            {
                SKPaint p = new SKPaint
                {
                    Color = Colour,
                    Style = SKPaintStyle.Fill,
                    IsAntialias = true
                };

                double angle = (ElectronAngle / 180d) * Math.PI;
                double a1 = angle + (Math.PI / 8);
                double a2 = angle - (Math.PI / 8);

                if (LoneElectronCount >= 2)
                {
                    float dPointX1 = X + (float)((FontSize * 0.9) * Math.Cos(a1));
                    float dPointY1 = Y + (float)((FontSize * 0.9) * Math.Sin(a1));

                    float dPointX2 = X + (float)((FontSize * 0.9) * Math.Cos(a2));
                    float dPointY2 = Y + (float)((FontSize * 0.9) * Math.Sin(a2));

                    diagram.DiagramSurface.Canvas.DrawCircle(dPointX1, dPointY1, FontSize / 10.5f, p);
                    diagram.DiagramSurface.Canvas.DrawCircle(dPointX2, dPointY2, FontSize / 10.5f, p);
                }
                else
                {
                    float dPointX = X + (float)((FontSize * 0.9) * Math.Cos(angle));
                    float dPointY = Y + (float)((FontSize * 0.9) * Math.Sin(angle));

                    diagram.DiagramSurface.Canvas.DrawCircle(dPointX, dPointY, FontSize / 10.5f, p);
                }
            }

            paint.Dispose();
        }

        public override void EditInternalParameters(Dictionary<string, object> parameters)
        {
            foreach (string key in parameters.Keys)
            {
                try
                {
                    // If dictionary has a key, attempt to set respective value.
                    switch (key)
                    {
                        case "X":
                            X = (float)parameters[key];
                            break;
                        case "Y":
                            Y = (float)parameters[key];
                            break;
                        case "Symbol":
                            Symbol = (string)parameters[key];
                            break;
                        case "FontFamily":
                            FontFamily = (string)parameters[key];
                            break;
                        case "FontSize":
                            FontSize = (float)parameters[key];
                            break;
                        case "Colour":
                            Colour = (SKColor)parameters[key];
                            break;
                        case "Charge":
                            Charge = (int)parameters[key];
                            break;
                        case "LoneElectronCount":
                            LoneElectronCount = (int)parameters[key];
                            break;
                        case "ElectronAngle":
                            ElectronAngle = (float)parameters[key];
                            break;
                        default:
                            continue;
                    }
                }
                catch (InvalidCastException e)
                {
                    // Dictionary has invalid data type. Error needs to be addressed.
                    ErrorLogger.ShowErrorMessageBox(e);
                    continue;
                }
                catch (Exception e)
                {
                    // Dictionary has an invalid key. Error can be left alone.
                    Log.LogMessageError("Miscellaneous error:", e);
                    continue;
                }
            }
        }

        public override Dictionary<string, object> GetInternalParameters()
        {
            try
            {
                return new Dictionary<string, object>
                {
                    { "X", X },
                    { "Y", Y },
                    { "Symbol", Symbol },
                    { "FontFamily", FontFamily },
                    { "FontSize", FontSize },
                    { "Colour", Colour },
                    { "Charge", Charge },
                    { "LoneElectronCount", LoneElectronCount },
                    { "ElectronAngle", ElectronAngle }
                };
            }
            catch (Exception e)
            {
                // Uncaught misc. error. Please log.
                ErrorLogger.ShowErrorMessageBox(e);
                return null;
            }
        }

        public override bool IsMouseIntersect(Point location)
        {
            return Math.Sqrt((location.X - X) * (location.X - X) + (location.Y - Y) * (location.Y - Y)) < (FontSize / 2);
        }

        public override void ReplaceInternalParameters(Dictionary<string, object> parameters)
        {
            if (parameters.Keys.Contains("Red") && parameters.Keys.Contains("Green") && parameters.Keys.Contains("Blue") && parameters.Keys.Contains("Alpha"))
            {
                if (parameters.Keys.Contains("Colour"))
                {
                    parameters["Colour"] = new SKColor(
                        (byte)parameters["Red"],
                        (byte)parameters["Green"],
                        (byte)parameters["Blue"],
                        (byte)parameters["Alpha"]
                        );
                }
                else
                {
                    parameters.Add("Colour", new SKColor(
                        (byte)parameters["Red"],
                        (byte)parameters["Green"],
                        (byte)parameters["Blue"],
                        (byte)parameters["Alpha"]
                        ));
                }
            }
            EditInternalParameters(parameters);
        }

        public override Dictionary<string, object> GetEditableParameters()
        {
            try
            {
                return new Dictionary<string, object>
                {
                    { "X", X },
                    { "Y", Y },
                    { "Symbol", Symbol },
                    { "FontFamily", FontFamily },
                    { "FontSize", FontSize },
                    { "Red", Colour.Red },
                    { "Green", Colour.Green },
                    { "Blue", Colour.Blue },
                    { "Alpha", Colour.Alpha },
                    { "Charge", Charge },
                    { "LoneElectronCount", LoneElectronCount },
                    { "ElectronAngle", ElectronAngle }
                };
            }
            catch (Exception e)
            {
                // Uncaught misc. error. Please log.
                ErrorLogger.ShowErrorMessageBox(e);
                return null;
            }
        }
    }
}
