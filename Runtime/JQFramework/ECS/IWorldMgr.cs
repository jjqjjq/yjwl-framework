using System.Collections.Generic;
using JQCore.ECS;

namespace JQFramework.ECS
{
    public interface IWorldMgr
    {
        public void GameStart();
        
        public void GameOver();

        public Dictionary<long, JQEntity> getTypeEntity(EntityType type);
    }
}