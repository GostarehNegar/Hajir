using System;
using System.Collections.Generic;
using System.Text;

namespace Hajir.Crm.Products
{
    /// <summary>
    /// Represents a set of cabinets;
    /// 
    /// </summary>
    public interface ICabinetSet
    {
        /// <summary>
        /// Total capacity of the cabinet set.
        /// This is the sum of capacities of cabinets 
        /// in this set.
        /// </summary>
        int Capacity { get; }
        /// <summary>
        /// Total free space on this set which is the 
        /// sum of the free spaces on all cabinets 
        /// in this set.
        /// </summary>
        int Free { get; }
        /// <summary>
        /// Number of cabinets.
        /// </summary>
        int NumberOfCabinets { get; }
        /// <summary>
        /// Cabinets in this set.
        /// </summary>
        IEnumerable<ICabinet> Cabinets { get; }
    }
}
