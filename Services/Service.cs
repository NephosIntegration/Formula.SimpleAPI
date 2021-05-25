using System;
using Formula.SimpleCore;

namespace Formula.SimpleAPI
{
    /// <summary>
    /// An status envelope for delivering response payloads
    /// </summary>
    /// <typeparam name="TData">The actual data being requested</typeparam>
    public class Status<TData> : TypedStatusBuilder<TData>
    {
        
    }
}
