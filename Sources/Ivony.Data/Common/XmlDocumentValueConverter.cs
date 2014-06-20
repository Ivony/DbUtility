using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Ivony.Data.Common
{
  public class XmlDocumentValueConverter : IDbValueConverter<XDocument>
  {
    public XDocument ConvertValueFrom( object dataValue, DbType dbDataType )
    {

      var text = (string) dataValue;
      return XDocument.Parse( text );

    }

    public object ConvertValueTo( XDocument value, DbType? dbDataType = null )
    {

      return value.ToString( SaveOptions.DisableFormatting | SaveOptions.OmitDuplicateNamespaces );

    }
  }
}
