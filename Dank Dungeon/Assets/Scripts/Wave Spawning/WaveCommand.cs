[System.Serializable]
public class WaveCommand{
    
    public int enemyIndex;
    public enum CommandType { Spawning, Utility }
    public CommandType type;
    public enum UtilityCommand { WaitNSeconds, WaitUntilNAlive }
    public UtilityCommand utility;

    public int N { get { return n; } }

    private int n = 5;

    /// <summary>
    /// FOR THE LOVE OF GOD DO NOT USE THIS METHOD UNLESS YOU ARE WORKING IN THE EDITOR SCRIPT. You will overwrite the Wave scriptable object at runtime.
    /// </summary>
    /// <param name="n"></param>
    public void SetNFromEditorScript(int n)
    {
        this.n = n;
    }
}