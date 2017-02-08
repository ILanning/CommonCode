﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;

namespace CommonCode.Drawing
{
    public class Spirograph : IModifiable3D, ICopyable<Spirograph>
    {
        /// <summary>
        /// r value, radius of moving circle
        /// </summary>
        public float smallerRadius;
        /// <summary>
        /// R value, radius of the central circle
        /// </summary>
        public float largerRadius;
        /// <summary>
        /// Q value, Radians to run the spirograph for.
        /// </summary>
        public float radians;
        /// <summary>
        /// R value, radius of center circle
        /// </summary>
        public float distToPoint;
        /// <summary>
        /// 
        /// </summary>
        public Vector3 scale;
        public float Cusps { get { return largerRadius/smallerRadius; } }

        protected Vector3 worldPosition = Vector3.Zero;
        protected Quaternion rotation = Quaternion.Identity;
        protected Color color;

        public bool DoneSlowDrawing { get{return !slowDrawing;} }
        protected bool slowDrawing = false;
        protected int timeForSlowDraw = 0;
        protected int timeSpentForSlowDraw = 0;

        public Vector3 WorldPosition { get { return worldPosition; } set { worldPosition = value; } }
        public Quaternion Rotation
        {
            get { return rotation; }
            set
            {
                rotation = value;
                if (rotation.LengthSquared() > 1.001f || rotation.LengthSquared() < 0.999f)
                    rotation.Normalize();
            }
        }
        public Vector3 Scale { get { return scale; } set { scale = value; } }
        public Color Color 
        { 
            get { return color; } 
            set 
            { 
                color = value;
                for (int i = 0; i < lineVertices.Length; i++ )
                    lineVertices[i].Color = value;
            } 
        }

        public VertexPositionColor[] lineVertices;
        protected int[] vertexOrder;

        public Spirograph() { }

        /// <summary>
        /// Sets the data for an epicycloid with the given parameters.
        /// </summary>
        /// <param name="r">Outer cirle radius.</param>
        /// <param name="p">Number of inner points.</param>
        /// <param name="q">Radians to draw out.</param>
        /// <param name="time">Time to run SlowDraw for.  Set to zero or less to not SlowDraw.</param>
        public Spirograph(float r, float R, float q, float DistToPoint, float scalar, int time, Color color)
        {
            smallerRadius = r*scalar;
            largerRadius = R*scalar;
            radians = q;
            distToPoint = DistToPoint*scalar;
            if(time > 0)
            {
                slowDrawing = true;
                timeForSlowDraw = time;
            }
            this.color = color;
        }

        public virtual void Generate(int lineCount)
        {
            lineVertices = new VertexPositionColor[lineCount];
            vertexOrder = new int[(lineCount * 2)];

            float step = (MathHelper.TwoPi * radians) / (float)(lineCount-1);

            for (int i = 0; i < lineCount; i++)
            {
                Vector2 tempVector = FindValue(step * (float)i);
                lineVertices[i] = new VertexPositionColor(new Vector3(0, tempVector.Y, tempVector.X), Color);
            }

            for (int i = 0; i < (lineCount) * 2; i++)
            {
                vertexOrder[i] = (i + 1) / 2;
            }
        }

        public Vector2 FindValue(float angle)
        {
            return new Vector2((float)((largerRadius + smallerRadius) * Math.Cos(angle) - distToPoint * Math.Cos((largerRadius + smallerRadius)/smallerRadius * angle)),
                               (float)((largerRadius + smallerRadius) * Math.Sin(angle) - distToPoint * Math.Sin((largerRadius + smallerRadius) / smallerRadius * angle)));
        }

        public void Update()
        {

            timeSpentForSlowDraw++;
            for (int i = 0; i < modifiers.Length; i++)
            {
                if (modifiers[i] != null)
                {
                    modifiers[i].Update();
                    if (modifiers[i].RemoveIfComplete && !modifiers[i].Active)
                    {
                        modifiers[i].Remove();
                        i--;
                    }
                }
            }
        }

        public void Draw(BasicEffect effect, GraphicsDevice graphics)
        {
            if (slowDrawing)
                SlowDraw(effect, graphics);
            else
            {
                effect.TextureEnabled = false;
                effect.VertexColorEnabled = true;
                effect.World = Matrix.CreateFromQuaternion(rotation) * Matrix.CreateTranslation(worldPosition);
                effect.CurrentTechnique.Passes[0].Apply();
                graphics.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.LineList,
                                                                    lineVertices, 0, lineVertices.Length, vertexOrder, 0, lineVertices.Length - 1);
            }
        }

        /// <summary>
        /// Will add lines on to the spirograph over [time] frames, starting with none and ending with a full spirograph.
        /// </summary>
        /// <param name="time"></param>
        public void SlowDraw(BasicEffect effect, GraphicsDevice graphics)
        {
            int vertsToDraw = (int)((float)lineVertices.Length * ((float)timeSpentForSlowDraw / (float)timeForSlowDraw));
            int[] resizedOrderArray = vertexOrder.Take(vertsToDraw * 2).ToArray();
            effect.TextureEnabled = false;
            effect.VertexColorEnabled = true;
            effect.World = Matrix.CreateFromQuaternion(rotation) * Matrix.CreateTranslation(worldPosition);
            graphics.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.LineList, lineVertices, 0,
                                                                vertsToDraw, vertexOrder, 0, vertsToDraw - 1);
            if (timeSpentForSlowDraw == timeForSlowDraw)
            {
                slowDrawing = false;
                timeForSlowDraw = 0;
                timeSpentForSlowDraw = 0;
            }
        }

        #region IModifiable Members

        IModifier3D[] modifiers = new IModifier3D[4];

        public IModifier3D[] Modifiers
        {
            get { return modifiers; }
        }

        public void AddModifier(IModifier3D modifier)
        {
            modifier.Owner = this;
            for (int i = 0; i <= modifiers.Length; i++)
            {
                if (i == modifiers.Length)
                {
                    IModifier3D[] newModifiersArray = new IModifier3D[modifiers.Length + 4];
                    for (int h = 0; h < modifiers.Length; h++)
                    {
                        newModifiersArray[h] = modifiers[h];
                    }
                    newModifiersArray[modifiers.Length] = modifier;
                    modifiers = newModifiersArray;
                }
                if (modifiers[i] == null)
                {
                    modifiers[i] = modifier;
                    break;
                }
            }
        }

        public void ClearModifiers()
        {
            modifiers = new IModifier3D[4];
        }

        #endregion

        #region ICopyable<Spirograph> Members

        public Spirograph ShallowCopy()
        {
            throw new NotImplementedException();
        }

        public Spirograph ShallowCopy(LoadArgs l)
        {
            throw new NotImplementedException();
        }

        public Spirograph DeepCopy()
        {
            throw new NotImplementedException();
        }

        public Spirograph DeepCopy(LoadArgs l)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
