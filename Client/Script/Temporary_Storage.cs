using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script
{
    /// <summary>
    /// 用於暫存
    /// </summary>
     static public class Temporary_Storage
    {
         static public bool? _Bool=null;
         static public byte? _Byte;
         static public Worldstate _WorldState;
         static public Dictionary<byte, object> _Dictionary;
    }
     /*
      * 當進入S4時 _WorldState 被賦予 初始世界狀態  
      * 當進入S4時 _Byte 被賦予 自己的 Player Number
      */
 }
