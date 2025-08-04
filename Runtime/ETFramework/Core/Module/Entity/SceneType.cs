namespace ET
{
    public enum SceneType
    {
        None = -1,
        Process = 0,
        Manager = 1,
        Realm = 2,
        Gate = 3,
        Http = 4,
        Location = 5,
        Map = 6,
        Router = 7,
        RouterManager = 8,
        Robot = 9,
        BenchmarkClient = 10,
        BenchmarkServer = 11,
        Benchmark = 12,
        Account = 13,
        LoginCenter = 14,
        Other = 15,

        // 客户端Model层
        Client = 31,
        Current = 34,
        // 客户端View层
        View = 35,
        
        //GameTool
        GameLog = 99,
    }
    
    public static class SceneName
    {
        public const string Realm = "Realm";
        public const string Gate = "Gate";
        public const string Http = "Http";
        public const string Location = "Location";
        // public const string Map = "Map"; 这个会是Map1 Map2 Map3
        public const string Router = "Router";
        public const string RouterManager = "RouterManager";
        public const string BenchmarkClient = "BenchmarkClient";
        public const string BenchmarkServer = "BenchmarkServer";
        public const string Benchmark = "Benchmark";
        public const string Account = "Account";
        public const string LoginCenter = "LoginCenter";
    }
}