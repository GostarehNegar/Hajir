namespace Hajir.Crm.Products
{
    /// <summary>
    /// Represents a Cabinet that is used to store
    /// batteries.
    /// </summary>
    public interface ICabinet
    {
        /// <summary>
        /// Number of rows in this cabinet.
        /// </summary>
        int NumberOfRows { get; }

        /// <summary>
        /// Number of columns in this cabinet.
        /// Note this depends on Battery Size.
        /// For example a cabinet row may have room for 16 7A battery
        /// while it can hold only 4 100A batteries.
        /// </summary>
        int NumberOfColumns { get; }

        /// <summary>
        /// Number of batteries that are put in this cabinet.
        /// This may be less than Capacity if the cabinet is not full.
        /// </summary>
        int Quantity { get; }

        /// <summary>
        /// Number of free cells on this cabinet.
        /// </summary>
        int Free { get; }

        /// <summary>
        /// Capacity of this cabinet that is
        /// the number of batteries this cabinet can hold.
        /// </summary>
        int Capacity { get; }
        /// <summary>
        /// Puts the quantity of batteries in the cabinet
        /// and returns the number actually put. 
        /// The returned number maybe less than the required
        /// quantity if there is no more free space in the cabinet.
        /// </summary>
        /// <param name="quantity"></param>
        /// <param name="clear"></param>
        /// <returns></returns>
        int Put(int quantity, bool clear = false);

    }
}
