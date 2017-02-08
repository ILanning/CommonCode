using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace CommonCode.Windows
{
    //Multiple columns
    //Leftmost column left justified, righmost  column right justified, all others center justified
    //Can choose spacing of columns
    public class TextListElement : Element, IDataDisplay
    {
        public static string IntendedType { get { return typeof(string[,]).Name; } }

        DataProvider boundFunction;
        string[,] data;
        string font;
        Color color;
        Coordinate[,] dataPositions;
        ColumnOptions[] columns;
        int interlineSpacing = 5;

        public TextListElement(ColumnOptions[] columns, Color textColor, Coordinate maxSize, Coordinate minSize, SideTack attachment = SideTack.Center, DataProvider provider = null, string fontName = "Default") 
        {
            font = fontName;
            this.columns = columns;
            boundFunction = provider;
            MaximumSize = maxSize;
            MinimumSize = minSize;
            this.color = textColor;
            SideAttachment = attachment;
            UpdateData();
        }

        public override void Move(Coordinate movement)
        {
            targetArea.Location += (Point)movement;
        }

        public override void Resize(Rectangle targetSpace)
        {
            Rectangle resultArea = Rectangle.Empty;

            resultArea.Width = targetSpace.Width > MaximumSize.X ? MaximumSize.X : targetSpace.Width;
            resultArea.Height = targetSpace.Height > MaximumSize.Y ? MaximumSize.Y : targetSpace.Height;

            resultArea.Location = (Point)SideStick(targetSpace, resultArea);
            targetArea = resultArea;
            if (targetArea != Rectangle.Empty)
                rebuildTable();
        }

        public bool BindData(DataProvider provider)
        {
            if (provider.Method.ReturnType.Name != IntendedType)
                return false;
            boundFunction = provider;
            return true;
        }

        public void UpdateData()
        {
            if (boundFunction == null)
                data = new string[,] { {"No Data"} };
            else
                data = (string[,])boundFunction.Invoke();
            if (columns.Length < data.GetLength(0))
            {
                List<ColumnOptions> columnList = new List<ColumnOptions>(columns);
                for (int i = columns.Length - 1; i < data.GetLength(0); i++)
                    columnList.Add(new ColumnOptions(Justification.Middle, 1));
                columns = columnList.ToArray();
            }
            if(targetArea != Rectangle.Empty)
                rebuildTable();
        }

        /// <summary>
        /// Finds the proper pixel locations of all entries in the text table, as well as the size that the table must be to prevent overlapping text.
        /// </summary>
        void rebuildTable()
        {
            dataPositions = new Coordinate[data.GetLength(0), data.GetLength(1)];
            //Figure out size of each piece of text
            Coordinate[,] stringSizes = new Coordinate[data.GetLength(0), data.GetLength(1)];
            //Figure out minimum size from that (widest row, number of rows * (text height + spacing))
            int tableMinX = 0;
            //Figure out the amount of space that each column needs
            int[] columnMins = new int[data.GetLength(0)];
            for (int y = 0; y < data.GetLength(1); y++)
            {
                int rowMinX = 0;
                for (int x = 0; x < data.GetLength(0); x++)
                {
                    stringSizes[x, y] = (Coordinate)ScreenManager.Globals.Fonts[this.font].MeasureString(data[x, y]);
                    rowMinX += stringSizes[x, y].X;
                    if (stringSizes[x, y].X > columnMins[x])
                        columnMins[x] = stringSizes[x, y].X;
                }
                if (rowMinX > tableMinX)
                    tableMinX = rowMinX;
            }

            //Divide available x space between each column, taking into account desired fraction of available space
            int[] columnAllottedXSpace = new int[columns.Length];
            int availableWidth = targetArea.Width;
            float total = 0;
            for (int i = 0; i < columns.Length; i++)
                total += columns[i].Spacing;
            for (int i = 0; i < columns.Length; i++)
            {
                columnAllottedXSpace[i] = (int)((columns[i].Spacing / total) * availableWidth);
                //Try to prevent overlapping by stealing space from the column to follow
                if (columnAllottedXSpace[i] < columnMins[i])
                {
                    availableWidth -= columnMins[i];
                    total -= columns[i].Spacing;
                    columnAllottedXSpace[i] = columnMins[i];
                }
            }

            //Place text according to column justification
            for (int y = 0; y < data.GetLength(1); y++)
            {
                int yCoord = y * (interlineSpacing + stringSizes[0, y].Y);
                int currentleftMost = 0;
                for (int x = 0; x < data.GetLength(0); x++)
                {
                    switch (columns[x].Justified)
                    { 
                        case Justification.Left:
                            dataPositions[x, y] = new Coordinate(currentleftMost, yCoord);
                            break;
                        case Justification.Middle:
                            dataPositions[x, y] = new Coordinate(currentleftMost + ((columnAllottedXSpace[x] - stringSizes[x, y].X) / 2), yCoord);
                            break;
                        case Justification.Right:
                            dataPositions[x, y] = new Coordinate((currentleftMost + columnAllottedXSpace[x]) - stringSizes[x, y].X, yCoord);
                            break;
                    }
                    currentleftMost += columnAllottedXSpace[x];
                }
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            for (int y = 0; y < data.GetLength(1); y++)
            {
                for (int x = 0; x < data.GetLength(0); x++)
                {
                    sb.DrawString(ScreenManager.Globals.Fonts[font], data[x, y], dataPositions[x, y] + targetArea.Location, color);
                }
            }
        }
    }

    public struct ColumnOptions
    {
        public Justification Justified;
        public float Spacing;

        public ColumnOptions(Justification justification, float spacing)
        {
            Justified = justification;
            if (spacing <= 0)
                throw new ArgumentException("Spacing must be a positive number.");
            Spacing = spacing;
        }
    }

    public enum Justification
    { 
        Left = 0,
        Middle,
        Right
    }
}