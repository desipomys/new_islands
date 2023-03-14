public interface IMovScriptNode
{
  /// <summary>
  /// 节点从逻辑上开始运行时
  /// </summary>
  /// <param name="eng"></param>
  void OnEnter(MovScriptEngine eng);
  void OnLeave(); 
  void OnPause();
  void OnResume();
   int OnUpdate();//0=无，1=运行到结束点，-1=运行出错
    void OnSkip();
    void OnDelete();
    /// <summary>
    /// 节点被从存档中读取出来时
    /// </summary>
    /// <param name="eng"></param>
    void OnInit(MovScriptEngine eng);
}