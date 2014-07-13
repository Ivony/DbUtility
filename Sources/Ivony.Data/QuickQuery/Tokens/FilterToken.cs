using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Data.QuickQuery.Tokens
{
  public struct FilterToken
  {


    private static FilterToken _instance = new FilterToken();

    public static object Value { get { return _instance; } }
  }
}
