using System;
using System.Collections.Generic;

namespace ContextawareFramework
{
    public class EntityList
    {
        
        Dictionary<Type, List<IEntity>> entityLists = new Dictionary<Type, List<IEntity>>();

        public List<IEntity> GetEntitiesByType(Type type)
        {
            foreach (var entityList in entityLists)
            {
                if (typeof(EntityList[0]) == type)
                {
                    
                }
            }
        }

    }
}