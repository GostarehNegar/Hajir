// Decompiled with JetBrains decompiler
// Type: SS.Crm.Linq.AnonymousBinding
// Assembly: SS.Crm.Linq, Version=1.0.6939.26200, Culture=neutral, PublicKeyToken=1eea6d0e8f401bee
// MVID: CA16C0DC-D52F-484E-A539-505517B5F7DC
// Assembly location: C:\Users\babak\Desktop\SS.Crm.Linq.dll

using System.Linq.Expressions;
using System.Reflection;

namespace SS.Crm.Linq
{
  internal class AnonymousBinding
  {
    public Expression SourceExpression { get; set; }

    public MemberInfo TargetMember { get; set; }
  }
}
