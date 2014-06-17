using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.Entities
{
  public class UpdateEntityQuery<T> : EntityQuery<T>
  {

    public UpdateEntityQuery( T entity )
    {
      Entity = entity;
    }




    public T Entity { get; private set; }

  }
}
