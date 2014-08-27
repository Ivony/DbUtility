using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ivony.Parser
{
  /// <summary>
  /// 定义一个字符枚举器
  /// </summary>
  public class TextScaner : IEnumerator<char>
  {


    private string _text;

    private int _initializeIndex;

    private int _index;


    /// <summary>
    /// 构建 TextScaner 对象
    /// </summary>
    /// <param name="syncRoot">用于同步的对象</param>
    /// <param name="text">要扫描的字符串</param>
    public TextScaner( object syncRoot, string text ) : this( syncRoot, text, 0 ) { }

    /// <summary>
    /// 构建 TextScaner 对象
    /// </summary>
    /// <param name="syncRoot">用于同步的对象</param>
    /// <param name="text">要扫描的字符串</param>
    /// <param name="index">开始扫描的位置，默认为 0 </param>
    public TextScaner( object syncRoot, string text, int index )
    {
      SyncRoot = syncRoot;
      _text = text;
      _initializeIndex = _index = index;
    }



    /// <summary>
    /// 获取当前索引位置的字符
    /// </summary>
    public char Current
    {
      get
      {
        lock ( SyncRoot )
        {
          if ( _index == _text.Length )
            throw new InvalidOperationException();

          return _text[_index];
        }
      }
    }


    /// <summary>
    /// 销毁当前对象，释放所有资源
    /// </summary>
    public void Dispose()
    {
    }


    object System.Collections.IEnumerator.Current
    {
      get { return Current; }
    }


    /// <summary>
    /// 将索引移动到下一个字符，若已经到字符串末尾，则返回 false ， 否则返回 true
    /// </summary>
    /// <returns>是否到达字符串末尾</returns>
    public bool MoveNext()
    {
      lock ( SyncRoot )
      {
        _index++;
        if ( _index < _text.Length )
          return true;

        if ( _index > _text.Length )
          _index = _text.Length;
        return false;
      }
    }


    /// <summary>
    /// 重置索引到构建对象时传入的位置
    /// </summary>
    public void Reset()
    {
      lock ( SyncRoot )
      {
        _index = _initializeIndex;
      }
    }


    /// <summary>
    /// 当前索引位置
    /// </summary>
    public int Offset
    {
      get { return _index; }
    }



    /// <summary>
    /// 判断当前是指针否已经达到字符串的末尾
    /// </summary>
    public bool IsEnd
    {
      get
      {
        lock ( SyncRoot )
        {
          return _index >= _text.Length;
        }
      }
    }


    /// <summary>
    /// 从扫描的字符串中截取一段
    /// </summary>
    /// <param name="offset">开始位置</param>
    /// <param name="length">要截取的长度</param>
    /// <returns>截取的子字符串</returns>
    public string SubString( int offset, int length )
    {
      return _text.Substring( offset, length );
    }


    /// <summary>
    /// 获取当前扫描的字符串
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      return _text;
    }


    /// <summary>
    /// 跳过指定数量的字符
    /// </summary>
    /// <param name="length">要跳过的字符长度</param>
    public void Skip( int length )
    {
      lock ( SyncRoot )
      {
        if ( _index + length > _text.Length )
          throw new InvalidOperationException();

        _index += length;
      }
    }


    /// <summary>
    /// 获取用于同步的对象
    /// </summary>
    public object SyncRoot { get; private set; }


    /// <summary>
    /// 获取原始字符串
    /// </summary>
    public string Text { get { return _text; } }
  }

}
