using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.Entities
{
  public abstract class EntityQuery<T> : IDbQuery
  {

    protected PropertyInfo[] GetKeyProperties()
    {
      throw new NotImplementedException();
    }

    

  }
}
