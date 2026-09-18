namespace HClTraining.App
{
    /// <summary>
    /// Compile-time demo gate. True only when the active Build Profile
    /// defines HCL_PPE_DEMO. Never derived from loaded scenes or PlayerPrefs.
    /// </summary>
    public static class PpePortfolioDemo
    {
        public const string ScriptingDefine = "HCL_PPE_DEMO";

        public static bool IsActive
        {
            get
            {
#if HCL_PPE_DEMO
                return true;
#else
                return false;
#endif
            }
        }
    }
}
