﻿using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Niam.XRM.Framework.Interfaces.Data
{
    public interface IColumnSet<out T>
        where T : Entity
    {
        bool AllColumns { get; set; }
        
        ColumnSet XrmColumnSet { get; }
        
        IEnumerable<string> Columns { get; }
    }
}
