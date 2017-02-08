namespace CommonCode.Windows
{
    /// <summary>
    /// Determines which side of its container an element will stick to.
    /// </summary>
    public enum SideTack
    { 
        Center = 0,
        Left,
        UpperLeft, 
        Up, 
        UpperRight, 
        Right,
        LowerRight,
        Down,
        LowerLeft
    }

    /// <summary>
    /// Determines how the element will resize if it is given more space.
    /// </summary>
    public enum ResizeKind
    {
        /// <summary>
        /// The element will fill all space available to it.
        /// </summary>
        FillSpace,
        /// <summary>
        /// The element will fill as much space as it can while keeping its proportions the same.
        /// </summary>
        FillRatio
    }
}
