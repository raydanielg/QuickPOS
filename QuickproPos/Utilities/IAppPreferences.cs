namespace QuickproPos.Utilities
{
    public interface IAppPreferences
    {
        T Get<T>(string key, T defaultValue);
        void Set<T>(string key, T value);
    }

    public class AppPreferences : IAppPreferences
    {
        private readonly IPreferences _preferences;
        public AppPreferences(IPreferences preferences) => _preferences = preferences;

        public T Get<T>(string key, T defaultValue) => _preferences.Get(key, defaultValue);
        public void Set<T>(string key, T value) => _preferences.Set(key, value);
    }
}