public interface IMovScriptNode
{
  /// <summary>
  /// �ڵ���߼��Ͽ�ʼ����ʱ
  /// </summary>
  /// <param name="eng"></param>
  void OnEnter(MovScriptEngine eng);
  void OnLeave(); 
  void OnPause();
  void OnResume();
   int OnUpdate();//0=�ޣ�1=���е������㣬-1=���г���
    void OnSkip();
    void OnDelete();
    /// <summary>
    /// �ڵ㱻�Ӵ浵�ж�ȡ����ʱ
    /// </summary>
    /// <param name="eng"></param>
    void OnInit(MovScriptEngine eng);
}