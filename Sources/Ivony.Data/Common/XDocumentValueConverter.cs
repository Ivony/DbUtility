using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Ivony.Data.Common
{

  public class XDocumentValueConverter : IDbValueConverter<XDocument>
  {
    XDocument IDbValueConverter<XDocument>.ConvertValueFrom( object dataValue, string dataTypeName )
    {

      var text = (string) dataValue;
      return XDocument.Parse( text );

    }

    object IDbValueConverter<XDocument>.ConvertValueTo( object value, string dataTypeName )
    {
      return ((XDocument) value).ToString( SaveOptions.DisableFormatting | SaveOptions.OmitDuplicateNamespaces );
    }
  }
}
